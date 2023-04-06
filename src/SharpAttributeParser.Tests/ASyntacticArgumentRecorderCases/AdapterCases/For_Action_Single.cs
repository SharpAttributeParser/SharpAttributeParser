namespace SharpAttributeParser.Tests.ASyntacticArgumentRecorderCases.AdapterCases;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;

using Xunit;

public class For_Action_Single
{
    private static bool TryRecordConstructorArgument(ASyntacticArgumentRecorder recorder, string parameterName, object? value) => recorder.TryRecordConstructorArgument(parameterName, value, Location.None);

    [Fact]
    public void Enum_SameType_True_RecorderPopulated()
    {
        EnumRecorder recorder = new();

        var parameterName = "Value";
        var value = StringComparison.OrdinalIgnoreCase;

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.True(actual);

        Assert.Equal(value, recorder.Value);
    }

    [Fact]
    public void Enum_NullableSameType_True_RecorderPopulated()
    {
        EnumRecorder recorder = new();

        var parameterName = "Value";
        StringComparison? value = StringComparison.OrdinalIgnoreCase;

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.True(actual);

        Assert.Equal(StringComparison.OrdinalIgnoreCase, recorder.Value);
    }

    [Fact]
    public void Enum_Int_True_RecorderPopulated()
    {
        EnumRecorder recorder = new();

        var parameterName = "Value";
        var value = 5;

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.True(actual);

        Assert.Equal((StringComparison)value, recorder.Value);
    }

    [Fact]
    public void Enum_Double_False_RecorderNotPopulated()
    {
        EnumRecorder recorder = new();

        var parameterName = "Value";
        var value = 4.2;

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.False(actual);

        Assert.False(recorder.ValueRecorded);
    }

    [Fact]
    public void Enum_AnotherEnum_True_RecorderPopulated()
    {
        EnumRecorder recorder = new();

        var parameterName = "Value";
        var value = StringSplitOptions.TrimEntries;

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.True(actual);

        Assert.Equal((StringComparison)StringSplitOptions.TrimEntries, recorder.Value);
    }

    [Fact]
    public void Enum_IntString_False_RecordedNotPopulated()
    {
        EnumRecorder recorder = new();

        var parameterName = "Value";
        var value = "5";

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.False(actual);

        Assert.False(recorder.ValueRecorded);
    }

    [Fact]
    public void Enum_EnumString_False_RecordedNotPopulated()
    {
        EnumRecorder recorder = new();

        var parameterName = "Value";
        var value = "OrdinalIgnoreCase";

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.False(actual);

        Assert.False(recorder.ValueRecorded);
    }

    [Fact]
    public void Enum_Null_False_RecorderNotPopulated()
    {
        EnumRecorder recorder = new();

        var parameterName = "Value";
        object? value = null;

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.False(actual);

        Assert.False(recorder.ValueRecorded);
    }

    [Fact]
    public void Int_Int_True_RecorderPopulated()
    {
        IntRecorder recorder = new();

        var parameterName = "Value";
        var value = 3;

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.True(actual);

        Assert.Equal(value, recorder.Value);
    }

    [Fact]
    public void Int_NullableInt_True_RecorderPopulated()
    {
        IntRecorder recorder = new();

        var parameterName = "Value";
        int? value = 3;

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.True(actual);

        Assert.Equal(value, recorder.Value);
    }

    [Fact]
    public void Int_Enum_True_RecorderPopulated()
    {
        IntRecorder recorder = new();

        var parameterName = "Value";
        var value = StringComparison.OrdinalIgnoreCase;

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.True(actual);

        Assert.Equal((int)value, recorder.Value);
    }

    [Fact]
    public void Int_Double_False_RecorderNotPopulated()
    {
        IntRecorder recorder = new();

        var parameterName = "Value";
        var value = 4.2;

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.False(actual);

        Assert.False(recorder.ValueRecorded);
    }

    [Fact]
    public void Int_IntString_False_RecordedNotPopulated()
    {
        IntRecorder recorder = new();

        var parameterName = "Value";
        var value = "5";

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.False(actual);

        Assert.False(recorder.ValueRecorded);
    }

    [Fact]
    public void Int_Null_False_RecorderNotPopulated()
    {
        IntRecorder recorder = new();

        var parameterName = "Value";
        object? value = null;

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.False(actual);

        Assert.False(recorder.ValueRecorded);
    }

    [Fact]
    public void IntArray_IntArray_True_RecorderPopulated()
    {
        ArrayRecorder recorder = new();

        var parameterName = "Value";
        var value = new[] { 1, 2, 3 };

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.True(actual);

        Assert.Equal(value, recorder.Value);
    }

    [Fact]
    public void IntArray_NullableIntArray_False_RecorderNotPopulated()
    {
        ArrayRecorder recorder = new();

        var parameterName = "Value";
        var value = new int?[] { 1, 2, 3 };

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.False(actual);

        Assert.False(recorder.ValueRecorded);
    }

    [Fact]
    public void IntArray_Null_False_RecorderNotPopulated()
    {
        ArrayRecorder recorder = new();

        var parameterName = "Value";
        object? value = null;

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.False(actual);

        Assert.False(recorder.ValueRecorded);
    }

    [Fact]
    public void String_String_True_RecorderPopulated()
    {
        StringRecorder recorder = new();

        var parameterName = "Value";
        var value = "1";

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.True(actual);

        Assert.Equal(value, recorder.Value);
    }

    [Fact]
    public void String_Enum_False_RecorderNotPopulated()
    {
        StringRecorder recorder = new();

        var parameterName = "Value";
        var value = StringComparison.OrdinalIgnoreCase;

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.False(actual);

        Assert.False(recorder.ValueRecorded);
    }

    [Fact]
    public void String_Null_False_RecorderNotPopulated()
    {
        StringRecorder recorder = new();

        var parameterName = "Value";
        object? value = null;

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.False(actual);

        Assert.False(recorder.ValueRecorded);
    }

    private sealed class EnumRecorder : ASyntacticArgumentRecorder
    {
        public StringComparison? Value { get; private set; }
        public bool ValueRecorded { get; private set; }

        protected override IEnumerable<(string, DSingleRecorder)> AddSingleRecorders()
        {
            yield return ("Value", Adapters.For<StringComparison>(RecordStringComparison).Invoke);
        }

        private void RecordStringComparison(StringComparison value, Location location)
        {
            Value = value;
            ValueRecorded = true;
        }
    }

    private sealed class IntRecorder : ASyntacticArgumentRecorder
    {
        public int? Value { get; private set; }
        public bool ValueRecorded { get; private set; }

        protected override IEnumerable<(string, DSingleRecorder)> AddSingleRecorders()
        {
            yield return ("Value", Adapters.For<int>(RecordInt).Invoke);
        }

        private void RecordInt(int value, Location location)
        {
            Value = value;
            ValueRecorded = true;
        }
    }

    private sealed class ArrayRecorder : ASyntacticArgumentRecorder
    {
        public IReadOnlyList<int>? Value { get; private set; }
        public bool ValueRecorded { get; private set; }

        protected override IEnumerable<(string, DSingleRecorder)> AddSingleRecorders()
        {
            yield return ("Value", Adapters.For<IReadOnlyList<int>>(RecordIntArray).Invoke);
        }

        private void RecordIntArray(IReadOnlyList<int> value, Location location)
        {
            Value = value;
            ValueRecorded = true;
        }
    }

    private sealed class StringRecorder : ASyntacticArgumentRecorder
    {
        public string? Value { get; private set; }
        public bool ValueRecorded { get; private set; }

        protected override IEnumerable<(string, DSingleRecorder)> AddSingleRecorders()
        {
            yield return ("Value", Adapters.For<string>(RecordString).Invoke);
        }

        private void RecordString(string value, Location location)
        {
            Value = value;
            ValueRecorded = true;
        }
    }
}
