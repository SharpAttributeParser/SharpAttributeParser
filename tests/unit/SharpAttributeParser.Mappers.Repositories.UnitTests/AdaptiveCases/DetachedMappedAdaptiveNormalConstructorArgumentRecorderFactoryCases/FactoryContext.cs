namespace SharpAttributeParser.Mappers.Repositories.AdaptiveCases.DetachedMappedAdaptiveNormalConstructorArgumentRecorderFactoryCases;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Adaptive;
using SharpAttributeParser.Mappers.Repositories.Combined;
using SharpAttributeParser.Mappers.Repositories.Semantic;

internal sealed class FactoryContext<TCombinedRecord, TSemanticRecord>
{
    public static FactoryContext<TCombinedRecord, TSemanticRecord> Create()
    {
        Mock<IDetachedMappedCombinedNormalConstructorArgumentRecorderFactory<TCombinedRecord>> combinedFactoryMock = new();
        Mock<IDetachedMappedSemanticConstructorArgumentRecorderFactory<TSemanticRecord>> semanticFactoryMock = new();

        DetachedMappedAdaptiveNormalConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> factory = new(combinedFactoryMock.Object, semanticFactoryMock.Object);

        return new(factory, combinedFactoryMock, semanticFactoryMock);
    }

    public DetachedMappedAdaptiveNormalConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> Factory { get; }

    public Mock<IDetachedMappedCombinedNormalConstructorArgumentRecorderFactory<TCombinedRecord>> CombinedFactoryMock { get; }
    public Mock<IDetachedMappedSemanticConstructorArgumentRecorderFactory<TSemanticRecord>> SemanticFactoryMock { get; }

    private FactoryContext(DetachedMappedAdaptiveNormalConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> factory, Mock<IDetachedMappedCombinedNormalConstructorArgumentRecorderFactory<TCombinedRecord>> combinedFactoryMock, Mock<IDetachedMappedSemanticConstructorArgumentRecorderFactory<TSemanticRecord>> semanticFactoryMock)
    {
        Factory = factory;

        CombinedFactoryMock = combinedFactoryMock;
        SemanticFactoryMock = semanticFactoryMock;
    }
}
