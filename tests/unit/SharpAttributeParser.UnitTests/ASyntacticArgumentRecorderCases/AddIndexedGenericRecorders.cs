namespace SharpAttributeParser.ASyntacticArgumentRecorderCases;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class AddIndexedGenericRecorders
{
    [Fact]
    public void Null_InvalidOperationExceptionWhenUsed()
    {
        NullGenericRecorder recorder = new();

        var exception = Record.Exception(() => RecordArgument(recorder));

        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void NegativeIndex_InvalidOperationExceptionWhenUsed()
    {
        NegativeIndexRecorder recorder = new();

        var exception = Record.Exception(() => RecordArgument(recorder));

        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void NullDelegate_InvalidOperationExceptionWhenUsed()
    {
        NullDelegateRecorder recorder = new();

        var exception = Record.Exception(() => RecordArgument(recorder));

        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void DuplicateIndex_InvalidOperationExceptionWhenUsed()
    {
        DuplicateIndexGenericRecorder recorder = new();

        var exception = Record.Exception(() => RecordArgument(recorder));

        Assert.IsType<InvalidOperationException>(exception);
    }

    private static void RecordArgument(ASyntacticArgumentRecorder recorder) => recorder.TryRecordNamedArgument(string.Empty, null, Location.None);

    private sealed class NullGenericRecorder : ASyntacticArgumentRecorder
    {
        protected override IEnumerable<(int, DSyntacticGenericRecorder)> AddIndexedGenericRecorders() => null!;
    }

    private sealed class NegativeIndexRecorder : ASyntacticArgumentRecorder
    {
        protected override IEnumerable<(int, DSyntacticGenericRecorder)> AddIndexedGenericRecorders()
        {
            yield return (-1, RecordValue);
        }

        private static bool RecordValue(ITypeSymbol value, Location location) => true;
    }

    private sealed class NullDelegateRecorder : ASyntacticArgumentRecorder
    {
        protected override IEnumerable<(int, DSyntacticGenericRecorder)> AddIndexedGenericRecorders()
        {
            yield return (0, null!);
        }
    }

    private sealed class DuplicateIndexGenericRecorder : ASyntacticArgumentRecorder
    {
        protected override IEnumerable<(int, DSyntacticGenericRecorder)> AddIndexedGenericRecorders()
        {
            yield return (0, RecordValue);
            yield return (0, RecordValue);
        }

        private static bool RecordValue(ITypeSymbol value, Location location) => true;
    }
}
