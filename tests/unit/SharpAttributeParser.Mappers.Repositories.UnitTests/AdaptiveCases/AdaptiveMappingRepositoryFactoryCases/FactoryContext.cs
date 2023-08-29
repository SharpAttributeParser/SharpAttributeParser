namespace SharpAttributeParser.Mappers.Repositories.AdaptiveCases.AdaptiveMappingRepositoryFactoryCases;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Adaptive;

internal sealed class FactoryContext<TCombinedRecord, TSemanticRecord>
{
    public static FactoryContext<TCombinedRecord, TSemanticRecord> Create()
    {
        Mock<ITypeMappingRepositoryFactory<IDetachedMappedAdaptiveTypeArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveTypeArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>>> typeRepositoryFactoryMock = new();
        Mock<IConstructorMappingRepositoryFactory<IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>>> constructorRepositoryFactoryMock = new();
        Mock<INamedMappingRepositoryFactory<IDetachedMappedAdaptiveNamedArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveNamedArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>>> namedRepositoryFactoryMock = new();

        AdaptiveMappingRepositoryFactory<TCombinedRecord, TSemanticRecord> factory = new(typeRepositoryFactoryMock.Object, constructorRepositoryFactoryMock.Object, namedRepositoryFactoryMock.Object);

        return new(factory, typeRepositoryFactoryMock, constructorRepositoryFactoryMock, namedRepositoryFactoryMock);
    }

    public AdaptiveMappingRepositoryFactory<TCombinedRecord, TSemanticRecord> Factory { get; }
    public Mock<ITypeMappingRepositoryFactory<IDetachedMappedAdaptiveTypeArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveTypeArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>>> TypeRepositoryFactoryMock { get; }
    public Mock<IConstructorMappingRepositoryFactory<IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>>> ConstructorRepositoryFactoryMock { get; }
    public Mock<INamedMappingRepositoryFactory<IDetachedMappedAdaptiveNamedArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveNamedArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>>> NamedRepositoryFactoryMock { get; }

    private FactoryContext(AdaptiveMappingRepositoryFactory<TCombinedRecord, TSemanticRecord> factory, Mock<ITypeMappingRepositoryFactory<IDetachedMappedAdaptiveTypeArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveTypeArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>>> typeRepositoryFactoryMock,
        Mock<IConstructorMappingRepositoryFactory<IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>>> constructorRepositoryFactoryMock,
        Mock<INamedMappingRepositoryFactory<IDetachedMappedAdaptiveNamedArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveNamedArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>>> namedRepositoryFactoryMock)
    {
        Factory = factory;

        TypeRepositoryFactoryMock = typeRepositoryFactoryMock;
        ConstructorRepositoryFactoryMock = constructorRepositoryFactoryMock;
        NamedRepositoryFactoryMock = namedRepositoryFactoryMock;
    }
}
