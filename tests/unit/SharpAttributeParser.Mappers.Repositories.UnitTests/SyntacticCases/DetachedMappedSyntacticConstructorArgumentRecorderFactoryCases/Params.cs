namespace SharpAttributeParser.Mappers.Repositories.SyntacticCases.DetachedMappedSyntacticConstructorArgumentRecorderFactoryCases;

using SharpAttributeParser.Mappers.Repositories.Syntactic;

using Xunit;

public sealed class Params
{
    private static IDetachedMappedSyntacticParamsConstructorArgumentRecorderFactory<TRecord> Target<TRecord>(IDetachedMappedSyntacticConstructorArgumentRecorderFactory<TRecord> factory) => factory.Params;

    [Fact]
    public void ReturnedFactoryIsSpecified()
    {
        var context = FactoryContext<object>.Create();

        var actual = Target(context.Factory);

        Assert.Same(context.ParamsFactory, actual);
    }
}
