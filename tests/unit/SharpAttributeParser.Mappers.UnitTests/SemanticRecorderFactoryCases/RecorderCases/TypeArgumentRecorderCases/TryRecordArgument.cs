namespace SharpAttributeParser.Mappers.SemanticRecorderFactoryCases.RecorderCases.TypeArgumentRecorderCases;

using Microsoft.CodeAnalysis;

using Moq;

using SharpAttributeParser.Mappers.MappedRecorders;

using System;

using Xunit;

public sealed class TryRecordArgument
{
    private static bool Target(ISemanticTypeArgumentRecorder recorder, ITypeParameterSymbol parameter, ITypeSymbol argument) => recorder.TryRecordArgument(parameter, argument);

    [Fact]
    public void NullParameter_ArgumentNullException()
    {
        var context = RecorderContext<object>.Create();

        var exception = Record.Exception(() => Target(context.Recorder, null!, Mock.Of<ITypeSymbol>()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullArgument_ArgumentNullException()
    {
        var context = RecorderContext<object>.Create();

        var exception = Record.Exception(() => Target(context.Recorder, Mock.Of<ITypeParameterSymbol>(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullReturningMapper_ReturnsFalseAndLogs()
    {
        var parameter = Mock.Of<ITypeParameterSymbol>();

        var context = RecorderContext<object>.Create();

        context.MapperMock.Setup(static (mapper) => mapper.TryMapTypeParameter(It.IsAny<ITypeParameterSymbol>(), It.IsAny<object>())).Returns((IMappedSemanticTypeArgumentRecorder?)null);

        var outcome = Target(context.Recorder, parameter, Mock.Of<ITypeSymbol>());

        Assert.False(outcome);

        context.MapperMock.Verify((mapper) => mapper.TryMapTypeParameter(parameter, context.DataRecordMock.Object), Times.Once);

        context.LoggerFactoryMock.Verify((factory) => factory.Create<ISemanticRecorder<object>>().TypeArgument.FailedToMapTypeParameterToRecorder(), Times.Once);
    }

    [Fact]
    public void TrueReturningRecorder_ReturnsTrue() => ValidRecorder_PropagatesReturnValue(true);

    [Fact]
    public void FalseReturningRecorder_ReturnsFalse() => ValidRecorder_PropagatesReturnValue(false);

    [AssertionMethod]
    private static void ValidRecorder_PropagatesReturnValue(bool recorderReturnValue)
    {
        var parameter = Mock.Of<ITypeParameterSymbol>();
        var argument = Mock.Of<ITypeSymbol>();

        var context = RecorderContext<object>.Create();

        context.MapperMock.Setup(static (mapper) => mapper.TryMapTypeParameter(It.IsAny<ITypeParameterSymbol>(), It.IsAny<object>())!.TryRecordArgument(It.IsAny<ITypeSymbol>())).Returns(recorderReturnValue);

        var outcome = Target(context.Recorder, parameter, argument);

        Assert.Equal(recorderReturnValue, outcome);

        context.MapperMock.Verify((mapper) => mapper.TryMapTypeParameter(parameter, context.DataRecordMock.Object)!.TryRecordArgument(argument), Times.Once);
    }
}
