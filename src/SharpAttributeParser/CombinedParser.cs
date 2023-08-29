namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser.Logging;

using System;
using System.Collections.Generic;
using System.Linq;

/// <inheritdoc cref="ICombinedParser"/>
public sealed class CombinedParser : ICombinedParser
{
    /// <summary>The singleton <see cref="CombinedParser"/>, with default behaviour.</summary>
    public static CombinedParser Singleton { get; } = new();

    private ISemanticParser SemanticParser { get; }
    private ISyntacticParser SyntacticParser { get; }

    private ICombinedParserLogger Logger { get; }

    /// <summary>Instantiates a <see cref="CombinedParser"/>, parsing the arguments of attributes.</summary>
    /// <param name="logger">The logger used to log messages.</param>
    public CombinedParser(ICombinedParserLogger<CombinedParser>? logger = null)
    {
        SemanticParser = new SemanticParser();
        SyntacticParser = new SyntacticParser();

        Logger = logger ?? NullCombinedParserLogger<CombinedParser>.Singleton;
    }

    /// <summary>Instantiates a <see cref="CombinedParser"/>, parsing the arguments of attributes.</summary>
    /// <param name="semanticParser">The parser used to semantically parse attribute arguments.</param>
    /// <param name="syntacticParser">The parser used to syntactically parse attribute arguments.</param>
    /// <param name="logger">The logger used to log messages.</param>
    /// <exception cref="ArgumentNullException"/>
    public CombinedParser(ISemanticParser semanticParser, ISyntacticParser syntacticParser, ICombinedParserLogger<CombinedParser>? logger = null)
    {
        SemanticParser = semanticParser ?? throw new ArgumentNullException(nameof(semanticParser));
        SyntacticParser = syntacticParser ?? throw new ArgumentNullException(nameof(syntacticParser));

        Logger = logger ?? NullCombinedParserLogger<CombinedParser>.Singleton;
    }

    /// <inheritdoc/>
    public bool TryParse(ICombinedRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax)
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

        SemanticArgumentRecorder semanticRecorder = new();
        SyntacticArgumentRecorder syntacticRecorder = new();

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

    private bool PropagateRecordedArguments(ICombinedRecorder recorder, SemanticArgumentRecorder semanticRecorder, SyntacticArgumentRecorder syntacticRecorder)
    {
        return PropagateRecordedTypeArguments(recorder, semanticRecorder, syntacticRecorder)
            && PropagateRecordedConstructorArguments(recorder, semanticRecorder, syntacticRecorder)
            && PropagateRecordedNamedArguments(recorder, semanticRecorder, syntacticRecorder);
    }

    private bool PropagateRecordedTypeArguments(ICombinedRecorder recorder, SemanticArgumentRecorder semanticRecorder, SyntacticArgumentRecorder syntacticRecorder)
    {
        using var _ = Logger.TypeArgument.BeginScopePropagateParsedTypeArguments(semanticRecorder.TypeArguments, syntacticRecorder.TypeArguments);

        if (semanticRecorder.TypeArguments.Count != syntacticRecorder.TypeArguments.Count)
        {
            Logger.TypeArgument.DifferentNumberOfSemanticallyAndSyntacticallyParsedTypeArguments();

            return false;
        }

        return semanticRecorder.TypeArguments.All((argument) => PropagateRecordedTypeArgument(recorder, syntacticRecorder, argument.Key, argument.Value));
    }

    private bool PropagateRecordedTypeArgument(ICombinedRecorder recorder, SyntacticArgumentRecorder syntacticRecorder, ITypeParameterSymbol parameter, ITypeSymbol semanticArgument)
    {
        using var _ = Logger.TypeArgument.BeginScopePropagateSemanticallyParsedTypeArgument(parameter, semanticArgument);

        if (syntacticRecorder.TypeArguments.TryGetValue(parameter, out var syntacticArgument) is false)
        {
            Logger.TypeArgument.TypeArgumentOnlyRecordedSemantically();

            return false;
        }

        using var __ = Logger.TypeArgument.BeginScopePropagateSyntacticallyParsedTypeArgument(syntacticArgument);

        return recorder.TypeArgument.TryRecordArgument(parameter, semanticArgument, syntacticArgument);
    }

    private bool PropagateRecordedConstructorArguments(ICombinedRecorder recorder, SemanticArgumentRecorder semanticRecorder, SyntacticArgumentRecorder syntacticRecorder)
    {
        using var _ = Logger.ConstructorArgument.BeginScopePropagateParsedConstructorArguments(semanticRecorder.ConstructorArguments, syntacticRecorder.NormalConstructorArguments, syntacticRecorder.ParamsConstructorArguments, syntacticRecorder.DefaultConstructorArguments);

        if (semanticRecorder.ConstructorArguments.Count != syntacticRecorder.NormalConstructorArguments.Count + syntacticRecorder.ParamsConstructorArguments.Count + syntacticRecorder.DefaultConstructorArguments.Count)
        {
            Logger.ConstructorArgument.DifferentNumberOfSemanticallyAndSyntacticallyParsedConstructorArguments();

            return false;
        }

        return semanticRecorder.ConstructorArguments.All((argument) => PropagateRecordedConstructorArgument(recorder, syntacticRecorder, argument.Key, argument.Value));
    }

    private bool PropagateRecordedConstructorArgument(ICombinedRecorder recorder, SyntacticArgumentRecorder syntacticRecorder, IParameterSymbol parameter, object? semanticArgument)
    {
        using var _ = Logger.ConstructorArgument.BeginScopePropagateSemanticallyParsedConstructorArgument(parameter, semanticArgument);

        if (syntacticRecorder.NormalConstructorArguments.TryGetValue(parameter, out var syntacticNormalArgument))
        {
            using var __ = Logger.ConstructorArgument.BeginScopePropagateSyntacticallyParsedNormalConstructorArgument(syntacticNormalArgument);

            return recorder.ConstructorArgument.TryRecordArgument(parameter, semanticArgument, syntacticNormalArgument);
        }

        if (syntacticRecorder.ParamsConstructorArguments.TryGetValue(parameter, out var syntacticParamsArguments))
        {
            using var __ = Logger.ConstructorArgument.BeginScopePropagateSyntacticallyParsedParamsConstructorArgument(syntacticParamsArguments);

            return recorder.ConstructorArgument.TryRecordParamsArgument(parameter, semanticArgument, syntacticParamsArguments);
        }

        if (syntacticRecorder.DefaultConstructorArguments.Contains(parameter))
        {
            using var __ = Logger.ConstructorArgument.BeginScopePropagateSyntacticallyParsedDefaultConstructorArgument();

            return recorder.ConstructorArgument.TryRecordDefaultArgument(parameter, semanticArgument);
        }

        Logger.ConstructorArgument.ConstructorArgumentOnlyRecordedSemantically();

        return false;
    }

    private bool PropagateRecordedNamedArguments(ICombinedRecorder recorder, SemanticArgumentRecorder semanticRecorder, SyntacticArgumentRecorder syntacticRecorder)
    {
        using var _ = Logger.NamedArgument.BeginScopePropagateParsedNamedArguments(semanticRecorder.NamedArguments, syntacticRecorder.NamedArguments);

        if (semanticRecorder.NamedArguments.Count != syntacticRecorder.NamedArguments.Count)
        {
            Logger.NamedArgument.DifferentNumberOfSemanticallyAndSyntacticallyParsedNamedArguments();

            return false;
        }

        return semanticRecorder.NamedArguments.All((argument) => PropagateRecordedNamedArgument(recorder, syntacticRecorder, argument.Key, argument.Value));
    }

    private bool PropagateRecordedNamedArgument(ICombinedRecorder recorder, SyntacticArgumentRecorder syntacticRecorder, string parameterName, object? semanticArgument)
    {
        using var _ = Logger.NamedArgument.BeginScopePropagateSemanticallyParsedNamedArgument(parameterName, semanticArgument);

        if (syntacticRecorder.NamedArguments.TryGetValue(parameterName, out var syntacticArgument) is false)
        {
            return false;
        }

        using var __ = Logger.NamedArgument.BeginScopePropagateSyntacticallyParsedNamedArgument(syntacticArgument);

        return recorder.NamedArgument.TryRecordArgument(parameterName, semanticArgument, syntacticArgument);
    }

    private sealed class SemanticArgumentRecorder : ISemanticRecorder
    {
        public IReadOnlyDictionary<ITypeParameterSymbol, ITypeSymbol> TypeArguments => TypeArgument.GetArguments();
        public IReadOnlyDictionary<IParameterSymbol, object?> ConstructorArguments => ConstructorArgument.GetArguments();
        public IReadOnlyDictionary<string, object?> NamedArguments => NamedArgument.GetArguments();

        private TypeArgumentRecorder TypeArgument { get; } = new();
        private ConstructorArgumentRecorder ConstructorArgument { get; } = new();
        private NamedArgumentRecorder NamedArgument { get; } = new();

        ISemanticTypeArgumentRecorder ISemanticRecorder.TypeArgument => TypeArgument;
        ISemanticConstructorArgumentRecorder ISemanticRecorder.ConstructorArgument => ConstructorArgument;
        ISemanticNamedArgumentRecorder ISemanticRecorder.NamedArgument => NamedArgument;

        private sealed class TypeArgumentRecorder : ISemanticTypeArgumentRecorder
        {
            private Dictionary<ITypeParameterSymbol, ITypeSymbol> Arguments { get; } = new(SymbolEqualityComparer.Default);

            bool ISemanticTypeArgumentRecorder.TryRecordArgument(ITypeParameterSymbol parameter, ITypeSymbol argument)
            {
                Arguments[parameter] = argument;

                return true;
            }

            public IReadOnlyDictionary<ITypeParameterSymbol, ITypeSymbol> GetArguments() => Arguments;
        }

        private sealed class ConstructorArgumentRecorder : ISemanticConstructorArgumentRecorder
        {
            private Dictionary<IParameterSymbol, object?> Arguments { get; } = new(SymbolEqualityComparer.Default);

            bool ISemanticConstructorArgumentRecorder.TryRecordArgument(IParameterSymbol parameter, object? argument)
            {
                Arguments[parameter] = argument;

                return true;
            }

            public IReadOnlyDictionary<IParameterSymbol, object?> GetArguments() => Arguments;
        }

        private sealed class NamedArgumentRecorder : ISemanticNamedArgumentRecorder
        {
            private Dictionary<string, object?> Arguments { get; } = new();

            bool ISemanticNamedArgumentRecorder.TryRecordArgument(string parameterName, object? argument)
            {
                Arguments[parameterName] = argument;

                return true;
            }

            public IReadOnlyDictionary<string, object?> GetArguments() => Arguments;
        }
    }

    private sealed class SyntacticArgumentRecorder : ISyntacticRecorder
    {
        public IReadOnlyDictionary<ITypeParameterSymbol, ExpressionSyntax> TypeArguments => TypeArgument.GetArguments();
        public IReadOnlyDictionary<IParameterSymbol, ExpressionSyntax> NormalConstructorArguments => ConstructorArgument.GetNormalArguments();
        public IReadOnlyDictionary<IParameterSymbol, IReadOnlyList<ExpressionSyntax>> ParamsConstructorArguments => ConstructorArgument.GetParamsArguments();
        public ISet<IParameterSymbol> DefaultConstructorArguments => ConstructorArgument.GetDefaultArguments();
        public IReadOnlyDictionary<string, ExpressionSyntax> NamedArguments => NamedArgument.GetArguments();

        private TypeArgumentRecorder TypeArgument { get; } = new();
        private ConstructorArgumentRecorder ConstructorArgument { get; } = new();
        private NamedArgumentRecorder NamedArgument { get; } = new();

        ISyntacticTypeArgumentRecorder ISyntacticRecorder.TypeArgument => TypeArgument;
        ISyntacticConstructorArgumentRecorder ISyntacticRecorder.ConstructorArgument => ConstructorArgument;
        ISyntacticNamedArgumentRecorder ISyntacticRecorder.NamedArgument => NamedArgument;

        private sealed class TypeArgumentRecorder : ISyntacticTypeArgumentRecorder
        {
            private Dictionary<ITypeParameterSymbol, ExpressionSyntax> Arguments { get; } = new(SymbolEqualityComparer.Default);

            bool ISyntacticTypeArgumentRecorder.TryRecordArgument(ITypeParameterSymbol parameter, ExpressionSyntax syntax)
            {
                Arguments[parameter] = syntax;

                return true;
            }

            public IReadOnlyDictionary<ITypeParameterSymbol, ExpressionSyntax> GetArguments() => Arguments;
        }

        private sealed class ConstructorArgumentRecorder : ISyntacticConstructorArgumentRecorder
        {
            private Dictionary<IParameterSymbol, ExpressionSyntax> NormalArguments { get; } = new(SymbolEqualityComparer.Default);
            private Dictionary<IParameterSymbol, IReadOnlyList<ExpressionSyntax>> ParamsArguments { get; } = new(SymbolEqualityComparer.Default);
            private ISet<IParameterSymbol> DefaultArguments { get; } = new HashSet<IParameterSymbol>(SymbolEqualityComparer.Default);

            bool ISyntacticConstructorArgumentRecorder.TryRecordArgument(IParameterSymbol parameter, ExpressionSyntax syntax)
            {
                NormalArguments[parameter] = syntax;

                return true;
            }

            bool ISyntacticConstructorArgumentRecorder.TryRecordParamsArgument(IParameterSymbol parameter, IReadOnlyList<ExpressionSyntax> elementSyntax)
            {
                ParamsArguments[parameter] = elementSyntax;

                return true;
            }

            bool ISyntacticConstructorArgumentRecorder.TryRecordDefaultArgument(IParameterSymbol parameter)
            {
                DefaultArguments.Add(parameter);

                return true;
            }

            public IReadOnlyDictionary<IParameterSymbol, ExpressionSyntax> GetNormalArguments() => NormalArguments;
            public IReadOnlyDictionary<IParameterSymbol, IReadOnlyList<ExpressionSyntax>> GetParamsArguments() => ParamsArguments;
            public ISet<IParameterSymbol> GetDefaultArguments() => DefaultArguments;
        }

        private sealed class NamedArgumentRecorder : ISyntacticNamedArgumentRecorder
        {
            private Dictionary<string, ExpressionSyntax> Arguments { get; } = new();

            bool ISyntacticNamedArgumentRecorder.TryRecordArgument(string parameterName, ExpressionSyntax syntax)
            {
                Arguments[parameterName] = syntax;

                return true;
            }

            public IReadOnlyDictionary<string, ExpressionSyntax> GetArguments() => Arguments;
        }
    }
}
