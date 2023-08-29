namespace SharpAttributeParser.Mappers.Repositories.SemanticCases.DetachedMappedSemanticNamedArgumentRecorderFactoryCases.PatternedArgumentRecorderCases;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Patterns;

using System;

internal sealed class RecorderContext<TRecord, TArgument>
{
    public static RecorderContext<TRecord, TArgument> Create()
    {
        Mock<IArgumentPattern<TArgument>> patternMock = new();
        Mock<Func<TRecord, TArgument, bool>> recorderDelegateMock = new();

        var recorderFactory = new DetachedMappedSemanticNamedArgumentRecorderFactory<TRecord>(Mock.Of<IArgumentPatternFactory>());

        var recorder = ((IDetachedMappedSemanticNamedArgumentRecorderFactory<TRecord>)recorderFactory).Create(patternMock.Object, recorderDelegateMock.Object);

        return new(recorder, patternMock, recorderDelegateMock);
    }

    public IDetachedMappedSemanticNamedArgumentRecorder<TRecord> Recorder { get; }

    public Mock<IArgumentPattern<TArgument>> PatternMock { get; }
    public Mock<Func<TRecord, TArgument, bool>> RecorderDelegateMock { get; }

    private RecorderContext(IDetachedMappedSemanticNamedArgumentRecorder<TRecord> recorder, Mock<IArgumentPattern<TArgument>> patternMock, Mock<Func<TRecord, TArgument, bool>> recorderDelegateMock)
    {
        Recorder = recorder;

        PatternMock = patternMock;
        RecorderDelegateMock = recorderDelegateMock;
    }
}
