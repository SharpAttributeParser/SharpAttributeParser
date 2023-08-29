namespace SharpAttributeParser.Mappers.Repositories.SemanticCases.DetachedMappedSemanticNamedArgumentRecorderFactoryCases;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Patterns;

internal sealed class FactoryContext<TRecord>
{
    public static FactoryContext<TRecord> Create()
    {
        var argumentPatternFactory = Mock.Of<IArgumentPatternFactory>();

        DetachedMappedSemanticNamedArgumentRecorderFactory<TRecord> factory = new(argumentPatternFactory);

        return new(factory, argumentPatternFactory);
    }

    public DetachedMappedSemanticNamedArgumentRecorderFactory<TRecord> Factory { get; }

    public IArgumentPatternFactory ArgumentPatternFactory { get; }

    private FactoryContext(DetachedMappedSemanticNamedArgumentRecorderFactory<TRecord> factory, IArgumentPatternFactory argumentPatternFactory)
    {
        Factory = factory;

        ArgumentPatternFactory = argumentPatternFactory;
    }
}
