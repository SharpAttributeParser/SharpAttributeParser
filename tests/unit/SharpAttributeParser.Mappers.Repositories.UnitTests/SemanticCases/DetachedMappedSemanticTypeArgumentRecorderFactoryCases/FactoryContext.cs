namespace SharpAttributeParser.Mappers.Repositories.SemanticCases.DetachedMappedSemanticTypeArgumentRecorderFactoryCases;

using SharpAttributeParser.Mappers.Repositories.Semantic;

internal sealed class FactoryContext<TRecord>
{
    public static FactoryContext<TRecord> Create()
    {
        DetachedMappedSemanticTypeArgumentRecorderFactory<TRecord> factory = new();

        return new(factory);
    }

    public DetachedMappedSemanticTypeArgumentRecorderFactory<TRecord> Factory { get; }

    private FactoryContext(DetachedMappedSemanticTypeArgumentRecorderFactory<TRecord> factory)
    {
        Factory = factory;
    }
}
