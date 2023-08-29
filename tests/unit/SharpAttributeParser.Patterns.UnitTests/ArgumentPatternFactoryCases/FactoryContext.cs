namespace SharpAttributeParser.Patterns.ArgumentPatternFactoryCases;

internal sealed class FactoryContext
{
    public static FactoryContext Create() => new(ArgumentPatternFactory.Singleton);

    public ArgumentPatternFactory Factory { get; }

    private FactoryContext(ArgumentPatternFactory factory)
    {
        Factory = factory;
    }
}
