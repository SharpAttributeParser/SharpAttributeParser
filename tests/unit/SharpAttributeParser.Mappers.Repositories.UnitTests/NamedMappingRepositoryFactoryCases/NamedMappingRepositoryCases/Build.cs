namespace SharpAttributeParser.Mappers.Repositories.NamedMappingRepositoryFactoryCases.NamedMappingRepositoryCases;

using System;

using Xunit;

public sealed class Build
{
    private static IBuiltNamedMappingRepository<TRecorder> Target<TRecorder, TRecorderFactory>(INamedMappingRepository<TRecorder, TRecorderFactory> factory) => factory.Build();

    [Fact]
    public void MultipleInvokations_ThrowOnMultipleBuildsDisabled_ReturnsBuiltRepositoryEveryTime()
    {
        var context = RepositoryContext<object, object>.Create(throwOnMultipleBuilds: false);

        var builtA = Target(context.Repository);
        var builtB = Target(context.Repository);

        Assert.NotNull(builtA);
        Assert.NotNull(builtB);
    }

    [Fact]
    public void MultipleInvokations_ThrowOnMultipleBuildsEnabled_InvalidOperationExceptionOnSecondInvokation()
    {
        var context = RepositoryContext<object, object>.Create(throwOnMultipleBuilds: true);

        var builtA = Target(context.Repository);

        var exception = Record.Exception(() => Target(context.Repository));

        Assert.NotNull(builtA);

        Assert.IsType<InvalidOperationException>(exception);
    }
}
