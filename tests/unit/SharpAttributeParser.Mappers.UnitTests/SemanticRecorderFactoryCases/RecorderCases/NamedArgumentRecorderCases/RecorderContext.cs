﻿namespace SharpAttributeParser.Mappers.SemanticRecorderFactoryCases.RecorderCases.NamedArgumentRecorderCases;

using Moq;

using SharpAttributeParser.Mappers.Logging;

internal sealed class RecorderContext<TRecord> where TRecord : class
{
    public static RecorderContext<TRecord> Create()
    {
        Mock<ISemanticArgumentRecorderLoggerFactory> loggerFactoryMock = new() { DefaultValue = DefaultValue.Mock };

        SemanticRecorderFactory factory = new(loggerFactoryMock.Object);

        Mock<ISemanticMapper<TRecord>> mapperMock = new();
        Mock<TRecord> dataRecordMock = new();

        var recorder = ((ISemanticRecorderFactory)factory).Create(mapperMock.Object, dataRecordMock.Object).NamedArgument;

        return new(recorder, mapperMock, dataRecordMock, loggerFactoryMock);
    }

    public ISemanticNamedArgumentRecorder Recorder { get; }

    public Mock<ISemanticMapper<TRecord>> MapperMock { get; }
    public Mock<TRecord> DataRecordMock { get; }

    public Mock<ISemanticArgumentRecorderLoggerFactory> LoggerFactoryMock { get; }

    private RecorderContext(ISemanticNamedArgumentRecorder recorder, Mock<ISemanticMapper<TRecord>> mapperMock, Mock<TRecord> dataRecordMock, Mock<ISemanticArgumentRecorderLoggerFactory> loggerFactoryMock)
    {
        Recorder = recorder;

        MapperMock = mapperMock;
        DataRecordMock = dataRecordMock;

        LoggerFactoryMock = loggerFactoryMock;
    }
}
