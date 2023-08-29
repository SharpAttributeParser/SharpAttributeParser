namespace SharpAttributeParser.Mappers.CombinedRecorderFactoryCases.RecorderCases.NamedArgumentRecorderCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Mappers.MappedRecorders;

using System;

using Xunit;

public sealed class TryRecordArgument
{
    private static bool Target(ICombinedNamedArgumentRecorder recorder, string parameterName, object? argument, ExpressionSyntax syntax) => recorder.TryRecordArgument(parameterName, argument, syntax);

    [Fact]
    public void NullParameterName_ArgumentNullException()
    {
        var context = RecorderContext<object>.Create();

        var exception = Record.Exception(() => Target(context.Recorder, null!, Mock.Of<object>(), ExpressionSyntaxFactory.Create()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullSyntax_ArgumentNullException()
    {
        var context = RecorderContext<object>.Create();

        var exception = Record.Exception(() => Target(context.Recorder, string.Empty, Mock.Of<object>(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullReturningMapper_ReturnsFalseAndLogs()
    {
        var parameterName = string.Empty;

        var context = RecorderContext<object>.Create();

        context.MapperMock.Setup(static (mapper) => mapper.TryMapNamedParameter(It.IsAny<string>(), It.IsAny<object>())).Returns((IMappedCombinedNamedArgumentRecorder?)null);

        var outcome = Target(context.Recorder, parameterName, Mock.Of<object>(), ExpressionSyntaxFactory.Create());

        Assert.False(outcome);

        context.MapperMock.Verify((mapper) => mapper.TryMapNamedParameter(parameterName, context.DataRecordMock.Object), Times.Once);

        context.LoggerFactoryMock.Verify((factory) => factory.Create<ICombinedRecorder<object>>().NamedArgument.FailedToMapNamedParameterToRecorder(), Times.Once);
    }

    [Fact]
    public void TrueReturningRecorder_ReturnsTrue() => ValidRecorder_PropagatesReturnValue(true);

    [Fact]
    public void FalseReturningRecorder_ReturnsFalse() => ValidRecorder_PropagatesReturnValue(false);

    [AssertionMethod]
    private static void ValidRecorder_PropagatesReturnValue(bool recorderReturnValue)
    {
        var parameterName = string.Empty;
        var argument = Mock.Of<object>();
        var syntax = ExpressionSyntaxFactory.Create();

        var context = RecorderContext<object>.Create();

        context.MapperMock.Setup(static (mapper) => mapper.TryMapNamedParameter(It.IsAny<string>(), It.IsAny<object>())!.TryRecordArgument(It.IsAny<object?>(), It.IsAny<ExpressionSyntax>())).Returns(recorderReturnValue);

        var outcome = Target(context.Recorder, parameterName, argument, syntax);

        Assert.Equal(recorderReturnValue, outcome);

        context.MapperMock.Verify((mapper) => mapper.TryMapNamedParameter(parameterName, context.DataRecordMock.Object)!.TryRecordArgument(argument, syntax), Times.Once);
    }
}
