namespace SharpAttributeParser.Mappers.Repositories.SemanticCases.MappedSemanticTypeArgumentRecorderFactoryCases.RecorderCases;

using Microsoft.CodeAnalysis;

using Moq;

using SharpAttributeParser.Mappers.MappedRecorders;

using System;

using Xunit;

public sealed class TryRecordArgument
{
    private static bool Target(IMappedSemanticTypeArgumentRecorder recorder, ITypeSymbol argument) => recorder.TryRecordArgument(argument);

    [Fact]
    public void NullArgument_ArgumentNullException()
    {
        var context = RecorderContext<object>.Create();

        var exception = Record.Exception(() => Target(context.Recorder, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void TrueReturningDetachedRecorder_RecordsArgumentAndReturnsTrue() => Valid_RecordsArgumentAndPropagatesReturnValue(true);

    [Fact]
    public void FalseReturningDetachedRecorder_RecordsArgumentAndReturnsFalse() => Valid_RecordsArgumentAndPropagatesReturnValue(false);

    [AssertionMethod]
    private static void Valid_RecordsArgumentAndPropagatesReturnValue(bool recorderReturnValue)
    {
        var argument = Mock.Of<ITypeSymbol>();

        var context = RecorderContext<object>.Create();

        context.DetachedRecorderMock.Setup(static (recorder) => recorder.TryRecordArgument(It.IsAny<object>(), It.IsAny<ITypeSymbol>())).Returns(recorderReturnValue);

        var outcome = Target(context.Recorder, argument);

        Assert.Equal(recorderReturnValue, outcome);

        context.DetachedRecorderMock.Verify((recorder) => recorder.TryRecordArgument(context.DataRecord, argument), Times.Once);
    }
}
