namespace SharpAttributeParser.Mappers.Repositories.SemanticCases.MappedSemanticConstructorArgumentRecorderFactoryCases.RecorderCases;

using Moq;

using SharpAttributeParser.Mappers.MappedRecorders;
using SharpAttributeParser.Mappers.Repositories.Semantic;

internal sealed class RecorderContext<TRecord> where TRecord : class
{
    public static RecorderContext<TRecord> Create()
    {
        var dataRecord = Mock.Of<TRecord>();
        Mock<IDetachedMappedSemanticConstructorArgumentRecorder<TRecord>> detachedRecorderMock = new();

        var recorder = ((IMappedSemanticConstructorArgumentRecorderFactory)new MappedSemanticConstructorArgumentRecorderFactory()).Create(dataRecord, detachedRecorderMock.Object);

        return new(recorder, dataRecord, detachedRecorderMock);
    }

    public IMappedSemanticConstructorArgumentRecorder Recorder { get; }

    public TRecord DataRecord { get; }
    public Mock<IDetachedMappedSemanticConstructorArgumentRecorder<TRecord>> DetachedRecorderMock { get; }

    private RecorderContext(IMappedSemanticConstructorArgumentRecorder recorder, TRecord dataRecord, Mock<IDetachedMappedSemanticConstructorArgumentRecorder<TRecord>> detachedRecorderMock)
    {
        Recorder = recorder;

        DataRecord = dataRecord;
        DetachedRecorderMock = detachedRecorderMock;
    }
}
