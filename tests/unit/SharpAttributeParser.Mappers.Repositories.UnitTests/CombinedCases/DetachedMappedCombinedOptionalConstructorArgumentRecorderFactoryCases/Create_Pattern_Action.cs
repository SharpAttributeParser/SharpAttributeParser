namespace SharpAttributeParser.Mappers.Repositories.CombinedCases.DetachedMappedCombinedOptionalConstructorArgumentRecorderFactoryCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using OneOf;
using OneOf.Types;

using SharpAttributeParser.Mappers.Repositories.Combined;
using SharpAttributeParser.Patterns;

using System;

using Xunit;

public sealed class Create_Pattern_Action
{
    private static IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> Target<TRecord, TArgument>(IDetachedMappedCombinedOptionalConstructorArgumentRecorderFactory<TRecord> factory, IArgumentPattern<TArgument> pattern, Action<TRecord, TArgument, OneOf<None, ExpressionSyntax>> recorder) => factory.Create(pattern, recorder);

    [Fact]
    public void NullPattern_ArgumentNullException()
    {
        var context = FactoryContext<object>.Create();

        var exception = Record.Exception(() => Target<object, object>(context.Factory, null!, static (dataRecord, argument, syntax) => { }));

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
    public void ValidPatternAndRecorder_ConstructedRecorderUsesPatternAndDelegateAndReturnsTrue()
    {
        var dataRecord = Mock.Of<object>();
        var argument = Mock.Of<object>();
        var syntax = ExpressionSyntaxFactory.Create();

        var fitArgument = Mock.Of<object>();

        var context = FactoryContext<object>.Create();

        Mock<IArgumentPattern<object>> patternMock = new();
        Mock<Action<object, object?, OneOf<None, ExpressionSyntax>>> recorderMock = new();

        patternMock.Setup(static (pattern) => pattern.TryFit(It.IsAny<object?>())).Returns(fitArgument);

        var recorder = Target(context.Factory, patternMock.Object, recorderMock.Object);

        var outcome = recorder.TryRecordArgument(dataRecord, argument, syntax);

        Assert.True(outcome);

        patternMock.Verify((pattern) => pattern.TryFit(argument), Times.Once);
        recorderMock.Verify((recorder) => recorder.Invoke(dataRecord, fitArgument, syntax), Times.Once);
    }
}
