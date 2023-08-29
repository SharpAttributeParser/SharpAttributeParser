namespace SharpAttributeParser.Mappers.SyntacticRecorderFactoryCases;

using Moq;

using SharpAttributeParser.Mappers.Logging;

internal sealed class FactoryContext
{
    public static FactoryContext Create()
    {
        Mock<ISyntacticArgumentRecorderLoggerFactory> loggerFactoryMock = new() { DefaultValue = DefaultValue.Mock };

        SyntacticRecorderFactory factory = new(loggerFactoryMock.Object);

        return new(factory, loggerFactoryMock);
    }

    public SyntacticRecorderFactory Factory { get; }

    public Mock<ISyntacticArgumentRecorderLoggerFactory> LoggerFactoryMock { get; }

    private FactoryContext(SyntacticRecorderFactory factory, Mock<ISyntacticArgumentRecorderLoggerFactory> loggerFactoryMock)
    {
        Factory = factory;

        LoggerFactoryMock = loggerFactoryMock;
    }
}
