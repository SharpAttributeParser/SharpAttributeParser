namespace SharpAttributeParser.Mappers.Repositories.TypeMappingRepositoryFactoryCases.TypeMappingRepositoryCases;

using Moq;

using System;

using Xunit;

public sealed class AddIndexedMapping_Recorder
{
    private static void Target<TRecorder, TRecorderFactory>(ITypeMappingRepository<TRecorder, TRecorderFactory> repository, int parameterIndex, TRecorder recorder) => repository.AddIndexedMapping(parameterIndex, recorder);

    [Fact]
    public void NullRecorder_ArgumentNullException()
    {
        var context = RepositoryContext<object, object>.CreateWithOrdinalComparer();

        var exception = Record.Exception(() => Target(context.Repository, 0, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void AlreadyExistingMappingForIndex_ArgumentException()
    {
        var parameterIndex = 42;

        var context = RepositoryContext<object, object>.CreateWithOrdinalComparer();

        context.Repository.AddIndexedMapping(parameterIndex, Mock.Of<object>());

        var exception = Record.Exception(() => Target(context.Repository, parameterIndex, Mock.Of<object>()));

        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public void AlreadyExistingNamedMappings_InvalidOperationException()
    {
        var context = RepositoryContext<object, object>.CreateWithOrdinalComparer();

        context.Repository.AddNamedMapping(string.Empty, Mock.Of<object>());

        var exception = Record.Exception(() => Target(context.Repository, 0, Mock.Of<object>()));

        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void AlreadyBuilt_InvalidOperationException()
    {
        var context = RepositoryContext<object, object>.CreateWithOrdinalComparer();

        context.Repository.Build();

        var exception = Record.Exception(() => Target(context.Repository, 0, Mock.Of<object>()));

        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void ValidRecorders_BuiltRepositoryContainsAllRecorders()
    {
        var parameterAIndex = 0;
        var parameterBIndex = 1;

        var recorderA = Mock.Of<object>();
        var recorderB = Mock.Of<object>();

        var context = RepositoryContext<object, object>.CreateWithOrdinalComparer();

        Target(context.Repository, parameterAIndex, recorderA);
        Target(context.Repository, parameterBIndex, recorderB);

        var built = context.Repository.Build();

        Assert.Equal(2, built.Indexed.Count);
        Assert.Empty(built.Named);

        Assert.Equal(built.Indexed[parameterAIndex], recorderA);
        Assert.Equal(built.Indexed[parameterBIndex], recorderB);
    }
}
