namespace SharpAttributeParser.Mappers.Repositories.SemanticCases.MappedSemanticNamedArgumentRecorderFactoryCases.RecorderCases;

using Moq;

using SharpAttributeParser.Mappers.MappedRecorders;
using SharpAttributeParser.Mappers.Repositories.Semantic;

internal sealed class RecorderContext<TRecord> where TRecord : class
{
    public static RecorderContext<TRecord> Create()
    {
        var dataRecord = Mock.Of<TRecord>();
        Mock<IDetachedMappedSemanticNamedArgumentRecorder<TRecord>> detachedRecorderMock = new();

        var recorder = ((IMappedSemanticNamedArgumentRecorderFactory)new MappedSemanticNamedArgumentRecorderFactory()).Create(dataRecord, detachedRecorderMock.Object);

        return new(recorder, dataRecord, detachedRecorderMock);
    }

    public IMappedSemanticNamedArgumentRecorder Recorder { get; }

    public TRecord DataRecord { get; }
    public Mock<IDetachedMappedSemanticNamedArgumentRecorder<TRecord>> DetachedRecorderMock { get; }

    private RecorderContext(IMappedSemanticNamedArgumentRecorder recorder, TRecord dataRecord, Mock<IDetachedMappedSemanticNamedArgumentRecorder<TRecord>> detachedRecorderMock)
    {
        Recorder = recorder;

        DataRecord = dataRecord;
        DetachedRecorderMock = detachedRecorderMock;
    }
}
