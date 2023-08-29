namespace SharpAttributeParser.Mappers.Repositories.SyntacticCases.DetachedMappedSyntacticConstructorArgumentRecorderFactoryCases;

using SharpAttributeParser.Mappers.Repositories.Syntactic;

using Xunit;

public sealed class Normal
{
    private static IDetachedMappedSyntacticNormalConstructorArgumentRecorderFactory<TRecord> Target<TRecord>(IDetachedMappedSyntacticConstructorArgumentRecorderFactory<TRecord> factory) => factory.Normal;

    [Fact]
    public void ReturnedFactoryIsSpecified()
    {
        var context = FactoryContext<object>.Create();

        var actual = Target(context.Factory);

        Assert.Same(context.NormalFactory, actual);
    }
}
