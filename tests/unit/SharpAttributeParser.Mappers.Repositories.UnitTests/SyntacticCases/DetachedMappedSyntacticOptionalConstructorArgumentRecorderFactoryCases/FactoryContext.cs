namespace SharpAttributeParser.Mappers.Repositories.SyntacticCases.DetachedMappedSyntacticOptionalConstructorArgumentRecorderFactoryCases;

using SharpAttributeParser.Mappers.Repositories.Syntactic;

internal sealed class FactoryContext<TRecord>
{
    public static FactoryContext<TRecord> Create()
    {
        DetachedMappedSyntacticOptionalConstructorArgumentRecorderFactory<TRecord> factory = new();

        return new(factory);
    }

    public DetachedMappedSyntacticOptionalConstructorArgumentRecorderFactory<TRecord> Factory { get; }

    private FactoryContext(DetachedMappedSyntacticOptionalConstructorArgumentRecorderFactory<TRecord> factory)
    {
        Factory = factory;
    }
}
