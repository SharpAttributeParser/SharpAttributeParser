namespace SharpAttributeParser.Mappers.Repositories.CombinedCases.DetachedMappedCombinedOptionalConstructorArgumentRecorderFactoryCases.PatternedArgumentRecorderCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using OneOf;
using OneOf.Types;

using SharpAttributeParser.Mappers.Repositories.Combined;

using System;

using Xunit;

public sealed class TryRecordDefaultArgument
{
    private static bool Target<TRecord, TArgument>(IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> recorder, TRecord dataRecord, TArgument argument) => recorder.TryRecordDefaultArgument(dataRecord, argument);

    [Fact]
    public void NullDataRecord_ArgumentNullException()
    {
        var context = RecorderContext<object, object>.Create();

        var exception = Record.Exception(() => Target(context.Recorder, null!, Mock.Of<object>()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ErrorReturningPatternAndTrueReturningDelegate_UsesPatternAndReturnsFalse()
    {
        var argument = Mock.Of<object>();

        var context = RecorderContext<object, object>.Create();

        context.PatternMock.Setup(static (pattern) => pattern.TryFit(It.IsAny<object?>())).Returns(new Error());
        context.RecorderDelegateMock.Setup(static (recorder) => recorder.Invoke(It.IsAny<object>(), It.IsAny<object>(), It.IsAny<OneOf<None, ExpressionSyntax>>())).Returns(true);

        var outcome = Target(context.Recorder, Mock.Of<object>(), argument);

        Assert.False(outcome);

        context.PatternMock.Verify((pattern) => pattern.TryFit(argument), Times.Once);
    }

    [Fact]
    public void ResultReturningPatternAndTrueReturningDelegate_UsesPatternAndDelegateAndReturnsTrue() => ResultReturningPatternAndValidDelegate_UsesPatternAndDelegateAndPropagatesReturnValue(true);

    [Fact]
    public void ResultReturningPatternAndFalseReturningDelegate_UsesPatternAndDelegateAndReturnsFalse() => ResultReturningPatternAndValidDelegate_UsesPatternAndDelegateAndPropagatesReturnValue(false);

    [AssertionMethod]
    private static void ResultReturningPatternAndValidDelegate_UsesPatternAndDelegateAndPropagatesReturnValue(bool recorderReturnValue)
    {
        var dataRecord = Mock.Of<object>();
        var argument = Mock.Of<object>();

        var fitArgument = Mock.Of<object>();

        var context = RecorderContext<object, object>.Create();

        context.PatternMock.Setup(static (pattern) => pattern.TryFit(It.IsAny<object?>())).Returns(fitArgument);
        context.RecorderDelegateMock.Setup(static (recorder) => recorder.Invoke(It.IsAny<object>(), It.IsAny<object>(), It.IsAny<OneOf<None, ExpressionSyntax>>())).Returns(recorderReturnValue);

        var outcome = Target(context.Recorder, dataRecord, argument);

        Assert.Equal(recorderReturnValue, outcome);

        context.PatternMock.Verify((pattern) => pattern.TryFit(argument), Times.Once);
        context.RecorderDelegateMock.Verify((recorder) => recorder.Invoke(dataRecord, fitArgument, new None()), Times.Once);
    }
}
