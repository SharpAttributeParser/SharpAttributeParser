﻿namespace SharpAttributeParser.Mappers.Repositories.SemanticCases.DetachedMappedSemanticTypeArgumentRecorderFactoryCases.ArgumentRecorderCases;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Semantic;

using System;

internal sealed class RecorderContext<TRecord>
{
    public static RecorderContext<TRecord> Create()
    {
        Mock<Func<TRecord, object?, bool>> recorderDelegateMock = new();

        var recorderFactory = new DetachedMappedSemanticTypeArgumentRecorderFactory<TRecord>();

        var recorder = ((IDetachedMappedSemanticTypeArgumentRecorderFactory<TRecord>)recorderFactory).Create(recorderDelegateMock.Object);

        return new(recorder, recorderDelegateMock);
    }

    public IDetachedMappedSemanticTypeArgumentRecorder<TRecord> Recorder { get; }

    public Mock<Func<TRecord, object?, bool>> RecorderDelegateMock { get; }

    private RecorderContext(IDetachedMappedSemanticTypeArgumentRecorder<TRecord> recorder, Mock<Func<TRecord, object?, bool>> recorderDelegateMock)
    {
        Recorder = recorder;

        RecorderDelegateMock = recorderDelegateMock;
    }
}
