namespace SharpAttributeParser.Tests.ASemanticArgumentRecorderCases.AdapterCases;

using System;
using System.Collections.Generic;

using Xunit;

public class ForNullable_Action_Single_Struct
{
    private static bool TryRecordConstructorArgument(ASemanticArgumentRecorder recorder, string parameterName, object? value) => recorder.TryRecordConstructorArgument(parameterName, value);

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

    private sealed class EnumRecorder : ASemanticArgumentRecorder
    {
        public StringComparison? Value { get; private set; }
        public bool ValueRecorded { get; private set; }

        protected override IEnumerable<(string, DSemanticSingleRecorder)> AddSingleRecorders()
        {
            yield return ("Value", Adapters.ForNullable<StringComparison>(RecordStringComparison));
        }

        private void RecordStringComparison(StringComparison? value)
        {
            Value = value;
            ValueRecorded = true;
        }
    }

    private sealed class IntRecorder : ASemanticArgumentRecorder
    {
        public int? Value { get; private set; }
        public bool ValueRecorded { get; private set; }

        protected override IEnumerable<(string, DSemanticSingleRecorder)> AddSingleRecorders()
        {
            yield return ("Value", Adapters.ForNullable<int>(RecordInt));
        }

        private void RecordInt(int? value)
        {
            Value = value;
            ValueRecorded = true;
        }
    }
}
