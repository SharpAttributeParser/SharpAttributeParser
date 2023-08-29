namespace SharpAttributeParser.Mappers.Repositories.CombinedCases.MappedCombinedConstructorArgumentRecorderFactoryCases.RecorderCases;

using Moq;

using SharpAttributeParser.Mappers.MappedRecorders;
using SharpAttributeParser.Mappers.Repositories.Combined;

internal sealed class RecorderContext<TRecord> where TRecord : class
{
    public static RecorderContext<TRecord> Create()
    {
        var dataRecord = Mock.Of<TRecord>();
        Mock<IDetachedMappedCombinedConstructorArgumentRecorder<TRecord>> detachedRecorderMock = new();

        var recorder = ((IMappedCombinedConstructorArgumentRecorderFactory)new MappedCombinedConstructorArgumentRecorderFactory()).Create(dataRecord, detachedRecorderMock.Object);

        return new(recorder, dataRecord, detachedRecorderMock);
    }

    public IMappedCombinedConstructorArgumentRecorder Recorder { get; }

    public TRecord DataRecord { get; }
    public Mock<IDetachedMappedCombinedConstructorArgumentRecorder<TRecord>> DetachedRecorderMock { get; }

    private RecorderContext(IMappedCombinedConstructorArgumentRecorder recorder, TRecord dataRecord, Mock<IDetachedMappedCombinedConstructorArgumentRecorder<TRecord>> detachedRecorderMock)
    {
        Recorder = recorder;

        DataRecord = dataRecord;
        DetachedRecorderMock = detachedRecorderMock;
    }
}
