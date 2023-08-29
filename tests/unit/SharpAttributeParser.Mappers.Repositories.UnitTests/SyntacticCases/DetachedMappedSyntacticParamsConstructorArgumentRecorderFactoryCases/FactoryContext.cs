namespace SharpAttributeParser.Mappers.Repositories.SyntacticCases.DetachedMappedSyntacticParamsConstructorArgumentRecorderFactoryCases;

using SharpAttributeParser.Mappers.Repositories.Syntactic;

internal sealed class FactoryContext<TRecord>
{
    public static FactoryContext<TRecord> Create()
    {
        DetachedMappedSyntacticParamsConstructorArgumentRecorderFactory<TRecord> factory = new();

        return new(factory);
    }

    public DetachedMappedSyntacticParamsConstructorArgumentRecorderFactory<TRecord> Factory { get; }

    private FactoryContext(DetachedMappedSyntacticParamsConstructorArgumentRecorderFactory<TRecord> factory)
    {
        Factory = factory;
    }
}
