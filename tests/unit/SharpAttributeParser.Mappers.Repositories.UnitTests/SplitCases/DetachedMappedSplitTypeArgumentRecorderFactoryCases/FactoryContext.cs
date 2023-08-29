namespace SharpAttributeParser.Mappers.Repositories.SplitCases.DetachedMappedSplitTypeArgumentRecorderFactoryCases;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Mappers.Repositories.Split;
using SharpAttributeParser.Mappers.Repositories.Syntactic;

internal sealed class FactoryContext<TSemanticRecord, TSyntacticRecord>
{
    public static FactoryContext<TSemanticRecord, TSyntacticRecord> Create()
    {
        Mock<IDetachedMappedSemanticTypeArgumentRecorderFactory<TSemanticRecord>> semanticFactoryMock = new();
        Mock<IDetachedMappedSyntacticTypeArgumentRecorderFactory<TSyntacticRecord>> syntacticFactoryMock = new();

        DetachedMappedSplitTypeArgumentRecorderFactory<TSemanticRecord, TSyntacticRecord> factory = new(semanticFactoryMock.Object, syntacticFactoryMock.Object);

        return new(factory, semanticFactoryMock, syntacticFactoryMock);
    }

    public DetachedMappedSplitTypeArgumentRecorderFactory<TSemanticRecord, TSyntacticRecord> Factory { get; }

    public Mock<IDetachedMappedSemanticTypeArgumentRecorderFactory<TSemanticRecord>> SemanticFactoryMock { get; }
    public Mock<IDetachedMappedSyntacticTypeArgumentRecorderFactory<TSyntacticRecord>> SyntacticFactoryMock { get; }

    private FactoryContext(DetachedMappedSplitTypeArgumentRecorderFactory<TSemanticRecord, TSyntacticRecord> factory, Mock<IDetachedMappedSemanticTypeArgumentRecorderFactory<TSemanticRecord>> semanticFactoryMock, Mock<IDetachedMappedSyntacticTypeArgumentRecorderFactory<TSyntacticRecord>> syntacticFactoryMock)
    {
        Factory = factory;

        SemanticFactoryMock = semanticFactoryMock;
        SyntacticFactoryMock = syntacticFactoryMock;
    }
}
