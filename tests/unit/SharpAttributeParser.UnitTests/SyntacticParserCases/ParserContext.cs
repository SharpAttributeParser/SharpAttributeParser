namespace SharpAttributeParser.SyntacticParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Logging;

using System.Collections.Generic;

internal sealed class ParserContext
{
    public static ParserContext Create()
    {
        Mock<ISyntacticRecorder<object>> recorderMock = new();

        Mock<ISyntacticParserLogger<SyntacticParser>> loggerMock = new() { DefaultValue = DefaultValue.Mock };

        recorderMock.Setup(static (recorder) => recorder.TypeArgument.TryRecordArgument(It.IsAny<ITypeParameterSymbol>(), It.IsAny<ExpressionSyntax>())).Returns(true);
        recorderMock.Setup(static (recorder) => recorder.ConstructorArgument.TryRecordArgument(It.IsAny<IParameterSymbol>(), It.IsAny<ExpressionSyntax>())).Returns(true);
        recorderMock.Setup(static (recorder) => recorder.ConstructorArgument.TryRecordParamsArgument(It.IsAny<IParameterSymbol>(), It.IsAny<IReadOnlyList<ExpressionSyntax>>())).Returns(true);
        recorderMock.Setup(static (recorder) => recorder.ConstructorArgument.TryRecordDefaultArgument(It.IsAny<IParameterSymbol>())).Returns(true);
        recorderMock.Setup(static (recorder) => recorder.NamedArgument.TryRecordArgument(It.IsAny<string>(), It.IsAny<ExpressionSyntax>())).Returns(true);

        SyntacticParser parser = new(loggerMock.Object);

        return new(parser, recorderMock, loggerMock);
    }

    public SyntacticParser Parser { get; }

    public Mock<ISyntacticRecorder<object>> RecorderMock { get; }
    public ISyntacticRecorder<object> Recorder => RecorderMock.Object;

    public Mock<ISyntacticParserLogger<SyntacticParser>> LoggerMock { get; }

    private ParserContext(SyntacticParser parser, Mock<ISyntacticRecorder<object>> recorderMock, Mock<ISyntacticParserLogger<SyntacticParser>> loggerMock)
    {
        Parser = parser;

        RecorderMock = recorderMock;

        LoggerMock = loggerMock;
    }
}
