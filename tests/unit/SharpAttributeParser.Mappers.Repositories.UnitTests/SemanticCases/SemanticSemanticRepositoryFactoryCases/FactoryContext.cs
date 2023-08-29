namespace SharpAttributeParser.Mappers.Repositories.SemanticCases.SemanticMappingRepositoryFactoryCases;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Semantic;

internal sealed class FactoryContext<TRecord>
{
    public static FactoryContext<TRecord> Create()
    {
        Mock<ITypeMappingRepositoryFactory<IDetachedMappedSemanticTypeArgumentRecorder<TRecord>, IDetachedMappedSemanticTypeArgumentRecorderFactory<TRecord>>> typeRepositoryFactoryMock = new();
        Mock<IConstructorMappingRepositoryFactory<IDetachedMappedSemanticConstructorArgumentRecorder<TRecord>, IDetachedMappedSemanticConstructorArgumentRecorderFactory<TRecord>>> constructorRepositoryFactoryMock = new();
        Mock<INamedMappingRepositoryFactory<IDetachedMappedSemanticNamedArgumentRecorder<TRecord>, IDetachedMappedSemanticNamedArgumentRecorderFactory<TRecord>>> namedRepositoryFactoryMock = new();

        SemanticMappingRepositoryFactory<TRecord> factory = new(typeRepositoryFactoryMock.Object, constructorRepositoryFactoryMock.Object, namedRepositoryFactoryMock.Object);

        return new(factory, typeRepositoryFactoryMock, constructorRepositoryFactoryMock, namedRepositoryFactoryMock);
    }

    public SemanticMappingRepositoryFactory<TRecord> Factory { get; }
    public Mock<ITypeMappingRepositoryFactory<IDetachedMappedSemanticTypeArgumentRecorder<TRecord>, IDetachedMappedSemanticTypeArgumentRecorderFactory<TRecord>>> TypeRepositoryFactoryMock { get; }
    public Mock<IConstructorMappingRepositoryFactory<IDetachedMappedSemanticConstructorArgumentRecorder<TRecord>, IDetachedMappedSemanticConstructorArgumentRecorderFactory<TRecord>>> ConstructorRepositoryFactoryMock { get; }
    public Mock<INamedMappingRepositoryFactory<IDetachedMappedSemanticNamedArgumentRecorder<TRecord>, IDetachedMappedSemanticNamedArgumentRecorderFactory<TRecord>>> NamedRepositoryFactoryMock { get; }

    private FactoryContext(SemanticMappingRepositoryFactory<TRecord> factory, Mock<ITypeMappingRepositoryFactory<IDetachedMappedSemanticTypeArgumentRecorder<TRecord>, IDetachedMappedSemanticTypeArgumentRecorderFactory<TRecord>>> typeRepositoryFactoryMock,
        Mock<IConstructorMappingRepositoryFactory<IDetachedMappedSemanticConstructorArgumentRecorder<TRecord>, IDetachedMappedSemanticConstructorArgumentRecorderFactory<TRecord>>> constructorRepositoryFactoryMock,
        Mock<INamedMappingRepositoryFactory<IDetachedMappedSemanticNamedArgumentRecorder<TRecord>, IDetachedMappedSemanticNamedArgumentRecorderFactory<TRecord>>> namedRepositoryFactoryMock)
    {
        Factory = factory;

        TypeRepositoryFactoryMock = typeRepositoryFactoryMock;
        ConstructorRepositoryFactoryMock = constructorRepositoryFactoryMock;
        NamedRepositoryFactoryMock = namedRepositoryFactoryMock;
    }
}
