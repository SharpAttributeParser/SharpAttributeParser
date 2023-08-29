namespace SharpAttributeParser.Mappers.Repositories.SyntacticCases.DetachedMappedSyntacticTypeArgumentRecorderFactoryCases;

using SharpAttributeParser.Mappers.Repositories.Syntactic;

internal sealed class FactoryContext<TRecord>
{
    public static FactoryContext<TRecord> Create()
    {
        DetachedMappedSyntacticTypeArgumentRecorderFactory<TRecord> factory = new();

        return new(factory);
    }

    public DetachedMappedSyntacticTypeArgumentRecorderFactory<TRecord> Factory { get; }

    private FactoryContext(DetachedMappedSyntacticTypeArgumentRecorderFactory<TRecord> factory)
    {
        Factory = factory;
    }
}
