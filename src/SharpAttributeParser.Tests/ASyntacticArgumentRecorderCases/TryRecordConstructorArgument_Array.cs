namespace SharpAttributeParser.Tests.ASyntacticArgumentRecorderCases;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;

using Xunit;

public class TryRecordConstructorArgument_Array
{
    private static bool Target(ASyntacticArgumentRecorder recorder, string parameterName, IReadOnlyList<object?>? value, Location collectionLocation, IReadOnlyList<Location> elementLocations) => recorder.TryRecordConstructorArgument(parameterName, value, collectionLocation, elementLocations);

    [Fact]
    public void NullParameterName_ArgumentNullException()
    {
        SyntacticArgumentRecorder recorder = new(StringComparer.OrdinalIgnoreCase);

        var parameterName = Datasets.GetNullParameterName();
        var value = Datasets.GetValidArrayArgument();
        var collectionLocation = Datasets.GetValidLocation();
        var elementLocations = Datasets.GetValidElementLocations();

        var exception = Record.Exception(() => Target(recorder, parameterName, value, collectionLocation, elementLocations));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullCollectionLocation_ArgumentNullException()
    {
        var comparerMock = ComparerMock.GetComparer(true);

        SyntacticArgumentRecorder recorder = new(comparerMock.Object);

        var parameterName = Datasets.GetValidParameterName();
        var value = Datasets.GetValidArrayArgument();
        var collectionLocation = Datasets.GetNullLocation();
        var elementLocations = Datasets.GetValidElementLocations();

        var exception = Record.Exception(() => Target(recorder, parameterName, value, collectionLocation, elementLocations));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullElementLocations_ArgumentNullException()
    {
        var comparerMock = ComparerMock.GetComparer(true);

        SyntacticArgumentRecorder recorder = new(comparerMock.Object);

        var parameterName = Datasets.GetValidParameterName();
        var value = Datasets.GetValidArrayArgument();
        var collectionLocation = Datasets.GetValidLocation();
        var elementLocations = Datasets.GetNullElementLocations();

        var exception = Record.Exception(() => Target(recorder, parameterName, value, collectionLocation, elementLocations));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullValue_True_ArgumentRecorded()
    {
        var comparerMock = ComparerMock.GetComparer(true);

        SyntacticArgumentRecorder recorder = new(comparerMock.Object);

        var parameterName = Datasets.GetValidParameterName();
        var value = Datasets.GetNullArrayArgument();
        var collectionLocation = Datasets.GetValidLocation();
        var elementLocations = Datasets.GetValidElementLocations();

        var actual = Target(recorder, parameterName, value, collectionLocation, elementLocations);

        Assert.True(actual);

        Assert.Null(recorder.Values);
        Assert.Equal(collectionLocation, recorder.ValuesCollectionLocation);
        Assert.Equal(elementLocations, recorder.ValuesElementLocations);
    }

    [Fact]
    public void Matching_True_ArgumentRecorded()
    {
        var comparerMock = ComparerMock.GetComparer(true);

        SyntacticArgumentRecorder recorder = new(comparerMock.Object);

        var parameterName = Datasets.GetValidParameterName();
        var value = Datasets.GetValidArrayArgument();
        var collectionLocation = Datasets.GetValidLocation();
        var elementLocations = Datasets.GetValidElementLocations();

        var actual = Target(recorder, parameterName, value, collectionLocation, elementLocations);

        Assert.True(actual);

        Assert.Equal(value, recorder.Values);
        Assert.Equal(collectionLocation, recorder.ValuesCollectionLocation);
        Assert.Equal(elementLocations, recorder.ValuesElementLocations);
    }

    [Fact]
    public void NotMatching_False_ArgumentNotRecorded()
    {
        var comparerMock = ComparerMock.GetComparer(false);

        SyntacticArgumentRecorder recorder = new(comparerMock.Object);

        var parameterName = Datasets.GetValidParameterName();
        var value = Datasets.GetValidArrayArgument();
        var collectionLocation = Datasets.GetValidLocation();
        var elementLocations = Datasets.GetValidElementLocations();

        var actual = Target(recorder, parameterName, value, collectionLocation, elementLocations);

        Assert.False(actual);

        Assert.Null(recorder.Values);
        Assert.Null(recorder.ValuesCollectionLocation);
        Assert.Null(recorder.ValuesElementLocations);
    }
}
