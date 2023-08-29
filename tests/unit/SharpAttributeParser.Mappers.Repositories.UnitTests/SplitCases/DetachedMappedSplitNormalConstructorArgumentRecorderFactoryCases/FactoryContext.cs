namespace SharpAttributeParser.Mappers.Repositories.SplitCases.DetachedMappedSplitNormalConstructorArgumentRecorderFactoryCases;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Mappers.Repositories.Split;
using SharpAttributeParser.Mappers.Repositories.Syntactic;

internal sealed class FactoryContext<TSemanticRecord, TSyntacticRecord>
{
    public static FactoryContext<TSemanticRecord, TSyntacticRecord> Create()
    {
        Mock<IDetachedMappedSemanticConstructorArgumentRecorderFactory<TSemanticRecord>> semanticFactoryMock = new();
        Mock<IDetachedMappedSyntacticNormalConstructorArgumentRecorderFactory<TSyntacticRecord>> syntacticFactoryMock = new();

        DetachedMappedSplitNormalConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> factory = new(semanticFactoryMock.Object, syntacticFactoryMock.Object);

        return new(factory, semanticFactoryMock, syntacticFactoryMock);
    }

    public DetachedMappedSplitNormalConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> Factory { get; }

    public Mock<IDetachedMappedSemanticConstructorArgumentRecorderFactory<TSemanticRecord>> SemanticFactoryMock { get; }
    public Mock<IDetachedMappedSyntacticNormalConstructorArgumentRecorderFactory<TSyntacticRecord>> SyntacticFactoryMock { get; }

    private FactoryContext(DetachedMappedSplitNormalConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> factory, Mock<IDetachedMappedSemanticConstructorArgumentRecorderFactory<TSemanticRecord>> semanticFactoryMock, Mock<IDetachedMappedSyntacticNormalConstructorArgumentRecorderFactory<TSyntacticRecord>> syntacticFactoryMock)
    {
        Factory = factory;

        SemanticFactoryMock = semanticFactoryMock;
        SyntacticFactoryMock = syntacticFactoryMock;
    }
}
