namespace SharpAttributeParser.SemanticParserCases;

using Microsoft.CodeAnalysis;

using Moq;

using SharpAttributeParser.Logging;

internal sealed class ParserContext
{
    public static ParserContext Create()
    {
        Mock<ISemanticRecorder<object>> recorderMock = new();

        Mock<ISemanticParserLogger<SemanticParser>> loggerMock = new() { DefaultValue = DefaultValue.Mock };

        recorderMock.Setup(static (recorder) => recorder.Type.TryRecordArgument(It.IsAny<ITypeParameterSymbol>(), It.IsAny<ITypeSymbol>())).Returns(true);
        recorderMock.Setup(static (recorder) => recorder.Constructor.TryRecordArgument(It.IsAny<IParameterSymbol>(), It.IsAny<object?>())).Returns(true);
        recorderMock.Setup(static (recorder) => recorder.Named.TryRecordArgument(It.IsAny<string>(), It.IsAny<object?>())).Returns(true);

        SemanticParser parser = new(loggerMock.Object);

        return new(parser, recorderMock, loggerMock);
    }

    public SemanticParser Parser { get; }

    public Mock<ISemanticRecorder<object>> RecorderMock { get; }
    public ISemanticRecorder<object> Recorder => RecorderMock.Object;

    public Mock<ISemanticParserLogger<SemanticParser>> LoggerMock { get; }

    private ParserContext(SemanticParser parser, Mock<ISemanticRecorder<object>> recorderMock, Mock<ISemanticParserLogger<SemanticParser>> loggerMock)
    {
        Parser = parser;

        RecorderMock = recorderMock;

        LoggerMock = loggerMock;
    }
}
