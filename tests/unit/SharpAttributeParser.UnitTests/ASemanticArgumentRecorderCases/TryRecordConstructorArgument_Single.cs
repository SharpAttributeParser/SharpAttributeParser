namespace SharpAttributeParser.ASemanticArgumentRecorderCases;

using Microsoft.CodeAnalysis;

using Moq;

using System;

using Xunit;

public sealed class TryRecordConstructorArgument_Single
{
    private static bool Target(ISemanticArgumentRecorder recorder, IParameterSymbol parameter, object? value) => recorder.TryRecordConstructorArgument(parameter, value);

    [Fact]
    public void NullParameter_ArgumentNullException()
    {
        SemanticArgumentRecorder recorder = new(StringComparer.OrdinalIgnoreCase);

        var exception = Record.Exception(() => Target(recorder, null!, null));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullValue_True_Recorded() => TrueAndRecorded(null);

    [Fact]
    public void NonNullValue_True_Recorded() => TrueAndRecorded(string.Empty);

    [Fact]
    public void NotMatching_False_NotRecorded()
    {
        SemanticArgumentRecorder recorder = new(StringComparerMock.CreateComparer(false));

        var actual = Target(recorder, Mock.Of<IParameterSymbol>(static (symbol) => symbol.Name == string.Empty), null);

        Assert.False(actual);

        Assert.False(recorder.SingleValueRecorded);
    }

    [AssertionMethod]
    private static void TrueAndRecorded(object? value)
    {
        SemanticArgumentRecorder recorder = new(StringComparerMock.CreateComparer(true));

        var actual = Target(recorder, Mock.Of<IParameterSymbol>(static (symbol) => symbol.Name == string.Empty), value);

        Assert.True(actual);

        Assert.Equal(value, recorder.SingleValue);
        Assert.True(recorder.SingleValueRecorded);
    }
}
