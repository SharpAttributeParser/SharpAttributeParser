namespace SharpAttributeParser.Mappers.Repositories.CombinedCases.DetachedMappedCombinedConstructorArgumentRecorderFactoryCases;

using SharpAttributeParser.Mappers.Repositories.Combined;

using Xunit;

public sealed class Normal
{
    private static IDetachedMappedCombinedNormalConstructorArgumentRecorderFactory<TRecord> Target<TRecord>(IDetachedMappedCombinedConstructorArgumentRecorderFactory<TRecord> factory) => factory.Normal;

    [Fact]
    public void ReturnedFactoryIsSpecified()
    {
        var context = FactoryContext<object>.Create();

        var actual = Target(context.Factory);

        Assert.Same(context.NormalFactory, actual);
    }
}
