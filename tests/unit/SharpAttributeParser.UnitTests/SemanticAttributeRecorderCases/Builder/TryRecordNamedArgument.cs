namespace SharpAttributeParser.SemanticAttributeRecorderCases.Builder;

using Moq;

using System;

using Xunit;

public sealed class TryRecordNamedArgument
{
    private ISemanticAttributeRecorderFactory RecorderFactory { get; }

    public TryRecordNamedArgument(ISemanticAttributeRecorderFactory factory)
    {
        RecorderFactory = factory;
    }

    private static bool Target(ISemanticAttributeRecorder recorder, string parameterName, object? argument) => recorder.TryRecordNamedArgument(parameterName, argument);

    [Fact]
    public void NullParameterName_ArgumentNullException()
    {
        var recorder = RecorderFactory.Create<string, IRecordBuilder<string>>(Mock.Of<ISemanticAttributeMapper<IRecordBuilder<string>>>(), Mock.Of<IRecordBuilder<string>>());

        var exception = Record.Exception(() => Target(recorder, null!, null));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullReturningMapper_ReturnsFalse()
    {
        var recorder = RecorderFactory.Create<string, IRecordBuilder<string>>(Mock.Of<ISemanticAttributeMapper<IRecordBuilder<string>>>(), Mock.Of<IRecordBuilder<string>>());

        var outcome = Target(recorder, string.Empty, null);

        Assert.False(outcome);
    }

    [Fact]
    public void FalseReturningRecorderDelegate_UsesProvidedDelegateAndReturnsFalse() => DelegateReturningMapper_UsesProvidedDelegateAndPropagatesValue(false);

    [Fact]
    public void DelegateReturningMapper_UsesProvidedDelegateAndReturnsTrue() => DelegateReturningMapper_UsesProvidedDelegateAndPropagatesValue(true);

    [AssertionMethod]
    private void DelegateReturningMapper_UsesProvidedDelegateAndPropagatesValue(bool returnValue)
    {
        Mock<ISemanticAttributeMapper<IRecordBuilder<string>>> mapperMock = new();

        Data data = new();

        var argument = Mock.Of<object>();

        mapperMock.Setup(static (mapper) => mapper.TryMapNamedParameter(It.IsAny<string>(), It.IsAny<IRecordBuilder<string>>())).Returns(tryMapNamedParameter());

        var recorder = RecorderFactory.Create(mapperMock.Object, Mock.Of<IRecordBuilder<string>>());

        var outcome = Target(recorder, string.Empty, argument);

        Assert.Equal(returnValue, outcome);

        Assert.Equal(argument, data.Argument);

        ISemanticAttributeArgumentRecorder? tryMapNamedParameter() => new SemanticAttributeArgumentRecorder((argument) =>
        {
            data.Argument = argument;

            return returnValue;
        });
    }

    private sealed class Data
    {
        public object? Argument { get; set; }
    }
}
