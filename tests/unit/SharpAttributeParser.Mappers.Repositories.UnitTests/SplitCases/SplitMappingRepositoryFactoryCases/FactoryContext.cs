namespace SharpAttributeParser.Mappers.Repositories.SplitCases.SplitMappingRepositoryFactoryCases;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Split;

internal sealed class FactoryContext<TSemanticRecord, TSyntacticRecord>
{
    public static FactoryContext<TSemanticRecord, TSyntacticRecord> Create()
    {
        Mock<ITypeMappingRepositoryFactory<IDetachedMappedSplitTypeArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitTypeArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>>> typeRepositoryFactoryMock = new();
        Mock<IConstructorMappingRepositoryFactory<IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>>> constructorRepositoryFactoryMock = new();
        Mock<INamedMappingRepositoryFactory<IDetachedMappedSplitNamedArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitNamedArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>>> namedRepositoryFactoryMock = new();

        SplitMappingRepositoryFactory<TSemanticRecord, TSyntacticRecord> factory = new(typeRepositoryFactoryMock.Object, constructorRepositoryFactoryMock.Object, namedRepositoryFactoryMock.Object);

        return new(factory, typeRepositoryFactoryMock, constructorRepositoryFactoryMock, namedRepositoryFactoryMock);
    }

    public SplitMappingRepositoryFactory<TSemanticRecord, TSyntacticRecord> Factory { get; }
    public Mock<ITypeMappingRepositoryFactory<IDetachedMappedSplitTypeArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitTypeArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>>> TypeRepositoryFactoryMock { get; }
    public Mock<IConstructorMappingRepositoryFactory<IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>>> ConstructorRepositoryFactoryMock { get; }
    public Mock<INamedMappingRepositoryFactory<IDetachedMappedSplitNamedArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitNamedArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>>> NamedRepositoryFactoryMock { get; }

    private FactoryContext(SplitMappingRepositoryFactory<TSemanticRecord, TSyntacticRecord> factory, Mock<ITypeMappingRepositoryFactory<IDetachedMappedSplitTypeArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitTypeArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>>> typeRepositoryFactoryMock,
        Mock<IConstructorMappingRepositoryFactory<IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>>> constructorRepositoryFactoryMock,
        Mock<INamedMappingRepositoryFactory<IDetachedMappedSplitNamedArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitNamedArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>>> namedRepositoryFactoryMock)
    {
        Factory = factory;

        TypeRepositoryFactoryMock = typeRepositoryFactoryMock;
        ConstructorRepositoryFactoryMock = constructorRepositoryFactoryMock;
        NamedRepositoryFactoryMock = namedRepositoryFactoryMock;
    }
}
