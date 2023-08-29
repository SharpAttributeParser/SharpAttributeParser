namespace SharpAttributeParser.Mappers.Repositories.AdaptiveCases.DetachedMappedAdaptiveTypeArgumentRecorderFactoryCases;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Adaptive;
using SharpAttributeParser.Mappers.Repositories.Combined;
using SharpAttributeParser.Mappers.Repositories.Semantic;

internal sealed class FactoryContext<TCombinedRecord, TSemanticRecord>
{
    public static FactoryContext<TCombinedRecord, TSemanticRecord> Create()
    {
        Mock<IDetachedMappedCombinedTypeArgumentRecorderFactory<TCombinedRecord>> combinedFactoryMock = new();
        Mock<IDetachedMappedSemanticTypeArgumentRecorderFactory<TSemanticRecord>> semanticFactoryMock = new();

        DetachedMappedAdaptiveTypeArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> factory = new(combinedFactoryMock.Object, semanticFactoryMock.Object);

        return new(factory, combinedFactoryMock, semanticFactoryMock);
    }

    public DetachedMappedAdaptiveTypeArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> Factory { get; }

    public Mock<IDetachedMappedCombinedTypeArgumentRecorderFactory<TCombinedRecord>> CombinedFactoryMock { get; }
    public Mock<IDetachedMappedSemanticTypeArgumentRecorderFactory<TSemanticRecord>> SemanticFactoryMock { get; }

    private FactoryContext(DetachedMappedAdaptiveTypeArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> factory, Mock<IDetachedMappedCombinedTypeArgumentRecorderFactory<TCombinedRecord>> combinedFactoryMock, Mock<IDetachedMappedSemanticTypeArgumentRecorderFactory<TSemanticRecord>> semanticFactoryMock)
    {
        Factory = factory;

        CombinedFactoryMock = combinedFactoryMock;
        SemanticFactoryMock = semanticFactoryMock;
    }
}
