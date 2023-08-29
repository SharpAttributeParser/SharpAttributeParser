namespace SharpAttributeParser.Mappers.Repositories.SyntacticCases.MappedSyntacticTypeArgumentRecorderFactoryCases.RecorderCases;

using Moq;

using SharpAttributeParser.Mappers.MappedRecorders;
using SharpAttributeParser.Mappers.Repositories.Syntactic;

internal sealed class RecorderContext<TRecord> where TRecord : class
{
    public static RecorderContext<TRecord> Create()
    {
        var dataRecord = Mock.Of<TRecord>();
        Mock<IDetachedMappedSyntacticTypeArgumentRecorder<TRecord>> detachedRecorderMock = new();

        var recorder = ((IMappedSyntacticTypeArgumentRecorderFactory)new MappedSyntacticTypeArgumentRecorderFactory()).Create(dataRecord, detachedRecorderMock.Object);

        return new(recorder, dataRecord, detachedRecorderMock);
    }

    public IMappedSyntacticTypeArgumentRecorder Recorder { get; }

    public TRecord DataRecord { get; }
    public Mock<IDetachedMappedSyntacticTypeArgumentRecorder<TRecord>> DetachedRecorderMock { get; }

    private RecorderContext(IMappedSyntacticTypeArgumentRecorder recorder, TRecord dataRecord, Mock<IDetachedMappedSyntacticTypeArgumentRecorder<TRecord>> detachedRecorderMock)
    {
        Recorder = recorder;

        DataRecord = dataRecord;
        DetachedRecorderMock = detachedRecorderMock;
    }
}
