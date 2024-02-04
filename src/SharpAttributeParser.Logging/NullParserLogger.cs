namespace SharpAttributeParser.Logging;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser.Logging.ParserLoggerComponents;

using System;
using System.Collections.Generic;

/// <summary>A <see cref="IParserLogger"/> with no behaviour.</summary>
/// <typeparam name="TCategoryName">The name of the logging category.</typeparam>
public sealed class NullParserLogger<TCategoryName> : IParserLogger<TCategoryName>
{
    /// <summary>A <see cref="IParserLogger"/> with no behaviour.</summary>
    public static IParserLogger<TCategoryName> Instance { get; } = new NullParserLogger<TCategoryName>();

    ITypeArgumentLogger IParserLogger.Type { get; } = new NullTypeArgumentLogger();
    IConstructorArgumentLogger IParserLogger.Constructor { get; } = new NullConstructorArgumentLogger();
    INamedArgumentLogger IParserLogger.Named { get; } = new NullNamedArgumentLogger();

    IDisposable? IParserLogger.BeginScopeParsingAttribute(Type recorderType, AttributeData attributeData, AttributeSyntax attributeSyntax) => null;

    void IParserLogger.SemanticParserFailedToParseAttribute() { }
    void IParserLogger.SyntacticParserFailedToParseAttribute() { }

    private NullParserLogger() { }

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
