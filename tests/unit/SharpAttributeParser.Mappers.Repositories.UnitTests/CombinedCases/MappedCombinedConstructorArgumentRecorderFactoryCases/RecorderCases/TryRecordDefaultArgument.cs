namespace SharpAttributeParser.Mappers.Repositories.CombinedCases.MappedCombinedConstructorArgumentRecorderFactoryCases.RecorderCases;

using Moq;

using SharpAttributeParser.Mappers.MappedRecorders;

using Xunit;

public sealed class TryRecordDefaultArgument
{
    private static bool Target(IMappedCombinedConstructorArgumentRecorder recorder, object? argument) => recorder.TryRecordDefaultArgument(argument);

    [Fact]
    public void TrueReturningDetachedRecorder_RecordsArgumentAndReturnsTrue() => Valid_RecordsArgumentAndPropagatesReturnValue(true);

    [Fact]
    public void FalseReturningDetachedRecorder_RecordsArgumentAndReturnsFalse() => Valid_RecordsArgumentAndPropagatesReturnValue(false);

    [AssertionMethod]
    private static void Valid_RecordsArgumentAndPropagatesReturnValue(bool recorderReturnValue)
    {
        var argument = Mock.Of<object>();

        var context = RecorderContext<object>.Create();

        context.DetachedRecorderMock.Setup(static (recorder) => recorder.TryRecordDefaultArgument(It.IsAny<object>(), It.IsAny<object?>())).Returns(recorderReturnValue);

        var outcome = Target(context.Recorder, argument);

        Assert.Equal(recorderReturnValue, outcome);

        context.DetachedRecorderMock.Verify((recorder) => recorder.TryRecordDefaultArgument(context.DataRecord, argument), Times.Once);
    }
}
