namespace SharpAttributeParser.Mappers.Repositories.CombinedCases.DetachedMappedCombinedConstructorArgumentRecorderFactoryCases;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Combined;

internal sealed class FactoryContext<TRecord>
{
    public static FactoryContext<TRecord> Create()
    {
        var normalFactory = Mock.Of<IDetachedMappedCombinedNormalConstructorArgumentRecorderFactory<TRecord>>();
        var paramsFactory = Mock.Of<IDetachedMappedCombinedParamsConstructorArgumentRecorderFactory<TRecord>>();
        var optionalFactory = Mock.Of<IDetachedMappedCombinedOptionalConstructorArgumentRecorderFactory<TRecord>>();

        DetachedMappedCombinedConstructorArgumentRecorderFactory<TRecord> factory = new(normalFactory, paramsFactory, optionalFactory);

        return new(factory, normalFactory, paramsFactory, optionalFactory);
    }

    public DetachedMappedCombinedConstructorArgumentRecorderFactory<TRecord> Factory { get; }

    public IDetachedMappedCombinedNormalConstructorArgumentRecorderFactory<TRecord> NormalFactory { get; }
    public IDetachedMappedCombinedParamsConstructorArgumentRecorderFactory<TRecord> ParamsFactory { get; }
    public IDetachedMappedCombinedOptionalConstructorArgumentRecorderFactory<TRecord> OptionalFactory { get; }

    private FactoryContext(DetachedMappedCombinedConstructorArgumentRecorderFactory<TRecord> factory, IDetachedMappedCombinedNormalConstructorArgumentRecorderFactory<TRecord> normalFactory, IDetachedMappedCombinedParamsConstructorArgumentRecorderFactory<TRecord> paramsFactory, IDetachedMappedCombinedOptionalConstructorArgumentRecorderFactory<TRecord> optionalFactory)
    {
        Factory = factory;

        NormalFactory = normalFactory;
        ParamsFactory = paramsFactory;
        OptionalFactory = optionalFactory;
    }
}
