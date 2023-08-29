namespace SharpAttributeParser.Mappers.SemanticRecorderFactoryCases.RecorderCases.NamedArgumentRecorderCases;

using Microsoft.CodeAnalysis;

using Moq;

using SharpAttributeParser.Mappers.MappedRecorders;

using System;

using Xunit;

public sealed class TryRecordArgument
{
    private static bool Target(ISemanticNamedArgumentRecorder recorder, string parameterName, object? argument) => recorder.TryRecordArgument(parameterName, argument);

    [Fact]
    public void NullParameterName_ArgumentNullException()
    {
        var context = RecorderContext<object>.Create();

        var exception = Record.Exception(() => Target(context.Recorder, null!, Mock.Of<ITypeSymbol>()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullReturningMapper_ReturnsFalseAndLogs()
    {
        var parameterName = string.Empty;

        var context = RecorderContext<object>.Create();

        context.MapperMock.Setup(static (mapper) => mapper.TryMapNamedParameter(It.IsAny<string>(), It.IsAny<object>())).Returns((IMappedSemanticNamedArgumentRecorder?)null);

        var outcome = Target(context.Recorder, parameterName, Mock.Of<object>());

        Assert.False(outcome);

        context.MapperMock.Verify((mapper) => mapper.TryMapNamedParameter(parameterName, context.DataRecordMock.Object), Times.Once);

        context.LoggerFactoryMock.Verify((factory) => factory.Create<ISemanticRecorder<object>>().NamedArgument.FailedToMapNamedParameterToRecorder(), Times.Once);
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

        var context = RecorderContext<object>.Create();

        context.MapperMock.Setup(static (mapper) => mapper.TryMapNamedParameter(It.IsAny<string>(), It.IsAny<object>())!.TryRecordArgument(It.IsAny<object?>())).Returns(recorderReturnValue);

        var outcome = Target(context.Recorder, parameterName, argument);

        Assert.Equal(recorderReturnValue, outcome);

        context.MapperMock.Verify((mapper) => mapper.TryMapNamedParameter(parameterName, context.DataRecordMock.Object)!.TryRecordArgument(argument), Times.Once);
    }
}
