namespace SharpAttributeParser.Mappers.Repositories.ConstructorMappingRepositoryFactoryCases.ConstructorMappingRepositoryCases;

using Moq;

using System;

using Xunit;

public sealed class AddNamedMapping_Recorder
{
    private static void Target<TRecorder, TRecorderFactory>(IConstructorMappingRepository<TRecorder, TRecorderFactory> repository, string parameterName, TRecorder recorder) => repository.AddNamedMapping(parameterName, recorder);

    [Fact]
    public void NullParameterName_ArgumentNullException()
    {
        var context = RepositoryContext<object, object>.CreateWithOrdinalComparer();

        var exception = Record.Exception(() => Target(context.Repository, null!, Mock.Of<object>()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullRecorder_ArgumentNullException()
    {
        var context = RepositoryContext<object, object>.CreateWithOrdinalComparer();

        var exception = Record.Exception(() => Target(context.Repository, string.Empty, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void AlreadyExistingMappingForName_UsesComparerAndThrowsArgumentException()
    {
        var parameterAName = "A";
        var parameterBName = "B";

        var context = RepositoryContext<object, object>.Create();

        context.ParameterNameComparerMock.Setup(static (comparer) => comparer.Equals(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

        context.Repository.AddNamedMapping(parameterAName, Mock.Of<object>());

        var exception = Record.Exception(() => Target(context.Repository, parameterBName, Mock.Of<object>()));

        Assert.IsType<ArgumentException>(exception);

        context.ParameterNameComparerMock.Verify((comparer) => comparer.Equals(parameterAName, parameterBName), Times.Once);
    }

    [Fact]
    public void AlreadyBuilt_InvalidOperationException()
    {
        var context = RepositoryContext<object, object>.CreateWithOrdinalComparer();

        context.Repository.Build();

        var exception = Record.Exception(() => Target(context.Repository, string.Empty, Mock.Of<object>()));

        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void ValidRecorders_BuiltRepositoryContainsAllRecorders()
    {
        var parameterAName = "A";
        var parameterBName = "B";

        var recorderA = Mock.Of<object>();
        var recorderB = Mock.Of<object>();

        var context = RepositoryContext<object, object>.CreateWithOrdinalComparer();

        Target(context.Repository, parameterAName, recorderA);
        Target(context.Repository, parameterBName, recorderB);

        var built = context.Repository.Build();

        Assert.Equal(2, built.Named.Count);

        Assert.Equal(built.Named[parameterAName], recorderA);
        Assert.Equal(built.Named[parameterBName], recorderB);
    }
}
