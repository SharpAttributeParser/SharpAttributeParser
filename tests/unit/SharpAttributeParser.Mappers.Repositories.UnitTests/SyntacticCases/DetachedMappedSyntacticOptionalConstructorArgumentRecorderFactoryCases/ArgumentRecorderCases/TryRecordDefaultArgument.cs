namespace SharpAttributeParser.Mappers.Repositories.SyntacticCases.DetachedMappedSyntacticOptionalConstructorArgumentRecorderFactoryCases.ArgumentRecorderCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using OneOf;
using OneOf.Types;

using SharpAttributeParser.Mappers.Repositories.Syntactic;

using System;

using Xunit;

public sealed class TryRecordDefaultArgument
{
    private static bool Target<TRecord>(IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord> recorder, TRecord dataRecord) => recorder.TryRecordDefaultArgument(dataRecord);

    [Fact]
    public void NullDataRecord_ArgumentNullException()
    {
        var context = RecorderContext<object>.Create();

        var exception = Record.Exception(() => Target(context.Recorder, null!));

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

        var context = RecorderContext<object>.Create();

        context.RecorderDelegateMock.Setup(static (recorder) => recorder.Invoke(It.IsAny<object>(), It.IsAny<OneOf<None, ExpressionSyntax>>())).Returns(recorderReturnValue);

        var outcome = Target(context.Recorder, dataRecord);

        Assert.Equal(recorderReturnValue, outcome);

        context.RecorderDelegateMock.Verify((recorder) => recorder.Invoke(dataRecord, new None()), Times.Once);
    }
}
