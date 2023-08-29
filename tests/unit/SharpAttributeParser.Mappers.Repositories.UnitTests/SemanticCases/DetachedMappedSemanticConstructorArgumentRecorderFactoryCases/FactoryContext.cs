namespace SharpAttributeParser.Mappers.Repositories.SemanticCases.DetachedMappedSemanticConstructorArgumentRecorderFactoryCases;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Patterns;

internal sealed class FactoryContext<TRecord>
{
    public static FactoryContext<TRecord> Create()
    {
        var argumentPatternFactory = Mock.Of<IArgumentPatternFactory>();

        DetachedMappedSemanticConstructorArgumentRecorderFactory<TRecord> factory = new(argumentPatternFactory);

        return new(factory, argumentPatternFactory);
    }

    public DetachedMappedSemanticConstructorArgumentRecorderFactory<TRecord> Factory { get; }

    public IArgumentPatternFactory ArgumentPatternFactory { get; }

    private FactoryContext(DetachedMappedSemanticConstructorArgumentRecorderFactory<TRecord> factory, IArgumentPatternFactory argumentPatternFactory)
    {
        Factory = factory;

        ArgumentPatternFactory = argumentPatternFactory;
    }
}
