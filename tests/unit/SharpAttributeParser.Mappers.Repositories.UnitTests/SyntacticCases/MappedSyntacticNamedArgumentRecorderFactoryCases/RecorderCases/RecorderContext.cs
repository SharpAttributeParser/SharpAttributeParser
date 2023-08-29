namespace SharpAttributeParser.Mappers.Repositories.SyntacticCases.MappedSyntacticNamedArgumentRecorderFactoryCases.RecorderCases;

using Moq;

using SharpAttributeParser.Mappers.MappedRecorders;
using SharpAttributeParser.Mappers.Repositories.Syntactic;

internal sealed class RecorderContext<TRecord> where TRecord : class
{
    public static RecorderContext<TRecord> Create()
    {
        var dataRecord = Mock.Of<TRecord>();
        Mock<IDetachedMappedSyntacticNamedArgumentRecorder<TRecord>> detachedRecorderMock = new();

        var recorder = ((IMappedSyntacticNamedArgumentRecorderFactory)new MappedSyntacticNamedArgumentRecorderFactory()).Create(dataRecord, detachedRecorderMock.Object);

        return new(recorder, dataRecord, detachedRecorderMock);
    }

    public IMappedSyntacticNamedArgumentRecorder Recorder { get; }

    public TRecord DataRecord { get; }
    public Mock<IDetachedMappedSyntacticNamedArgumentRecorder<TRecord>> DetachedRecorderMock { get; }

    private RecorderContext(IMappedSyntacticNamedArgumentRecorder recorder, TRecord dataRecord, Mock<IDetachedMappedSyntacticNamedArgumentRecorder<TRecord>> detachedRecorderMock)
    {
        Recorder = recorder;

        DataRecord = dataRecord;
        DetachedRecorderMock = detachedRecorderMock;
    }
}
