namespace SharpAttributeParser.Mappers.Repositories.CombinedCases.CombinedMappingRepositoryFactoryCases;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Combined;

internal sealed class FactoryContext<TRecord>
{
    public static FactoryContext<TRecord> Create()
    {
        Mock<ITypeMappingRepositoryFactory<IDetachedMappedCombinedTypeArgumentRecorder<TRecord>, IDetachedMappedCombinedTypeArgumentRecorderFactory<TRecord>>> typeRepositoryFactoryMock = new();
        Mock<IConstructorMappingRepositoryFactory<IDetachedMappedCombinedConstructorArgumentRecorder<TRecord>, IDetachedMappedCombinedConstructorArgumentRecorderFactory<TRecord>>> constructorRepositoryFactoryMock = new();
        Mock<INamedMappingRepositoryFactory<IDetachedMappedCombinedNamedArgumentRecorder<TRecord>, IDetachedMappedCombinedNamedArgumentRecorderFactory<TRecord>>> namedRepositoryFactoryMock = new();

        CombinedMappingRepositoryFactory<TRecord> factory = new(typeRepositoryFactoryMock.Object, constructorRepositoryFactoryMock.Object, namedRepositoryFactoryMock.Object);

        return new(factory, typeRepositoryFactoryMock, constructorRepositoryFactoryMock, namedRepositoryFactoryMock);
    }

    public CombinedMappingRepositoryFactory<TRecord> Factory { get; }
    public Mock<ITypeMappingRepositoryFactory<IDetachedMappedCombinedTypeArgumentRecorder<TRecord>, IDetachedMappedCombinedTypeArgumentRecorderFactory<TRecord>>> TypeRepositoryFactoryMock { get; }
    public Mock<IConstructorMappingRepositoryFactory<IDetachedMappedCombinedConstructorArgumentRecorder<TRecord>, IDetachedMappedCombinedConstructorArgumentRecorderFactory<TRecord>>> ConstructorRepositoryFactoryMock { get; }
    public Mock<INamedMappingRepositoryFactory<IDetachedMappedCombinedNamedArgumentRecorder<TRecord>, IDetachedMappedCombinedNamedArgumentRecorderFactory<TRecord>>> NamedRepositoryFactoryMock { get; }

    private FactoryContext(CombinedMappingRepositoryFactory<TRecord> factory, Mock<ITypeMappingRepositoryFactory<IDetachedMappedCombinedTypeArgumentRecorder<TRecord>, IDetachedMappedCombinedTypeArgumentRecorderFactory<TRecord>>> typeRepositoryFactoryMock,
        Mock<IConstructorMappingRepositoryFactory<IDetachedMappedCombinedConstructorArgumentRecorder<TRecord>, IDetachedMappedCombinedConstructorArgumentRecorderFactory<TRecord>>> constructorRepositoryFactoryMock,
        Mock<INamedMappingRepositoryFactory<IDetachedMappedCombinedNamedArgumentRecorder<TRecord>, IDetachedMappedCombinedNamedArgumentRecorderFactory<TRecord>>> namedRepositoryFactoryMock)
    {
        Factory = factory;

        TypeRepositoryFactoryMock = typeRepositoryFactoryMock;
        ConstructorRepositoryFactoryMock = constructorRepositoryFactoryMock;
        NamedRepositoryFactoryMock = namedRepositoryFactoryMock;
    }
}
