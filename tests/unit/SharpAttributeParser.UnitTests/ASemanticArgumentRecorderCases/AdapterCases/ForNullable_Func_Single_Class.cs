namespace SharpAttributeParser.ASemanticArgumentRecorderCases.AdapterCases;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class ForNullable_Func_Single_Class
{
    private static DSemanticSingleRecorder Target<T>(ISemanticAdapterProvider adapters, Func<T?, bool> recorder) where T : class => adapters.ForNullable(recorder);

    [Fact]
    public void NullDelegate_ArgumentNullExceptionWhenUsed()
    {
        NullDelegateRecorder recorder = new();

        var exception = Record.Exception(() => RecordArgument(recorder, null));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void IntArray_NullElement_True_Recorded()
    {
        var value = new int?[] { null, 2, 3 };

        TrueAndRecorded(value, value);
    }

    [Fact]
    public void IntArray_NullCollection_True_Recorded()
    {
        IReadOnlyList<int?>? value = null;

        TrueAndRecorded(value, value);
    }

    [Fact]
    public void String_SameType_True_Recorded()
    {
        var value = "1";

        TrueAndRecorded(value, value);
    }

    [Fact]
    public void String_Enum_False_NotRecorded()
    {
        var value = StringComparison.OrdinalIgnoreCase;

        FalseAndNotRecorded<string, StringComparison>(value);
    }

    [Fact]
    public void String_Null_True_Recorded()
    {
        string? value = null;

        TrueAndRecorded(value, value);
    }

    [Fact]
    public void FalseReturningRecorder_False_Recorded()
    {
        Recorder<string> recorder = new(false);

        var value = "3";

        var actual = RecordArgument(recorder, value);

        Assert.False(actual);

        Assert.Equal(value, recorder.Value);
        Assert.True(recorder.ValueRecorded);
    }

    [AssertionMethod]
    private static void TrueAndRecorded<T1, T2>(T1? expected, T2? value) where T1 : class
    {
        Recorder<T1> recorder = new(true);

        var actual = RecordArgument(recorder, value);

        Assert.True(actual);

        Assert.Equal(expected, recorder.Value);
        Assert.True(recorder.ValueRecorded);
    }

    [AssertionMethod]
    private static void FalseAndNotRecorded<T1, T2>(T2? value) where T1 : class
    {
        Recorder<T1> recorder = new(true);

        var actual = RecordArgument(recorder, value);

        Assert.False(actual);

        Assert.False(recorder.ValueRecorded);
    }

    private static bool RecordArgument(ASemanticArgumentRecorder recorder, object? value) => recorder.TryRecordNamedArgument(string.Empty, value);

    private sealed class NullDelegateRecorder : ASemanticArgumentRecorder
    {
        protected override IEqualityComparer<string> Comparer { get; } = StringComparerMock.CreateComparer(true);

        protected override IEnumerable<(string, DSemanticSingleRecorder)> AddSingleRecorders()
        {
            yield return (string.Empty, Target<string>(Adapters, null!));
        }
    }

    private sealed class Recorder<T> : ASemanticArgumentRecorder where T : class
    {
        public T? Value { get; private set; }
        public bool ValueRecorded { get; private set; }

        private bool ReturnValue { get; }

        protected override IEqualityComparer<string> Comparer { get; } = StringComparerMock.CreateComparer(true);

        public Recorder(bool returnValue)
        {
            ReturnValue = returnValue;
        }

        protected override IEnumerable<(string, DSemanticSingleRecorder)> AddSingleRecorders()
        {
            yield return (string.Empty, Target<T>(Adapters, RecordValue));
        }

        private bool RecordValue(T? value)
        {
            Value = value;
            ValueRecorded = true;

            return ReturnValue;
        }
    }
}
