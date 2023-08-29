namespace SharpAttributeParser.Mappers.Repositories.SyntacticCases.MappedSyntacticNamedArgumentRecorderFactoryCases;

using SharpAttributeParser.Mappers.Repositories.Syntactic;

internal sealed class FactoryContext
{
    public static FactoryContext Create()
    {
        MappedSyntacticNamedArgumentRecorderFactory factory = new();

        return new(factory);
    }

    public MappedSyntacticNamedArgumentRecorderFactory Factory { get; }

    private FactoryContext(MappedSyntacticNamedArgumentRecorderFactory factory)
    {
        Factory = factory;
    }
}
