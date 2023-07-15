namespace SharpAttributeParser.ASemanticArgumentRecorderCases;

using System;

using Xunit;

public sealed class TryRecordNamedArgument_Single
{
    private static bool Target(ISemanticArgumentRecorder recorder, string parameterName, object? value) => recorder.TryRecordNamedArgument(parameterName, value);

    [Fact]
    public void NullParameterName_ArgumentNullException()
    {
        var comparerMock = StringComparerMock.CreateMock(true);

        SemanticArgumentRecorder recorder = new(comparerMock.Object);

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

        var actual = Target(recorder, string.Empty, null);

        Assert.False(actual);

        Assert.False(recorder.SingleValueRecorded);
    }

    [AssertionMethod]
    private static void TrueAndRecorded(object? value)
    {
        SemanticArgumentRecorder recorder = new(StringComparerMock.CreateComparer(true));

        var actual = Target(recorder, string.Empty, value);

        Assert.True(actual);

        Assert.Equal(value, recorder.SingleValue);
        Assert.True(recorder.SingleValueRecorded);
    }
}
