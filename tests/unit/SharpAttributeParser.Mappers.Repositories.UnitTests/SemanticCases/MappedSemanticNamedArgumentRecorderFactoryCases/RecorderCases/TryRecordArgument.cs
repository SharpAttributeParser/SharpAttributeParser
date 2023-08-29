namespace SharpAttributeParser.Mappers.Repositories.SemanticCases.MappedSemanticNamedArgumentRecorderFactoryCases.RecorderCases;

using Moq;

using SharpAttributeParser.Mappers.MappedRecorders;

using Xunit;

public sealed class TryRecordArgument
{
    private static bool Target(IMappedSemanticNamedArgumentRecorder recorder, object? argument) => recorder.TryRecordArgument(argument);

    [Fact]
    public void TrueReturningDetachedRecorder_RecordsArgumentAndReturnsTrue() => Valid_RecordsArgumentAndPropagatesReturnValue(true);

    [Fact]
    public void FalseReturningDetachedRecorder_RecordsArgumentAndReturnsFalse() => Valid_RecordsArgumentAndPropagatesReturnValue(false);

    [AssertionMethod]
    private static void Valid_RecordsArgumentAndPropagatesReturnValue(bool recorderReturnValue)
    {
        var argument = Mock.Of<object>();

        var context = RecorderContext<object>.Create();

        context.DetachedRecorderMock.Setup(static (recorder) => recorder.TryRecordArgument(It.IsAny<object>(), It.IsAny<object?>())).Returns(recorderReturnValue);

        var outcome = Target(context.Recorder, argument);

        Assert.Equal(recorderReturnValue, outcome);

        context.DetachedRecorderMock.Verify((recorder) => recorder.TryRecordArgument(context.DataRecord, argument), Times.Once);
    }
}
