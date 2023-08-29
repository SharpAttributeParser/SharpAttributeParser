namespace SharpAttributeParser.Mappers.SyntacticRecorderFactoryCases.RecorderCases.TypeArgumentRecorderCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Mappers.MappedRecorders;

using System;

using Xunit;

public sealed class TryRecordArgument
{
    private static bool Target(ISyntacticTypeArgumentRecorder recorder, ITypeParameterSymbol parameter, ExpressionSyntax syntax) => recorder.TryRecordArgument(parameter, syntax);

    [Fact]
    public void NullParameter_ArgumentNullException()
    {
        var context = RecorderContext<object>.Create();

        var exception = Record.Exception(() => Target(context.Recorder, null!, ExpressionSyntaxFactory.Create()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullSyntax_ArgumentNullException()
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

        context.MapperMock.Setup(static (mapper) => mapper.TryMapTypeParameter(It.IsAny<ITypeParameterSymbol>(), It.IsAny<object>())).Returns((IMappedSyntacticTypeArgumentRecorder?)null);

        var outcome = Target(context.Recorder, parameter, ExpressionSyntaxFactory.Create());

        Assert.False(outcome);

        context.MapperMock.Verify((mapper) => mapper.TryMapTypeParameter(parameter, context.DataRecordMock.Object), Times.Once);

        context.LoggerFactoryMock.Verify((factory) => factory.Create<ISyntacticRecorder<object>>().TypeArgument.FailedToMapTypeParameterToRecorder(), Times.Once);
    }

    [Fact]
    public void TrueReturningRecorder_ReturnsTrue() => ValidRecorder_PropagatesReturnValue(true);

    [Fact]
    public void FalseReturningRecorder_ReturnsFalse() => ValidRecorder_PropagatesReturnValue(false);

    [AssertionMethod]
    private static void ValidRecorder_PropagatesReturnValue(bool recorderReturnValue)
    {
        var parameter = Mock.Of<ITypeParameterSymbol>();
        var syntax = ExpressionSyntaxFactory.Create();

        var context = RecorderContext<object>.Create();

        context.MapperMock.Setup(static (mapper) => mapper.TryMapTypeParameter(It.IsAny<ITypeParameterSymbol>(), It.IsAny<object>())!.TryRecordArgument(It.IsAny<ExpressionSyntax>())).Returns(recorderReturnValue);

        var outcome = Target(context.Recorder, parameter, syntax);

        Assert.Equal(recorderReturnValue, outcome);

        context.MapperMock.Verify((mapper) => mapper.TryMapTypeParameter(parameter, context.DataRecordMock.Object)!.TryRecordArgument(syntax), Times.Once);
    }
}
