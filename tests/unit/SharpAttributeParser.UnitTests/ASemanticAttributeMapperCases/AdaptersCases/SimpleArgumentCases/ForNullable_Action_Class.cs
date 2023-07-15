namespace SharpAttributeParser.ASemanticAttributeMapperCases.AdaptersCases.SimpleArgumentCases;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Xunit;

public sealed class ForNullable_Action_Class
{
    [Fact]
    public void NullDelegate_ArgumentNullExceptionWhenUsed()
    {
        var exception = Record.Exception(() => Mapper<string>.Target(null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidRecorder_NullDataRecord_ArgumentNullExceptionWhenUsed()
    {
        var recorder = Mapper<string>.Target(Data<string>.Recorder);

        var exception = Record.Exception(() => recorder(null!, "3"));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullableIntArray_NullElement_TrueAndRecorded()
    {
        var value = new int?[] { 3, null };

        TrueAndRecorded(value, value);
    }

    [Fact]
    public void IntArray_NullCollection_TrueAndRecorded()
    {
        IReadOnlyList<int?>? value = null;

        TrueAndRecorded(value, value);
    }

    [Fact]
    public void String_SameType_TrueAndRecorded()
    {
        var value = "3";

        TrueAndRecorded(value, value);
    }

    [Fact]
    public void String_StringCastToObject_TrueAndRecorded()
    {
        object value = "3";

        TrueAndRecorded((string)value, value);
    }

    [Fact]
    public void String_Enum_FalseAndNotRecorded()
    {
        var value = StringComparison.OrdinalIgnoreCase;

        FalseAndNotRecorded<string>(value);
    }

    [Fact]
    public void String_Null_TrueAndRecorded()
    {
        string? value = null;

        TrueAndRecorded(value, value);
    }

    [AssertionMethod]
    private static void TrueAndRecorded<T>(T? expected, object? value) where T : class
    {
        var recorder = Mapper<T>.Target(Data<T>.Recorder);

        Data<T> data = new();

        var outcome = recorder(data, value);

        Assert.True(outcome);

        Assert.Equal(expected, data.Value);
        Assert.True(data.ValueRecorded);
    }

    [AssertionMethod]
    private static void FalseAndNotRecorded<T>(object? value) where T : class
    {
        var recorder = Mapper<T>.Target(Data<T>.Recorder);

        Data<T> data = new();

        var outcome = recorder(data, value);

        Assert.False(outcome);

        Assert.False(data.ValueRecorded);
    }

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used to expose static member of ASemanticAttributeMapper.")]
    private sealed class Mapper<T> : ASemanticAttributeMapper<Data<T>> where T : class
    {
        public static Func<Data<T>, object?, bool> Target(Action<Data<T>, T?> recorder) => Adapters.SimpleArgument.ForNullable(recorder).Invoke;
    }

    private sealed class Data<T>
    {
        public static Action<Data<T>, T?> Recorder => (dataRecord, argument) =>
        {
            dataRecord.Value = argument;
            dataRecord.ValueRecorded = true;
        };

        public T? Value { get; set; }
        public bool ValueRecorded { get; set; }
    }
}
