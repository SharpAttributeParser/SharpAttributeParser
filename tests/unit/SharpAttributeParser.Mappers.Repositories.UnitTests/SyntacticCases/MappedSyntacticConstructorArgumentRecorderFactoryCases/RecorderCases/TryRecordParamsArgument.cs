namespace SharpAttributeParser.Mappers.Repositories.SyntacticCases.MappedSyntacticConstructorArgumentRecorderFactoryCases.RecorderCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Mappers.MappedRecorders;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class TryRecordParamsArgument
{
    private static bool Target(IMappedSyntacticConstructorArgumentRecorder recorder, IReadOnlyList<ExpressionSyntax> elementSyntax) => recorder.TryRecordParamsArgument(elementSyntax);

    [Fact]
    public void NullElementSyntax_ArgumentNullException()
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
        var elementSyntax = Mock.Of<IReadOnlyList<ExpressionSyntax>>();

        var context = RecorderContext<object>.Create();

        context.DetachedRecorderMock.Setup(static (recorder) => recorder.TryRecordParamsArgument(It.IsAny<object>(), It.IsAny<IReadOnlyList<ExpressionSyntax>>())).Returns(recorderReturnValue);

        var outcome = Target(context.Recorder, elementSyntax);

        Assert.Equal(recorderReturnValue, outcome);

        context.DetachedRecorderMock.Verify((recorder) => recorder.TryRecordParamsArgument(context.DataRecord, elementSyntax), Times.Once);
    }
}
