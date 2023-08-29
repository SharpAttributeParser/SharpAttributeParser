namespace SharpAttributeParser.Mappers.Repositories.SemanticCases.DetachedMappedSemanticNamedArgumentRecorderFactoryCases.UnpatternedArgumentRecorderCases;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Semantic;

using System;

using Xunit;

public sealed class TryRecordArgument
{
    private static bool Target<TRecord>(IDetachedMappedSemanticNamedArgumentRecorder<TRecord> recorder, TRecord dataRecord, object? argument) => recorder.TryRecordArgument(dataRecord, argument);

    [Fact]
    public void NullDataRecord_ArgumentNullException()
    {
        var context = RecorderContext<object>.Create();

        var exception = Record.Exception(() => Target(context.Recorder, null!, Mock.Of<object>()));

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

        var context = RecorderContext<object>.Create();

        context.RecorderDelegateMock.Setup(static (recorder) => recorder.Invoke(It.IsAny<object>(), It.IsAny<object?>())).Returns(recorderReturnValue);

        var outcome = Target(context.Recorder, dataRecord, argument);

        Assert.Equal(recorderReturnValue, outcome);

        context.RecorderDelegateMock.Verify((recorder) => recorder.Invoke(dataRecord, argument), Times.Once);
    }
}
