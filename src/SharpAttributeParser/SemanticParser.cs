namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using SharpAttributeParser.Logging;

using System;
using System.Collections.Generic;
using System.Linq;

/// <inheritdoc cref="ISemanticParser"/>
public sealed class SemanticParser : ISemanticParser
{
    private readonly ISemanticParserLogger Logger;

    /// <summary>Instantiates a <see cref="SyntacticParser"/>, parsing the arguments of attributes.</summary>
    /// <param name="logger">The logger used to log messages.</param>
    public SemanticParser(ISemanticParserLogger<SemanticParser>? logger = null)
    {
        Logger = logger ?? NullSemanticParserLogger<SemanticParser>.Singleton;
    }

    /// <inheritdoc/>
    public bool TryParse(ISemanticRecorder recorder, AttributeData attributeData)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        if (attributeData is null)
        {
            throw new ArgumentNullException(nameof(attributeData));
        }

        using var _ = Logger.BeginScopeParsingAttribute(recorder.GetType(), attributeData);

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

        return TryParse(recorder, attributeClass, targetConstructor, attributeData);
    }

    private bool TryParse(ISemanticRecorder recorder, INamedTypeSymbol attributeClass, IMethodSymbol targetConstructor, AttributeData attributeData)
    {
        return TypeArgumentsParser.TryParse(recorder, attributeClass, Logger)
            && ConstructorArgumentsParser.TryParse(recorder, targetConstructor, attributeData, Logger)
            && NamedArgumentsParser.TryParse(recorder, attributeData, Logger);
    }

    private sealed class TypeArgumentsParser
    {
        public static bool TryParse(ISemanticRecorder recorder, INamedTypeSymbol attributeClass, ISemanticParserLogger logger)
        {
            TypeArgumentsParser parser = new(recorder, attributeClass, logger);

            return parser.TryParse();
        }

        private readonly ISemanticRecorder Recorder;
        private readonly IReadOnlyList<ITypeParameterSymbol> Parameters;
        private readonly IReadOnlyList<ITypeSymbol> Arguments;

        private readonly ISemanticParserLogger Logger;

        private TypeArgumentsParser(ISemanticRecorder recorder, INamedTypeSymbol attributeClass, ISemanticParserLogger logger)
        {
            Recorder = recorder;
            Parameters = attributeClass.TypeParameters;
            Arguments = attributeClass.TypeArguments;

            Logger = logger;
        }

        private bool TryParse()
        {
            using var _ = Logger.TypeArgument.BeginScopeParsingTypeArguments(Parameters, Arguments);

            if (Parameters.Count is 0)
            {
                return true;
            }

            if (Parameters.Count != Arguments.Count)
            {
                Logger.TypeArgument.UnexpectedNumberOfTypeArguments();

                return false;
            }

            return Enumerable.Range(0, Parameters.Count).All((index) => TryParseArgument(Parameters[index], Arguments[index]));
        }

        private bool TryParseArgument(ITypeParameterSymbol parameter, ITypeSymbol argument)
        {
            using var _ = Logger.TypeArgument.BeginScopeParsingTypeArgument(parameter, argument);

            if (argument.Kind is SymbolKind.ErrorType)
            {
                Logger.TypeArgument.InvalidTypeArgument();

                return false;
            }

            return Recorder.TypeArgument.TryRecordArgument(parameter, argument);
        }
    }

    private sealed class ConstructorArgumentsParser
    {
        public static bool TryParse(ISemanticRecorder recorder, IMethodSymbol targetConstructor, AttributeData attributeData, ISemanticParserLogger logger)
        {
            ConstructorArgumentsParser parser = new(recorder, targetConstructor, attributeData, logger);

            return parser.TryParse();
        }

        private readonly ISemanticRecorder Recorder;
        private readonly IReadOnlyList<IParameterSymbol> Parameters;
        private readonly IReadOnlyList<TypedConstant> Arguments;

        private readonly ISemanticParserLogger Logger;

        private ConstructorArgumentsParser(ISemanticRecorder recorder, IMethodSymbol targetConstructor, AttributeData attributeData, ISemanticParserLogger logger)
        {
            Recorder = recorder;
            Parameters = targetConstructor.Parameters;
            Arguments = attributeData.ConstructorArguments;

            Logger = logger;
        }

        private bool TryParse()
        {
            using var _ = Logger.ConstructorArgument.BeginScopeParsingConstructorArguments(Parameters, Arguments);

            if (Arguments.Count is 0)
            {
                return true;
            }

            if (Arguments.Count != Parameters.Count())
            {
                Logger.ConstructorArgument.UnexpectedNumberOfConstructorArguments();

                return false;
            }

            return Enumerable.Range(0, Arguments.Count).All((index) => TryParseArgument(Parameters[index], Arguments[index]));
        }

        private bool TryParseArgument(IParameterSymbol parameter, TypedConstant argument)
        {
            using var _ = Logger.ConstructorArgument.BeginScopeParsingConstructorArgument(parameter, argument);

            if (argument.Kind is TypedConstantKind.Error)
            {
                Logger.ConstructorArgument.InvalidConstructorArgument();

                return false;
            }

            if (argument.Kind is TypedConstantKind.Array)
            {
                return Recorder.ConstructorArgument.TryRecordArgument(parameter, ParseArrayArguments(argument));
            }

            return Recorder.ConstructorArgument.TryRecordArgument(parameter, argument.Value);
        }
    }

    private sealed class NamedArgumentsParser
    {
        public static bool TryParse(ISemanticRecorder recorder, AttributeData attributeData, ISemanticParserLogger logger)
        {
            NamedArgumentsParser parser = new(recorder, attributeData, logger);

            return parser.TryParse();
        }

        private readonly ISemanticRecorder Recorder;
        private readonly IReadOnlyList<KeyValuePair<string, TypedConstant>> Arguments;

        private readonly ISemanticParserLogger Logger;

        private NamedArgumentsParser(ISemanticRecorder recorder, AttributeData attributeData, ISemanticParserLogger logger)
        {
            Recorder = recorder;
            Arguments = attributeData.NamedArguments;

            Logger = logger;
        }

        private bool TryParse()
        {
            Logger.NamedArgument.BeginScopeParsingNamedArguments(Arguments);

            if (Arguments.Count is 0)
            {
                return true;
            }

            return Enumerable.Range(0, Arguments.Count).All((index) => TryParseArgument(Arguments[index].Key, Arguments[index].Value));
        }

        private bool TryParseArgument(string parameterName, TypedConstant argument)
        {
            Logger.NamedArgument.BeginScopeParsingNamedArgument(parameterName, argument);

            if (argument.Kind is TypedConstantKind.Error)
            {
                Logger.NamedArgument.InvalidNamedArgument();

                return true;
            }

            if (argument.Kind is TypedConstantKind.Array)
            {
                return Recorder.NamedArgument.TryRecordArgument(parameterName, ParseArrayArguments(argument));
            }

            return Recorder.NamedArgument.TryRecordArgument(parameterName, argument.Value);
        }
    }

    private static object?[]? ParseArrayArguments(TypedConstant value)
    {
        if (value.IsNull)
        {
            return null;
        }

        if (value.Values.IsEmpty)
        {
            return [];
        }

        var arrayConstants = value.Values;
        var arrayValues = new object?[arrayConstants.Length];

        for (var i = 0; i < arrayConstants.Length; i++)
        {
            if (arrayConstants[i].Kind is TypedConstantKind.Array)
            {
                arrayValues[i] = ParseArrayArguments(arrayConstants[i]);

                continue;
            }

            arrayValues[i] = arrayConstants[i].Value;
        }

        return arrayValues;
    }
}
