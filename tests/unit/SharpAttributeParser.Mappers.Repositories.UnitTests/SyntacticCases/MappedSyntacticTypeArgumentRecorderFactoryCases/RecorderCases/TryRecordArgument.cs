namespace SharpAttributeParser.Mappers.Repositories.SyntacticCases.MappedSyntacticTypeArgumentRecorderFactoryCases.RecorderCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Mappers.MappedRecorders;

using System;

using Xunit;

public sealed class TryRecordArgument
{
    private static bool Target(IMappedSyntacticTypeArgumentRecorder recorder, ExpressionSyntax syntax) => recorder.TryRecordArgument(syntax);

    [Fact]
    public void NullSyntax_ArgumentNullException()
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
        var syntax = ExpressionSyntaxFactory.Create();

        var context = RecorderContext<object>.Create();

        context.DetachedRecorderMock.Setup(static (recorder) => recorder.TryRecordArgument(It.IsAny<object>(), It.IsAny<ExpressionSyntax>())).Returns(recorderReturnValue);

        var outcome = Target(context.Recorder, syntax);

        Assert.Equal(recorderReturnValue, outcome);

        context.DetachedRecorderMock.Verify((recorder) => recorder.TryRecordArgument(context.DataRecord, syntax), Times.Once);
    }
}
