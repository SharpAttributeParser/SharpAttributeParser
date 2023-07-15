namespace SharpAttributeParser.ASyntacticArgumentRecorderCases.AdapterCases;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class ForNullable_Action_Single_Class
{
    private static DSyntacticSingleRecorder Target<T>(ISyntacticAdapterProvider adapters, Action<T?, Location> recorder) where T : class => adapters.ForNullable(recorder);

    [Fact]
    public void NullDelegate_ArgumentNullExceptionWhenUsed()
    {
        NullDelegateRecorder recorder = new();

        var exception = Record.Exception(() => RecordArgument(recorder, null, Location.None));

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

    [AssertionMethod]
    private static void TrueAndRecorded<T1, T2>(T1? expected, T2? value) where T1 : class
    {
        Recorder<T1> recorder = new();

        var location = CustomLocation.Create();

        var actual = RecordArgument(recorder, value, location);

        Assert.True(actual);

        Assert.Equal(expected, recorder.Value);
        Assert.True(recorder.ValueRecorded);
        Assert.Equal(location, recorder.ValueLocation, ReferenceEqualityComparer.Instance);
    }

    [AssertionMethod]
    private static void FalseAndNotRecorded<T1, T2>(T2? value) where T1 : class
    {
        Recorder<T1> recorder = new();

        var actual = RecordArgument(recorder, value, Location.None);

        Assert.False(actual);

        Assert.False(recorder.ValueRecorded);
    }

    private static bool RecordArgument(ASyntacticArgumentRecorder recorder, object? value, Location location) => recorder.TryRecordNamedArgument(string.Empty, value, location);

    private sealed class NullDelegateRecorder : ASyntacticArgumentRecorder
    {
        protected override IEqualityComparer<string> Comparer { get; } = StringComparerMock.CreateComparer(true);

        protected override IEnumerable<(string, DSyntacticSingleRecorder)> AddSingleRecorders()
        {
            yield return (string.Empty, Target<string>(Adapters, null!));
        }
    }

    private sealed class Recorder<T> : ASyntacticArgumentRecorder where T : class
    {
        public T? Value { get; private set; }
        public bool ValueRecorded { get; private set; }
        public Location? ValueLocation { get; private set; }

        protected override IEqualityComparer<string> Comparer { get; } = StringComparerMock.CreateComparer(true);

        protected override IEnumerable<(string, DSyntacticSingleRecorder)> AddSingleRecorders()
        {
            yield return (string.Empty, Target<T>(Adapters, RecordValue));
        }

        private void RecordValue(T? value, Location location)
        {
            Value = value;
            ValueRecorded = true;
            ValueLocation = location;
        }
    }
}
