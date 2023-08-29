namespace SharpAttributeParser.Mappers.CombinedRecorderFactoryCases.RecorderCases.ConstructorArgumentRecorderCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Mappers.MappedRecorders;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class TryRecordParamsArgument
{
    private static bool Target(ICombinedConstructorArgumentRecorder recorder, IParameterSymbol parameter, object? argument, IReadOnlyList<ExpressionSyntax> elementSyntax) => recorder.TryRecordParamsArgument(parameter, argument, elementSyntax);

    [Fact]
    public void NullParameter_ArgumentNullException()
    {
        var context = RecorderContext<object>.Create();

        var exception = Record.Exception(() => Target(context.Recorder, null!, Mock.Of<object>(), Mock.Of<IReadOnlyList<ExpressionSyntax>>()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullElementSyntax_ArgumentNullException()
    {
        var context = RecorderContext<object>.Create();

        var exception = Record.Exception(() => Target(context.Recorder, Mock.Of<IParameterSymbol>(), Mock.Of<object>(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullReturningMapper_ReturnsFalseAndLogs()
    {
        var parameter = Mock.Of<IParameterSymbol>();

        var context = RecorderContext<object>.Create();

        context.MapperMock.Setup(static (mapper) => mapper.TryMapConstructorParameter(It.IsAny<IParameterSymbol>(), It.IsAny<object>())).Returns((IMappedCombinedConstructorArgumentRecorder?)null);

        var outcome = Target(context.Recorder, parameter, Mock.Of<object>(), Mock.Of<IReadOnlyList<ExpressionSyntax>>());

        Assert.False(outcome);

        context.MapperMock.Verify((mapper) => mapper.TryMapConstructorParameter(parameter, context.DataRecordMock.Object), Times.Once);

        context.LoggerFactoryMock.Verify((factory) => factory.Create<ICombinedRecorder<object>>().ConstructorArgument.FailedToMapConstructorParameterToRecorder(), Times.Once);
    }

    [Fact]
    public void TrueReturningRecorder_ReturnsTrue() => ValidRecorder_PropagatesReturnValue(true);

    [Fact]
    public void FalseReturningRecorder_ReturnsFalse() => ValidRecorder_PropagatesReturnValue(false);

    [AssertionMethod]
    private static void ValidRecorder_PropagatesReturnValue(bool recorderReturnValue)
    {
        var parameter = Mock.Of<IParameterSymbol>();
        var argument = Mock.Of<object>();
        var elementSyntax = Mock.Of<IReadOnlyList<ExpressionSyntax>>();

        var context = RecorderContext<object>.Create();

        context.MapperMock.Setup(static (mapper) => mapper.TryMapConstructorParameter(It.IsAny<IParameterSymbol>(), It.IsAny<object>())!.TryRecordParamsArgument(It.IsAny<object?>(), It.IsAny<IReadOnlyList<ExpressionSyntax>>())).Returns(recorderReturnValue);

        var outcome = Target(context.Recorder, parameter, argument, elementSyntax);

        Assert.Equal(recorderReturnValue, outcome);

        context.MapperMock.Verify((mapper) => mapper.TryMapConstructorParameter(parameter, context.DataRecordMock.Object)!.TryRecordParamsArgument(argument, elementSyntax), Times.Once);
    }
}
