namespace SharpAttributeParser.Mappers.Repositories.CombinedCases.DetachedMappedCombinedConstructorArgumentRecorderFactoryCases;

using SharpAttributeParser.Mappers.Repositories.Combined;

using Xunit;

public sealed class Params
{
    private static IDetachedMappedCombinedParamsConstructorArgumentRecorderFactory<TRecord> Target<TRecord>(IDetachedMappedCombinedConstructorArgumentRecorderFactory<TRecord> factory) => factory.Params;

    [Fact]
    public void ReturnedFactoryIsSpecified()
    {
        var context = FactoryContext<object>.Create();

        var actual = Target(context.Factory);

        Assert.Same(context.ParamsFactory, actual);
    }
}
