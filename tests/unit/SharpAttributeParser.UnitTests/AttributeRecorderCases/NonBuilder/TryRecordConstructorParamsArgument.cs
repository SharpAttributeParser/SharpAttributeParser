namespace SharpAttributeParser.AttributeRecorderCases.NonBuilder;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class TryRecordConstructorParamsArgument
{
    private IAttributeRecorderFactory RecorderFactory { get; }

    public TryRecordConstructorParamsArgument(IAttributeRecorderFactory factory)
    {
        RecorderFactory = factory;
    }

    private static bool Target(IAttributeRecorder recorder, IParameterSymbol parameter, object? argument, IReadOnlyList<ExpressionSyntax> syntax) => recorder.TryRecordConstructorParamsArgument(parameter, argument, syntax);

    [Fact]
    public void NullParameter_ArgumentNullException()
    {
        var recorder = RecorderFactory.Create(Mock.Of<IAttributeMapper<string>>(), string.Empty);

        var exception = Record.Exception(() => Target(recorder, null!, null, ExpressionSyntaxFactory.CreateCollection()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullSyntaxCollection_ArgumentNullException()
    {
        var recorder = RecorderFactory.Create(Mock.Of<IAttributeMapper<string>>(), string.Empty);

        var exception = Record.Exception(() => Target(recorder, Mock.Of<IParameterSymbol>(), null, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullReturningMapper_ReturnsFalse()
    {
        var recorder = RecorderFactory.Create(Mock.Of<IAttributeMapper<string>>(), string.Empty);

        var outcome = Target(recorder, Mock.Of<IParameterSymbol>(), null, ExpressionSyntaxFactory.CreateCollection());

        Assert.False(outcome);
    }

    [Fact]
    public void FalseReturningRecorderDelegate_UsesProvidedDelegateAndReturnsFalse() => DelegateReturningMapper_UsesProvidedDelegateAndPropagatesValue(false);

    [Fact]
    public void DelegateReturningMapper_UsesProvidedDelegateAndReturnsTrue() => DelegateReturningMapper_UsesProvidedDelegateAndPropagatesValue(true);

    [AssertionMethod]
    private void DelegateReturningMapper_UsesProvidedDelegateAndPropagatesValue(bool returnValue)
    {
        Mock<IAttributeMapper<string>> mapperMock = new();

        Data data = new();

        var argument = Mock.Of<object>();
        var syntax = ExpressionSyntaxFactory.CreateCollection();

        mapperMock.Setup(static (mapper) => mapper.TryMapConstructorParameter(It.IsAny<IParameterSymbol>(), It.IsAny<string>())).Returns(tryMapConstructorParameter());

        var recorder = RecorderFactory.Create(mapperMock.Object, string.Empty);

        var outcome = Target(recorder, Mock.Of<IParameterSymbol>(), argument, syntax);

        Assert.Equal(returnValue, outcome);

        Assert.Equal(argument, data.Argument);
        Assert.Equal(syntax, data.Syntax);

        IAttributeConstructorArgumentRecorder? tryMapConstructorParameter() => new AttributeArgumentRecorder((argument, syntax) =>
        {
            data.Argument = argument;
            data.Syntax = syntax.AsT1;

            return returnValue;
        });
    }

    private sealed class Data
    {
        public object? Argument { get; set; }
        public IReadOnlyList<ExpressionSyntax>? Syntax { get; set; }
    }
}
