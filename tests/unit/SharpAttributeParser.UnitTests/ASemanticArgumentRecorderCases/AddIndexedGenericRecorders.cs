namespace SharpAttributeParser.ASemanticArgumentRecorderCases;

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

    private static void RecordArgument(ASemanticArgumentRecorder recorder) => recorder.TryRecordNamedArgument(string.Empty, null);

    private sealed class NullGenericRecorder : ASemanticArgumentRecorder
    {
        protected override IEnumerable<(int, DSemanticGenericRecorder)> AddIndexedGenericRecorders() => null!;
    }

    private sealed class NegativeIndexRecorder : ASemanticArgumentRecorder
    {
        protected override IEnumerable<(int, DSemanticGenericRecorder)> AddIndexedGenericRecorders()
        {
            yield return (-1, RecordValue);
        }

        private static bool RecordValue(ITypeSymbol _) => true;
    }

    private sealed class NullDelegateRecorder : ASemanticArgumentRecorder
    {
        protected override IEnumerable<(int, DSemanticGenericRecorder)> AddIndexedGenericRecorders()
        {
            yield return (0, null!);
        }
    }

    private sealed class DuplicateIndexGenericRecorder : ASemanticArgumentRecorder
    {
        protected override IEnumerable<(int, DSemanticGenericRecorder)> AddIndexedGenericRecorders()
        {
            yield return (0, RecordValue);
            yield return (0, RecordValue);
        }

        private static bool RecordValue(ITypeSymbol _) => true;
    }
}
