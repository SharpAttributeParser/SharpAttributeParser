namespace SharpAttributeParser.Mappers.Repositories.CombinedCases.MappedCombinedTypeArgumentRecorderFactoryCases.RecorderCases;

using Moq;

using SharpAttributeParser.Mappers.MappedRecorders;
using SharpAttributeParser.Mappers.Repositories.Combined;

internal sealed class RecorderContext<TRecord> where TRecord : class
{
    public static RecorderContext<TRecord> Create()
    {
        var dataRecord = Mock.Of<TRecord>();
        Mock<IDetachedMappedCombinedTypeArgumentRecorder<TRecord>> detachedRecorderMock = new();

        var recorder = ((IMappedCombinedTypeArgumentRecorderFactory)new MappedCombinedTypeArgumentRecorderFactory()).Create(dataRecord, detachedRecorderMock.Object);

        return new(recorder, dataRecord, detachedRecorderMock);
    }

    public IMappedCombinedTypeArgumentRecorder Recorder { get; }

    public TRecord DataRecord { get; }
    public Mock<IDetachedMappedCombinedTypeArgumentRecorder<TRecord>> DetachedRecorderMock { get; }

    private RecorderContext(IMappedCombinedTypeArgumentRecorder recorder, TRecord dataRecord, Mock<IDetachedMappedCombinedTypeArgumentRecorder<TRecord>> detachedRecorderMock)
    {
        Recorder = recorder;

        DataRecord = dataRecord;
        DetachedRecorderMock = detachedRecorderMock;
    }
}
