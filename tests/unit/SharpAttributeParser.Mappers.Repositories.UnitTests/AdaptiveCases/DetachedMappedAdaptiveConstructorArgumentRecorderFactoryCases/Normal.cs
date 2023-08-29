namespace SharpAttributeParser.Mappers.Repositories.AdaptiveCases.DetachedMappedAdaptiveConstructorArgumentRecorderFactoryCases;

using SharpAttributeParser.Mappers.Repositories.Adaptive;

using Xunit;

public sealed class Normal
{
    private static IDetachedMappedAdaptiveNormalConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> Target<TCombinedRecord, TSemanticRecord>(IDetachedMappedAdaptiveConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> factory) => factory.Normal;

    [Fact]
    public void ReturnedFactoryIsSpecified()
    {
        var context = FactoryContext<object, object>.Create();

        var actual = Target(context.Factory);

        Assert.Same(context.NormalFactory, actual);
    }
}
