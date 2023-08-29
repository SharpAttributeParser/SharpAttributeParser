namespace SharpAttributeParser.Logging;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser.Logging.CombinedParserComponents;

using System;
using System.Collections.Generic;

/// <summary>A <see cref="ICombinedParserLogger"/> with no behaviour.</summary>
/// <typeparam name="TCategoryName">The name of the logging category.</typeparam>
public sealed class NullCombinedParserLogger<TCategoryName> : ICombinedParserLogger<TCategoryName>
{
    /// <summary>The singleton <see cref="NullCombinedParserLogger{TCategoryName}"/>.</summary>
    public static NullCombinedParserLogger<TCategoryName> Singleton { get; } = new();

    ITypeArgumentLogger ICombinedParserLogger.TypeArgument { get; } = new NullTypeArgumentLogger();
    IConstructorArgumentLogger ICombinedParserLogger.ConstructorArgument { get; } = new NullConstructorArgumentLogger();
    INamedArgumentLogger ICombinedParserLogger.NamedArgument { get; } = new NullNamedArgumentLogger();

    IDisposable? ICombinedParserLogger.BeginScopeParsingAttribute(Type recorderType, AttributeData attributeData, AttributeSyntax attributeSyntax) => null;

    void ICombinedParserLogger.SemanticParserFailedToParseAttribute() { }
    void ICombinedParserLogger.SyntacticParserFailedToParseAttribute() { }

    /// <summary>Instantiates a <see cref="NullCombinedParserLogger{TCategoryName}"/>, a <see cref="ICombinedParserLogger"/> with no behaviour.</summary>
    public NullCombinedParserLogger() { }

    private sealed class NullTypeArgumentLogger : ITypeArgumentLogger
    {
        IDisposable? ITypeArgumentLogger.BeginScopePropagateParsedTypeArguments(IReadOnlyDictionary<ITypeParameterSymbol, ITypeSymbol> semanticArguments, IReadOnlyDictionary<ITypeParameterSymbol, ExpressionSyntax> syntacticArguments) => null;
        IDisposable? ITypeArgumentLogger.BeginScopePropagateSemanticallyParsedTypeArgument(ITypeParameterSymbol parameter, ITypeSymbol argument) => null;
        IDisposable? ITypeArgumentLogger.BeginScopePropagateSyntacticallyParsedTypeArgument(ExpressionSyntax syntax) => null;

        void ITypeArgumentLogger.DifferentNumberOfSemanticallyAndSyntacticallyParsedTypeArguments() { }
        void ITypeArgumentLogger.TypeArgumentOnlyRecordedSemantically() { }
    }

    private sealed class NullConstructorArgumentLogger : IConstructorArgumentLogger
    {
        IDisposable? IConstructorArgumentLogger.BeginScopePropagateParsedConstructorArguments(IReadOnlyDictionary<IParameterSymbol, object?> semanticArguments, IReadOnlyDictionary<IParameterSymbol, ExpressionSyntax> nonParamsSyntacticArguments, IReadOnlyDictionary<IParameterSymbol, IReadOnlyList<ExpressionSyntax>> paramsSyntacticArguments, ISet<IParameterSymbol> defaultSyntacticArguments) => null;
        IDisposable? IConstructorArgumentLogger.BeginScopePropagateSemanticallyParsedConstructorArgument(IParameterSymbol parameter, object? argument) => null;
        IDisposable? IConstructorArgumentLogger.BeginScopePropagateSyntacticallyParsedNormalConstructorArgument(ExpressionSyntax syntax) => null;
        IDisposable? IConstructorArgumentLogger.BeginScopePropagateSyntacticallyParsedParamsConstructorArgument(IReadOnlyList<ExpressionSyntax> elementSyntax) => null;
        IDisposable? IConstructorArgumentLogger.BeginScopePropagateSyntacticallyParsedDefaultConstructorArgument() => null;

        void IConstructorArgumentLogger.DifferentNumberOfSemanticallyAndSyntacticallyParsedConstructorArguments() { }
        void IConstructorArgumentLogger.ConstructorArgumentOnlyRecordedSemantically() { }
    }

    private sealed class NullNamedArgumentLogger : INamedArgumentLogger
    {
        IDisposable? INamedArgumentLogger.BeginScopePropagateParsedNamedArguments(IReadOnlyDictionary<string, object?> semanticArguments, IReadOnlyDictionary<string, ExpressionSyntax> syntacticArguments) => null;
        IDisposable? INamedArgumentLogger.BeginScopePropagateSemanticallyParsedNamedArgument(string parameterName, object? argument) => null;
        IDisposable? INamedArgumentLogger.BeginScopePropagateSyntacticallyParsedNamedArgument(ExpressionSyntax syntax) => null;

        void INamedArgumentLogger.DifferentNumberOfSemanticallyAndSyntacticallyParsedNamedArguments() { }
        void INamedArgumentLogger.NamedArgumentOnlyRecordedSemantically() { }
    }
}
