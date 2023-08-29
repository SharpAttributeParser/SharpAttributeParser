namespace SharpAttributeParser.Mappers.Repositories.SemanticCases.MappedSemanticTypeArgumentRecorderFactoryCases;

using SharpAttributeParser.Mappers.Repositories.Semantic;

internal sealed class FactoryContext
{
    public static FactoryContext Create()
    {
        MappedSemanticTypeArgumentRecorderFactory factory = new();

        return new(factory);
    }

    public MappedSemanticTypeArgumentRecorderFactory Factory { get; }

    private FactoryContext(MappedSemanticTypeArgumentRecorderFactory factory)
    {
        Factory = factory;
    }
}
