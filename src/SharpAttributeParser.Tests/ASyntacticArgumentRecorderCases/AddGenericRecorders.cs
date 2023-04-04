namespace SharpAttributeParser.Tests.ASyntacticArgumentRecorderCases;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;

using Xunit;

public class AddGenericRecorders
{
    private static void TryRecordNamedArgument(ASyntacticArgumentRecorder recorder, string parameterName, object? value, Location location) => recorder.TryRecordNamedArgument(parameterName, value, location);

    [Fact]
    public void Null_InvalidOperationExceptionWhenUsed()
    {
        NullGenericRecorder recorder = new();

        var parameterName = Datasets.GetValidParameterName();
        var value = Datasets.GetValidTypeSymbol();
        var location = Datasets.GetValidLocation();

        var exception = Record.Exception(() => TryRecordNamedArgument(recorder, parameterName, value, location));

        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void NullName_InvalidOperationExceptionWhenUsed()
    {
        NullNameRecorder recorder = new();

        var parameterName = Datasets.GetValidParameterName();
        var value = Datasets.GetValidTypeSymbol();
        var location = Datasets.GetValidLocation();

        var exception = Record.Exception(() => TryRecordNamedArgument(recorder, parameterName, value, location));

        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void NullDelegate_InvalidOperationExceptionWhenUsed()
    {
        NullDelegateRecorder recorder = new();

        var parameterName = Datasets.GetValidParameterName();
        var value = Datasets.GetValidTypeSymbol();
        var location = Datasets.GetValidLocation();

        var exception = Record.Exception(() => TryRecordNamedArgument(recorder, parameterName, value, location));

        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void Duplicate_InvalidOperationExceptionWhenUsed()
    {
        DuplicateGenericRecorder recorder = new();

        var parameterName = Datasets.GetValidParameterName();
        var value = Datasets.GetValidTypeSymbol();
        var location = Datasets.GetValidLocation();

        var exception = Record.Exception(() => TryRecordNamedArgument(recorder, parameterName, value, location));

        Assert.IsType<InvalidOperationException>(exception);
    }

    private sealed class NullGenericRecorder : ASyntacticArgumentRecorder
    {
        protected override IEnumerable<(string, DGenericRecorder)> AddGenericRecorders() => null!;
    }

    private sealed class NullNameRecorder : ASyntacticArgumentRecorder
    {
        protected override IEnumerable<(string, DGenericRecorder)> AddGenericRecorders()
        {
            yield return (null!, RecordValue);
        }

        private static bool RecordValue(ITypeSymbol value, Location location) => true;
    }

    private sealed class NullDelegateRecorder : ASyntacticArgumentRecorder
    {
        protected override IEnumerable<(string, DGenericRecorder)> AddGenericRecorders()
        {
            yield return ("A", null!);
        }
    }

    private sealed class DuplicateGenericRecorder : ASyntacticArgumentRecorder
    {
        protected override IEnumerable<(string, DGenericRecorder)> AddGenericRecorders()
        {
            yield return ("A", RecordValue);
            yield return ("A", RecordValue);
        }

        private static bool RecordValue(ITypeSymbol value, Location location) => true;
    }
}
