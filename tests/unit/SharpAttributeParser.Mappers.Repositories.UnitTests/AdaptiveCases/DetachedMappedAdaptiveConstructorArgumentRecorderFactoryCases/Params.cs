namespace SharpAttributeParser.Mappers.Repositories.AdaptiveCases.DetachedMappedAdaptiveConstructorArgumentRecorderFactoryCases;

using SharpAttributeParser.Mappers.Repositories.Adaptive;

using Xunit;

public sealed class Params
{
    private static IDetachedMappedAdaptiveParamsConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> Target<TCombinedRecord, TSemanticRecord>(IDetachedMappedAdaptiveConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> factory) => factory.Params;

    [Fact]
    public void ReturnedFactoryIsSpecified()
    {
        var context = FactoryContext<object, object>.Create();

        var actual = Target(context.Factory);

        Assert.Same(context.ParamsFactory, actual);
    }
}
