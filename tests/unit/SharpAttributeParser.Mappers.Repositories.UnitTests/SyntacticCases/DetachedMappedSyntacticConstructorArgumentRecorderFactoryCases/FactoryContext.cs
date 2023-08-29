namespace SharpAttributeParser.Mappers.Repositories.SyntacticCases.DetachedMappedSyntacticConstructorArgumentRecorderFactoryCases;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Syntactic;

internal sealed class FactoryContext<TRecord>
{
    public static FactoryContext<TRecord> Create()
    {
        var normalFactory = Mock.Of<IDetachedMappedSyntacticNormalConstructorArgumentRecorderFactory<TRecord>>();
        var paramsFactory = Mock.Of<IDetachedMappedSyntacticParamsConstructorArgumentRecorderFactory<TRecord>>();
        var optionalFactory = Mock.Of<IDetachedMappedSyntacticOptionalConstructorArgumentRecorderFactory<TRecord>>();

        DetachedMappedSyntacticConstructorArgumentRecorderFactory<TRecord> factory = new(normalFactory, paramsFactory, optionalFactory);

        return new(factory, normalFactory, paramsFactory, optionalFactory);
    }

    public DetachedMappedSyntacticConstructorArgumentRecorderFactory<TRecord> Factory { get; }

    public IDetachedMappedSyntacticNormalConstructorArgumentRecorderFactory<TRecord> NormalFactory { get; }
    public IDetachedMappedSyntacticParamsConstructorArgumentRecorderFactory<TRecord> ParamsFactory { get; }
    public IDetachedMappedSyntacticOptionalConstructorArgumentRecorderFactory<TRecord> OptionalFactory { get; }

    private FactoryContext(DetachedMappedSyntacticConstructorArgumentRecorderFactory<TRecord> factory, IDetachedMappedSyntacticNormalConstructorArgumentRecorderFactory<TRecord> normalFactory, IDetachedMappedSyntacticParamsConstructorArgumentRecorderFactory<TRecord> paramsFactory, IDetachedMappedSyntacticOptionalConstructorArgumentRecorderFactory<TRecord> optionalFactory)
    {
        Factory = factory;

        NormalFactory = normalFactory;
        ParamsFactory = paramsFactory;
        OptionalFactory = optionalFactory;
    }
}
