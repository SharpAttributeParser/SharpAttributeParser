namespace SharpAttributeParser.Mappers.CombinedRecorderFactoryCases;

using Moq;

using SharpAttributeParser.Mappers.Logging;

internal sealed class FactoryContext
{
    public static FactoryContext Create()
    {
        Mock<ICombinedArgumentRecorderLoggerFactory> loggerFactoryMock = new() { DefaultValue = DefaultValue.Mock };

        CombinedRecorderFactory factory = new(loggerFactoryMock.Object);

        return new(factory, loggerFactoryMock);
    }

    public CombinedRecorderFactory Factory { get; }

    public Mock<ICombinedArgumentRecorderLoggerFactory> LoggerFactoryMock { get; }

    private FactoryContext(CombinedRecorderFactory factory, Mock<ICombinedArgumentRecorderLoggerFactory> loggerFactoryMock)
    {
        Factory = factory;

        LoggerFactoryMock = loggerFactoryMock;
    }
}
