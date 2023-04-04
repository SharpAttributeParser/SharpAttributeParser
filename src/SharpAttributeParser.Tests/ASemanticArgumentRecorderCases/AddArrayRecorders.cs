namespace SharpAttributeParser.Tests.ASemanticArgumentRecorderCases;

using System;
using System.Collections.Generic;

using Xunit;

public class AddArrayRecorders
{
    private static void TryRecordNamedArgument(ASemanticArgumentRecorder recorder, string parameterName, object? value) => recorder.TryRecordNamedArgument(parameterName, value);

    [Fact]
    public void Null_InvalidOperationExceptionWhenUsed()
    {
        NullGenericRecorder recorder = new();

        var parameterName = Datasets.GetValidParameterName();
        var value = Datasets.GetValidTypeSymbol();

        var exception = Record.Exception(() => TryRecordNamedArgument(recorder, parameterName, value));

        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void NullName_InvalidOperationExceptionWhenUsed()
    {
        NullNameRecorder recorder = new();

        var parameterName = Datasets.GetValidParameterName();
        var value = Datasets.GetValidTypeSymbol();

        var exception = Record.Exception(() => TryRecordNamedArgument(recorder, parameterName, value));

        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void NullDelegate_InvalidOperationExceptionWhenUsed()
    {
        NullDelegateRecorder recorder = new();

        var parameterName = Datasets.GetValidParameterName();
        var value = Datasets.GetValidTypeSymbol();

        var exception = Record.Exception(() => TryRecordNamedArgument(recorder, parameterName, value));

        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void Duplicate_InvalidOperationExceptionWhenUsed()
    {
        DuplicateGenericRecorder recorder = new();

        var parameterName = Datasets.GetValidParameterName();
        var value = Datasets.GetValidTypeSymbol();

        var exception = Record.Exception(() => TryRecordNamedArgument(recorder, parameterName, value));

        Assert.IsType<InvalidOperationException>(exception);
    }

    private sealed class NullGenericRecorder : ASemanticArgumentRecorder
    {
        protected override IEnumerable<(string, DArrayRecorder)> AddArrayRecorders() => null!;
    }

    private sealed class NullNameRecorder : ASemanticArgumentRecorder
    {
        protected override IEnumerable<(string, DArrayRecorder)> AddArrayRecorders()
        {
            yield return (null!, RecordValue);
        }

        private static bool RecordValue(IReadOnlyList<object?>? _) => true;
    }

    private sealed class NullDelegateRecorder : ASemanticArgumentRecorder
    {
        protected override IEnumerable<(string, DArrayRecorder)> AddArrayRecorders()
        {
            yield return ("A", null!);
        }
    }

    private sealed class DuplicateGenericRecorder : ASemanticArgumentRecorder
    {
        protected override IEnumerable<(string, DArrayRecorder)> AddArrayRecorders()
        {
            yield return ("A", RecordValue);
            yield return ("A", RecordValue);
        }

        private static bool RecordValue(IReadOnlyList<object?>? _) => true;
    }
}
