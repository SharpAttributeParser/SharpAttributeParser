namespace SharpAttributeParser.Tests.ASemanticArgumentRecorderCases;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;

using Xunit;

public class Comparer
{
    private static void TryRecordGenericArgument(ASemanticArgumentRecorder recorder, ITypeParameterSymbol parameter, ITypeSymbol value) => recorder.TryRecordGenericArgument(parameter, value);
    private static void TryRecordConstructorArgument(ASemanticArgumentRecorder recorder, IParameterSymbol parameter, object? value) => recorder.TryRecordConstructorArgument(parameter, value);
    private static void TryRecordConstructorArgument(ASemanticArgumentRecorder recorder, IParameterSymbol parameter, IReadOnlyList<object?>? value) => recorder.TryRecordConstructorArgument(parameter, value);
    private static void TryRecordNamedArgument(ASemanticArgumentRecorder recorder, string parameterName, object? value) => recorder.TryRecordNamedArgument(parameterName, value);
    private static void TryRecordNamedArgument(ASemanticArgumentRecorder recorder, string parameterName, IReadOnlyList<object?>? value) => recorder.TryRecordNamedArgument(parameterName, value);

    [Fact]
    public void Null_InvalidOperationExceptionWhenUsed()
    {
        var comparer = Datasets.GetNullComparer();

        SemanticArgumentRecorder recorder = new(comparer);

        var parameter = Datasets.GetMockedTypeParameter();
        var value = Datasets.GetValidTypeSymbol();

        var exception = Record.Exception(() => TryRecordGenericArgument(recorder, parameter, value));

        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void TryRecordGenericArgument_InvokedAtLeastOnce()
    {
        var comparerMock = ComparerMock.GetComparer(true);

        SemanticArgumentRecorder recorder = new(comparerMock.Object);

        var parameter = Datasets.GetMockedTypeParameter();
        var value = Datasets.GetValidTypeSymbol();

        TryRecordGenericArgument(recorder, parameter, value);

        ComparerMock.VerifyInvoked(comparerMock);
    }

    [Fact]
    public void TryRecordConstructorArgument_Single_InvokedAtLeastOnce()
    {
        var comparerMock = ComparerMock.GetComparer(true);

        SemanticArgumentRecorder recorder = new(comparerMock.Object);

        var parameter = Datasets.GetMockedParameter();
        var value = Datasets.GetValidTypeSymbol();

        TryRecordConstructorArgument(recorder, parameter, value);

        ComparerMock.VerifyInvoked(comparerMock);
    }

    [Fact]
    public void TryRecordConstructorArgument_Array_InvokedAtLeastOnce()
    {
        var comparerMock = ComparerMock.GetComparer(true);

        SemanticArgumentRecorder recorder = new(comparerMock.Object);

        var parameter = Datasets.GetMockedParameter();
        var value = new[] { Datasets.GetValidTypeSymbol() };

        TryRecordConstructorArgument(recorder, parameter, value);

        ComparerMock.VerifyInvoked(comparerMock);
    }

    [Fact]
    public void TryRecordNamedArgument_Single_InvokedAtLeastOnce()
    {
        var comparerMock = ComparerMock.GetComparer(true);

        SemanticArgumentRecorder recorder = new(comparerMock.Object);

        var parameterName = Datasets.GetValidParameterName();
        var value = Datasets.GetValidTypeSymbol();

        TryRecordNamedArgument(recorder, parameterName, value);

        ComparerMock.VerifyInvoked(comparerMock);
    }

    [Fact]
    public void TryRecordNamedArgument_Array_InvokedAtLeastOnce()
    {
        var comparerMock = ComparerMock.GetComparer(true);

        SemanticArgumentRecorder recorder = new(comparerMock.Object);

        var parameterName = Datasets.GetValidParameterName();
        var value = new[] { Datasets.GetValidTypeSymbol() };

        TryRecordNamedArgument(recorder, parameterName, value);

        ComparerMock.VerifyInvoked(comparerMock);
    }
}
