namespace SharpAttributeParser.Mappers.Repositories.CombinedCases.MappedCombinedNamedArgumentRecorderFactoryCases.RecorderCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Mappers.MappedRecorders;

using System;

using Xunit;

public sealed class TryRecordArgument
{
    private static bool Target(IMappedCombinedNamedArgumentRecorder recorder, object? argument, ExpressionSyntax syntax) => recorder.TryRecordArgument(argument, syntax);

    [Fact]
    public void NullSyntax_ArgumentNullException()
    {
        var context = RecorderContext<object>.Create();

        var exception = Record.Exception(() => Target(context.Recorder, Mock.Of<object>(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void TrueReturningDetachedRecorder_RecordsArgumentAndReturnsTrue() => Valid_RecordsArgumentAndPropagatesReturnValue(true);

    [Fact]
    public void FalseReturningDetachedRecorder_RecordsArgumentAndReturnsFalse() => Valid_RecordsArgumentAndPropagatesReturnValue(false);

    [AssertionMethod]
    private static void Valid_RecordsArgumentAndPropagatesReturnValue(bool recorderReturnValue)
    {
        var argument = Mock.Of<object>();
        var syntax = ExpressionSyntaxFactory.Create();

        var context = RecorderContext<object>.Create();

        context.DetachedRecorderMock.Setup(static (recorder) => recorder.TryRecordArgument(It.IsAny<object>(), It.IsAny<object?>(), It.IsAny<ExpressionSyntax>())).Returns(recorderReturnValue);

        var outcome = Target(context.Recorder, argument, syntax);

        Assert.Equal(recorderReturnValue, outcome);

        context.DetachedRecorderMock.Verify((recorder) => recorder.TryRecordArgument(context.DataRecord, argument, syntax), Times.Once);
    }
}
