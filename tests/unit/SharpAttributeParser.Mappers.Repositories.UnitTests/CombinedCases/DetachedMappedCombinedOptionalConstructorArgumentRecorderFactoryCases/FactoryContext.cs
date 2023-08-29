namespace SharpAttributeParser.Mappers.Repositories.CombinedCases.DetachedMappedCombinedOptionalConstructorArgumentRecorderFactoryCases;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Combined;
using SharpAttributeParser.Patterns;

internal sealed class FactoryContext<TRecord>
{
    public static FactoryContext<TRecord> Create()
    {
        var argumentPatternFactory = Mock.Of<IArgumentPatternFactory>();

        DetachedMappedCombinedOptionalConstructorArgumentRecorderFactory<TRecord> factory = new(argumentPatternFactory);

        return new(factory, argumentPatternFactory);
    }

    public DetachedMappedCombinedOptionalConstructorArgumentRecorderFactory<TRecord> Factory { get; }

    public IArgumentPatternFactory ArgumentPatternFactory { get; }

    private FactoryContext(DetachedMappedCombinedOptionalConstructorArgumentRecorderFactory<TRecord> factory, IArgumentPatternFactory argumentPatternFactory)
    {
        Factory = factory;

        ArgumentPatternFactory = argumentPatternFactory;
    }
}
