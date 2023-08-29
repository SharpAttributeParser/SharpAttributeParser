namespace SharpAttributeParser.Mappers.Repositories.AdaptiveCases.DetachedMappedAdaptiveOptionalConstructorArgumentRecorderFactoryCases;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Adaptive;
using SharpAttributeParser.Mappers.Repositories.Combined;
using SharpAttributeParser.Mappers.Repositories.Semantic;

internal sealed class FactoryContext<TCombinedRecord, TSemanticRecord>
{
    public static FactoryContext<TCombinedRecord, TSemanticRecord> Create()
    {
        Mock<IDetachedMappedCombinedOptionalConstructorArgumentRecorderFactory<TCombinedRecord>> combinedFactoryMock = new();
        Mock<IDetachedMappedSemanticConstructorArgumentRecorderFactory<TSemanticRecord>> semanticFactoryMock = new();

        DetachedMappedAdaptiveOptionalConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> factory = new(combinedFactoryMock.Object, semanticFactoryMock.Object);

        return new(factory, combinedFactoryMock, semanticFactoryMock);
    }

    public DetachedMappedAdaptiveOptionalConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> Factory { get; }

    public Mock<IDetachedMappedCombinedOptionalConstructorArgumentRecorderFactory<TCombinedRecord>> CombinedFactoryMock { get; }
    public Mock<IDetachedMappedSemanticConstructorArgumentRecorderFactory<TSemanticRecord>> SemanticFactoryMock { get; }

    private FactoryContext(DetachedMappedAdaptiveOptionalConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> factory, Mock<IDetachedMappedCombinedOptionalConstructorArgumentRecorderFactory<TCombinedRecord>> combinedFactoryMock, Mock<IDetachedMappedSemanticConstructorArgumentRecorderFactory<TSemanticRecord>> semanticFactoryMock)
    {
        Factory = factory;

        CombinedFactoryMock = combinedFactoryMock;
        SemanticFactoryMock = semanticFactoryMock;
    }
}
