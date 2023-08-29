namespace SharpAttributeParser.Mappers.Repositories.SyntacticCases.DetachedMappedSyntacticConstructorArgumentRecorderFactoryCases;

using SharpAttributeParser.Mappers.Repositories.Syntactic;

using Xunit;

public sealed class Optional
{
    private static IDetachedMappedSyntacticOptionalConstructorArgumentRecorderFactory<TRecord> Target<TRecord>(IDetachedMappedSyntacticConstructorArgumentRecorderFactory<TRecord> factory) => factory.Optional;

    [Fact]
    public void ReturnedFactoryIsSpecified()
    {
        var context = FactoryContext<object>.Create();

        var actual = Target(context.Factory);

        Assert.Same(context.OptionalFactory, actual);
    }
}
