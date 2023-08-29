namespace SharpAttributeParser.Mappers.Repositories.CombinedCases.MappedCombinedConstructorArgumentRecorderFactoryCases;

using SharpAttributeParser.Mappers.Repositories.Combined;

internal sealed class FactoryContext
{
    public static FactoryContext Create()
    {
        MappedCombinedConstructorArgumentRecorderFactory factory = new();

        return new(factory);
    }

    public MappedCombinedConstructorArgumentRecorderFactory Factory { get; }

    private FactoryContext(MappedCombinedConstructorArgumentRecorderFactory factory)
    {
        Factory = factory;
    }
}
