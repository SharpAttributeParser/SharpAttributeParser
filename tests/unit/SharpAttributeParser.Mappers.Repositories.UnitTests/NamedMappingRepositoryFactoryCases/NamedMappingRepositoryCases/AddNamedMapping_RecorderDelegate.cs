namespace SharpAttributeParser.Mappers.Repositories.NamedMappingRepositoryFactoryCases.NamedMappingRepositoryCases;

using Moq;

using System;

using Xunit;

public sealed class AddNamedMapping_RecorderDelegate
{
    private static void Target<TRecorder, TRecorderFactory>(INamedMappingRepository<TRecorder, TRecorderFactory> repository, string parameterName, Func<TRecorderFactory, TRecorder> recorderDelegate) => repository.AddNamedMapping(parameterName, recorderDelegate);

    [Fact]
    public void NullParameterName_ArgumentNullException()
    {
        var context = RepositoryContext<object, object>.CreateWithOrdinalComparer();

        var exception = Record.Exception(() => Target(context.Repository, null!, (object factory) => Mock.Of<object>()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullRecorderDelegate_ArgumentNullException()
    {
        var context = RepositoryContext<object, object>.CreateWithOrdinalComparer();

        var exception = Record.Exception(() => Target(context.Repository, string.Empty, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullReturningDelegate_ArgumentException()
    {
        var context = RepositoryContext<object, object>.CreateWithOrdinalComparer();

        var exception = Record.Exception(() => Target(context.Repository, string.Empty, recorderDelegate));

        Assert.IsType<ArgumentException>(exception);

        static object recorderDelegate(object factory) => null!;
    }

    [Fact]
    public void AlreadyExistingMappingForName_UsesComparerAndThrowsArgumentException()
    {
        var parameterAName = "A";
        var parameterBName = "B";

        var context = RepositoryContext<object, object>.Create();

        context.ComparerMock.Setup(static (comparer) => comparer.Name.Equals(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

        context.Repository.AddNamedMapping(parameterAName, Mock.Of<object>());

        var exception = Record.Exception(() => Target(context.Repository, parameterBName, (object factory) => Mock.Of<object>()));

        Assert.IsType<ArgumentException>(exception);

        context.ComparerMock.Verify((comparer) => comparer.Name.Equals(parameterAName, parameterBName), Times.Once);
    }

    [Fact]
    public void AlreadyBuilt_InvalidOperationException()
    {
        var context = RepositoryContext<object, object>.CreateWithOrdinalComparer();

        context.Repository.Build();

        var exception = Record.Exception(() => Target(context.Repository, string.Empty, (object factory) => Mock.Of<object>()));

        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void ProvidedFactoryIsSpecified()
    {
        var context = RepositoryContext<object, object>.CreateWithOrdinalComparer();

        Target(context.Repository, string.Empty, recorderDelegate);

        object recorderDelegate(object factory)
        {
            Assert.Same(context.FactoryContext.RecorderFactory, factory);

            return Mock.Of<object>();
        }
    }

    [Fact]
    public void ValidRecorders_BuiltRepositoryContainsAllRecorders()
    {
        var parameterAName = "A";
        var parameterBName = "B";

        var recorderA = Mock.Of<object>();
        var recorderB = Mock.Of<object>();

        var context = RepositoryContext<object, object>.CreateWithOrdinalComparer();

        Target(context.Repository, parameterAName, recorderADelegate);
        Target(context.Repository, parameterBName, recorderBDelegate);

        var built = context.Repository.Build();

        Assert.Equal(2, built.Named.Count);

        Assert.Equal(built.Named[parameterAName], recorderA);
        Assert.Equal(built.Named[parameterBName], recorderB);

        object recorderADelegate(object factory) => recorderA;
        object recorderBDelegate(object factory) => recorderB;
    }
}
