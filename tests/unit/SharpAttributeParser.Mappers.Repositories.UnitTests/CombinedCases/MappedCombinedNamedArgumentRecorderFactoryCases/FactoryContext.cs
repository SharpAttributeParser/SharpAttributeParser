namespace SharpAttributeParser.Mappers.Repositories.CombinedCases.MappedCombinedNamedArgumentRecorderFactoryCases;

using SharpAttributeParser.Mappers.Repositories.Combined;

internal sealed class FactoryContext
{
    public static FactoryContext Create()
    {
        MappedCombinedNamedArgumentRecorderFactory factory = new();

        return new(factory);
    }

    public MappedCombinedNamedArgumentRecorderFactory Factory { get; }

    private FactoryContext(MappedCombinedNamedArgumentRecorderFactory factory)
    {
        Factory = factory;
    }
}
