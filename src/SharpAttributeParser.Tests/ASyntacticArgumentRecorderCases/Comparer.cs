namespace SharpAttributeParser.Tests.ASyntacticArgumentRecorderCases;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;

using Xunit;

public class Comparer
{
    private static void TryRecordGenericArgument(ASyntacticArgumentRecorder recorder, ITypeParameterSymbol parameter, ITypeSymbol value, Location location) => recorder.TryRecordGenericArgument(parameter, value, location);
    private static void TryRecordConstructorArgument(ASyntacticArgumentRecorder recorder, IParameterSymbol parameter, object? value, Location location) => recorder.TryRecordConstructorArgument(parameter, value, location);
    private static void TryRecordConstructorArgument(ASyntacticArgumentRecorder recorder, IParameterSymbol parameter, IReadOnlyList<object?>? value, Location collectionLocation, IReadOnlyList<Location> elementLocations) => recorder.TryRecordConstructorArgument(parameter, value, collectionLocation, elementLocations);
    private static void TryRecordNamedArgument(ASyntacticArgumentRecorder recorder, string parameterName, object? value, Location location) => recorder.TryRecordNamedArgument(parameterName, value, location);
    private static void TryRecordNamedArgument(ASyntacticArgumentRecorder recorder, string parameterName, IReadOnlyList<object?>? value, Location collectionLocation, IReadOnlyList<Location> elementLocations) => recorder.TryRecordNamedArgument(parameterName, value, collectionLocation, elementLocations);

    [Fact]
    public void Null_InvalidOperationExceptionWhenUsed()
    {
        var comparer = Datasets.GetNullComparer();

        SyntacticArgumentRecorder recorder = new(comparer);

        var parameter = Datasets.GetMockedTypeParameter();
        var value = Datasets.GetValidTypeSymbol();
        var location = Datasets.GetValidLocation();

        var exception = Record.Exception(() => TryRecordGenericArgument(recorder, parameter, value, location));

        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void TryRecordGenericArgument_InvokedAtLeastOnce()
    {
        var comparerMock = ComparerMock.GetComparer(true);

        SyntacticArgumentRecorder recorder = new(comparerMock.Object);

        var parameter = Datasets.GetMockedTypeParameter();
        var value = Datasets.GetValidTypeSymbol();
        var location = Datasets.GetValidLocation();

        TryRecordGenericArgument(recorder, parameter, value, location);

        ComparerMock.VerifyInvoked(comparerMock);
    }

    [Fact]
    public void TryRecordConstructorArgument_Single_InvokedAtLeastOnce()
    {
        var comparerMock = ComparerMock.GetComparer(true);

        SyntacticArgumentRecorder recorder = new(comparerMock.Object);

        var parameter = Datasets.GetMockedParameter();
        var value = Datasets.GetValidTypeSymbol();
        var location = Datasets.GetValidLocation();

        TryRecordConstructorArgument(recorder, parameter, value, location);

        ComparerMock.VerifyInvoked(comparerMock);
    }

    [Fact]
    public void TryRecordConstructorArgument_Array_InvokedAtLeastOnce()
    {
        var comparerMock = ComparerMock.GetComparer(true);

        SyntacticArgumentRecorder recorder = new(comparerMock.Object);

        var parameter = Datasets.GetMockedParameter();
        var value = new[] { Datasets.GetValidTypeSymbol() };
        var collectionLocation = Datasets.GetValidLocation();
        var elementLocations = Datasets.GetValidElementLocations();

        TryRecordConstructorArgument(recorder, parameter, value, collectionLocation, elementLocations);

        ComparerMock.VerifyInvoked(comparerMock);
    }

    [Fact]
    public void TryRecordNamedArgument_Single_InvokedAtLeastOnce()
    {
        var comparerMock = ComparerMock.GetComparer(true);

        SyntacticArgumentRecorder recorder = new(comparerMock.Object);

        var parameterName = Datasets.GetValidParameterName();
        var value = Datasets.GetValidTypeSymbol();
        var location = Datasets.GetValidLocation();

        TryRecordNamedArgument(recorder, parameterName, value, location);

        ComparerMock.VerifyInvoked(comparerMock);
    }

    [Fact]
    public void TryRecordNamedArgument_Array_InvokedAtLeastOnce()
    {
        var comparerMock = ComparerMock.GetComparer(true);

        SyntacticArgumentRecorder recorder = new(comparerMock.Object);

        var parameterName = Datasets.GetValidParameterName();
        var value = new[] { Datasets.GetValidTypeSymbol() };
        var collectionLocation = Datasets.GetValidLocation();
        var elementLocations = Datasets.GetValidElementLocations();

        TryRecordNamedArgument(recorder, parameterName, value, collectionLocation, elementLocations);

        ComparerMock.VerifyInvoked(comparerMock);
    }
}
