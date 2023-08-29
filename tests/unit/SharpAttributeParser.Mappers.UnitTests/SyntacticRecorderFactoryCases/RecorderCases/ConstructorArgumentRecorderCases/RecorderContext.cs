﻿namespace SharpAttributeParser.Mappers.SyntacticRecorderFactoryCases.RecorderCases.ConstructorArgumentRecorderCases;

using Moq;

using SharpAttributeParser.Mappers.Logging;

internal sealed class RecorderContext<TRecord> where TRecord : class
{
    public static RecorderContext<TRecord> Create()
    {
        Mock<ISyntacticArgumentRecorderLoggerFactory> loggerFactoryMock = new() { DefaultValue = DefaultValue.Mock };

        SyntacticRecorderFactory factory = new(loggerFactoryMock.Object);

        Mock<ISyntacticMapper<TRecord>> mapperMock = new();
        Mock<TRecord> dataRecordMock = new();

        var recorder = ((ISyntacticRecorderFactory)factory).Create(mapperMock.Object, dataRecordMock.Object).ConstructorArgument;

        return new(recorder, mapperMock, dataRecordMock, loggerFactoryMock);
    }

    public ISyntacticConstructorArgumentRecorder Recorder { get; }

    public Mock<ISyntacticMapper<TRecord>> MapperMock { get; }
    public Mock<TRecord> DataRecordMock { get; }

    public Mock<ISyntacticArgumentRecorderLoggerFactory> LoggerFactoryMock { get; }

    private RecorderContext(ISyntacticConstructorArgumentRecorder recorder, Mock<ISyntacticMapper<TRecord>> mapperMock, Mock<TRecord> dataRecordMock, Mock<ISyntacticArgumentRecorderLoggerFactory> loggerFactoryMock)
    {
        Recorder = recorder;

        MapperMock = mapperMock;
        DataRecordMock = dataRecordMock;

        LoggerFactoryMock = loggerFactoryMock;
    }
}