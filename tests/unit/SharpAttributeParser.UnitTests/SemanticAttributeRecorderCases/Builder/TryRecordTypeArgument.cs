namespace SharpAttributeParser.SemanticAttributeRecorderCases.Builder;

using Microsoft.CodeAnalysis;

using Moq;

using System;

using Xunit;

public sealed class TryRecordTypeArgument
{
    private ISemanticAttributeRecorderFactory RecorderFactory { get; }

    public TryRecordTypeArgument(ISemanticAttributeRecorderFactory factory)
    {
        RecorderFactory = factory;
    }

    private static bool Target(ISemanticAttributeRecorder recorder, ITypeParameterSymbol parameter, ITypeSymbol argument) => recorder.TryRecordTypeArgument(parameter, argument);

    [Fact]
    public void NullParameter_ArgumentNullException()
    {
        var recorder = RecorderFactory.Create<string, IRecordBuilder<string>>(Mock.Of<ISemanticAttributeMapper<IRecordBuilder<string>>>(), Mock.Of<IRecordBuilder<string>>());

        var exception = Record.Exception(() => Target(recorder, null!, Mock.Of<ITypeSymbol>()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullArgument_ArgumentNullException()
    {
        var recorder = RecorderFactory.Create<string, IRecordBuilder<string>>(Mock.Of<ISemanticAttributeMapper<IRecordBuilder<string>>>(), Mock.Of<IRecordBuilder<string>>());

        var exception = Record.Exception(() => Target(recorder, Mock.Of<ITypeParameterSymbol>(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullReturningMapper_ReturnsFalse()
    {
        var recorder = RecorderFactory.Create<string, IRecordBuilder<string>>(Mock.Of<ISemanticAttributeMapper<IRecordBuilder<string>>>(), Mock.Of<IRecordBuilder<string>>());

        var outcome = Target(recorder, Mock.Of<ITypeParameterSymbol>(), Mock.Of<ITypeSymbol>());

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

        var argumentRecorded = false;

        mapperMock.Setup(static (mapper) => mapper.TryMapTypeParameter(It.IsAny<ITypeParameterSymbol>(), It.IsAny<IRecordBuilder<string>>())).Returns(tryMapTypeParameter());

        var recorder = RecorderFactory.Create<string, IRecordBuilder<string>>(mapperMock.Object, Mock.Of<IRecordBuilder<string>>());

        var outcome = Target(recorder, Mock.Of<ITypeParameterSymbol>(), Mock.Of<ITypeSymbol>());

        Assert.Equal(returnValue, outcome);

        Assert.True(argumentRecorded);

        ISemanticAttributeArgumentRecorder? tryMapTypeParameter() => new SemanticAttributeArgumentRecorder((object? argument) =>
        {
            argumentRecorded = true;

            return returnValue;
        });
    }
}
