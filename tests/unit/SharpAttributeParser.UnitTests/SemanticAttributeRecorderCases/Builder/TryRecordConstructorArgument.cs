namespace SharpAttributeParser.SemanticAttributeRecorderCases.Builder;

using Microsoft.CodeAnalysis;

using Moq;

using System;

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
        var recorder = RecorderFactory.Create<string, IAttributeDataBuilder<string>>(Mock.Of<ISemanticAttributeMapper<IAttributeDataBuilder<string>>>(), Mock.Of<IAttributeDataBuilder<string>>());

        var exception = Record.Exception(() => Target(recorder, null!, null));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullReturningMapper_ReturnsFalse()
    {
        var recorder = RecorderFactory.Create<string, IAttributeDataBuilder<string>>(Mock.Of<ISemanticAttributeMapper<IAttributeDataBuilder<string>>>(), Mock.Of<IAttributeDataBuilder<string>>());

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
        Mock<ISemanticAttributeMapper<IAttributeDataBuilder<string>>> mapperMock = new();

        var argumentRecorded = false;

        mapperMock.Setup(static (mapper) => mapper.TryMapConstructorParameter(It.IsAny<IAttributeDataBuilder<string>>(), It.IsAny<IParameterSymbol>())).Returns(recorderDelegate);

        var recorder = RecorderFactory.Create(mapperMock.Object, Mock.Of<IAttributeDataBuilder<string>>());

        var outcome = Target(recorder, Mock.Of<IParameterSymbol>(), null);

        Assert.Equal(returnValue, outcome);

        Assert.True(argumentRecorded);

        bool recorderDelegate(object? argument)
        {
            argumentRecorded = true;

            return returnValue;
        }
    }
}
