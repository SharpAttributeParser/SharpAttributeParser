namespace SharpAttributeParser.Tests.ASemanticArgumentRecorderCases.AdapterCases;

using System;
using System.Collections.Generic;

using Xunit;

public class ForNullable_Action_Single_Class
{
    private static bool TryRecordConstructorArgument(ASemanticArgumentRecorder recorder, string parameterName, object? value) => recorder.TryRecordConstructorArgument(parameterName, value);

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
    public void IntArray_Null_True_RecorderPopulated()
    {
        ArrayRecorder recorder = new();

        var parameterName = "Value";
        object? value = null;

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.True(actual);

        Assert.Null(recorder.Value);
        Assert.True(recorder.ValueRecorded);
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
    public void String_Null_True_RecorderPopulated()
    {
        StringRecorder recorder = new();

        var parameterName = "Value";
        object? value = null;

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.True(actual);

        Assert.Null(recorder.Value);
        Assert.True(recorder.ValueRecorded);
    }
    private sealed class ArrayRecorder : ASemanticArgumentRecorder
    {
        public IReadOnlyList<int>? Value { get; private set; }
        public bool ValueRecorded { get; private set; }

        protected override IEnumerable<(string, DSemanticSingleRecorder)> AddSingleRecorders()
        {
            yield return ("Value", Adapters.ForNullable<IReadOnlyList<int>>(RecordIntArray));
        }

        private void RecordIntArray(IReadOnlyList<int>? value)
        {
            Value = value;
            ValueRecorded = true;
        }
    }

    private sealed class StringRecorder : ASemanticArgumentRecorder
    {
        public string? Value { get; private set; }
        public bool ValueRecorded { get; private set; }

        protected override IEnumerable<(string, DSemanticSingleRecorder)> AddSingleRecorders()
        {
            yield return ("Value", Adapters.ForNullable<string>(RecordString));
        }

        private void RecordString(string? value)
        {
            Value = value;
            ValueRecorded = true;
        }
    }
}
