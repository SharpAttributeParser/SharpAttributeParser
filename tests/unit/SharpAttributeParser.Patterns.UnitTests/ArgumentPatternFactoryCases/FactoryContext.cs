namespace SharpAttributeParser.Patterns.ArgumentPatternFactoryCases;

internal sealed class FactoryContext
{
    public static FactoryContext Create() => new(new ArgumentPatternFactory());

    public ArgumentPatternFactory Factory { get; }

    private FactoryContext(ArgumentPatternFactory factory)
    {
        Factory = factory;
    }
}
