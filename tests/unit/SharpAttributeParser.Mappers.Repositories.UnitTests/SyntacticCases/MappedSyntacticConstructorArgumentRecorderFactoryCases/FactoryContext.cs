namespace SharpAttributeParser.Mappers.Repositories.SyntacticCases.MappedSyntacticConstructorArgumentRecorderFactoryCases;

using SharpAttributeParser.Mappers.Repositories.Syntactic;

internal sealed class FactoryContext
{
    public static FactoryContext Create()
    {
        MappedSyntacticConstructorArgumentRecorderFactory factory = new();

        return new(factory);
    }

    public MappedSyntacticConstructorArgumentRecorderFactory Factory { get; }

    private FactoryContext(MappedSyntacticConstructorArgumentRecorderFactory factory)
    {
        Factory = factory;
    }
}
