namespace SharpAttributeParser.Mappers.Repositories.AdaptiveCases.DetachedMappedAdaptiveConstructorArgumentRecorderFactoryCases;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Adaptive;

internal sealed class FactoryContext<TCombinedRecord, TSemanticRecord>
{
    public static FactoryContext<TCombinedRecord, TSemanticRecord> Create()
    {
        var normalFactory = Mock.Of<IDetachedMappedAdaptiveNormalConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>>();
        var paramsFactory = Mock.Of<IDetachedMappedAdaptiveParamsConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>>();
        var optionalFactory = Mock.Of<IDetachedMappedAdaptiveOptionalConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>>();

        DetachedMappedAdaptiveConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> factory = new(normalFactory, paramsFactory, optionalFactory);

        return new(factory, normalFactory, paramsFactory, optionalFactory);
    }

    public DetachedMappedAdaptiveConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> Factory { get; }

    public IDetachedMappedAdaptiveNormalConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> NormalFactory { get; }
    public IDetachedMappedAdaptiveParamsConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> ParamsFactory { get; }
    public IDetachedMappedAdaptiveOptionalConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> OptionalFactory { get; }

    private FactoryContext(DetachedMappedAdaptiveConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> factory, IDetachedMappedAdaptiveNormalConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> normalFactory, IDetachedMappedAdaptiveParamsConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> paramsFactory, IDetachedMappedAdaptiveOptionalConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> optionalFactory)
    {
        Factory = factory;

        NormalFactory = normalFactory;
        ParamsFactory = paramsFactory;
        OptionalFactory = optionalFactory;
    }
}
