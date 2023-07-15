namespace SharpAttributeParser.ASemanticArgumentRecorderCases;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class AddSingleRecorders
{
    [Fact]
    public void Null_InvalidOperationExceptionWhenUsed()
    {
        NullGenericRecorder recorder = new();

        var exception = Record.Exception(() => RecordArgument(recorder));

        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void NullName_InvalidOperationExceptionWhenUsed()
    {
        NullNameRecorder recorder = new();

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
    public void DuplicateName_InvalidOperationExceptionWhenUsed()
    {
        DuplicateNameGenericRecorder recorder = new();

        var exception = Record.Exception(() => RecordArgument(recorder));

        Assert.IsType<InvalidOperationException>(exception);
    }

    private static void RecordArgument(ASemanticArgumentRecorder recorder) => recorder.TryRecordNamedArgument(string.Empty, null);

    private sealed class NullGenericRecorder : ASemanticArgumentRecorder
    {
        protected override IEnumerable<(string, DSemanticSingleRecorder)> AddSingleRecorders() => null!;
    }

    private sealed class NullNameRecorder : ASemanticArgumentRecorder
    {
        protected override IEnumerable<(string, DSemanticSingleRecorder)> AddSingleRecorders()
        {
            yield return (null!, RecordValue);
        }

        private static bool RecordValue(object? _) => true;
    }

    private sealed class NullDelegateRecorder : ASemanticArgumentRecorder
    {
        protected override IEnumerable<(string, DSemanticSingleRecorder)> AddSingleRecorders()
        {
            yield return ("A", null!);
        }
    }

    private sealed class DuplicateNameGenericRecorder : ASemanticArgumentRecorder
    {
        protected override IEnumerable<(string, DSemanticSingleRecorder)> AddSingleRecorders()
        {
            yield return ("A", RecordValue);
            yield return ("A", RecordValue);
        }

        private static bool RecordValue(object? _) => true;
    }
}
