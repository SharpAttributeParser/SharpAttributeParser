namespace SharpAttributeParser.Logging;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser.Logging.SyntacticParserLoggerComponents;

using System;
using System.Collections.Generic;

/// <summary>A <see cref="ISyntacticParserLogger"/> with no behaviour.</summary>
/// <typeparam name="TCategoryName">The name of the logging category.</typeparam>
public sealed class NullSyntacticParserLogger<TCategoryName> : ISyntacticParserLogger<TCategoryName>
{
    ITypeArgumentLogger ISyntacticParserLogger.Type { get; } = new NullTypeArgumentLogger();
    IConstructorArgumentLogger ISyntacticParserLogger.Constructor { get; } = new NullConstructorArgumentLogger();
    INamedArgumentLogger ISyntacticParserLogger.Named { get; } = new NullNamedArgumentLogger();

    /// <summary>Instantiates a <see cref="NullSyntacticParserLogger{TCategoryName}"/>, a <see cref="ISyntacticParserLogger"/> with no behaviour.</summary>
    public NullSyntacticParserLogger() { }

    IDisposable? ISyntacticParserLogger.BeginScopeParsingAttribute(Type recorderType, AttributeData attributeData, AttributeSyntax attributeSyntax) => null;
    IDisposable? ISyntacticParserLogger.BeginScopeDeterminedAttributeClass(INamedTypeSymbol attributeClass) => null;
    IDisposable? ISyntacticParserLogger.BeginScopeDeterminedTargetConstructor(IMethodSymbol targetConstructor) => null;

    void ISyntacticParserLogger.UndeterminedAttributeClass() { }
    void ISyntacticParserLogger.UnrecognizedAttributeClass() { }
    void ISyntacticParserLogger.UndeterminedTargetConstructor() { }

    private sealed class NullTypeArgumentLogger : ITypeArgumentLogger
    {
        IDisposable? ITypeArgumentLogger.BeginScopeParsingTypeArguments(IReadOnlyList<ITypeParameterSymbol> parameters, NameSyntax nameSyntax) => null;
        IDisposable? ITypeArgumentLogger.BeginScopeParsingTypeArgument(ITypeParameterSymbol parameter, TypeSyntax syntax) => null;
        IDisposable? ITypeArgumentLogger.BeginScopeRecognizedGenericExpression(GenericNameSyntax genericNameSyntax) => null;

        void ITypeArgumentLogger.SyntaxNotRecognizedAsGenericExpression() { }
        void ITypeArgumentLogger.UnexpectedNumberOfSyntacticTypeArguments() { }
    }

    private sealed class NullConstructorArgumentLogger : IConstructorArgumentLogger
    {
        IDisposable? IConstructorArgumentLogger.BeginScopeParsingConstructorArguments(AttributeData attributeData, AttributeSyntax attributeSyntax) => null;
        IDisposable? IConstructorArgumentLogger.BeginScopeParsingConstructorArgument(IParameterSymbol parameter) => null;
        IDisposable? IConstructorArgumentLogger.BeginScopeParsingNormalConstructorArgument(ExpressionSyntax syntax) => null;
        IDisposable? IConstructorArgumentLogger.BeginScopeParsingParamsConstructorArgument(IReadOnlyList<ExpressionSyntax> elementSyntax) => null;
        IDisposable? IConstructorArgumentLogger.BeginScopeParsingDefaultConstructorArgument() => null;

        void IConstructorArgumentLogger.MissingOneOrMoreRequiredConstructorArgument() { }
        void IConstructorArgumentLogger.OutOfOrderLabelledConstructorArgumentsFollowedByUnlabelled() { }
        void IConstructorArgumentLogger.LabelledConstructorArgumentHadNoMatchingParameter() { }
        void IConstructorArgumentLogger.UnexpectedNumberOfSemanticConstructorArguments() { }
    }

    private sealed class NullNamedArgumentLogger : INamedArgumentLogger
    {
        IDisposable? INamedArgumentLogger.BeginScopeParsingNamedArguments(AttributeData attributeData, AttributeSyntax attributeSyntax) => null;
        IDisposable? INamedArgumentLogger.BeginScopeParsingNamedArgument(string parameterName, ExpressionSyntax syntax) => null;
    }
}
