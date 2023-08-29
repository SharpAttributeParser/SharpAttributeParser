namespace SharpAttributeParser.Mappers.Repositories.SyntacticCases.MappedSyntacticConstructorArgumentRecorderFactoryCases.RecorderCases;

using Moq;

using SharpAttributeParser.Mappers.MappedRecorders;
using SharpAttributeParser.Mappers.Repositories.Syntactic;

internal sealed class RecorderContext<TRecord> where TRecord : class
{
    public static RecorderContext<TRecord> Create()
    {
        var dataRecord = Mock.Of<TRecord>();
        Mock<IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord>> detachedRecorderMock = new();

        var recorder = ((IMappedSyntacticConstructorArgumentRecorderFactory)new MappedSyntacticConstructorArgumentRecorderFactory()).Create(dataRecord, detachedRecorderMock.Object);

        return new(recorder, dataRecord, detachedRecorderMock);
    }

    public IMappedSyntacticConstructorArgumentRecorder Recorder { get; }

    public TRecord DataRecord { get; }
    public Mock<IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord>> DetachedRecorderMock { get; }

    private RecorderContext(IMappedSyntacticConstructorArgumentRecorder recorder, TRecord dataRecord, Mock<IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord>> detachedRecorderMock)
    {
        Recorder = recorder;

        DataRecord = dataRecord;
        DetachedRecorderMock = detachedRecorderMock;
    }
}
