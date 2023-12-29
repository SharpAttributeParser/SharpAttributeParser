namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser.Logging;

using System;
using System.Collections.Generic;
using System.Linq;

/// <inheritdoc cref="ISyntacticParser"/>
public sealed class SyntacticParser : ISyntacticParser
{
    private readonly ISyntacticParserLogger Logger;

    /// <summary>Instantiates a <see cref="SyntacticParser"/>, extracting syntactic information about the arguments of attributes.</summary>
    /// <param name="logger">The logger used to log messages.</param>
    public SyntacticParser(ISyntacticParserLogger<SyntacticParser>? logger = null)
    {
        Logger = logger ?? NullSyntacticParserLogger<SyntacticParser>.Singleton;
    }

    /// <inheritdoc/>
    public bool TryParse(ISyntacticRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        if (attributeData is null)
        {
            throw new ArgumentNullException(nameof(attributeData));
        }

        if (attributeSyntax is null)
        {
            throw new ArgumentNullException(nameof(attributeSyntax));
        }

        using var _ = Logger.BeginScopeParsingAttribute(recorder.GetType(), attributeData, attributeSyntax);

        if (attributeData.AttributeClass is not INamedTypeSymbol attributeClass)
        {
            Logger.UndeterminedAttributeClass();

            return false;
        }

        using var __ = Logger.BeginScopeDeterminedAttributeClass(attributeClass);

        if (attributeClass.TypeKind is TypeKind.Error)
        {
            Logger.UnrecognizedAttributeClass();

            return false;
        }

        if (attributeData.AttributeConstructor is not IMethodSymbol targetConstructor)
        {
            Logger.UndeterminedTargetConstructor();

            return false;
        }

        using var ___ = Logger.BeginScopeDeterminedTargetConstructor(targetConstructor);

        return TryParseArguments(recorder, attributeClass, targetConstructor, attributeData, attributeSyntax);
    }

    private bool TryParseArguments(ISyntacticRecorder recorder, INamedTypeSymbol attributeClass, IMethodSymbol targetConstructor, AttributeData attributeData, AttributeSyntax attributeSyntax)
    {
        return TypeArgumentsParser.TryParse(recorder, attributeClass, attributeSyntax, Logger)
            && ConstructorArgumentsParser.TryParse(recorder, targetConstructor, attributeData, attributeSyntax, Logger)
            && NamedArgumentsParser.TryParse(recorder, attributeData, attributeSyntax, Logger);
    }

    private sealed class TypeArgumentsParser
    {
        public static bool TryParse(ISyntacticRecorder recorder, INamedTypeSymbol attributeClass, AttributeSyntax attributeSyntax, ISyntacticParserLogger logger)
        {
            using var _ = logger.TypeArgument.BeginScopeParsingTypeArguments(attributeClass.TypeParameters, attributeSyntax.Name);

            TypeArgumentsParser parser = new(recorder, attributeClass, attributeSyntax, logger);

            return parser.TryParse();
        }

        private readonly ISyntacticRecorder Recorder;
        private readonly IReadOnlyList<ITypeParameterSymbol> Parameters;
        private readonly NameSyntax AttributeNameSyntax;

        private readonly ISyntacticParserLogger Logger;

        private TypeArgumentsParser(ISyntacticRecorder recorder, INamedTypeSymbol attributeClass, AttributeSyntax attributeSyntax, ISyntacticParserLogger logger)
        {
            Recorder = recorder;
            Parameters = attributeClass.TypeParameters;
            AttributeNameSyntax = attributeSyntax.Name;

            Logger = logger;
        }

        private bool TryParse()
        {
            if (Parameters.Count is 0)
            {
                return true;
            }

            if (GetGenericNameSyntax() is not GenericNameSyntax genericNameSyntax)
            {
                Logger.TypeArgument.SyntaxNotRecognizedAsGenericExpression();

                return false;
            }

            using var __ = Logger.TypeArgument.BeginScopeRecognizedGenericExpression(genericNameSyntax);

            if (Parameters.Count != genericNameSyntax.TypeArgumentList.Arguments.Count)
            {
                Logger.TypeArgument.UnexpectedNumberOfSyntacticTypeArguments();

                return false;
            }

            if (Enumerable.Range(0, Parameters.Count).All((index) => TryParseArgument(Parameters[index], genericNameSyntax.TypeArgumentList.Arguments[index])) is false)
            {
                return false;
            }

            return true;
        }

        private bool TryParseArgument(ITypeParameterSymbol parameter, TypeSyntax syntax)
        {
            using var _ = Logger.TypeArgument.BeginScopeParsingTypeArgument(parameter, syntax);

            return Recorder.TypeArgument.TryRecordArgument(parameter, syntax);
        }

        private GenericNameSyntax? GetGenericNameSyntax()
        {
            if (AttributeNameSyntax is GenericNameSyntax genericNameSyntax)
            {
                return genericNameSyntax;
            }

            if (AttributeNameSyntax is QualifiedNameSyntax qualifiedNameSyntax && qualifiedNameSyntax.Right is GenericNameSyntax qualifiedGenericNameSyntax)
            {
                return qualifiedGenericNameSyntax;
            }

            if (AttributeNameSyntax is AliasQualifiedNameSyntax aliasQualifiedNameSyntax && aliasQualifiedNameSyntax.Name is GenericNameSyntax aliasQualifiedGenericNameSyntax)
            {
                return aliasQualifiedGenericNameSyntax;
            }

            return null;
        }
    }

    private sealed class ConstructorArgumentsParser
    {
        public static bool TryParse(ISyntacticRecorder recorder, IMethodSymbol targetConstructor, AttributeData attributeData, AttributeSyntax attributeSyntax, ISyntacticParserLogger logger)
        {
            using var _ = logger.ConstructorArgument.BeginScopeParsingConstructorArguments(attributeData, attributeSyntax);

            ConstructorArgumentsParser parser = new(recorder, targetConstructor, attributeData, attributeSyntax, logger);

            return parser.TryParse();
        }

        private readonly ISyntacticRecorder Recorder;
        private readonly IReadOnlyList<IParameterSymbol> Parameters;
        private readonly AttributeData AttributeData;

        private readonly ParsingData Data;

        private readonly ISyntacticParserLogger Logger;

        private ConstructorArgumentsParser(ISyntacticRecorder recorder, IMethodSymbol targetConstructor, AttributeData attributeData, AttributeSyntax attributeSyntax, ISyntacticParserLogger logger)
        {
            Recorder = recorder;
            Parameters = targetConstructor.Parameters;
            AttributeData = attributeData;

            Data = ParsingData.Create(targetConstructor, attributeData, attributeSyntax);

            Logger = logger;
        }

        private bool TryParse()
        {
            if (Data.OutOfOrderLabelledArgumentFollowedByUnlabelled)
            {
                Logger.ConstructorArgument.OutOfOrderLabelledConstructorArgumentsFollowedByUnlabelled();

                return false;
            }

            if (Data.MissingRequiredArgument)
            {
                Logger.ConstructorArgument.MissingOneOrMoreRequiredConstructorArgument();

                return false;
            }

            if (Parameters.Count is 0)
            {
                return true;
            }

            if (Parameters.Count != AttributeData.ConstructorArguments.Length)
            {
                Logger.ConstructorArgument.UnexpectedNumberOfSemanticConstructorArguments();

                return false;
            }

            return Parameters.All(TryParseArgument);
        }

        private bool TryParseArgument(IParameterSymbol parameter)
        {
            using var _ = Logger.ConstructorArgument.BeginScopeParsingConstructorArgument(parameter);

            if (Data.GetNormalArgumentExpression(parameter) is ExpressionSyntax syntax)
            {
                using var __ = Logger.ConstructorArgument.BeginScopeParsingNormalConstructorArgument(syntax);

                return Recorder.ConstructorArgument.TryRecordArgument(parameter, syntax);
            }

            if (parameter.IsParams)
            {
                var elementSyntax = Data.GetParamsArgumentExpressions();

                using var __ = Logger.ConstructorArgument.BeginScopeParsingParamsConstructorArgument(elementSyntax);

                return Recorder.ConstructorArgument.TryRecordParamsArgument(parameter, elementSyntax);
            }

            using var ___ = Logger.ConstructorArgument.BeginScopeParsingDefaultConstructorArgument();

            return Recorder.ConstructorArgument.TryRecordDefaultArgument(parameter);
        }

        private sealed class ParsingData
        {
            public static ParsingData Create(IMethodSymbol targetConstructor, AttributeData attributeData, AttributeSyntax attributeSyntax)
            {
                ParsingData data = new(attributeData, targetConstructor.Parameters);

                if (attributeSyntax.ArgumentList is null)
                {
                    data.SetMissingRequiredArguments();

                    return data;
                }

                for (var i = 0; i < Math.Min(attributeSyntax.ArgumentList.Arguments.Count, targetConstructor.Parameters.Length); i++)
                {
                    data.ParseArgument(i, attributeSyntax.ArgumentList.Arguments);

                    if (data.OutOfOrderLabelledArgumentFollowedByUnlabelled)
                    {
                        break;
                    }
                }

                data.SetMissingRequiredArguments();

                return data;
            }

            private readonly AttributeData AttributeData;
            private readonly IReadOnlyList<IParameterSymbol> Parameters;

            private bool HasEncounteredOutOfOrderLabelledArgument;

            public bool MissingRequiredArgument { get; private set; }
            public bool OutOfOrderLabelledArgumentFollowedByUnlabelled { get; private set; }

            private readonly IDictionary<string, IParameterSymbol> UnparsedParameters;

            private readonly IDictionary<IParameterSymbol, ExpressionSyntax> NormalArguments = new Dictionary<IParameterSymbol, ExpressionSyntax>(SymbolEqualityComparer.Default);
            private IReadOnlyList<ExpressionSyntax>? ParamsArgument;

            public ParsingData(AttributeData attributeData, IReadOnlyList<IParameterSymbol> parameters)
            {
                AttributeData = attributeData;
                Parameters = parameters;

                UnparsedParameters = GetParametersByName(parameters);
            }

            private void SetMissingRequiredArguments() => MissingRequiredArgument = UnparsedParameters.Values.Any(static (parameter) => parameter.IsOptional is false && parameter.IsParams is false);

            public ExpressionSyntax? GetNormalArgumentExpression(IParameterSymbol parameter)
            {
                if (NormalArguments.TryGetValue(parameter, out var argumentSyntax))
                {
                    return argumentSyntax;
                }

                return null;
            }

            public IReadOnlyList<ExpressionSyntax> GetParamsArgumentExpressions() => ParamsArgument ?? Array.Empty<ExpressionSyntax>();

            private void ParseArgument(int index, IReadOnlyList<AttributeArgumentSyntax> syntacticArguments)
            {
                if (syntacticArguments[index].NameEquals is not null)
                {
                    return;
                }

                if (syntacticArguments[index].NameColon is NameColonSyntax nameColonSyntax)
                {
                    ParseNameColonArgument(index, syntacticArguments, nameColonSyntax);

                    return;
                }

                if (HasEncounteredOutOfOrderLabelledArgument)
                {
                    OutOfOrderLabelledArgumentFollowedByUnlabelled = true;

                    return;
                }

                if (UnparsedParameters.Remove(Parameters[index].Name) is false)
                {
                    return;
                }

                if (Parameters[index].IsParams)
                {
                    ParseParamsArgument(index, syntacticArguments);

                    return;
                }

                NormalArguments.Add(Parameters[index], syntacticArguments[index].Expression);
            }

            private void ParseNameColonArgument(int index, IReadOnlyList<AttributeArgumentSyntax> syntacticArguments, NameColonSyntax nameColonSyntax)
            {
                if (nameColonSyntax.Name.Identifier.Text != Parameters[index].Name)
                {
                    HasEncounteredOutOfOrderLabelledArgument = true;
                }

                if (UnparsedParameters.TryGetValue(nameColonSyntax.Name.Identifier.Text, out var parameter))
                {
                    UnparsedParameters.Remove(nameColonSyntax.Name.Identifier.Text);

                    NormalArguments.Add(parameter, syntacticArguments[index].Expression);
                }
            }

            private void ParseParamsArgument(int index, IReadOnlyList<AttributeArgumentSyntax> syntacticArguments)
            {
                if (syntacticArguments.Count > index + 1 && syntacticArguments[index + 1].NameEquals is null)
                {
                    ParamsArgument = CollectParamsArgumentExpressions(index, syntacticArguments);

                    return;
                }

                if (DoesSyntaxMatchArrayStructure(AttributeData.ConstructorArguments[index], syntacticArguments[index].Expression))
                {
                    NormalArguments.Add(Parameters[index], syntacticArguments[index].Expression);
                }

                ParamsArgument = new[] { syntacticArguments[index].Expression };
            }

            private static IReadOnlyList<ExpressionSyntax> CollectParamsArgumentExpressions(int index, IReadOnlyList<AttributeArgumentSyntax> syntacticArguments)
            {
                var paramsSyntacticArguments = new List<ExpressionSyntax>(syntacticArguments.Count - index);

                foreach (var syntacticArgument in syntacticArguments.Skip(index))
                {
                    if (syntacticArgument.NameEquals is not null || syntacticArgument.NameColon is not null)
                    {
                        break;
                    }

                    paramsSyntacticArguments.Add(syntacticArgument.Expression);
                }

                return paramsSyntacticArguments;
            }

            private static IDictionary<string, IParameterSymbol> GetParametersByName(IReadOnlyList<IParameterSymbol> parameters)
            {
                Dictionary<string, IParameterSymbol> parametersByName = new(parameters.Count);

                foreach (var parameter in parameters)
                {
                    parametersByName.Add(parameter.Name, parameter);
                }

                return parametersByName;
            }

            private static bool DoesSyntaxMatchArrayStructure(TypedConstant value, ExpressionSyntax expression) => expression switch
            {
                ArrayCreationExpressionSyntax arrayCreationExpression => DoesSyntaxMatchArrayStructure(value, arrayCreationExpression),
                InitializerExpressionSyntax initializerExpression => DoesSyntaxMatchArrayStructure(value, initializerExpression),
                ImplicitArrayCreationExpressionSyntax implicitArrayCreationExpression => DoesSyntaxMatchArrayStructure(value, implicitArrayCreationExpression.Initializer),
                CastExpressionSyntax castExpression => DoesSyntaxMatchArrayStructure(value, castExpression.Expression),
                ParenthesizedExpressionSyntax parenthesizedExpression => DoesSyntaxMatchArrayStructure(value, parenthesizedExpression.Expression),
                LiteralExpressionSyntax literalExpression => DoesSyntaxMatchArrayStructure(value, literalExpression),
                DefaultExpressionSyntax => DoesSyntaxMatchArrayStructure(value),
                _ => false
            };

            private static bool DoesSyntaxMatchArrayStructure(TypedConstant value, ArrayCreationExpressionSyntax syntax)
            {
                if (value.Kind is not TypedConstantKind.Array)
                {
                    return false;
                }

                if (value.Values.IsDefault)
                {
                    return false;
                }

                if (syntax.Initializer is null)
                {
                    return value.Values.Length is 0;
                }

                return DoesSyntaxMatchArrayStructure(value, syntax.Initializer);
            }

            private static bool DoesSyntaxMatchArrayStructure(TypedConstant value, InitializerExpressionSyntax syntax)
            {
                if (syntax.IsKind(SyntaxKind.ArrayInitializerExpression) is false)
                {
                    return value.Kind is not TypedConstantKind.Array;
                }

                if (value.Values.IsDefault)
                {
                    return false;
                }

                if (syntax.Expressions.Count is 0)
                {
                    return value.Values.Length is 0;
                }

                if (value.Values.Length != syntax.Expressions.Count)
                {
                    return false;
                }

                return DoesSyntaxMatchArrayStructure(value.Values[0], syntax.Expressions[0]);
            }

            private static bool DoesSyntaxMatchArrayStructure(TypedConstant value, LiteralExpressionSyntax syntax)
            {
                if (syntax.IsKind(SyntaxKind.NullLiteralExpression) || syntax.IsKind(SyntaxKind.DefaultLiteralExpression))
                {
                    if (value.Kind is TypedConstantKind.Array)
                    {
                        return value.Values.IsDefault;
                    }

                    return true;
                }

                return value.Kind is not TypedConstantKind.Array;
            }

            private static bool DoesSyntaxMatchArrayStructure(TypedConstant value)
            {
                return value.Kind is TypedConstantKind.Array && value.Values.IsDefault;
            }
        }
    }

    private sealed class NamedArgumentsParser
    {
        public static bool TryParse(ISyntacticRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax, ISyntacticParserLogger logger)
        {
            using var _ = logger.NamedArgument.BeginScopeParsingNamedArguments(attributeData, attributeSyntax);

            NamedArgumentsParser parser = new(recorder, attributeData, attributeSyntax, logger);

            return parser.TryParse();
        }

        private readonly ISyntacticRecorder Recorder;

        private readonly IReadOnlyDictionary<string, ExpressionSyntax> ParameterSyntaxPairs;

        private readonly ISyntacticParserLogger Logger;

        public NamedArgumentsParser(ISyntacticRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax, ISyntacticParserLogger logger)
        {
            Recorder = recorder;

            ParameterSyntaxPairs = CreateParameterSyntaxPairDictionary(attributeData, attributeSyntax);

            Logger = logger;
        }

        private bool TryParse()
        {
            if (ParameterSyntaxPairs.Count is 0)
            {
                return true;
            }

            return ParameterSyntaxPairs.All((pair) => TryParseArgument(pair.Key, pair.Value));
        }

        private bool TryParseArgument(string parameterName, ExpressionSyntax syntax)
        {
            using var _ = Logger.NamedArgument.BeginScopeParsingNamedArgument(parameterName, syntax);

            return Recorder.NamedArgument.TryRecordArgument(parameterName, syntax);
        }

        private static IReadOnlyDictionary<string, ExpressionSyntax> CreateParameterSyntaxPairDictionary(AttributeData attributeData, AttributeSyntax attributeSyntax)
        {
            HashSet<string> namedParameters = new(attributeData.NamedArguments.Select(static (namedArgument) => namedArgument.Key));

            if (attributeSyntax.ArgumentList is null)
            {
                return new Dictionary<string, ExpressionSyntax>();
            }

            Dictionary<string, ExpressionSyntax> dictionary = new(attributeSyntax.ArgumentList.Arguments.Count);

            foreach (var argumentSyntax in attributeSyntax.ArgumentList.Arguments)
            {
                if (argumentSyntax.NameEquals is NameEqualsSyntax nameEqualsSyntax && namedParameters.Remove(nameEqualsSyntax.Name.Identifier.Text))
                {
                    dictionary.Add(nameEqualsSyntax.Name.Identifier.Text, argumentSyntax.Expression);
                }
            }

            return dictionary;
        }
    }
}
