namespace SharpAttributeParser.ASyntacticArgumentRecorderCases;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class TryRecordNamedArgument_Single
{
    private static bool Target(ISyntacticArgumentRecorder recorder, string parameterName, object? value, Location location) => recorder.TryRecordNamedArgument(parameterName, value, location);

    [Fact]
    public void NullParameterName_ArgumentNullException()
    {
        var comparerMock = StringComparerMock.CreateMock(true);

        SyntacticArgumentRecorder recorder = new(comparerMock.Object);

        var exception = Record.Exception(() => Target(recorder, null!, null, Location.None));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullLocation_ArgumentNullException()
    {
        var comparerMock = StringComparerMock.CreateMock(true);

        SyntacticArgumentRecorder recorder = new(comparerMock.Object);

        var exception = Record.Exception(() => Target(recorder, string.Empty, null, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullValue_True_Recorded() => TrueAndRecorded(null);

    [Fact]
    public void NonNullValue_True_Recorded() => TrueAndRecorded(string.Empty);

    [Fact]
    public void NotMatching_False_NotRecorded()
    {
        SyntacticArgumentRecorder recorder = new(StringComparerMock.CreateComparer(false));

        var actual = Target(recorder, string.Empty, null, Location.None);

        Assert.False(actual);

        Assert.False(recorder.SingleValueRecorded);
    }

    [AssertionMethod]
    private static void TrueAndRecorded(object? value)
    {
        SyntacticArgumentRecorder recorder = new(StringComparerMock.CreateComparer(true));

        var location = CustomLocation.Create();

        var actual = Target(recorder, string.Empty, value, location);

        Assert.True(actual);

        Assert.True(recorder.SingleValueRecorded);
        Assert.Equal(value, recorder.SingleValue);
        Assert.Equal(location, recorder.ValueLocation, ReferenceEqualityComparer.Instance);
    }
}
