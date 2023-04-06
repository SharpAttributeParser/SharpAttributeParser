namespace SharpAttributeParser.Tests.ASyntacticArgumentRecorderCases.AdapterCases;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;

using Xunit;

public class ForNullable_Func_Single_Struct
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

        Assert.Equal(value.Value, recorder.Value);
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
    public void Enum_AnotherEnum_False_RecorderNotPopulated()
    {
        EnumRecorder recorder = new();

        var parameterName = "Value";
        var value = StringSplitOptions.TrimEntries;

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.False(actual);

        Assert.False(recorder.ValueRecorded);
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
    public void Enum_Null_True_RecorderPopulated()
    {
        EnumRecorder recorder = new();

        var parameterName = "Value";
        object? value = null;

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.True(actual);

        Assert.Null(recorder.Value);
        Assert.True(recorder.ValueRecorded);
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
    public void Int_Null_True_RecorderPopulated()
    {
        IntRecorder recorder = new();

        var parameterName = "Value";
        object? value = null;

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.True(actual);

        Assert.Null(recorder.Value);
        Assert.True(recorder.ValueRecorded);
    }

    [Fact]
    public void FalseRecorder_False_RecorderPopulated()
    {
        FalseRecorder recorder = new();

        var parameterName = "Value";
        var value = 5;

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.False(actual);

        Assert.Equal(value, recorder.Value);
    }

    private sealed class EnumRecorder : ASyntacticArgumentRecorder
    {
        public StringComparison? Value { get; private set; }
        public bool ValueRecorded { get; private set; }

        protected override IEnumerable<(string, DSyntacticSingleRecorder)> AddSingleRecorders()
        {
            yield return ("Value", Adapters.ForNullable<StringComparison>(RecordStringComparison));
        }

        private bool RecordStringComparison(StringComparison? value, Location location)
        {
            Value = value;
            ValueRecorded = true;

            return true;
        }
    }

    private sealed class IntRecorder : ASyntacticArgumentRecorder
    {
        public int? Value { get; private set; }
        public bool ValueRecorded { get; private set; }

        protected override IEnumerable<(string, DSyntacticSingleRecorder)> AddSingleRecorders()
        {
            yield return ("Value", Adapters.ForNullable<int>(RecordInt));
        }

        private bool RecordInt(int? value, Location location)
        {
            Value = value;
            ValueRecorded = true;

            return true;
        }
    }

    private sealed class FalseRecorder : ASyntacticArgumentRecorder
    {
        public int? Value { get; private set; }
        public bool ValueRecorded { get; private set; }

        protected override IEnumerable<(string, DSyntacticSingleRecorder)> AddSingleRecorders()
        {
            yield return ("Value", Adapters.ForNullable<int>(RecordInt));
        }

        private bool RecordInt(int? value, Location location)
        {
            Value = value;
            ValueRecorded = true;

            return false;
        }
    }
}
