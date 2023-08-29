namespace SharpAttributeParser.Mappers.Repositories.CombinedCases.DetachedMappedCombinedNamedArgumentRecorderFactoryCases;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Combined;
using SharpAttributeParser.Patterns;

internal sealed class FactoryContext<TRecord>
{
    public static FactoryContext<TRecord> Create()
    {
        var argumentPatternFactory = Mock.Of<IArgumentPatternFactory>();

        DetachedMappedCombinedNamedArgumentRecorderFactory<TRecord> factory = new(argumentPatternFactory);

        return new(factory, argumentPatternFactory);
    }

    public DetachedMappedCombinedNamedArgumentRecorderFactory<TRecord> Factory { get; }

    public IArgumentPatternFactory ArgumentPatternFactory { get; }

    private FactoryContext(DetachedMappedCombinedNamedArgumentRecorderFactory<TRecord> factory, IArgumentPatternFactory argumentPatternFactory)
    {
        Factory = factory;

        ArgumentPatternFactory = argumentPatternFactory;
    }
}
