namespace SharpAttributeParser.Mappers.Repositories.SyntacticCases.DetachedMappedSyntacticTypeArgumentRecorderFactoryCases.ArgumentRecorderCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Syntactic;

using System;

using Xunit;

public sealed class TryRecordArgument
{
    private static bool Target<TRecord>(IDetachedMappedSyntacticTypeArgumentRecorder<TRecord> recorder, TRecord dataRecord, ExpressionSyntax syntax) => recorder.TryRecordArgument(dataRecord, syntax);

    [Fact]
    public void NullDataRecord_ArgumentNullException()
    {
        var context = RecorderContext<object>.Create();

        var exception = Record.Exception(() => Target(context.Recorder, null!, ExpressionSyntaxFactory.Create()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullSyntax_ArgumentNullException()
    {
        var context = RecorderContext<object>.Create();

        var exception = Record.Exception(() => Target(context.Recorder, Mock.Of<object>(), null!));

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
        var syntax = ExpressionSyntaxFactory.Create();

        var context = RecorderContext<object>.Create();

        context.RecorderDelegateMock.Setup(static (recorder) => recorder.Invoke(It.IsAny<object>(), It.IsAny<ExpressionSyntax>())).Returns(recorderReturnValue);

        var outcome = Target(context.Recorder, dataRecord, syntax);

        Assert.Equal(recorderReturnValue, outcome);

        context.RecorderDelegateMock.Verify((recorder) => recorder.Invoke(dataRecord, syntax), Times.Once);
    }
}
