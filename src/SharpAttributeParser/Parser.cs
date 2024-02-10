namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser.Logging;

using System;
using System.Collections.Generic;
using System.Linq;

/// <inheritdoc cref="IParser"/>
public sealed class Parser : IParser
{
    private readonly ISemanticParser SemanticParser;
    private readonly ISyntacticParser SyntacticParser;

    private readonly IParserLogger Logger;

    /// <summary>Instantiates a <see cref="Parser"/>, parsing the arguments of attributes.</summary>
    /// <param name="logger">The logger used to log messages.</param>
    public Parser(IParserLogger<Parser>? logger = null)
    {
        SemanticParser = new SemanticParser();
        SyntacticParser = new SyntacticParser();

        Logger = logger ?? NullParserLogger<Parser>.Instance;
    }

    /// <summary>Instantiates a <see cref="Parser"/>, parsing the arguments of attributes.</summary>
    /// <param name="semanticParser">The parser used to semantically parse attribute arguments.</param>
    /// <param name="syntacticParser">The parser used to syntactically parse attribute arguments.</param>
    /// <param name="logger">The logger used to log messages.</param>
    public Parser(ISemanticParser semanticParser, ISyntacticParser syntacticParser, IParserLogger<Parser>? logger = null)
    {
        SemanticParser = semanticParser ?? throw new ArgumentNullException(nameof(semanticParser));
        SyntacticParser = syntacticParser ?? throw new ArgumentNullException(nameof(syntacticParser));

        Logger = logger ?? NullParserLogger<Parser>.Instance;
    }

    /// <inheritdoc/>
    public bool TryParse(IRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax)
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

        SemanticRecorder semanticRecorder = new();
        SyntacticRecorder syntacticRecorder = new();

        if (SemanticParser.TryParse(semanticRecorder, attributeData) is false)
        {
            Logger.SemanticParserFailedToParseAttribute();

            return false;
        }

        if (SyntacticParser.TryParse(syntacticRecorder, attributeData, attributeSyntax) is false)
        {
            Logger.SyntacticParserFailedToParseAttribute();

            return false;
        }

        return PropagateRecordedArguments(recorder, semanticRecorder, syntacticRecorder);
    }

    private bool PropagateRecordedArguments(IRecorder recorder, SemanticRecorder semanticRecorder, SyntacticRecorder syntacticRecorder)
    {
        return PropagateRecordedTypeArguments(recorder, semanticRecorder, syntacticRecorder)
            && PropagateRecordedConstructorArguments(recorder, semanticRecorder, syntacticRecorder)
            && PropagateRecordedNamedArguments(recorder, semanticRecorder, syntacticRecorder);
    }

    private bool PropagateRecordedTypeArguments(IRecorder recorder, SemanticRecorder semanticRecorder, SyntacticRecorder syntacticRecorder)
    {
        using var _ = Logger.Type.BeginScopePropagateParsedTypeArguments(semanticRecorder.TypeArguments, syntacticRecorder.TypeArguments);

        if (semanticRecorder.TypeArguments.Count != syntacticRecorder.TypeArguments.Count)
        {
            Logger.Type.DifferentNumberOfSemanticallyAndSyntacticallyParsedTypeArguments();

            return false;
        }

        return semanticRecorder.TypeArguments.All((argument) => PropagateRecordedTypeArgument(recorder, syntacticRecorder, argument.Key, argument.Value));
    }

    private bool PropagateRecordedTypeArgument(IRecorder recorder, SyntacticRecorder syntacticRecorder, ITypeParameterSymbol parameter, ITypeSymbol semanticArgument)
    {
        using var _ = Logger.Type.BeginScopePropagateSemanticallyParsedTypeArgument(parameter, semanticArgument);

        if (syntacticRecorder.TypeArguments.TryGetValue(parameter, out var syntacticArgument) is false)
        {
            Logger.Type.TypeArgumentOnlyRecordedSemantically();

            return false;
        }

        using var __ = Logger.Type.BeginScopePropagateSyntacticallyParsedTypeArgument(syntacticArgument);

        return recorder.Type.TryRecordArgument(parameter, semanticArgument, syntacticArgument);
    }

    private bool PropagateRecordedConstructorArguments(IRecorder recorder, SemanticRecorder semanticRecorder, SyntacticRecorder syntacticRecorder)
    {
        using var _ = Logger.Constructor.BeginScopePropagateParsedConstructorArguments(semanticRecorder.ConstructorArguments, syntacticRecorder.NormalConstructorArguments, syntacticRecorder.ParamsConstructorArguments, syntacticRecorder.DefaultConstructorArguments);

        if (semanticRecorder.ConstructorArguments.Count != syntacticRecorder.NormalConstructorArguments.Count + syntacticRecorder.ParamsConstructorArguments.Count + syntacticRecorder.DefaultConstructorArguments.Count)
        {
            Logger.Constructor.DifferentNumberOfSemanticallyAndSyntacticallyParsedConstructorArguments();

            return false;
        }

        return semanticRecorder.ConstructorArguments.All((argument) => PropagateRecordedConstructorArgument(recorder, syntacticRecorder, argument.Key, argument.Value));
    }

    private bool PropagateRecordedConstructorArgument(IRecorder recorder, SyntacticRecorder syntacticRecorder, IParameterSymbol parameter, object? semanticArgument)
    {
        using var _ = Logger.Constructor.BeginScopePropagateSemanticallyParsedConstructorArgument(parameter, semanticArgument);

        if (syntacticRecorder.NormalConstructorArguments.TryGetValue(parameter, out var syntacticNormalArgument))
        {
            using var __ = Logger.Constructor.BeginScopePropagateSyntacticallyParsedNormalConstructorArgument(syntacticNormalArgument);

            return recorder.Constructor.Normal.TryRecordArgument(parameter, semanticArgument, syntacticNormalArgument);
        }

        if (syntacticRecorder.ParamsConstructorArguments.TryGetValue(parameter, out var syntacticParamsArguments))
        {
            using var __ = Logger.Constructor.BeginScopePropagateSyntacticallyParsedParamsConstructorArgument(syntacticParamsArguments);

            return recorder.Constructor.Params.TryRecordArgument(parameter, semanticArgument, syntacticParamsArguments);
        }

        if (syntacticRecorder.DefaultConstructorArguments.Contains(parameter))
        {
            using var __ = Logger.Constructor.BeginScopePropagateSyntacticallyParsedDefaultConstructorArgument();

            return recorder.Constructor.Default.TryRecordArgument(parameter, semanticArgument);
        }

        Logger.Constructor.ConstructorArgumentOnlyRecordedSemantically();

        return false;
    }

    private bool PropagateRecordedNamedArguments(IRecorder recorder, SemanticRecorder semanticRecorder, SyntacticRecorder syntacticRecorder)
    {
        using var _ = Logger.Named.BeginScopePropagateParsedNamedArguments(semanticRecorder.NamedArguments, syntacticRecorder.NamedArguments);

        if (semanticRecorder.NamedArguments.Count != syntacticRecorder.NamedArguments.Count)
        {
            Logger.Named.DifferentNumberOfSemanticallyAndSyntacticallyParsedNamedArguments();

            return false;
        }

        return semanticRecorder.NamedArguments.All((argument) => PropagateRecordedNamedArgument(recorder, syntacticRecorder, argument.Key, argument.Value));
    }

    private bool PropagateRecordedNamedArgument(IRecorder recorder, SyntacticRecorder syntacticRecorder, string parameterName, object? semanticArgument)
    {
        using var _ = Logger.Named.BeginScopePropagateSemanticallyParsedNamedArgument(parameterName, semanticArgument);

        if (syntacticRecorder.NamedArguments.TryGetValue(parameterName, out var syntacticArgument) is false)
        {
            return false;
        }

        using var __ = Logger.Named.BeginScopePropagateSyntacticallyParsedNamedArgument(syntacticArgument);

        return recorder.Named.TryRecordArgument(parameterName, semanticArgument, syntacticArgument);
    }

    private sealed class SemanticRecorder : ISemanticRecorder
    {
        public IReadOnlyDictionary<ITypeParameterSymbol, ITypeSymbol> TypeArguments => Type.GetArguments();
        public IReadOnlyDictionary<IParameterSymbol, object?> ConstructorArguments => Constructor.GetArguments();
        public IReadOnlyDictionary<string, object?> NamedArguments => Named.GetArguments();

        private readonly TypeRecorder Type = new();
        private readonly ConstructorRecorder Constructor = new();
        private readonly NamedRecorder Named = new();

        ISemanticTypeRecorder ISemanticRecorder.Type => Type;
        ISemanticConstructorRecorder ISemanticRecorder.Constructor => Constructor;
        ISemanticNamedRecorder ISemanticRecorder.Named => Named;

        private sealed class TypeRecorder : ISemanticTypeRecorder
        {
            private readonly Dictionary<ITypeParameterSymbol, ITypeSymbol> Arguments = new(SymbolEqualityComparer.Default);

            bool ISemanticTypeRecorder.TryRecordArgument(ITypeParameterSymbol parameter, ITypeSymbol argument)
            {
                Arguments[parameter] = argument;

                return true;
            }

            public IReadOnlyDictionary<ITypeParameterSymbol, ITypeSymbol> GetArguments() => Arguments;
        }

        private sealed class ConstructorRecorder : ISemanticConstructorRecorder
        {
            private readonly Dictionary<IParameterSymbol, object?> Arguments = new(SymbolEqualityComparer.Default);

            bool ISemanticConstructorRecorder.TryRecordArgument(IParameterSymbol parameter, object? argument)
            {
                Arguments[parameter] = argument;

                return true;
            }

            public IReadOnlyDictionary<IParameterSymbol, object?> GetArguments() => Arguments;
        }

        private sealed class NamedRecorder : ISemanticNamedRecorder
        {
            private readonly Dictionary<string, object?> Arguments = [];

            bool ISemanticNamedRecorder.TryRecordArgument(string parameterName, object? argument)
            {
                Arguments[parameterName] = argument;

                return true;
            }

            public IReadOnlyDictionary<string, object?> GetArguments() => Arguments;
        }
    }

    private sealed class SyntacticRecorder : ISyntacticRecorder
    {
        public IReadOnlyDictionary<ITypeParameterSymbol, ExpressionSyntax> TypeArguments => Type.GetArguments();
        public IReadOnlyDictionary<IParameterSymbol, ExpressionSyntax> NormalConstructorArguments => Constructor.GetNormalArguments();
        public IReadOnlyDictionary<IParameterSymbol, IReadOnlyList<ExpressionSyntax>> ParamsConstructorArguments => Constructor.GetParamsArguments();
        public ISet<IParameterSymbol> DefaultConstructorArguments => Constructor.GetDefaultArguments();
        public IReadOnlyDictionary<string, ExpressionSyntax> NamedArguments => Named.GetArguments();

        private readonly TypeRecorder Type = new();
        private readonly ConstructorRecorder Constructor = new();
        private readonly NamedRecorder Named = new();

        ISyntacticTypeRecorder ISyntacticRecorder.Type => Type;
        ISyntacticConstructorRecorder ISyntacticRecorder.Constructor => Constructor;
        ISyntacticNamedRecorder ISyntacticRecorder.Named => Named;

        private sealed class TypeRecorder : ISyntacticTypeRecorder
        {
            private readonly Dictionary<ITypeParameterSymbol, ExpressionSyntax> Arguments = new(SymbolEqualityComparer.Default);

            bool ISyntacticTypeRecorder.TryRecordArgument(ITypeParameterSymbol parameter, ExpressionSyntax syntax)
            {
                Arguments[parameter] = syntax;

                return true;
            }

            public IReadOnlyDictionary<ITypeParameterSymbol, ExpressionSyntax> GetArguments() => Arguments;
        }

        private sealed class ConstructorRecorder : ISyntacticConstructorRecorder
        {
            private readonly NormalConstructorRecorder Normal = new();
            private readonly ParamsConstructorRecorder Params = new();
            private readonly DefaultConstructorRecorder Default = new();

            ISyntacticNormalConstructorRecorder ISyntacticConstructorRecorder.Normal => Normal;
            ISyntacticParamsConstructorRecorder ISyntacticConstructorRecorder.Params => Params;
            ISyntacticDefaultConstructorRecorder ISyntacticConstructorRecorder.Default => Default;

            public IReadOnlyDictionary<IParameterSymbol, ExpressionSyntax> GetNormalArguments() => Normal.GetArguments();
            public IReadOnlyDictionary<IParameterSymbol, IReadOnlyList<ExpressionSyntax>> GetParamsArguments() => Params.GetArguments();
            public ISet<IParameterSymbol> GetDefaultArguments() => Default.GetArguments();

            private sealed class NormalConstructorRecorder : ISyntacticNormalConstructorRecorder
            {
                private readonly Dictionary<IParameterSymbol, ExpressionSyntax> Arguments = new(SymbolEqualityComparer.Default);

                bool ISyntacticNormalConstructorRecorder.TryRecordArgument(IParameterSymbol parameter, ExpressionSyntax syntax)
                {
                    Arguments[parameter] = syntax;

                    return true;
                }

                public IReadOnlyDictionary<IParameterSymbol, ExpressionSyntax> GetArguments() => Arguments;
            }

            private sealed class ParamsConstructorRecorder : ISyntacticParamsConstructorRecorder
            {
                private readonly Dictionary<IParameterSymbol, IReadOnlyList<ExpressionSyntax>> Arguments = new(SymbolEqualityComparer.Default);

                bool ISyntacticParamsConstructorRecorder.TryRecordArgument(IParameterSymbol parameter, IReadOnlyList<ExpressionSyntax> elementSyntax)
                {
                    Arguments[parameter] = elementSyntax;

                    return true;
                }

                public IReadOnlyDictionary<IParameterSymbol, IReadOnlyList<ExpressionSyntax>> GetArguments() => Arguments;
            }

            private sealed class DefaultConstructorRecorder : ISyntacticDefaultConstructorRecorder
            {
                private readonly ISet<IParameterSymbol> Arguments = new HashSet<IParameterSymbol>(SymbolEqualityComparer.Default);

                bool ISyntacticDefaultConstructorRecorder.TryRecordArgument(IParameterSymbol parameter)
                {
                    Arguments.Add(parameter);

                    return true;
                }

                public ISet<IParameterSymbol> GetArguments() => Arguments;
            }
        }

        private sealed class NamedRecorder : ISyntacticNamedRecorder
        {
            private readonly Dictionary<string, ExpressionSyntax> Arguments = [];

            bool ISyntacticNamedRecorder.TryRecordArgument(string parameterName, ExpressionSyntax syntax)
            {
                Arguments[parameterName] = syntax;

                return true;
            }

            public IReadOnlyDictionary<string, ExpressionSyntax> GetArguments() => Arguments;
        }
    }
}
