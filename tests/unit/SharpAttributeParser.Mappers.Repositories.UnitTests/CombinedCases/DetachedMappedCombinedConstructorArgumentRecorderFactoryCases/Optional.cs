namespace SharpAttributeParser.Mappers.Repositories.CombinedCases.DetachedMappedCombinedConstructorArgumentRecorderFactoryCases;

using SharpAttributeParser.Mappers.Repositories.Combined;

using Xunit;

public sealed class Optional
{
    private static IDetachedMappedCombinedOptionalConstructorArgumentRecorderFactory<TRecord> Target<TRecord>(IDetachedMappedCombinedConstructorArgumentRecorderFactory<TRecord> factory) => factory.Optional;

    [Fact]
    public void ReturnedFactoryIsSpecified()
    {
        var context = FactoryContext<object>.Create();

        var actual = Target(context.Factory);

        Assert.Same(context.OptionalFactory, actual);
    }
}
