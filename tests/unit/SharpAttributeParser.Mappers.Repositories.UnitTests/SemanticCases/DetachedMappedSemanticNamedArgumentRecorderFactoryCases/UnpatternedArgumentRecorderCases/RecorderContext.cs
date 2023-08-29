namespace SharpAttributeParser.Mappers.Repositories.SemanticCases.DetachedMappedSemanticNamedArgumentRecorderFactoryCases.UnpatternedArgumentRecorderCases;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Patterns;

using System;

internal sealed class RecorderContext<TRecord>
{
    public static RecorderContext<TRecord> Create()
    {
        Mock<Func<TRecord, object?, bool>> recorderDelegateMock = new();

        var recorderFactory = new DetachedMappedSemanticNamedArgumentRecorderFactory<TRecord>(Mock.Of<IArgumentPatternFactory>());

        var recorder = ((IDetachedMappedSemanticNamedArgumentRecorderFactory<TRecord>)recorderFactory).Create(recorderDelegateMock.Object);

        return new(recorder, recorderDelegateMock);
    }

    public IDetachedMappedSemanticNamedArgumentRecorder<TRecord> Recorder { get; }

    public Mock<Func<TRecord, object?, bool>> RecorderDelegateMock { get; }

    private RecorderContext(IDetachedMappedSemanticNamedArgumentRecorder<TRecord> recorder, Mock<Func<TRecord, object?, bool>> recorderDelegateMock)
    {
        Recorder = recorder;

        RecorderDelegateMock = recorderDelegateMock;
    }
}
