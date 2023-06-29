namespace SharpAttributeParser.Tests.ASyntacticArgumentRecorderCases.AdapterCases;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;

using Xunit;

public class ForNullableCollection_Func
{
    private static bool TryRecordConstructorArgument(ASyntacticArgumentRecorder recorder, string parameterName, IReadOnlyList<object?>? value) => recorder.TryRecordNamedArgument(parameterName, value, Location.None, Array.Empty<Location>());

    [Fact]
    public void Enum_SameType_True_RecorderPopulated()
    {
        StringComparisonRecorder recorder = new();

        var parameterName = "Value";
        IReadOnlyList<object> value = new object[] { StringComparison.Ordinal, StringComparison.OrdinalIgnoreCase };

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.True(actual);

        Assert.Equal(new[] { StringComparison.Ordinal, StringComparison.OrdinalIgnoreCase }, recorder.Value);
    }

    [Fact]
    public void Enum_NullableSameType_True_RecorderPopulated()
    {
        StringComparisonRecorder recorder = new();

        var parameterName = "Value";
        IReadOnlyList<object> value = new object[] { (StringComparison?)StringComparison.Ordinal, StringComparison.OrdinalIgnoreCase };

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.True(actual);

        Assert.Equal(new[] { StringComparison.Ordinal, StringComparison.OrdinalIgnoreCase }, recorder.Value);
    }

    [Fact]
    public void Enum_NullEnum_False_RecorderNotPopulated()
    {
        StringComparisonRecorder recorder = new();

        var parameterName = "Value";
        IReadOnlyList<object?> value = new object?[] { null, StringComparison.OrdinalIgnoreCase };

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.False(actual);

        Assert.False(recorder.ValueRecorded);
    }

    [Fact]
    public void Enum_Int_True_RecorderPopulated()
    {
        StringComparisonRecorder recorder = new();

        var parameterName = "Value";
        IReadOnlyList<object> value = new object[] { StringComparison.Ordinal, 5 };

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.True(actual);

        Assert.Equal(new[] { StringComparison.Ordinal, (StringComparison)5 }, recorder.Value);
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
    public void Enum_AnotherEnum_True_RecorderPopulated()
    {
        StringComparisonRecorder recorder = new();

        var parameterName = "Value";
        IReadOnlyList<object> value = new object[] { StringComparison.Ordinal, StringSplitOptions.TrimEntries };

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.True(actual);

        Assert.Equal(new[] { StringComparison.Ordinal, (StringComparison)StringSplitOptions.TrimEntries }, recorder.Value);
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
    public void Enum_Null_True_RecorderPopulated()
    {
        StringComparisonRecorder recorder = new();

        var parameterName = "Value";
        IReadOnlyList<object>? value = null;

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
        IReadOnlyList<object> value = new object[] { 1, 2 };

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.True(actual);

        Assert.Equal(new[] { 1, 2 }, recorder.Value);
    }

    [Fact]
    public void Int_NullableInt_True_RecorderPopulated()
    {
        IntRecorder recorder = new();

        var parameterName = "Value";
        IReadOnlyList<object> value = new object[] { (int?)1, 2 };

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.True(actual);

        Assert.Equal(new[] { 1, 2 }, recorder.Value);
    }

    [Fact]
    public void Int_NullInt_False_RecorderNotPopulated()
    {
        IntRecorder recorder = new();

        var parameterName = "Value";
        IReadOnlyList<object?> value = new object?[] { null, 2 };

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.False(actual);

        Assert.False(recorder.ValueRecorded);
    }

    [Fact]
    public void Int_Enum_True_RecorderPopulated()
    {
        IntRecorder recorder = new();

        var parameterName = "Value";
        IReadOnlyList<object> value = new object[] { StringComparison.OrdinalIgnoreCase, 2 };

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.True(actual);

        Assert.Equal(new[] { (int)StringComparison.OrdinalIgnoreCase, 2 }, recorder.Value);
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
    public void Int_Null_True_RecorderPopulated()
    {
        IntRecorder recorder = new();

        var parameterName = "Value";
        IReadOnlyList<object>? value = null;

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
        IReadOnlyList<object?> value = new object?[] { "1", "2" };

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.True(actual);

        Assert.Equal(new[] { "1", "2" }, recorder.Value);
    }

    [Fact]
    public void String_Enum_False_RecorderNotPopulated()
    {
        StringRecorder recorder = new();

        var parameterName = "Value";
        IReadOnlyList<object?> value = new object?[] { StringComparison.OrdinalIgnoreCase, "2" };

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.False(actual);

        Assert.False(recorder.ValueRecorded);
    }

    [Fact]
    public void String_NullString_False_RecorderNotPopulated()
    {
        StringRecorder recorder = new();

        var parameterName = "Value";
        IReadOnlyList<object?> value = new object?[] { null, "2" };

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.False(actual);

        Assert.False(recorder.ValueRecorded);
    }

    [Fact]
    public void String_Null_True_RecorderPopulated()
    {
        StringRecorder recorder = new();

        var parameterName = "Value";
        IReadOnlyList<object>? value = null;

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
        IReadOnlyList<object>? value = new object[] { 1, 2 };

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.False(actual);

        Assert.Equal(new[] { 1, 2 }, recorder.Value);
    }

    private sealed class StringComparisonRecorder : ASyntacticArgumentRecorder
    {
        public IReadOnlyList<StringComparison>? Value { get; private set; }
        public bool ValueRecorded { get; private set; }

        protected override IEnumerable<(string, DSyntacticArrayRecorder)> AddArrayRecorders()
        {
            yield return ("Value", Adapters.ForNullableCollection<StringComparison>(RecordStringComparisonArray));
        }

        private bool RecordStringComparisonArray(IReadOnlyList<StringComparison>? value, Location collectionLocation, IReadOnlyList<Location> elementLocations)
        {
            Value = value;
            ValueRecorded = true;

            return true;
        }
    }

    private sealed class IntRecorder : ASyntacticArgumentRecorder
    {
        public IReadOnlyList<int>? Value { get; private set; }
        public bool ValueRecorded { get; private set; }

        protected override IEnumerable<(string, DSyntacticArrayRecorder)> AddArrayRecorders()
        {
            yield return ("Value", Adapters.ForNullableCollection<int>(RecordIntArray));
        }

        private bool RecordIntArray(IReadOnlyList<int>? value, Location collectionLocation, IReadOnlyList<Location> elementLocations)
        {
            Value = value;
            ValueRecorded = true;

            return true;
        }
    }

    private sealed class StringRecorder : ASyntacticArgumentRecorder
    {
        public IReadOnlyList<string>? Value { get; private set; }
        public bool ValueRecorded { get; private set; }

        protected override IEnumerable<(string, DSyntacticArrayRecorder)> AddArrayRecorders()
        {
            yield return ("Value", Adapters.ForNullableCollection<string>(RecordStringArray));
        }

        private bool RecordStringArray(IReadOnlyList<string>? value, Location collectionLocation, IReadOnlyList<Location> elementLocations)
        {
            Value = value;
            ValueRecorded = true;

            return true;
        }
    }

    private sealed class FalseRecorder : ASyntacticArgumentRecorder
    {
        public IReadOnlyList<int>? Value { get; private set; }
        public bool ValueRecorded { get; private set; }

        protected override IEnumerable<(string, DSyntacticArrayRecorder)> AddArrayRecorders()
        {
            yield return ("Value", Adapters.ForNullableCollection<int>(RecordIntArray));
        }

        private bool RecordIntArray(IReadOnlyList<int>? value, Location collectionLocation, IReadOnlyList<Location> elementLocations)
        {
            Value = value;
            ValueRecorded = true;

            return false;
        }
    }
}
