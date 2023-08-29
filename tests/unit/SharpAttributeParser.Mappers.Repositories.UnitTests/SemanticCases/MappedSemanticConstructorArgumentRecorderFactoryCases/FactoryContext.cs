namespace SharpAttributeParser.Mappers.Repositories.SemanticCases.MappedSemanticConstructorArgumentRecorderFactoryCases;

using SharpAttributeParser.Mappers.Repositories.Semantic;

internal sealed class FactoryContext
{
    public static FactoryContext Create()
    {
        MappedSemanticConstructorArgumentRecorderFactory factory = new();

        return new(factory);
    }

    public MappedSemanticConstructorArgumentRecorderFactory Factory { get; }

    private FactoryContext(MappedSemanticConstructorArgumentRecorderFactory factory)
    {
        Factory = factory;
    }
}
