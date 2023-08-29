namespace SharpAttributeParser.Mappers.Repositories.CombinedCases.DetachedMappedCombinedTypeArgumentRecorderFactoryCases;

using SharpAttributeParser.Mappers.Repositories.Combined;

internal sealed class FactoryContext<TRecord>
{
    public static FactoryContext<TRecord> Create()
    {
        DetachedMappedCombinedTypeArgumentRecorderFactory<TRecord> factory = new();

        return new(factory);
    }

    public DetachedMappedCombinedTypeArgumentRecorderFactory<TRecord> Factory { get; }

    private FactoryContext(DetachedMappedCombinedTypeArgumentRecorderFactory<TRecord> factory)
    {
        Factory = factory;
    }
}
