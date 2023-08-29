namespace SharpAttributeParser.Mappers.Repositories.SplitCases.DetachedMappedSplitConstructorArgumentRecorderFactoryCases;

using SharpAttributeParser.Mappers.Repositories.Split;

using Xunit;

public sealed class Params
{
    private static IDetachedMappedSplitParamsConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> Target<TSemanticRecord, TSyntacticRecord>(IDetachedMappedSplitConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> factory) => factory.Params;

    [Fact]
    public void ReturnedFactoryIsSpecified()
    {
        var context = FactoryContext<object, object>.Create();

        var actual = Target(context.Factory);

        Assert.Same(context.ParamsFactory, actual);
    }
}
