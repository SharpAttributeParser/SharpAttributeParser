namespace SharpAttributeParser.Mappers.Repositories.CombinedCases.DetachedMappedCombinedParamsConstructorArgumentRecorderFactoryCases;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Combined;
using SharpAttributeParser.Patterns;

internal sealed class FactoryContext<TRecord>
{
    public static FactoryContext<TRecord> Create()
    {
        var argumentPatternFactory = Mock.Of<IArgumentPatternFactory>();

        DetachedMappedCombinedParamsConstructorArgumentRecorderFactory<TRecord> factory = new(argumentPatternFactory);

        return new(factory, argumentPatternFactory);
    }

    public DetachedMappedCombinedParamsConstructorArgumentRecorderFactory<TRecord> Factory { get; }

    public IArgumentPatternFactory ArgumentPatternFactory { get; }

    private FactoryContext(DetachedMappedCombinedParamsConstructorArgumentRecorderFactory<TRecord> factory, IArgumentPatternFactory argumentPatternFactory)
    {
        Factory = factory;

        ArgumentPatternFactory = argumentPatternFactory;
    }
}
