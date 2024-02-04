namespace SharpAttributeParser.Logging;

using Microsoft.CodeAnalysis;

using SharpAttributeParser.Logging.SemanticParserLoggerComponents;

using System;
using System.Collections.Generic;

/// <summary>A <see cref="ISemanticParserLogger"/> with no behaviour.</summary>
/// <typeparam name="TCategoryName">The name of the logging category.</typeparam>
public sealed class NullSemanticParserLogger<TCategoryName> : ISemanticParserLogger<TCategoryName>
{
    /// <summary>A <see cref="ISemanticParserLogger"/> with no behaviour.</summary>
    public static ISemanticParserLogger<TCategoryName> Instance { get; } = new NullSemanticParserLogger<TCategoryName>();

    ITypeArgumentLogger ISemanticParserLogger.Type { get; } = new NullTypeArgumentLogger();
    IConstructorArgumentLogger ISemanticParserLogger.Constructor { get; } = new NullConstructorArgumentLogger();
    INamedArgumentLogger ISemanticParserLogger.Named { get; } = new NullNamedArgumentLogger();

    private NullSemanticParserLogger() { }

    IDisposable? ISemanticParserLogger.BeginScopeParsingAttribute(Type recorderType, AttributeData attributeData) => null;
    IDisposable? ISemanticParserLogger.BeginScopeDeterminedAttributeClass(INamedTypeSymbol attributeClass) => null;
    IDisposable? ISemanticParserLogger.BeginScopeDeterminedTargetConstructor(IMethodSymbol targetConstructor) => null;

    void ISemanticParserLogger.UndeterminedAttributeClass() { }
    void ISemanticParserLogger.UndeterminedTargetConstructor() { }
    void ISemanticParserLogger.UnrecognizedAttributeClass() { }

    private sealed class NullTypeArgumentLogger : ITypeArgumentLogger
    {
        IDisposable? ITypeArgumentLogger.BeginScopeParsingTypeArguments(IReadOnlyList<ITypeParameterSymbol> parameters, IReadOnlyList<ITypeSymbol> arguments) => null;
        IDisposable? ITypeArgumentLogger.BeginScopeParsingTypeArgument(ITypeParameterSymbol parameter, ITypeSymbol argument) => null;

        void ITypeArgumentLogger.InvalidTypeArgument() { }
        void ITypeArgumentLogger.UnexpectedNumberOfTypeArguments() { }
    }

    private sealed class NullConstructorArgumentLogger : IConstructorArgumentLogger
    {
        IDisposable? IConstructorArgumentLogger.BeginScopeParsingConstructorArguments(IReadOnlyList<IParameterSymbol> parameters, IReadOnlyList<TypedConstant> arguments) => null;
        IDisposable? IConstructorArgumentLogger.BeginScopeParsingConstructorArgument(IParameterSymbol parameter, TypedConstant argument) => null;

        void IConstructorArgumentLogger.InvalidConstructorArgument() { }
        void IConstructorArgumentLogger.UnexpectedNumberOfConstructorArguments() { }
    }

    private sealed class NullNamedArgumentLogger : INamedArgumentLogger
    {
        IDisposable? INamedArgumentLogger.BeginScopeParsingNamedArguments(IReadOnlyList<KeyValuePair<string, TypedConstant>> arguments) => null;
        IDisposable? INamedArgumentLogger.BeginScopeParsingNamedArgument(string parameterName, TypedConstant argument) => null;

        void INamedArgumentLogger.InvalidNamedArgument() { }
    }
}
