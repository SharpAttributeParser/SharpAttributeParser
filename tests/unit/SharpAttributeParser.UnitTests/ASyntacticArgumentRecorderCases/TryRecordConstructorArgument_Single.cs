namespace SharpAttributeParser.ASyntacticArgumentRecorderCases;

using Microsoft.CodeAnalysis;

using Moq;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class TryRecordConstructorArgument_Single
{
    private static bool Target(ISyntacticArgumentRecorder recorder, IParameterSymbol parameter, object? value, Location location) => recorder.TryRecordConstructorArgument(parameter, value, location);

    [Fact]
    public void NullParameter_ArgumentNullException()
    {
        SyntacticArgumentRecorder recorder = new(StringComparer.OrdinalIgnoreCase);

        var exception = Record.Exception(() => Target(recorder, null!, null, Location.None));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullLocation_ArgumentNullException()
    {
        SyntacticArgumentRecorder recorder = new(StringComparer.OrdinalIgnoreCase);

        var exception = Record.Exception(() => Target(recorder, Mock.Of<IParameterSymbol>(), null, null!));

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

        var actual = Target(recorder, Mock.Of<IParameterSymbol>(static (symbol) => symbol.Name == string.Empty), null, Location.None);

        Assert.False(actual);

        Assert.False(recorder.SingleValueRecorded);
    }

    [AssertionMethod]
    private static void TrueAndRecorded(object? value)
    {
        SyntacticArgumentRecorder recorder = new(StringComparerMock.CreateComparer(true));

        var location = CustomLocation.Create();

        var actual = Target(recorder, Mock.Of<IParameterSymbol>(static (symbol) => symbol.Name == string.Empty), value, location);

        Assert.True(actual);

        Assert.Equal(value, recorder.SingleValue);
        Assert.True(recorder.SingleValueRecorded);
        Assert.Equal(location, recorder.ValueLocation, ReferenceEqualityComparer.Instance);
    }
}
