namespace SharpAttributeParser.ASemanticArgumentRecorderCases;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class TryRecordNamedArgument_Array
{
    private static bool Target(ISemanticArgumentRecorder recorder, string parameterName, IReadOnlyList<object?>? value) => recorder.TryRecordNamedArgument(parameterName, value);

    [Fact]
    public void NullParameterName_ArgumentNullException()
    {
        SemanticArgumentRecorder recorder = new(StringComparer.OrdinalIgnoreCase);

        var exception = Record.Exception(() => Target(recorder, null!, null));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullValue_True_Recorded() => TrueAndRecorded(null);

    [Fact]
    public void NonNullValue_True_Recorded() => TrueAndRecorded(Array.Empty<object?>());

    [Fact]
    public void NotMatching_False_NotRecorded()
    {
        SemanticArgumentRecorder recorder = new(StringComparerMock.CreateComparer(false));

        var actual = Target(recorder, string.Empty, null);

        Assert.False(actual);

        Assert.False(recorder.ArrayValueRecorded);
    }

    [AssertionMethod]
    private static void TrueAndRecorded(IReadOnlyList<object?>? value)
    {
        SemanticArgumentRecorder recorder = new(StringComparerMock.CreateComparer(true));

        var actual = Target(recorder, string.Empty, value);

        Assert.True(actual);

        Assert.Equal(value, recorder.ArrayValue);
        Assert.True(recorder.ArrayValueRecorded);
    }
}
