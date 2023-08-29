namespace SharpAttributeParser.Mappers.Repositories.CombinedCases.MappedCombinedConstructorArgumentRecorderFactoryCases.RecorderCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Mappers.MappedRecorders;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class TryRecordParamsArgument
{
    private static bool Target(IMappedCombinedConstructorArgumentRecorder recorder, object? argument, IReadOnlyList<ExpressionSyntax> elementSyntax) => recorder.TryRecordParamsArgument(argument, elementSyntax);

    [Fact]
    public void NullElementSyntax_ArgumentNullException()
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
        var elementSyntax = Mock.Of<IReadOnlyList<ExpressionSyntax>>();

        var context = RecorderContext<object>.Create();

        context.DetachedRecorderMock.Setup(static (recorder) => recorder.TryRecordParamsArgument(It.IsAny<object>(), It.IsAny<object?>(), It.IsAny<IReadOnlyList<ExpressionSyntax>>())).Returns(recorderReturnValue);

        var outcome = Target(context.Recorder, argument, elementSyntax);

        Assert.Equal(recorderReturnValue, outcome);

        context.DetachedRecorderMock.Verify((recorder) => recorder.TryRecordParamsArgument(context.DataRecord, argument, elementSyntax), Times.Once);
    }
}
