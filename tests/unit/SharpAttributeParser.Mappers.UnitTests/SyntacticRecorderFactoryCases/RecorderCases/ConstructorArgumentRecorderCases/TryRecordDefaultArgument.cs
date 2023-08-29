namespace SharpAttributeParser.Mappers.SyntacticRecorderFactoryCases.RecorderCases.ConstructorArgumentRecorderCases;

using Microsoft.CodeAnalysis;

using Moq;

using SharpAttributeParser.Mappers.MappedRecorders;

using System;

using Xunit;

public sealed class TryRecordDefaultArgument
{
    private static bool Target(ISyntacticConstructorArgumentRecorder recorder, IParameterSymbol parameter) => recorder.TryRecordDefaultArgument(parameter);

    [Fact]
    public void NullParameter_ArgumentNullException()
    {
        var context = RecorderContext<object>.Create();

        var exception = Record.Exception(() => Target(context.Recorder, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullReturningMapper_ReturnsFalseAndLogs()
    {
        var parameter = Mock.Of<IParameterSymbol>();

        var context = RecorderContext<object>.Create();

        context.MapperMock.Setup(static (mapper) => mapper.TryMapConstructorParameter(It.IsAny<IParameterSymbol>(), It.IsAny<object>())).Returns((IMappedSyntacticConstructorArgumentRecorder?)null);

        var outcome = Target(context.Recorder, parameter);

        Assert.False(outcome);

        context.MapperMock.Verify((mapper) => mapper.TryMapConstructorParameter(parameter, context.DataRecordMock.Object), Times.Once);

        context.LoggerFactoryMock.Verify((factory) => factory.Create<ISyntacticRecorder<object>>().ConstructorArgument.FailedToMapConstructorParameterToRecorder(), Times.Once);
    }

    [Fact]
    public void TrueReturningRecorder_ReturnsTrue() => ValidRecorder_PropagatesReturnValue(true);

    [Fact]
    public void FalseReturningRecorder_ReturnsFalse() => ValidRecorder_PropagatesReturnValue(false);

    [AssertionMethod]
    private static void ValidRecorder_PropagatesReturnValue(bool recorderReturnValue)
    {
        var parameter = Mock.Of<IParameterSymbol>();

        var context = RecorderContext<object>.Create();

        context.MapperMock.Setup(static (mapper) => mapper.TryMapConstructorParameter(It.IsAny<IParameterSymbol>(), It.IsAny<object>())!.TryRecordDefaultArgument()).Returns(recorderReturnValue);

        var outcome = Target(context.Recorder, parameter);

        Assert.Equal(recorderReturnValue, outcome);

        context.MapperMock.Verify((mapper) => mapper.TryMapConstructorParameter(parameter, context.DataRecordMock.Object)!.TryRecordDefaultArgument(), Times.Once);
    }
}
