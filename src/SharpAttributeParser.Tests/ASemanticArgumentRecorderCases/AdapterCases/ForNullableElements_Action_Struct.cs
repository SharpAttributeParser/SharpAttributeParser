﻿namespace SharpAttributeParser.Tests.ASemanticArgumentRecorderCases.AdapterCases;

using System;
using System.Collections.Generic;

using Xunit;

public class ForNullableElements_Action_Struct
{
    private static bool TryRecordConstructorArgument(ASemanticArgumentRecorder recorder, string parameterName, IReadOnlyList<object?>? value) => recorder.TryRecordConstructorArgument(parameterName, value);

    [Fact]
    public void Enum_SameType_True_RecorderPopulated()
    {
        StringComparisonRecorder recorder = new();

        var parameterName = "Value";
        IReadOnlyList<object> value = new object[] { StringComparison.Ordinal, StringComparison.OrdinalIgnoreCase };

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.True(actual);

        Assert.Equal(new StringComparison?[] { StringComparison.Ordinal, StringComparison.OrdinalIgnoreCase }, recorder.Value);
    }

    [Fact]
    public void Enum_NullableSameType_True_RecorderPopulated()
    {
        StringComparisonRecorder recorder = new();

        var parameterName = "Value";
        IReadOnlyList<object> value = new object[] { (StringComparison?)StringComparison.Ordinal, StringComparison.OrdinalIgnoreCase };

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.True(actual);

        Assert.Equal(new StringComparison?[] { StringComparison.Ordinal, StringComparison.OrdinalIgnoreCase }, recorder.Value);
    }

    [Fact]
    public void Enum_NullEnum_True_RecorderPopulated()
    {
        StringComparisonRecorder recorder = new();

        var parameterName = "Value";
        IReadOnlyList<object?> value = new object?[] { null, StringComparison.OrdinalIgnoreCase };

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.True(actual);

        Assert.Equal(new StringComparison?[] { null, StringComparison.OrdinalIgnoreCase }, recorder.Value);
    }

    [Fact]
    public void Enum_Int_True_RecorderPopulated()
    {
        StringComparisonRecorder recorder = new();

        var parameterName = "Value";
        IReadOnlyList<object> value = new object[] { StringComparison.Ordinal, 5 };

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.True(actual);

        Assert.Equal(new StringComparison?[] { StringComparison.Ordinal, (StringComparison)5 }, recorder.Value);
    }

    [Fact]
    public void Enum_Double_False_RecorderNotPopulated()
    {
        StringComparisonRecorder recorder = new();

        var parameterName = "Value";
        IReadOnlyList<object> value = new object[] { StringComparison.Ordinal, 4.2 };

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.False(actual);

        Assert.False(recorder.ValueRecorded);
    }

    [Fact]
    public void Enum_AnotherEnum_False_RecorderNotPopulated()
    {
        StringComparisonRecorder recorder = new();

        var parameterName = "Value";
        IReadOnlyList<object> value = new object[] { StringComparison.Ordinal, StringSplitOptions.TrimEntries };

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.False(actual);

        Assert.False(recorder.ValueRecorded);
    }

    [Fact]
    public void Enum_EnumString_False_RecorderNotPopulated()
    {
        StringComparisonRecorder recorder = new();

        var parameterName = "Value";
        IReadOnlyList<object> value = new object[] { StringComparison.Ordinal, "OrdinalIgnoreCase" };

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.False(actual);

        Assert.False(recorder.ValueRecorded);
    }

    [Fact]
    public void Enum_IntString_False_RecorderNotPopulated()
    {
        StringComparisonRecorder recorder = new();

        var parameterName = "Value";
        IReadOnlyList<object> value = new object[] { StringComparison.Ordinal, "5" };

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.False(actual);

        Assert.False(recorder.ValueRecorded);
    }

    [Fact]
    public void Enum_Null_False_RecorderNotPopulated()
    {
        StringComparisonRecorder recorder = new();

        var parameterName = "Value";
        IReadOnlyList<object>? value = null;

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.False(actual);

        Assert.False(recorder.ValueRecorded);
    }

    [Fact]
    public void Int_Int_True_RecorderPopulated()
    {
        IntRecorder recorder = new();

        var parameterName = "Value";
        IReadOnlyList<object> value = new object[] { 1, 2 };

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.True(actual);

        Assert.Equal(new int?[] { 1, 2 }, recorder.Value);
    }

    [Fact]
    public void Int_NullableInt_True_RecorderPopulated()
    {
        IntRecorder recorder = new();

        var parameterName = "Value";
        IReadOnlyList<object> value = new object[] { (int?)1, 2 };

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.True(actual);

        Assert.Equal(new int?[] { 1, 2 }, recorder.Value);
    }

    [Fact]
    public void Int_NullInt_True_RecorderPopulated()
    {
        IntRecorder recorder = new();

        var parameterName = "Value";
        IReadOnlyList<object?> value = new object?[] { null, 2 };

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.True(actual);

        Assert.Equal(new int?[] { null, 2 }, recorder.Value);
    }

    [Fact]
    public void Int_Enum_True_RecorderPopulated()
    {
        IntRecorder recorder = new();

        var parameterName = "Value";
        IReadOnlyList<object> value = new object[] { StringComparison.OrdinalIgnoreCase, 2 };

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.True(actual);

        Assert.Equal(new int?[] { (int)StringComparison.OrdinalIgnoreCase, 2 }, recorder.Value);
    }

    [Fact]
    public void Int_Double_False_RecorderNotPopulated()
    {
        IntRecorder recorder = new();

        var parameterName = "Value";
        IReadOnlyList<object> value = new object[] { 1, 4.2 };

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.False(actual);

        Assert.False(recorder.ValueRecorded);
    }

    [Fact]
    public void Int_IntString_False_RecordedNotPopulated()
    {
        IntRecorder recorder = new();

        var parameterName = "Value";
        IReadOnlyList<object> value = new object[] { "1", 2 };

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.False(actual);

        Assert.False(recorder.ValueRecorded);
    }

    [Fact]
    public void Int_Null_False_RecorderNotPopulated()
    {
        IntRecorder recorder = new();

        var parameterName = "Value";
        IReadOnlyList<object>? value = null;

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.False(actual);

        Assert.False(recorder.ValueRecorded);
    }

    private sealed class StringComparisonRecorder : ASemanticArgumentRecorder
    {
        public IReadOnlyList<StringComparison?>? Value { get; private set; }
        public bool ValueRecorded { get; private set; }

        protected override IEnumerable<(string, DArrayRecorder)> AddArrayRecorders()
        {
            yield return ("Value", Adapters.ForNullableElements<StringComparison>(RecordStringComparisonArray).Invoke);
        }

        private void RecordStringComparisonArray(IReadOnlyList<StringComparison?> value)
        {
            Value = value;
            ValueRecorded = true;
        }
    }

    private sealed class IntRecorder : ASemanticArgumentRecorder
    {
        public IReadOnlyList<int?>? Value { get; private set; }
        public bool ValueRecorded { get; private set; }

        protected override IEnumerable<(string, DArrayRecorder)> AddArrayRecorders()
        {
            yield return ("Value", Adapters.ForNullableElements<int>(RecordIntArray).Invoke);
        }

        private void RecordIntArray(IReadOnlyList<int?> value)
        {
            Value = value;
            ValueRecorded = true;
        }
    }
}
