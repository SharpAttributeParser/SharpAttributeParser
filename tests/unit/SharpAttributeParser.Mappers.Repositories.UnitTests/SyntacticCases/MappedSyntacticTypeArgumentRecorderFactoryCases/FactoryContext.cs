namespace SharpAttributeParser.Mappers.Repositories.SyntacticCases.MappedSyntacticTypeArgumentRecorderFactoryCases;

using SharpAttributeParser.Mappers.Repositories.Syntactic;

internal sealed class FactoryContext
{
    public static FactoryContext Create()
    {
        MappedSyntacticTypeArgumentRecorderFactory factory = new();

        return new(factory);
    }

    public MappedSyntacticTypeArgumentRecorderFactory Factory { get; }

    private FactoryContext(MappedSyntacticTypeArgumentRecorderFactory factory)
    {
        Factory = factory;
    }
}
