namespace SharpAttributeParser.Mappers.Repositories.SplitCases.DetachedMappedSplitNamedArgumentRecorderFactoryCases;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Mappers.Repositories.Split;
using SharpAttributeParser.Mappers.Repositories.Syntactic;

internal sealed class FactoryContext<TSemanticRecord, TSyntacticRecord>
{
    public static FactoryContext<TSemanticRecord, TSyntacticRecord> Create()
    {
        Mock<IDetachedMappedSemanticNamedArgumentRecorderFactory<TSemanticRecord>> semanticFactoryMock = new();
        Mock<IDetachedMappedSyntacticNamedArgumentRecorderFactory<TSyntacticRecord>> syntacticFactoryMock = new();

        DetachedMappedSplitNamedArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> factory = new(semanticFactoryMock.Object, syntacticFactoryMock.Object);

        return new(factory, semanticFactoryMock, syntacticFactoryMock);
    }

    public DetachedMappedSplitNamedArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> Factory { get; }

    public Mock<IDetachedMappedSemanticNamedArgumentRecorderFactory<TSemanticRecord>> SemanticFactoryMock { get; }
    public Mock<IDetachedMappedSyntacticNamedArgumentRecorderFactory<TSyntacticRecord>> SyntacticFactoryMock { get; }

    private FactoryContext(DetachedMappedSplitNamedArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> factory, Mock<IDetachedMappedSemanticNamedArgumentRecorderFactory<TSemanticRecord>> semanticFactoryMock, Mock<IDetachedMappedSyntacticNamedArgumentRecorderFactory<TSyntacticRecord>> syntacticFactoryMock)
    {
        Factory = factory;

        SemanticFactoryMock = semanticFactoryMock;
        SyntacticFactoryMock = syntacticFactoryMock;
    }
}
