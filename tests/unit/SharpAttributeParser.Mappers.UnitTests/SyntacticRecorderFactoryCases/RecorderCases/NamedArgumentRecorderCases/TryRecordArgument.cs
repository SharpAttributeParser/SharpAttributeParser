namespace SharpAttributeParser.Mappers.SyntacticRecorderFactoryCases.RecorderCases.NamedArgumentRecorderCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Mappers.MappedRecorders;

using System;

using Xunit;

public sealed class TryRecordArgument
{
    private static bool Target(ISyntacticNamedArgumentRecorder recorder, string parameterName, ExpressionSyntax syntax) => recorder.TryRecordArgument(parameterName, syntax);

    [Fact]
    public void NullParameterName_ArgumentNullException()
    {
        var context = RecorderContext<object>.Create();

        var exception = Record.Exception(() => Target(context.Recorder, null!, ExpressionSyntaxFactory.Create()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullSyntax_ArgumentNullException()
    {
        var context = RecorderContext<object>.Create();

        var exception = Record.Exception(() => Target(context.Recorder, string.Empty, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullReturningMapper_ReturnsFalseAndLogs()
    {
        var parameterName = string.Empty;

        var context = RecorderContext<object>.Create();

        context.MapperMock.Setup(static (mapper) => mapper.TryMapNamedParameter(It.IsAny<string>(), It.IsAny<object>())).Returns((IMappedSyntacticNamedArgumentRecorder?)null);

        var outcome = Target(context.Recorder, parameterName, ExpressionSyntaxFactory.Create());

        Assert.False(outcome);

        context.MapperMock.Verify((mapper) => mapper.TryMapNamedParameter(parameterName, context.DataRecordMock.Object), Times.Once);

        context.LoggerFactoryMock.Verify((factory) => factory.Create<ISyntacticRecorder<object>>().NamedArgument.FailedToMapNamedParameterToRecorder(), Times.Once);
    }

    [Fact]
    public void TrueReturningRecorder_ReturnsTrue() => ValidRecorder_PropagatesReturnValue(true);

    [Fact]
    public void FalseReturningRecorder_ReturnsFalse() => ValidRecorder_PropagatesReturnValue(false);

    [AssertionMethod]
    private static void ValidRecorder_PropagatesReturnValue(bool recorderReturnValue)
    {
        var parameterName = string.Empty;
        var syntax = ExpressionSyntaxFactory.Create();

        var context = RecorderContext<object>.Create();

        context.MapperMock.Setup(static (mapper) => mapper.TryMapNamedParameter(It.IsAny<string>(), It.IsAny<object>())!.TryRecordArgument(It.IsAny<ExpressionSyntax>())).Returns(recorderReturnValue);

        var outcome = Target(context.Recorder, parameterName, syntax);

        Assert.Equal(recorderReturnValue, outcome);

        context.MapperMock.Verify((mapper) => mapper.TryMapNamedParameter(parameterName, context.DataRecordMock.Object)!.TryRecordArgument(syntax), Times.Once);
    }
}
