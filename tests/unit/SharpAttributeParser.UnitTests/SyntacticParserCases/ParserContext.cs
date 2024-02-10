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
        Mock<ISyntacticRecorder> recorderMock = new();

        Mock<ISyntacticParserLogger<SyntacticParser>> loggerMock = new() { DefaultValue = DefaultValue.Mock };

        recorderMock.Setup(static (recorder) => recorder.Type.TryRecordArgument(It.IsAny<ITypeParameterSymbol>(), It.IsAny<ExpressionSyntax>())).Returns(true);
        recorderMock.Setup(static (recorder) => recorder.Constructor.TryRecordArgument(It.IsAny<IParameterSymbol>(), It.IsAny<ExpressionSyntax>())).Returns(true);
        recorderMock.Setup(static (recorder) => recorder.Constructor.TryRecordParamsArgument(It.IsAny<IParameterSymbol>(), It.IsAny<IReadOnlyList<ExpressionSyntax>>())).Returns(true);
        recorderMock.Setup(static (recorder) => recorder.Constructor.TryRecordDefaultArgument(It.IsAny<IParameterSymbol>())).Returns(true);
        recorderMock.Setup(static (recorder) => recorder.Named.TryRecordArgument(It.IsAny<string>(), It.IsAny<ExpressionSyntax>())).Returns(true);

        SyntacticParser parser = new(loggerMock.Object);

        return new(parser, recorderMock, loggerMock);
    }

    public SyntacticParser Parser { get; }

    public Mock<ISyntacticRecorder> RecorderMock { get; }
    public ISyntacticRecorder Recorder => RecorderMock.Object;

    public Mock<ISyntacticParserLogger<SyntacticParser>> LoggerMock { get; }

    private ParserContext(SyntacticParser parser, Mock<ISyntacticRecorder> recorderMock, Mock<ISyntacticParserLogger<SyntacticParser>> loggerMock)
    {
        Parser = parser;

        RecorderMock = recorderMock;

        LoggerMock = loggerMock;
    }
}
