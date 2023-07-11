namespace SharpAttributeParser.SemanticAttributeRecorderCases.NonBuilder;

using Microsoft.CodeAnalysis;

using Moq;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class TryRecordConstructorArgument
{
    private ISemanticAttributeRecorderFactory RecorderFactory { get; }

    public TryRecordConstructorArgument(ISemanticAttributeRecorderFactory factory)
    {
        RecorderFactory = factory;
    }

    private static bool Target(ISemanticAttributeRecorder recorder, IParameterSymbol parameter, object? argument) => recorder.TryRecordConstructorArgument(parameter, argument);

    [Fact]
    public void NullParameter_ArgumentNullException()
    {
        var recorder = RecorderFactory.Create(Mock.Of<ISemanticAttributeMapper<string>>(), string.Empty);

        var exception = Record.Exception(() => Target(recorder, null!, null));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullReturningMapper_ReturnsFalse()
    {
        var recorder = RecorderFactory.Create(Mock.Of<ISemanticAttributeMapper<string>>(), string.Empty);

        var outcome = Target(recorder, Mock.Of<IParameterSymbol>(), null);

        Assert.False(outcome);
    }

    [Fact]
    public void FalseReturningRecorderDelegate_UsesProvidedDelegateAndReturnsFalse() => DelegateReturningMapper_UsesProvidedDelegateAndPropagatesValue(false);

    [Fact]
    public void DelegateReturningMapper_UsesProvidedDelegateAndReturnsTrue() => DelegateReturningMapper_UsesProvidedDelegateAndPropagatesValue(true);

    [AssertionMethod]
    private void DelegateReturningMapper_UsesProvidedDelegateAndPropagatesValue(bool returnValue)
    {
        Mock<ISemanticAttributeMapper<string>> mapperMock = new();

        object? recordedArgument = null;
        var argument = Mock.Of<object>();

        mapperMock.Setup(static (mapper) => mapper.TryMapConstructorParameter(It.IsAny<IParameterSymbol>(), It.IsAny<string>())).Returns(tryMapConstructorParameter());

        var recorder = RecorderFactory.Create(mapperMock.Object, string.Empty);

        var outcome = Target(recorder, Mock.Of<IParameterSymbol>(), argument);

        Assert.Equal(returnValue, outcome);

        Assert.Equal(argument, recordedArgument, ReferenceEqualityComparer.Instance);

        ISemanticAttributeArgumentRecorder? tryMapConstructorParameter() => new SemanticAttributeArgumentRecorder((argument) =>
        {
            recordedArgument = argument;

            return returnValue;
        });
    }
}
