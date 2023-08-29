namespace SharpAttributeParser.Mappers.Repositories.SplitCases.DetachedMappedSplitConstructorArgumentRecorderFactoryCases;

using SharpAttributeParser.Mappers.Repositories.Split;

using Xunit;

public sealed class Normal
{
    private static IDetachedMappedSplitNormalConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> Target<TSemanticRecord, TSyntacticRecord>(IDetachedMappedSplitConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> factory) => factory.Normal;

    [Fact]
    public void ReturnedFactoryIsSpecified()
    {
        var context = FactoryContext<object, object>.Create();

        var actual = Target(context.Factory);

        Assert.Same(context.NormalFactory, actual);
    }
}
