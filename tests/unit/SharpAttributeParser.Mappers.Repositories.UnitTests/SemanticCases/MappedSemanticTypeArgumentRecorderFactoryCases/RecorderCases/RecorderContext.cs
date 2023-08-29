namespace SharpAttributeParser.Mappers.Repositories.SemanticCases.MappedSemanticTypeArgumentRecorderFactoryCases.RecorderCases;

using Moq;

using SharpAttributeParser.Mappers.MappedRecorders;
using SharpAttributeParser.Mappers.Repositories.Semantic;

internal sealed class RecorderContext<TRecord> where TRecord : class
{
    public static RecorderContext<TRecord> Create()
    {
        var dataRecord = Mock.Of<TRecord>();
        Mock<IDetachedMappedSemanticTypeArgumentRecorder<TRecord>> detachedRecorderMock = new();

        var recorder = ((IMappedSemanticTypeArgumentRecorderFactory)new MappedSemanticTypeArgumentRecorderFactory()).Create(dataRecord, detachedRecorderMock.Object);

        return new(recorder, dataRecord, detachedRecorderMock);
    }

    public IMappedSemanticTypeArgumentRecorder Recorder { get; }

    public TRecord DataRecord { get; }
    public Mock<IDetachedMappedSemanticTypeArgumentRecorder<TRecord>> DetachedRecorderMock { get; }

    private RecorderContext(IMappedSemanticTypeArgumentRecorder recorder, TRecord dataRecord, Mock<IDetachedMappedSemanticTypeArgumentRecorder<TRecord>> detachedRecorderMock)
    {
        Recorder = recorder;

        DataRecord = dataRecord;
        DetachedRecorderMock = detachedRecorderMock;
    }
}
