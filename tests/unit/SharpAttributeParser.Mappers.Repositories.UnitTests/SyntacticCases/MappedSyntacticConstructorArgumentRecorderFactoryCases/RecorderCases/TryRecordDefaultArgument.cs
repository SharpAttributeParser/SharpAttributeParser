namespace SharpAttributeParser.Mappers.Repositories.SyntacticCases.MappedSyntacticConstructorArgumentRecorderFactoryCases.RecorderCases;

using Moq;

using SharpAttributeParser.Mappers.MappedRecorders;

using Xunit;

public sealed class TryRecordDefaultArgument
{
    private static bool Target(IMappedSyntacticConstructorArgumentRecorder recorder) => recorder.TryRecordDefaultArgument();

    [Fact]
    public void TrueReturningDetachedRecorder_RecordsArgumentAndReturnsTrue() => Valid_RecordsArgumentAndPropagatesReturnValue(true);

    [Fact]
    public void FalseReturningDetachedRecorder_RecordsArgumentAndReturnsFalse() => Valid_RecordsArgumentAndPropagatesReturnValue(false);

    [AssertionMethod]
    private static void Valid_RecordsArgumentAndPropagatesReturnValue(bool recorderReturnValue)
    {
        var context = RecorderContext<object>.Create();

        context.DetachedRecorderMock.Setup(static (recorder) => recorder.TryRecordDefaultArgument(It.IsAny<object>())).Returns(recorderReturnValue);

        var outcome = Target(context.Recorder);

        Assert.Equal(recorderReturnValue, outcome);

        context.DetachedRecorderMock.Verify((recorder) => recorder.TryRecordDefaultArgument(context.DataRecord), Times.Once);
    }
}
