namespace SharpAttributeParser.Mappers.SemanticRecorderFactoryCases;

using Moq;

using SharpAttributeParser.Mappers.Logging;

internal sealed class FactoryContext
{
    public static FactoryContext Create()
    {
        Mock<ISemanticArgumentRecorderLoggerFactory> loggerFactoryMock = new() { DefaultValue = DefaultValue.Mock };

        SemanticRecorderFactory factory = new(loggerFactoryMock.Object);

        return new(factory, loggerFactoryMock);
    }

    public SemanticRecorderFactory Factory { get; }

    public Mock<ISemanticArgumentRecorderLoggerFactory> LoggerFactoryMock { get; }

    private FactoryContext(SemanticRecorderFactory factory, Mock<ISemanticArgumentRecorderLoggerFactory> loggerFactoryMock)
    {
        Factory = factory;

        LoggerFactoryMock = loggerFactoryMock;
    }
}
