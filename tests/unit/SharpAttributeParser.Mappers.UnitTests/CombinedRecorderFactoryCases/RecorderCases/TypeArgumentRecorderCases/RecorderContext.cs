namespace SharpAttributeParser.Mappers.CombinedRecorderFactoryCases.RecorderCases.TypeArgumentRecorderCases;

using Moq;

using SharpAttributeParser.Mappers.Logging;

internal sealed class RecorderContext<TRecord> where TRecord : class
{
    public static RecorderContext<TRecord> Create()
    {
        Mock<ICombinedArgumentRecorderLoggerFactory> loggerFactoryMock = new() { DefaultValue = DefaultValue.Mock };

        CombinedRecorderFactory factory = new(loggerFactoryMock.Object);

        Mock<ICombinedMapper<TRecord>> mapperMock = new();
        Mock<TRecord> dataRecordMock = new();

        var recorder = ((ICombinedRecorderFactory)factory).Create(mapperMock.Object, dataRecordMock.Object).TypeArgument;

        return new(recorder, mapperMock, dataRecordMock, loggerFactoryMock);
    }

    public ICombinedTypeArgumentRecorder Recorder { get; }

    public Mock<ICombinedMapper<TRecord>> MapperMock { get; }
    public Mock<TRecord> DataRecordMock { get; }

    public Mock<ICombinedArgumentRecorderLoggerFactory> LoggerFactoryMock { get; }

    private RecorderContext(ICombinedTypeArgumentRecorder recorder, Mock<ICombinedMapper<TRecord>> mapperMock, Mock<TRecord> dataRecordMock, Mock<ICombinedArgumentRecorderLoggerFactory> loggerFactoryMock)
    {
        Recorder = recorder;

        MapperMock = mapperMock;
        DataRecordMock = dataRecordMock;

        LoggerFactoryMock = loggerFactoryMock;
    }
}
