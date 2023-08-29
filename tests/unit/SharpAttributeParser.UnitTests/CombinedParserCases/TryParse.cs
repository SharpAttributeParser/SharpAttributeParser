namespace SharpAttributeParser.CombinedParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class TryParse
{
    private bool Target(ICombinedRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => Context.Parser.TryParse(recorder, attributeData, attributeSyntax);

    private ParserContext Context { get; } = ParserContext.Create();

    [Fact]
    public void NullRecorder_ArgumentNullException()
    {
        var exception = Record.Exception(() => Target(null!, Mock.Of<AttributeData>(), AttributeSyntaxFactory.Create()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullAttributeData_ArgumentNullException()
    {
        var exception = Record.Exception(() => Target(Mock.Of<ICombinedRecorder>(), null!, AttributeSyntaxFactory.Create()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullAttributeSyntax_ArgumentNullException()
    {
        var exception = Record.Exception(() => Target(Mock.Of<ICombinedRecorder>(), Mock.Of<AttributeData>(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void FalseReturningTypeArgumentRecorder_ReturnsFalse()
    {
        var recorderMock = CreateTrueReturningRecorderMock();

        recorderMock.Setup(static (recorder) => recorder.TypeArgument.TryRecordArgument(It.IsAny<ITypeParameterSymbol>(), It.IsAny<ITypeSymbol>(), It.IsAny<ExpressionSyntax>())).Returns(false);

        FalseReturningRecorder_ReturnsFalse(recorderMock.Object);
    }

    [Fact]
    public void FalseReturningNonParamsConstructorArgumentRecorder_ReturnsFalse()
    {
        var recorderMock = CreateTrueReturningRecorderMock();

        recorderMock.Setup(static (recorder) => recorder.ConstructorArgument.TryRecordArgument(It.IsAny<IParameterSymbol>(), It.IsAny<object?>(), It.IsAny<ExpressionSyntax>())).Returns(false);

        FalseReturningRecorder_ReturnsFalse(recorderMock.Object);
    }

    [Fact]
    public void FalseReturningParamsConstructorArgumentRecorder_ReturnsFalse()
    {
        var recorderMock = CreateTrueReturningRecorderMock();

        recorderMock.Setup(static (recorder) => recorder.ConstructorArgument.TryRecordParamsArgument(It.IsAny<IParameterSymbol>(), It.IsAny<object?>(), It.IsAny<IReadOnlyList<ExpressionSyntax>>())).Returns(false);

        FalseReturningRecorder_ReturnsFalse(recorderMock.Object);
    }

    [Fact]
    public void FalseReturningDefaultConstructorArgumentRecorder_ReturnsFalse()
    {
        var recorderMock = CreateTrueReturningRecorderMock();

        recorderMock.Setup(static (recorder) => recorder.ConstructorArgument.TryRecordDefaultArgument(It.IsAny<IParameterSymbol>(), It.IsAny<object?>())).Returns(false);

        FalseReturningRecorder_ReturnsFalse(recorderMock.Object);
    }

    [Fact]
    public void FalseReturningNamedArgumentRecorder_ReturnsFalse()
    {
        var recorderMock = CreateTrueReturningRecorderMock();

        recorderMock.Setup(static (recorder) => recorder.NamedArgument.TryRecordArgument(It.IsAny<string>(), It.IsAny<object?>(), It.IsAny<ExpressionSyntax>())).Returns(false);

        FalseReturningRecorder_ReturnsFalse(recorderMock.Object);
    }

    [Fact]
    public void FalseReturningSemanticParser_ReturnsFalseAndLogs()
    {
        Context.SetupFalseSemantic();

        var result = Target(Mock.Of<ICombinedRecorder>(), Mock.Of<AttributeData>(), AttributeSyntaxFactory.Create());

        Assert.False(result);

        Context.LoggerMock.Verify(static (logger) => logger.SemanticParserFailedToParseAttribute(), Times.Once);
    }

    [Fact]
    public void FalseReturningSyntacticParser_ReturnsFalseAndLogs()
    {
        Context.SetupFalseSyntactic();

        var result = Target(Mock.Of<ICombinedRecorder>(), Mock.Of<AttributeData>(), AttributeSyntaxFactory.Create());

        Assert.False(result);

        Context.LoggerMock.Verify(static (logger) => logger.SyntacticParserFailedToParseAttribute(), Times.Once);
    }

    [Fact]
    public void DifferentNumberOfSemanticAndSyntacticTypeArguments_ReturnsFalseAndLogs()
    {
        Context.SetupDifferentNumberOfTypeArguments();

        var result = Target(Mock.Of<ICombinedRecorder>(), Mock.Of<AttributeData>(), AttributeSyntaxFactory.Create());

        Assert.False(result);

        Context.LoggerMock.Verify(static (logger) => logger.TypeArgument.DifferentNumberOfSemanticallyAndSyntacticallyParsedTypeArguments(), Times.Once);
    }

    [Fact]
    public void DifferentNumberOfSemanticAndSyntacticConstructorArguments_ReturnsFalseAndLogs()
    {
        Context.SetupDifferentNumberOfConstructorArguments();

        var result = Target(Mock.Of<ICombinedRecorder>(), Mock.Of<AttributeData>(), AttributeSyntaxFactory.Create());

        Assert.False(result);

        Context.LoggerMock.Verify(static (logger) => logger.ConstructorArgument.DifferentNumberOfSemanticallyAndSyntacticallyParsedConstructorArguments(), Times.Once);
    }

    [Fact]
    public void DifferentNumberOfSemanticAndSyntacticNamedArguments_ReturnsFalseAndLogs()
    {
        Context.SetupDifferentNumberOfNamedArguments();

        var result = Target(Mock.Of<ICombinedRecorder>(), Mock.Of<AttributeData>(), AttributeSyntaxFactory.Create());

        Assert.False(result);

        Context.LoggerMock.Verify(static (logger) => logger.NamedArgument.DifferentNumberOfSemanticallyAndSyntacticallyParsedNamedArguments(), Times.Once);
    }

    [Fact]
    public void Valid_ReturnsTrueAndRecords()
    {
        var recorderMock = CreateTrueReturningRecorderMock();

        Mock<ITypeParameterSymbol> typeParameterMock = new();
        Mock<IParameterSymbol> nonParamsConstructorParameterMock = new();
        Mock<IParameterSymbol> paramsConstructorParameterMock = new();
        Mock<IParameterSymbol> defaultConstructorParameterMock = new();
        var namedParameter = "NamedParameter";

        Mock<ITypeSymbol> typeArgumentMock = new();
        var nonParamsConstructorArgument = Mock.Of<object>();
        var paramsConstructorArgument = Mock.Of<object>();
        var defaultConstructorArgument = Mock.Of<object>();
        var namedArgument = Mock.Of<object>();

        var typeArgumentSyntax = ExpressionSyntaxFactory.Create();
        var nonParamsConstructorArgumentSyntax = ExpressionSyntaxFactory.Create();
        var paramsConstructorArgumentSyntax = Mock.Of<IReadOnlyList<ExpressionSyntax>>();
        var namedArgumentSyntax = ExpressionSyntaxFactory.Create();

        setupSymbolReferenceEquality(typeParameterMock);
        setupSymbolReferenceEquality(nonParamsConstructorParameterMock);
        setupSymbolReferenceEquality(paramsConstructorParameterMock);
        setupSymbolReferenceEquality(defaultConstructorParameterMock);

        Context.SemanticParserMock.Setup(static (parser) => parser.TryParse(It.IsAny<ISemanticRecorder>(), It.IsAny<AttributeData>())).Callback<ISemanticRecorder, AttributeData>(semanticParse).Returns(true);
        Context.SyntacticParserMock.Setup(static (parser) => parser.TryParse(It.IsAny<ISyntacticRecorder>(), It.IsAny<AttributeData>(), It.IsAny<AttributeSyntax>())).Callback<ISyntacticRecorder, AttributeData, AttributeSyntax>(syntacticParse).Returns(true);

        var result = Target(recorderMock.Object, Mock.Of<AttributeData>(), AttributeSyntaxFactory.Create());

        Assert.True(result);

        recorderMock.Verify((recorder) => recorder.TypeArgument.TryRecordArgument(typeParameterMock.Object, typeArgumentMock.Object, typeArgumentSyntax), Times.Once);
        recorderMock.Verify((recorder) => recorder.ConstructorArgument.TryRecordArgument(nonParamsConstructorParameterMock.Object, nonParamsConstructorArgument, nonParamsConstructorArgumentSyntax), Times.Once);
        recorderMock.Verify((recorder) => recorder.ConstructorArgument.TryRecordParamsArgument(paramsConstructorParameterMock.Object, paramsConstructorArgument, paramsConstructorArgumentSyntax), Times.Once);
        recorderMock.Verify((recorder) => recorder.ConstructorArgument.TryRecordDefaultArgument(defaultConstructorParameterMock.Object, defaultConstructorArgument), Times.Once);
        recorderMock.Verify((recorder) => recorder.NamedArgument.TryRecordArgument(namedParameter, namedArgument, namedArgumentSyntax), Times.Once);

        void semanticParse(ISemanticRecorder recorder, AttributeData attributeData)
        {
            recorder.TypeArgument.TryRecordArgument(typeParameterMock.Object, typeArgumentMock.Object);
            recorder.ConstructorArgument.TryRecordArgument(nonParamsConstructorParameterMock.Object, nonParamsConstructorArgument);
            recorder.ConstructorArgument.TryRecordArgument(paramsConstructorParameterMock.Object, paramsConstructorArgument);
            recorder.ConstructorArgument.TryRecordArgument(defaultConstructorParameterMock.Object, defaultConstructorArgument);
            recorder.NamedArgument.TryRecordArgument(namedParameter, namedArgument);
        }

        void syntacticParse(ISyntacticRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax)
        {
            recorder.TypeArgument.TryRecordArgument(typeParameterMock.Object, typeArgumentSyntax);
            recorder.ConstructorArgument.TryRecordArgument(nonParamsConstructorParameterMock.Object, nonParamsConstructorArgumentSyntax);
            recorder.ConstructorArgument.TryRecordParamsArgument(paramsConstructorParameterMock.Object, paramsConstructorArgumentSyntax);
            recorder.ConstructorArgument.TryRecordDefaultArgument(defaultConstructorParameterMock.Object);
            recorder.NamedArgument.TryRecordArgument(namedParameter, namedArgumentSyntax);
        }

        static void setupSymbolReferenceEquality<TSymbol>(Mock<TSymbol> symbolMock) where TSymbol : class, ISymbol => symbolMock.Setup(static (symbol) => symbol.Equals(It.IsAny<ISymbol>(), It.IsAny<SymbolEqualityComparer>())).Returns<ISymbol, SymbolEqualityComparer>((symbol, comparer) => ReferenceEquals(symbolMock.Object, symbol));
    }

    [AssertionMethod]
    private void FalseReturningRecorder_ReturnsFalse(ICombinedRecorder recorder)
    {
        Context.SetupValid();

        var result = Target(recorder, Mock.Of<AttributeData>(), AttributeSyntaxFactory.Create());

        Assert.False(result);
    }

    private static Mock<ICombinedRecorder> CreateTrueReturningRecorderMock()
    {
        Mock<ICombinedRecorder> recorderMock = new();

        recorderMock.Setup(static (recorder) => recorder.TypeArgument.TryRecordArgument(It.IsAny<ITypeParameterSymbol>(), It.IsAny<ITypeSymbol>(), It.IsAny<ExpressionSyntax>())).Returns(true);
        recorderMock.Setup(static (recorder) => recorder.ConstructorArgument.TryRecordArgument(It.IsAny<IParameterSymbol>(), It.IsAny<object?>(), It.IsAny<ExpressionSyntax>())).Returns(true);
        recorderMock.Setup(static (recorder) => recorder.ConstructorArgument.TryRecordParamsArgument(It.IsAny<IParameterSymbol>(), It.IsAny<object?>(), It.IsAny<IReadOnlyList<ExpressionSyntax>>())).Returns(true);
        recorderMock.Setup(static (recorder) => recorder.ConstructorArgument.TryRecordDefaultArgument(It.IsAny<IParameterSymbol>(), It.IsAny<object?>())).Returns(true);
        recorderMock.Setup(static (recorder) => recorder.NamedArgument.TryRecordArgument(It.IsAny<string>(), It.IsAny<object?>(), It.IsAny<ExpressionSyntax>())).Returns(true);

        return recorderMock;
    }
}
