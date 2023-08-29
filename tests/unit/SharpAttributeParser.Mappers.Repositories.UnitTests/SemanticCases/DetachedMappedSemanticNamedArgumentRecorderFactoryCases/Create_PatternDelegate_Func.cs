namespace SharpAttributeParser.Mappers.Repositories.SemanticCases.DetachedMappedSemanticNamedArgumentRecorderFactoryCases;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Patterns;

using System;

using Xunit;

public sealed class Create_PatternDelegate_Func
{
    private static IDetachedMappedSemanticNamedArgumentRecorder<TRecord> Target<TRecord, TArgument>(IDetachedMappedSemanticNamedArgumentRecorderFactory<TRecord> factory, Func<IArgumentPatternFactory, IArgumentPattern<TArgument>> patternDelegate, Func<TRecord, TArgument, bool> recorder) => factory.Create(patternDelegate, recorder);

    [Fact]
    public void NullPatternDelegate_ArgumentNullException()
    {
        var context = FactoryContext<object>.Create();

        var exception = Record.Exception(() => Target<object, object>(context.Factory, null!, static (dataRecord, argument) => true));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullRecorder_ArgumentNullException()
    {
        var context = FactoryContext<object>.Create();

        var exception = Record.Exception(() => Target(context.Factory, static (factory) => Mock.Of<IArgumentPattern<object>>(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullReturningPatternDelegate_ArgumentException()
    {
        var context = FactoryContext<object>.Create();

        var exception = Record.Exception(() => Target<object, object>(context.Factory, static (factory) => null!, static (dataRecord, argument) => true));

        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public void ProvidedPatternFactoryIsSpecified()
    {
        var context = FactoryContext<object>.Create();

        Target(context.Factory, patternDelegate, static (dataRecord, argument) => true);

        IArgumentPattern<object> patternDelegate(IArgumentPatternFactory factory)
        {
            Assert.Same(context.ArgumentPatternFactory, factory);

            return Mock.Of<IArgumentPattern<object>>();
        }
    }

    [Fact]
    public void ValidPatternAndRecorder_ConstructedRecorderUsesPatternAndDelegate()
    {
        var dataRecord = Mock.Of<object>();
        var argument = Mock.Of<object>();

        var fitArgument = Mock.Of<object>();

        var context = FactoryContext<object>.Create();

        Mock<IArgumentPattern<object>> patternMock = new();
        Mock<Func<object, object?, bool>> recorderMock = new();

        patternMock.Setup(static (pattern) => pattern.TryFit(It.IsAny<object?>())).Returns(fitArgument);

        var recorder = Target(context.Factory, (factory) => patternMock.Object, recorderMock.Object);

        recorder.TryRecordArgument(dataRecord, argument);

        patternMock.Verify((pattern) => pattern.TryFit(argument), Times.Once);
        recorderMock.Verify((recorder) => recorder.Invoke(dataRecord, fitArgument), Times.Once);
    }
}
