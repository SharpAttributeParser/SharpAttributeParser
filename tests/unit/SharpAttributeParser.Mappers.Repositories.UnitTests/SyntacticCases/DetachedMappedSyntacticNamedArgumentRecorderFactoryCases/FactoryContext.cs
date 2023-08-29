namespace SharpAttributeParser.Mappers.Repositories.SyntacticCases.DetachedMappedSyntacticNamedArgumentRecorderFactoryCases;

using SharpAttributeParser.Mappers.Repositories.Syntactic;

internal sealed class FactoryContext<TRecord>
{
    public static FactoryContext<TRecord> Create()
    {
        DetachedMappedSyntacticNamedArgumentRecorderFactory<TRecord> factory = new();

        return new(factory);
    }

    public DetachedMappedSyntacticNamedArgumentRecorderFactory<TRecord> Factory { get; }

    private FactoryContext(DetachedMappedSyntacticNamedArgumentRecorderFactory<TRecord> factory)
    {
        Factory = factory;
    }
}
