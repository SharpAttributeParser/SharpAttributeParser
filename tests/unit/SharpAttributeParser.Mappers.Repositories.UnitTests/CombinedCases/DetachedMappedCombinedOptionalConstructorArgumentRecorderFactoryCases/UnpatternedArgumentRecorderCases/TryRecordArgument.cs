namespace SharpAttributeParser.Mappers.Repositories.CombinedCases.DetachedMappedCombinedOptionalConstructorArgumentRecorderFactoryCases.UnpatternedArgumentRecorderCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using OneOf;
using OneOf.Types;

using SharpAttributeParser.Mappers.Repositories.Combined;

using System;

using Xunit;

public sealed class TryRecordArgument
{
    private static bool Target<TRecord>(IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> recorder, TRecord dataRecord, object? argument, ExpressionSyntax syntax) => recorder.TryRecordArgument(dataRecord, argument, syntax);

    [Fact]
    public void NullDataRecord_ArgumentNullException()
    {
        var context = RecorderContext<object>.Create();

        var exception = Record.Exception(() => Target(context.Recorder, null!, Mock.Of<object>(), ExpressionSyntaxFactory.Create()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullSyntax_ArgumentNullException()
    {
        var context = RecorderContext<object>.Create();

        var exception = Record.Exception(() => Target(context.Recorder, Mock.Of<object>(), Mock.Of<object>(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void TrueReturningDelegate_UsesDelegateAndReturnsTrue() => ValidDelegate_UsesDelegateAndPropagatesReturnValue(true);

    [Fact]
    public void FalseReturningDelegate_UsesDelegateAndReturnsFalse() => ValidDelegate_UsesDelegateAndPropagatesReturnValue(false);

    [AssertionMethod]
    private static void ValidDelegate_UsesDelegateAndPropagatesReturnValue(bool recorderReturnValue)
    {
        var dataRecord = Mock.Of<object>();
        var argument = Mock.Of<object>();
        var syntax = ExpressionSyntaxFactory.Create();

        var context = RecorderContext<object>.Create();

        context.RecorderDelegateMock.Setup(static (recorder) => recorder.Invoke(It.IsAny<object>(), It.IsAny<object?>(), It.IsAny<OneOf<None, ExpressionSyntax>>())).Returns(recorderReturnValue);

        var outcome = Target(context.Recorder, dataRecord, argument, syntax);

        Assert.Equal(recorderReturnValue, outcome);

        context.RecorderDelegateMock.Verify((recorder) => recorder.Invoke(dataRecord, argument, syntax), Times.Once);
    }
}
