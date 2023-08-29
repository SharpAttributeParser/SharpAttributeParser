namespace SharpAttributeParser.Mappers.Repositories.SplitCases.DetachedMappedSplitParamsConstructorArgumentRecorderFactoryCases;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Mappers.Repositories.Split;
using SharpAttributeParser.Mappers.Repositories.Syntactic;

internal sealed class FactoryContext<TSemanticRecord, TSyntacticRecord>
{
    public static FactoryContext<TSemanticRecord, TSyntacticRecord> Create()
    {
        Mock<IDetachedMappedSemanticConstructorArgumentRecorderFactory<TSemanticRecord>> semanticFactoryMock = new();
        Mock<IDetachedMappedSyntacticParamsConstructorArgumentRecorderFactory<TSyntacticRecord>> syntacticFactoryMock = new();

        DetachedMappedSplitParamsConstructorArgumentRecorderFactory<TSemanticRecord, TSyntacticRecord> factory = new(semanticFactoryMock.Object, syntacticFactoryMock.Object);

        return new(factory, semanticFactoryMock, syntacticFactoryMock);
    }

    public DetachedMappedSplitParamsConstructorArgumentRecorderFactory<TSemanticRecord, TSyntacticRecord> Factory { get; }

    public Mock<IDetachedMappedSemanticConstructorArgumentRecorderFactory<TSemanticRecord>> SemanticFactoryMock { get; }
    public Mock<IDetachedMappedSyntacticParamsConstructorArgumentRecorderFactory<TSyntacticRecord>> SyntacticFactoryMock { get; }

    private FactoryContext(DetachedMappedSplitParamsConstructorArgumentRecorderFactory<TSemanticRecord, TSyntacticRecord> factory, Mock<IDetachedMappedSemanticConstructorArgumentRecorderFactory<TSemanticRecord>> semanticFactoryMock, Mock<IDetachedMappedSyntacticParamsConstructorArgumentRecorderFactory<TSyntacticRecord>> syntacticFactoryMock)
    {
        Factory = factory;

        SemanticFactoryMock = semanticFactoryMock;
        SyntacticFactoryMock = syntacticFactoryMock;
    }
}
