namespace SharpAttributeParser.Mappers.Repositories.TypeMappingRepositoryFactoryCases;

using Moq;

internal sealed class FactoryContext<TRecorder, TRecorderFactory> where TRecorderFactory : class
{
    public static FactoryContext<TRecorder, TRecorderFactory> Create()
    {
        Mock<TRecorderFactory> recorderFactoryMock = new();

        TypeMappingRepositoryFactory<TRecorder, TRecorderFactory> factory = new(recorderFactoryMock.Object);

        return new(factory, recorderFactoryMock);
    }

    public TypeMappingRepositoryFactory<TRecorder, TRecorderFactory> Factory { get; }

    public Mock<TRecorderFactory> RecorderFactoryMock { get; }
    public TRecorderFactory RecorderFactory => RecorderFactoryMock.Object;

    private FactoryContext(TypeMappingRepositoryFactory<TRecorder, TRecorderFactory> factory, Mock<TRecorderFactory> recorderFactoryMock)
    {
        Factory = factory;

        RecorderFactoryMock = recorderFactoryMock;
    }
}
