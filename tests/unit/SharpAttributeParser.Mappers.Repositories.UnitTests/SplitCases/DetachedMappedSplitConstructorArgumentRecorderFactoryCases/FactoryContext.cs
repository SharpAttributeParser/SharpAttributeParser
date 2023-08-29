namespace SharpAttributeParser.Mappers.Repositories.SplitCases.DetachedMappedSplitConstructorArgumentRecorderFactoryCases;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Split;

internal sealed class FactoryContext<TSemanticRecord, TSyntacticRecord>
{
    public static FactoryContext<TSemanticRecord, TSyntacticRecord> Create()
    {
        var normalFactory = Mock.Of<IDetachedMappedSplitNormalConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>>();
        var paramsFactory = Mock.Of<IDetachedMappedSplitParamsConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>>();
        var optionalFactory = Mock.Of<IDetachedMappedSplitOptionalConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>>();

        DetachedMappedSplitConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> factory = new(normalFactory, paramsFactory, optionalFactory);

        return new(factory, normalFactory, paramsFactory, optionalFactory);
    }

    public DetachedMappedSplitConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> Factory { get; }

    public IDetachedMappedSplitNormalConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> NormalFactory { get; }
    public IDetachedMappedSplitParamsConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> ParamsFactory { get; }
    public IDetachedMappedSplitOptionalConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> OptionalFactory { get; }

    private FactoryContext(DetachedMappedSplitConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> factory, IDetachedMappedSplitNormalConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> normalFactory, IDetachedMappedSplitParamsConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> paramsFactory, IDetachedMappedSplitOptionalConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> optionalFactory)
    {
        Factory = factory;

        NormalFactory = normalFactory;
        ParamsFactory = paramsFactory;
        OptionalFactory = optionalFactory;
    }
}
