namespace SharpAttributeParser.ASyntacticArgumentRecorderCases;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class TryRecordNamedArgument_Array
{
    private static bool Target(ISyntacticArgumentRecorder recorder, string parameterName, IReadOnlyList<object?>? value, CollectionLocation location) => recorder.TryRecordNamedArgument(parameterName, value, location);

    [Fact]
    public void NullParameterName_ArgumentNullException()
    {
        SyntacticArgumentRecorder recorder = new(StringComparer.OrdinalIgnoreCase);

        var exception = Record.Exception(() => Target(recorder, null!, null, CollectionLocation.None));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullLocation_ArgumentNullException()
    {
        SyntacticArgumentRecorder recorder = new(StringComparer.OrdinalIgnoreCase);

        var exception = Record.Exception(() => Target(recorder, string.Empty, null, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullValue_True_Recorded() => TrueAndRecorded(null);

    [Fact]
    public void NonNullValue_True_Recorded() => TrueAndRecorded(Array.Empty<object?>());

    [Fact]
    public void NotMatching_False_NotRecorded()
    {
        SyntacticArgumentRecorder recorder = new(StringComparerMock.CreateComparer(false));

        var actual = Target(recorder, string.Empty, null, CollectionLocation.None);

        Assert.False(actual);

        Assert.False(recorder.ArrayValueRecorded);
    }

    [AssertionMethod]
    private static void TrueAndRecorded(IReadOnlyList<object?>? value)
    {
        SyntacticArgumentRecorder recorder = new(StringComparerMock.CreateComparer(true));

        var location = CustomLocation.CreateCollection();

        var actual = Target(recorder, string.Empty, value, location);

        Assert.True(actual);

        Assert.True(recorder.ArrayValueRecorded);
        Assert.Equal(value, recorder.ArrayValue);
        Assert.Equal(location, recorder.ArrayValueLocation, ReferenceEqualityComparer.Instance);
    }
}
