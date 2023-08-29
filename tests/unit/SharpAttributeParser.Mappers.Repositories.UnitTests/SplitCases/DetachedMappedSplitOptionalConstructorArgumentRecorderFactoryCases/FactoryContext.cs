namespace SharpAttributeParser.Mappers.Repositories.SplitCases.DetachedMappedSplitOptionalConstructorArgumentRecorderFactoryCases;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Mappers.Repositories.Split;
using SharpAttributeParser.Mappers.Repositories.Syntactic;

internal sealed class FactoryContext<TSemanticRecord, TSyntacticRecord>
{
    public static FactoryContext<TSemanticRecord, TSyntacticRecord> Create()
    {
        Mock<IDetachedMappedSemanticConstructorArgumentRecorderFactory<TSemanticRecord>> semanticFactoryMock = new();
        Mock<IDetachedMappedSyntacticOptionalConstructorArgumentRecorderFactory<TSyntacticRecord>> syntacticFactoryMock = new();

        DetachedMappedSplitOptionalConstructorArgumentRecorderFactory<TSemanticRecord, TSyntacticRecord> factory = new(semanticFactoryMock.Object, syntacticFactoryMock.Object);

        return new(factory, semanticFactoryMock, syntacticFactoryMock);
    }

    public DetachedMappedSplitOptionalConstructorArgumentRecorderFactory<TSemanticRecord, TSyntacticRecord> Factory { get; }

    public Mock<IDetachedMappedSemanticConstructorArgumentRecorderFactory<TSemanticRecord>> SemanticFactoryMock { get; }
    public Mock<IDetachedMappedSyntacticOptionalConstructorArgumentRecorderFactory<TSyntacticRecord>> SyntacticFactoryMock { get; }

    private FactoryContext(DetachedMappedSplitOptionalConstructorArgumentRecorderFactory<TSemanticRecord, TSyntacticRecord> factory, Mock<IDetachedMappedSemanticConstructorArgumentRecorderFactory<TSemanticRecord>> semanticFactoryMock, Mock<IDetachedMappedSyntacticOptionalConstructorArgumentRecorderFactory<TSyntacticRecord>> syntacticFactoryMock)
    {
        Factory = factory;

        SemanticFactoryMock = semanticFactoryMock;
        SyntacticFactoryMock = syntacticFactoryMock;
    }
}
