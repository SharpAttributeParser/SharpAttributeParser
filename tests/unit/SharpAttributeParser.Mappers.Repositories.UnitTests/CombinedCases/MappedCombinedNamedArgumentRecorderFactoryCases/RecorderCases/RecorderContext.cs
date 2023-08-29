namespace SharpAttributeParser.Mappers.Repositories.CombinedCases.MappedCombinedNamedArgumentRecorderFactoryCases.RecorderCases;

using Moq;

using SharpAttributeParser.Mappers.MappedRecorders;
using SharpAttributeParser.Mappers.Repositories.Combined;

internal sealed class RecorderContext<TRecord> where TRecord : class
{
    public static RecorderContext<TRecord> Create()
    {
        var dataRecord = Mock.Of<TRecord>();
        Mock<IDetachedMappedCombinedNamedArgumentRecorder<TRecord>> detachedRecorderMock = new();

        var recorder = ((IMappedCombinedNamedArgumentRecorderFactory)new MappedCombinedNamedArgumentRecorderFactory()).Create(dataRecord, detachedRecorderMock.Object);

        return new(recorder, dataRecord, detachedRecorderMock);
    }

    public IMappedCombinedNamedArgumentRecorder Recorder { get; }

    public TRecord DataRecord { get; }
    public Mock<IDetachedMappedCombinedNamedArgumentRecorder<TRecord>> DetachedRecorderMock { get; }

    private RecorderContext(IMappedCombinedNamedArgumentRecorder recorder, TRecord dataRecord, Mock<IDetachedMappedCombinedNamedArgumentRecorder<TRecord>> detachedRecorderMock)
    {
        Recorder = recorder;

        DataRecord = dataRecord;
        DetachedRecorderMock = detachedRecorderMock;
    }
}
