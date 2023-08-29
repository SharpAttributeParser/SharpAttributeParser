namespace SharpAttributeParser.Mappers.Repositories.SyntacticCases.DetachedMappedSyntacticNormalConstructorArgumentRecorderFactoryCases;

using SharpAttributeParser.Mappers.Repositories.Syntactic;

internal sealed class FactoryContext<TRecord>
{
    public static FactoryContext<TRecord> Create()
    {
        DetachedMappedSyntacticNormalConstructorArgumentRecorderFactory<TRecord> factory = new();

        return new(factory);
    }

    public DetachedMappedSyntacticNormalConstructorArgumentRecorderFactory<TRecord> Factory { get; }

    private FactoryContext(DetachedMappedSyntacticNormalConstructorArgumentRecorderFactory<TRecord> factory)
    {
        Factory = factory;
    }
}
