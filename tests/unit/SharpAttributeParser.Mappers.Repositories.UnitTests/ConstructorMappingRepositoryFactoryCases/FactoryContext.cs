namespace SharpAttributeParser.Mappers.Repositories.ConstructorMappingRepositoryFactoryCases;

using Moq;

internal sealed class FactoryContext<TRecorder, TRecorderFactory> where TRecorderFactory : class
{
    public static FactoryContext<TRecorder, TRecorderFactory> Create()
    {
        Mock<TRecorderFactory> recorderFactoryMock = new();

        ConstructorMappingRepositoryFactory<TRecorder, TRecorderFactory> factory = new(recorderFactoryMock.Object);

        return new(factory, recorderFactoryMock);
    }

    public ConstructorMappingRepositoryFactory<TRecorder, TRecorderFactory> Factory { get; }

    public Mock<TRecorderFactory> RecorderFactoryMock { get; }
    public TRecorderFactory RecorderFactory => RecorderFactoryMock.Object;

    private FactoryContext(ConstructorMappingRepositoryFactory<TRecorder, TRecorderFactory> factory, Mock<TRecorderFactory> recorderFactoryMock)
    {
        Factory = factory;

        RecorderFactoryMock = recorderFactoryMock;
    }
}
