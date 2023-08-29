namespace SharpAttributeParser.Mappers.Repositories.CombinedCases.DetachedMappedCombinedNamedArgumentRecorderFactoryCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Combined;
using SharpAttributeParser.Patterns;

using System;

using Xunit;

public sealed class Create_PatternDelegate_Action
{
    private static IDetachedMappedCombinedNamedArgumentRecorder<TRecord> Target<TRecord, TArgument>(IDetachedMappedCombinedNamedArgumentRecorderFactory<TRecord> factory, Func<IArgumentPatternFactory, IArgumentPattern<TArgument>> patternDelegate, Action<TRecord, TArgument, ExpressionSyntax> recorder) => factory.Create(patternDelegate, recorder);

    [Fact]
    public void NullPatternDelegate_ArgumentNullException()
    {
        var context = FactoryContext<object>.Create();

        var exception = Record.Exception(() => Target<object, object>(context.Factory, null!, static (dataRecord, argument, syntax) => { }));

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

        var exception = Record.Exception(() => Target<object, object>(context.Factory, static (factory) => null!, static (dataRecord, argument, syntax) => { }));

        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public void ProvidedPatternFactoryIsSpecified()
    {
        var context = FactoryContext<object>.Create();

        Target(context.Factory, patternDelegate, static (dataRecord, argument, syntax) => { });

        IArgumentPattern<object> patternDelegate(IArgumentPatternFactory factory)
        {
            Assert.Same(context.ArgumentPatternFactory, factory);

            return Mock.Of<IArgumentPattern<object>>();
        }
    }

    [Fact]
    public void ValidPatternAndRecorder_ConstructedRecorderUsesPatternAndDelegateAndReturnsTrue()
    {
        var dataRecord = Mock.Of<object>();
        var argument = Mock.Of<object>();
        var syntax = ExpressionSyntaxFactory.Create();

        var fitArgument = Mock.Of<object>();

        var context = FactoryContext<object>.Create();

        Mock<IArgumentPattern<object>> patternMock = new();
        Mock<Action<object, object?, ExpressionSyntax>> recorderMock = new();

        patternMock.Setup(static (pattern) => pattern.TryFit(It.IsAny<object?>())).Returns(fitArgument);

        var recorder = Target(context.Factory, (factory) => patternMock.Object, recorderMock.Object);

        var outcome = recorder.TryRecordArgument(dataRecord, argument, syntax);

        Assert.True(outcome);

        patternMock.Verify((pattern) => pattern.TryFit(argument), Times.Once);
        recorderMock.Verify((recorder) => recorder.Invoke(dataRecord, fitArgument, syntax), Times.Once);
    }
}
