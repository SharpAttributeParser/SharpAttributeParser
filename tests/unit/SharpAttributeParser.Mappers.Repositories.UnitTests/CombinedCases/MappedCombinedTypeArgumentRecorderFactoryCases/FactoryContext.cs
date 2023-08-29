namespace SharpAttributeParser.Mappers.Repositories.CombinedCases.MappedCombinedTypeArgumentRecorderFactoryCases;

using SharpAttributeParser.Mappers.Repositories.Combined;

internal sealed class FactoryContext
{
    public static FactoryContext Create()
    {
        MappedCombinedTypeArgumentRecorderFactory factory = new();

        return new(factory);
    }

    public MappedCombinedTypeArgumentRecorderFactory Factory { get; }

    private FactoryContext(MappedCombinedTypeArgumentRecorderFactory factory)
    {
        Factory = factory;
    }
}
