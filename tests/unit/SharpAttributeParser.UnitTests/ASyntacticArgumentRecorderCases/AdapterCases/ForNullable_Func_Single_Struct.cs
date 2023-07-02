namespace SharpAttributeParser.ASyntacticArgumentRecorderCases.AdapterCases;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class ForNullable_Func_Single_Struct
{
    private static DSyntacticSingleRecorder Target<T>(ISyntacticAdapterProvider adapters, Func<T?, Location, bool> recorder) where T : struct => adapters.ForNullable(recorder);

    [Fact]
    public void NullDelegate_ArgumentNullExceptionWhenUsed()
    {
        NullDelegateRecorder recorder = new();

        var exception = Record.Exception(() => RecordArgument(recorder, null, Location.None));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Enum_SameType_True_Recorded()
    {
        var value = StringComparison.OrdinalIgnoreCase;

        TrueAndRecorded<StringComparison, StringComparison>(value, value);
    }

    [Fact]
    public void Enum_DifferentEnumType_False_NotRecorded()
    {
        var value = StringSplitOptions.TrimEntries;

        FalseAndNotRecorded<StringComparison, StringSplitOptions>(value);
    }

    [Fact]
    public void Enum_Null_True_Recorded()
    {
        StringComparison? value = null;

        TrueAndRecorded(value, value);
    }

    [Fact]
    public void Enum_Int_False_NotRecorded()
    {
        var value = 3;

        FalseAndNotRecorded<StringComparison, int>(value);
    }

    [Fact]
    public void Int_SameType_True_Recorded()
    {
        var value = 3;

        TrueAndRecorded<int, int>(value, value);
    }

    [Fact]
    public void Int_Enum_False_NotRecorded()
    {
        var value = StringComparison.Ordinal;

        FalseAndNotRecorded<int, StringComparison>(value);
    }

    [Fact]
    public void Int_Double_False_NotRecorded()
    {
        var value = 2.718;

        FalseAndNotRecorded<int, double>(value);
    }

    [Fact]
    public void Int_String_False_NotRecorded()
    {
        var value = "5";

        FalseAndNotRecorded<int, string>(value);
    }

    [Fact]
    public void Int_Null_True_Recorded()
    {
        int? value = null;

        TrueAndRecorded(value, value);
    }

    [Fact]
    public void FalseReturningRecorder_False_Recorded()
    {
        Recorder<int> recorder = new(false);

        var value = 3;

        var location = CustomLocation.Create();

        var actual = RecordArgument(recorder, value, location);

        Assert.False(actual);

        Assert.Equal(value, recorder.Value);
        Assert.True(recorder.ValueRecorded);
        Assert.Equal(location, recorder.ValueLocation, ReferenceEqualityComparer.Instance);
    }

    [AssertionMethod]
    private static void TrueAndRecorded<T1, T2>(T1? expected, T2? value) where T1 : struct
    {
        Recorder<T1> recorder = new(true);

        var location = CustomLocation.Create();

        var actual = RecordArgument(recorder, value, location);

        Assert.True(actual);

        Assert.Equal(expected, recorder.Value);
        Assert.True(recorder.ValueRecorded);
        Assert.Equal(location, recorder.ValueLocation, ReferenceEqualityComparer.Instance);
    }

    [AssertionMethod]
    private static void FalseAndNotRecorded<T1, T2>(T2? value) where T1 : struct
    {
        Recorder<T1> recorder = new(true);

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
            yield return (string.Empty, Target<StringComparison>(Adapters, null!));
        }
    }

    private sealed class Recorder<T> : ASyntacticArgumentRecorder where T : struct
    {
        public T? Value { get; private set; }
        public bool ValueRecorded { get; private set; }
        public Location? ValueLocation { get; private set; }

        private bool ReturnValue { get; }

        protected override IEqualityComparer<string> Comparer { get; } = StringComparerMock.CreateComparer(true);

        public Recorder(bool returnValue)
        {
            ReturnValue = returnValue;
        }

        protected override IEnumerable<(string, DSyntacticSingleRecorder)> AddSingleRecorders()
        {
            yield return (string.Empty, Target<T>(Adapters, RecordValue));
        }

        private bool RecordValue(T? value, Location location)
        {
            Value = value;
            ValueRecorded = true;
            ValueLocation = location;

            return ReturnValue;
        }
    }
}
