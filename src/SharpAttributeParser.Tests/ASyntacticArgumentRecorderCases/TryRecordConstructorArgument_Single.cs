namespace SharpAttributeParser.Tests.ASyntacticArgumentRecorderCases;

using Microsoft.CodeAnalysis;

using System;

using Xunit;

public class TryRecordConstructorArgument_Single
{
    private static bool Target(ASyntacticArgumentRecorder recorder, string parameterName, object? value, Location location) => recorder.TryRecordConstructorArgument(parameterName, value, location);

    [Fact]
    public void NullParameterName_ArgumentNullException()
    {
        SyntacticArgumentRecorder recorder = new(StringComparer.OrdinalIgnoreCase);

        var parameterName = Datasets.GetNullParameterName();
        var value = Datasets.GetValidTypeSymbol();
        var location = Datasets.GetValidLocation();

        var exception = Record.Exception(() => Target(recorder, parameterName, value, location));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullLocation_ArgumentNullException()
    {
        var comparerMock = ComparerMock.GetComparer(true);

        SyntacticArgumentRecorder recorder = new(comparerMock.Object);

        var parameterName = Datasets.GetValidParameterName();
        var value = Datasets.GetValidTypeSymbol();
        var location = Datasets.GetNullLocation();

        var exception = Record.Exception(() => Target(recorder, parameterName, value, location));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullValue_True_ArgumentRecorded()
    {
        var comparerMock = ComparerMock.GetComparer(true);

        SyntacticArgumentRecorder recorder = new(comparerMock.Object);

        var parameterName = Datasets.GetValidParameterName();
        var value = Datasets.GetNullTypeSymbol();
        var location = Datasets.GetValidLocation();

        var actual = Target(recorder, parameterName, value, location);

        Assert.True(actual);

        Assert.Null(recorder.Value);
        Assert.Equal(location, recorder.ValueLocation);
    }

    [Fact]
    public void Matching_True_ArgumentRecorded()
    {
        var comparerMock = ComparerMock.GetComparer(true);

        SyntacticArgumentRecorder recorder = new(comparerMock.Object);

        var parameterName = Datasets.GetValidParameterName();
        var value = Datasets.GetValidTypeSymbol();
        var location = Datasets.GetValidLocation();

        var actual = Target(recorder, parameterName, value, location);

        Assert.True(actual);

        Assert.Equal(value, recorder.Value);
        Assert.Equal(location, recorder.ValueLocation);
    }

    [Fact]
    public void NotMatching_False_ArgumentNotRecorded()
    {
        var comparerMock = ComparerMock.GetComparer(false);

        SyntacticArgumentRecorder recorder = new(comparerMock.Object);

        var parameterName = Datasets.GetValidParameterName();
        var value = Datasets.GetValidTypeSymbol();
        var location = Datasets.GetValidLocation();

        var actual = Target(recorder, parameterName, value, location);

        Assert.False(actual);

        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueLocation);
    }
}
