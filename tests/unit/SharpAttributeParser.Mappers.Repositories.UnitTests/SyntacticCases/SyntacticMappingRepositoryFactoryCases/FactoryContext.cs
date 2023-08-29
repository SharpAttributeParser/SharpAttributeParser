namespace SharpAttributeParser.Mappers.Repositories.SyntacticCases.SyntacticMappingRepositoryFactoryCases;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Syntactic;

internal sealed class FactoryContext<TRecord>
{
    public static FactoryContext<TRecord> Create()
    {
        Mock<ITypeMappingRepositoryFactory<IDetachedMappedSyntacticTypeArgumentRecorder<TRecord>, IDetachedMappedSyntacticTypeArgumentRecorderFactory<TRecord>>> typeRepositoryFactoryMock = new();
        Mock<IConstructorMappingRepositoryFactory<IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord>, IDetachedMappedSyntacticConstructorArgumentRecorderFactory<TRecord>>> constructorRepositoryFactoryMock = new();
        Mock<INamedMappingRepositoryFactory<IDetachedMappedSyntacticNamedArgumentRecorder<TRecord>, IDetachedMappedSyntacticNamedArgumentRecorderFactory<TRecord>>> namedRepositoryFactoryMock = new();

        SyntacticMappingRepositoryFactory<TRecord> factory = new(typeRepositoryFactoryMock.Object, constructorRepositoryFactoryMock.Object, namedRepositoryFactoryMock.Object);

        return new(factory, typeRepositoryFactoryMock, constructorRepositoryFactoryMock, namedRepositoryFactoryMock);
    }

    public SyntacticMappingRepositoryFactory<TRecord> Factory { get; }
    public Mock<ITypeMappingRepositoryFactory<IDetachedMappedSyntacticTypeArgumentRecorder<TRecord>, IDetachedMappedSyntacticTypeArgumentRecorderFactory<TRecord>>> TypeRepositoryFactoryMock { get; }
    public Mock<IConstructorMappingRepositoryFactory<IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord>, IDetachedMappedSyntacticConstructorArgumentRecorderFactory<TRecord>>> ConstructorRepositoryFactoryMock { get; }
    public Mock<INamedMappingRepositoryFactory<IDetachedMappedSyntacticNamedArgumentRecorder<TRecord>, IDetachedMappedSyntacticNamedArgumentRecorderFactory<TRecord>>> NamedRepositoryFactoryMock { get; }

    private FactoryContext(SyntacticMappingRepositoryFactory<TRecord> factory, Mock<ITypeMappingRepositoryFactory<IDetachedMappedSyntacticTypeArgumentRecorder<TRecord>, IDetachedMappedSyntacticTypeArgumentRecorderFactory<TRecord>>> typeRepositoryFactoryMock,
        Mock<IConstructorMappingRepositoryFactory<IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord>, IDetachedMappedSyntacticConstructorArgumentRecorderFactory<TRecord>>> constructorRepositoryFactoryMock,
        Mock<INamedMappingRepositoryFactory<IDetachedMappedSyntacticNamedArgumentRecorder<TRecord>, IDetachedMappedSyntacticNamedArgumentRecorderFactory<TRecord>>> namedRepositoryFactoryMock)
    {
        Factory = factory;

        TypeRepositoryFactoryMock = typeRepositoryFactoryMock;
        ConstructorRepositoryFactoryMock = constructorRepositoryFactoryMock;
        NamedRepositoryFactoryMock = namedRepositoryFactoryMock;
    }
}
