namespace SharpAttributeParser.Mappers.Repositories.AdaptiveCases.DetachedMappedAdaptiveParamsConstructorArgumentRecorderFactoryCases;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Adaptive;
using SharpAttributeParser.Mappers.Repositories.Combined;
using SharpAttributeParser.Mappers.Repositories.Semantic;

internal sealed class FactoryContext<TCombinedRecord, TSemanticRecord>
{
    public static FactoryContext<TCombinedRecord, TSemanticRecord> Create()
    {
        Mock<IDetachedMappedCombinedParamsConstructorArgumentRecorderFactory<TCombinedRecord>> combinedFactoryMock = new();
        Mock<IDetachedMappedSemanticConstructorArgumentRecorderFactory<TSemanticRecord>> semanticFactoryMock = new();

        DetachedMappedAdaptiveParamsConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> factory = new(combinedFactoryMock.Object, semanticFactoryMock.Object);

        return new(factory, combinedFactoryMock, semanticFactoryMock);
    }

    public DetachedMappedAdaptiveParamsConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> Factory { get; }

    public Mock<IDetachedMappedCombinedParamsConstructorArgumentRecorderFactory<TCombinedRecord>> CombinedFactoryMock { get; }
    public Mock<IDetachedMappedSemanticConstructorArgumentRecorderFactory<TSemanticRecord>> SemanticFactoryMock { get; }

    private FactoryContext(DetachedMappedAdaptiveParamsConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> factory, Mock<IDetachedMappedCombinedParamsConstructorArgumentRecorderFactory<TCombinedRecord>> combinedFactoryMock, Mock<IDetachedMappedSemanticConstructorArgumentRecorderFactory<TSemanticRecord>> semanticFactoryMock)
    {
        Factory = factory;

        CombinedFactoryMock = combinedFactoryMock;
        SemanticFactoryMock = semanticFactoryMock;
    }
}
