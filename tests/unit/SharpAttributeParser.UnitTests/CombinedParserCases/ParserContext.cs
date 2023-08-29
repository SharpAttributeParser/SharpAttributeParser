namespace SharpAttributeParser.CombinedParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Logging;

using System;

internal sealed class ParserContext
{
    public static ParserContext Create()
    {
        Mock<ICombinedParserLogger<CombinedParser>> loggerMock = new() { DefaultValue = DefaultValue.Mock };

        Mock<ISemanticParser> semanticParserMock = new();
        Mock<ISyntacticParser> syntacticParserMock = new();

        CombinedParser parser = new(semanticParserMock.Object, syntacticParserMock.Object, loggerMock.Object);

        return new(parser, semanticParserMock, syntacticParserMock, loggerMock);
    }

    public CombinedParser Parser { get; }
    public Mock<ISemanticParser> SemanticParserMock { get; }
    public Mock<ISyntacticParser> SyntacticParserMock { get; }

    public Mock<ICombinedParserLogger<CombinedParser>> LoggerMock { get; }

    private ParserContext(CombinedParser parser, Mock<ISemanticParser> semanticParserMock, Mock<ISyntacticParser> syntacticParserMock, Mock<ICombinedParserLogger<CombinedParser>> loggerMock)
    {
        Parser = parser;
        SemanticParserMock = semanticParserMock;
        SyntacticParserMock = syntacticParserMock;

        LoggerMock = loggerMock;
    }

    public void SetupValid()
    {
        Mock<ITypeParameterSymbol> typeParameterMock = new();
        Mock<IParameterSymbol> constructorNonParamsParameterMock = new();
        Mock<IParameterSymbol> constructorParamsParameterMock = new();
        Mock<IParameterSymbol> constructorDefaultParameterMock = new();

        setupSymbolReferenceEquality(typeParameterMock);
        setupSymbolReferenceEquality(constructorNonParamsParameterMock);
        setupSymbolReferenceEquality(constructorParamsParameterMock);
        setupSymbolReferenceEquality(constructorDefaultParameterMock);

        SemanticParserMock.Setup(static (parser) => parser.TryParse(It.IsAny<ISemanticRecorder>(), It.IsAny<AttributeData>())).Callback<ISemanticRecorder, AttributeData>(semanticParse).Returns(true);
        SyntacticParserMock.Setup(static (parser) => parser.TryParse(It.IsAny<ISyntacticRecorder>(), It.IsAny<AttributeData>(), It.IsAny<AttributeSyntax>())).Callback<ISyntacticRecorder, AttributeData, AttributeSyntax>(syntacticParase).Returns(true);

        void semanticParse(ISemanticRecorder recorder, AttributeData attributeData)
        {
            recorder.TypeArgument.TryRecordArgument(typeParameterMock.Object, Mock.Of<ITypeSymbol>());
            recorder.ConstructorArgument.TryRecordArgument(constructorNonParamsParameterMock.Object, null);
            recorder.ConstructorArgument.TryRecordArgument(constructorParamsParameterMock.Object, null);
            recorder.ConstructorArgument.TryRecordArgument(constructorDefaultParameterMock.Object, null);
            recorder.NamedArgument.TryRecordArgument(string.Empty, null);
        }

        void syntacticParase(ISyntacticRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax)
        {
            recorder.TypeArgument.TryRecordArgument(typeParameterMock.Object, ExpressionSyntaxFactory.Create());
            recorder.ConstructorArgument.TryRecordArgument(constructorNonParamsParameterMock.Object, ExpressionSyntaxFactory.Create());
            recorder.ConstructorArgument.TryRecordParamsArgument(constructorParamsParameterMock.Object, Array.Empty<ExpressionSyntax>());
            recorder.ConstructorArgument.TryRecordDefaultArgument(constructorDefaultParameterMock.Object);
            recorder.NamedArgument.TryRecordArgument(string.Empty, ExpressionSyntaxFactory.Create());
        }

        static void setupSymbolReferenceEquality<TSymbol>(Mock<TSymbol> symbolMock) where TSymbol : class, ISymbol => symbolMock.Setup(static (symbol) => symbol.Equals(It.IsAny<ISymbol>(), It.IsAny<SymbolEqualityComparer>())).Returns<ISymbol, SymbolEqualityComparer>((symbol, comparer) => ReferenceEquals(symbolMock.Object, symbol));
    }

    public void SetupFalseSemantic()
    {
        SemanticParserMock.Setup(static (parser) => parser.TryParse(It.IsAny<ISemanticRecorder>(), It.IsAny<AttributeData>())).Returns(false);
        SyntacticParserMock.Setup(static (parser) => parser.TryParse(It.IsAny<ISyntacticRecorder>(), It.IsAny<AttributeData>(), It.IsAny<AttributeSyntax>())).Returns(true);
    }

    public void SetupFalseSyntactic()
    {
        SemanticParserMock.Setup(static (parser) => parser.TryParse(It.IsAny<ISemanticRecorder>(), It.IsAny<AttributeData>())).Returns(true);
        SyntacticParserMock.Setup(static (parser) => parser.TryParse(It.IsAny<ISyntacticRecorder>(), It.IsAny<AttributeData>(), It.IsAny<AttributeSyntax>())).Returns(false);
    }

    public void SetupDifferentNumberOfTypeArguments()
    {
        SemanticParserMock.Setup(static (parser) => parser.TryParse(It.IsAny<ISemanticRecorder>(), It.IsAny<AttributeData>())).Callback<ISemanticRecorder, AttributeData>(semanticParse).Returns(true);
        SyntacticParserMock.Setup(static (parser) => parser.TryParse(It.IsAny<ISyntacticRecorder>(), It.IsAny<AttributeData>(), It.IsAny<AttributeSyntax>())).Returns(true);

        static void semanticParse(ISemanticRecorder recorder, AttributeData attributeData) => recorder.TypeArgument.TryRecordArgument(Mock.Of<ITypeParameterSymbol>(), Mock.Of<ITypeSymbol>());
    }

    public void SetupDifferentNumberOfConstructorArguments()
    {
        SemanticParserMock.Setup(static (parser) => parser.TryParse(It.IsAny<ISemanticRecorder>(), It.IsAny<AttributeData>())).Callback<ISemanticRecorder, AttributeData>(semanticParse).Returns(true);
        SyntacticParserMock.Setup(static (parser) => parser.TryParse(It.IsAny<ISyntacticRecorder>(), It.IsAny<AttributeData>(), It.IsAny<AttributeSyntax>())).Returns(true);

        static void semanticParse(ISemanticRecorder recorder, AttributeData attributeData) => recorder.ConstructorArgument.TryRecordArgument(Mock.Of<IParameterSymbol>(), null);
    }

    public void SetupDifferentNumberOfNamedArguments()
    {
        SemanticParserMock.Setup(static (parser) => parser.TryParse(It.IsAny<ISemanticRecorder>(), It.IsAny<AttributeData>())).Callback<ISemanticRecorder, AttributeData>(semanticParse).Returns(true);
        SyntacticParserMock.Setup(static (parser) => parser.TryParse(It.IsAny<ISyntacticRecorder>(), It.IsAny<AttributeData>(), It.IsAny<AttributeSyntax>())).Returns(true);

        static void semanticParse(ISemanticRecorder recorder, AttributeData attributeData) => recorder.NamedArgument.TryRecordArgument(string.Empty, null);
    }
}
