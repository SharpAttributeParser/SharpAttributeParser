namespace SharpAttributeParser.Mappers.Repositories.SemanticCases.DetachedMappedSemanticConstructorArgumentRecorderFactoryCases;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Patterns;

using System;

using Xunit;

public sealed class Create_Pattern_Func
{
    private static IDetachedMappedSemanticConstructorArgumentRecorder<TRecord> Target<TRecord, TArgument>(IDetachedMappedSemanticConstructorArgumentRecorderFactory<TRecord> factory, IArgumentPattern<TArgument> pattern, Func<TRecord, TArgument, bool> recorder) => factory.Create(pattern, recorder);

    [Fact]
    public void NullPattern_ArgumentNullException()
    {
        var context = FactoryContext<object>.Create();

        var exception = Record.Exception(() => Target<object, object>(context.Factory, null!, static (dataRecord, argument) => true));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullRecorder_ArgumentNullException()
    {
        var context = FactoryContext<object>.Create();

        var exception = Record.Exception(() => Target(context.Factory, Mock.Of<IArgumentPattern<object>>(), null!));

        Assert.IsType<ArgumentNullException>(exception);
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

        var recorder = Target(context.Factory, patternMock.Object, recorderMock.Object);

        recorder.TryRecordArgument(dataRecord, argument);

        patternMock.Verify((pattern) => pattern.TryFit(argument), Times.Once);
        recorderMock.Verify((recorder) => recorder.Invoke(dataRecord, fitArgument), Times.Once);
    }
}
