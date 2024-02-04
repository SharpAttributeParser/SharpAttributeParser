namespace SharpAttributeParser.ParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Logging;

using System;

internal sealed class ParserContext
{
    public static ParserContext Create()
    {
        Mock<IParserLogger<Parser>> loggerMock = new() { DefaultValue = DefaultValue.Mock };

        Mock<ISemanticParser> semanticParserMock = new();
        Mock<ISyntacticParser> syntacticParserMock = new();

        Parser parser = new(semanticParserMock.Object, syntacticParserMock.Object, loggerMock.Object);

        return new(parser, semanticParserMock, syntacticParserMock, loggerMock);
    }

    public Parser Parser { get; }
    public Mock<ISemanticParser> SemanticParserMock { get; }
    public Mock<ISyntacticParser> SyntacticParserMock { get; }

    public Mock<IParserLogger<Parser>> LoggerMock { get; }

    private ParserContext(Parser parser, Mock<ISemanticParser> semanticParserMock, Mock<ISyntacticParser> syntacticParserMock, Mock<IParserLogger<Parser>> loggerMock)
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
            recorder.Type.TryRecordArgument(typeParameterMock.Object, Mock.Of<ITypeSymbol>());
            recorder.Constructor.TryRecordArgument(constructorNonParamsParameterMock.Object, null);
            recorder.Constructor.TryRecordArgument(constructorParamsParameterMock.Object, null);
            recorder.Constructor.TryRecordArgument(constructorDefaultParameterMock.Object, null);
            recorder.Named.TryRecordArgument(string.Empty, null);
        }

        void syntacticParase(ISyntacticRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax)
        {
            recorder.Type.TryRecordArgument(typeParameterMock.Object, ExpressionSyntaxFactory.Create());
            recorder.Constructor.TryRecordArgument(constructorNonParamsParameterMock.Object, ExpressionSyntaxFactory.Create());
            recorder.Constructor.TryRecordParamsArgument(constructorParamsParameterMock.Object, Array.Empty<ExpressionSyntax>());
            recorder.Constructor.TryRecordDefaultArgument(constructorDefaultParameterMock.Object);
            recorder.Named.TryRecordArgument(string.Empty, ExpressionSyntaxFactory.Create());
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

        static void semanticParse(ISemanticRecorder recorder, AttributeData attributeData) => recorder.Type.TryRecordArgument(Mock.Of<ITypeParameterSymbol>(), Mock.Of<ITypeSymbol>());
    }

    public void SetupDifferentNumberOfConstructorArguments()
    {
        SemanticParserMock.Setup(static (parser) => parser.TryParse(It.IsAny<ISemanticRecorder>(), It.IsAny<AttributeData>())).Callback<ISemanticRecorder, AttributeData>(semanticParse).Returns(true);
        SyntacticParserMock.Setup(static (parser) => parser.TryParse(It.IsAny<ISyntacticRecorder>(), It.IsAny<AttributeData>(), It.IsAny<AttributeSyntax>())).Returns(true);

        static void semanticParse(ISemanticRecorder recorder, AttributeData attributeData) => recorder.Constructor.TryRecordArgument(Mock.Of<IParameterSymbol>(), null);
    }

    public void SetupDifferentNumberOfNamedArguments()
    {
        SemanticParserMock.Setup(static (parser) => parser.TryParse(It.IsAny<ISemanticRecorder>(), It.IsAny<AttributeData>())).Callback<ISemanticRecorder, AttributeData>(semanticParse).Returns(true);
        SyntacticParserMock.Setup(static (parser) => parser.TryParse(It.IsAny<ISyntacticRecorder>(), It.IsAny<AttributeData>(), It.IsAny<AttributeSyntax>())).Returns(true);

        static void semanticParse(ISemanticRecorder recorder, AttributeData attributeData) => recorder.Named.TryRecordArgument(string.Empty, null);
    }
}
