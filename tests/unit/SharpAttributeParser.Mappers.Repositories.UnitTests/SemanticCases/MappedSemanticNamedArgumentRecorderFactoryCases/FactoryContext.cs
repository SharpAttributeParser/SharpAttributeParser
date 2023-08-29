namespace SharpAttributeParser.Mappers.Repositories.SemanticCases.MappedSemanticNamedArgumentRecorderFactoryCases;

using SharpAttributeParser.Mappers.Repositories.Semantic;

internal sealed class FactoryContext
{
    public static FactoryContext Create()
    {
        MappedSemanticNamedArgumentRecorderFactory factory = new();

        return new(factory);
    }

    public MappedSemanticNamedArgumentRecorderFactory Factory { get; }

    private FactoryContext(MappedSemanticNamedArgumentRecorderFactory factory)
    {
        Factory = factory;
    }
}
