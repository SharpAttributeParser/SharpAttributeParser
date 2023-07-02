namespace SharpAttributeParser.ASyntacticArgumentRecorderCases;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class AddArrayRecorders
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

    private static void RecordArgument(ASyntacticArgumentRecorder recorder) => recorder.TryRecordNamedArgument(string.Empty, null, CollectionLocation.None);

    private sealed class NullGenericRecorder : ASyntacticArgumentRecorder
    {
        protected override IEnumerable<(string, DSyntacticArrayRecorder)> AddArrayRecorders() => null!;
    }

    private sealed class NullNameRecorder : ASyntacticArgumentRecorder
    {
        protected override IEnumerable<(string, DSyntacticArrayRecorder)> AddArrayRecorders()
        {
            yield return (null!, RecordValue);
        }

        private static bool RecordValue(IReadOnlyList<object?>? value, CollectionLocation location) => true;
    }

    private sealed class NullDelegateRecorder : ASyntacticArgumentRecorder
    {
        protected override IEnumerable<(string, DSyntacticArrayRecorder)> AddArrayRecorders()
        {
            yield return ("A", null!);
        }
    }

    private sealed class DuplicateNameGenericRecorder : ASyntacticArgumentRecorder
    {
        protected override IEnumerable<(string, DSyntacticArrayRecorder)> AddArrayRecorders()
        {
            yield return ("A", RecordValue);
            yield return ("A", RecordValue);
        }

        private static bool RecordValue(IReadOnlyList<object?>? value, CollectionLocation location) => true;
    }
}
