namespace SharpAttributeParser.Mappers.Repositories.AdaptiveCases.DetachedMappedAdaptiveNamedArgumentRecorderFactoryCases;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Adaptive;
using SharpAttributeParser.Mappers.Repositories.Combined;
using SharpAttributeParser.Mappers.Repositories.Semantic;

internal sealed class FactoryContext<TCombinedRecord, TSemanticRecord>
{
    public static FactoryContext<TCombinedRecord, TSemanticRecord> Create()
    {
        Mock<IDetachedMappedCombinedNamedArgumentRecorderFactory<TCombinedRecord>> combinedFactoryMock = new();
        Mock<IDetachedMappedSemanticNamedArgumentRecorderFactory<TSemanticRecord>> semanticFactoryMock = new();

        DetachedMappedAdaptiveNamedArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> factory = new(combinedFactoryMock.Object, semanticFactoryMock.Object);

        return new(factory, combinedFactoryMock, semanticFactoryMock);
    }

    public DetachedMappedAdaptiveNamedArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> Factory { get; }

    public Mock<IDetachedMappedCombinedNamedArgumentRecorderFactory<TCombinedRecord>> CombinedFactoryMock { get; }
    public Mock<IDetachedMappedSemanticNamedArgumentRecorderFactory<TSemanticRecord>> SemanticFactoryMock { get; }

    private FactoryContext(DetachedMappedAdaptiveNamedArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> factory, Mock<IDetachedMappedCombinedNamedArgumentRecorderFactory<TCombinedRecord>> combinedFactoryMock, Mock<IDetachedMappedSemanticNamedArgumentRecorderFactory<TSemanticRecord>> semanticFactoryMock)
    {
        Factory = factory;

        CombinedFactoryMock = combinedFactoryMock;
        SemanticFactoryMock = semanticFactoryMock;
    }
}
