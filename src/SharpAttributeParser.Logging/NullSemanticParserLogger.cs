namespace SharpAttributeParser.Logging;

using Microsoft.CodeAnalysis;

using SharpAttributeParser.Logging.SemanticParserComponents;

using System;
using System.Collections.Generic;

/// <summary>A <see cref="ISemanticParserLogger"/> with no behaviour.</summary>
/// <typeparam name="TCategoryName">The name of the logging category.</typeparam>
public sealed class NullSemanticParserLogger<TCategoryName> : ISemanticParserLogger<TCategoryName>
{
    /// <summary>The singleton <see cref="NullSemanticParserLogger{TCategoryName}"/>.</summary>
    public static NullSemanticParserLogger<TCategoryName> Singleton { get; } = new();

    ITypeArgumentLogger ISemanticParserLogger.TypeArgument { get; } = new NullTypeArgumentLogger();
    IConstructorArgumentLogger ISemanticParserLogger.ConstructorArgument { get; } = new NullConstructorArgumentLogger();
    INamedArgumentLogger ISemanticParserLogger.NamedArgument { get; } = new NullNamedArgumentLogger();

    /// <summary>Instantiates a <see cref="NullSemanticParserLogger{TCategoryName}"/>, a <see cref="ISemanticParserLogger"/> with no behaviour.</summary>
    public NullSemanticParserLogger() { }

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
