namespace SharpAttributeParser.ASemanticAttributeMapperCases.AdaptersCases.SimpleCases;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Xunit;

public sealed class For_Func
{
    [Fact]
    public void NullDelegate_ArgumentNullExceptionWhenUsed()
    {
        var exception = Record.Exception(() => Mapper<int>.Target(null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Enum_DifferentEnumType_FalseAndNotRecorded()
    {
        var value = StringSplitOptions.TrimEntries;

        FalseAndNotRecorded<StringComparison>(value);
    }

    [Fact]
    public void Enum_Int_FalseAndNotRecorded()
    {
        var value = 3;

        FalseAndNotRecorded<StringComparison>(value);
    }

    [Fact]
    public void Int_SameType_TrueAndRecorded()
    {
        var value = 3;

        TrueAndRecorded(value, value);
    }

    [Fact]
    public void Int_IntCastToObject_TrueAndRecorded()
    {
        object value = 3;

        TrueAndRecorded((int)value, value);
    }

    [Fact]
    public void Int_Enum_FalseAndNotRecorded()
    {
        var value = StringComparison.Ordinal;

        FalseAndNotRecorded<int>(value);
    }

    [Fact]
    public void Int_Double_FalseAndNotRecorded()
    {
        var value = 3.14;

        FalseAndNotRecorded<int>(value);
    }

    [Fact]
    public void Int_String_FalseAndNotRecorded()
    {
        var value = "3";

        FalseAndNotRecorded<int>(value);
    }

    [Fact]
    public void Int_NullableIntWithValue_TrueAndRecorded()
    {
        int? value = 3;

        TrueAndRecorded(value.Value, value);
    }

    [Fact]
    public void Int_Null_FalseAndNotRecorded()
    {
        int? value = null;

        FalseAndNotRecorded<int>(value);
    }

    [Fact]
    public void Double_Int_FalseAndNotRecorded()
    {
        var value = 3;

        FalseAndNotRecorded<double>(value);
    }

    [Fact]
    public void NullableIntArray_NullElement_TrueAndRecorded()
    {
        var value = new int?[] { 3, null };

        TrueAndRecorded(value, value);
    }

    [Fact]
    public void NullableIntArray_NullCollection_FalseAndNotRecorded()
    {
        IReadOnlyList<int?>? value = null;

        FalseAndNotRecorded<IReadOnlyList<int?>>(value);
    }

    [Fact]
    public void String_SameType_TrueAndRecorded()
    {
        var value = "3";

        TrueAndRecorded(value, value);
    }

    [Fact]
    public void String_Enum_FalseAndNotRecorded()
    {
        var value = StringComparison.OrdinalIgnoreCase;

        FalseAndNotRecorded<string>(value);
    }

    [Fact]
    public void String_Null_FalseAndNotRecorded()
    {
        string? value = null;

        FalseAndNotRecorded<string>(value);
    }

    [Fact]
    public void FalseReturningRecorder_FalseAndRecorded()
    {
        var recorder = Mapper<int>.Target(Data<int>.FalseRecorder);

        var value = 3;

        var data = new Data<int>();

        var outcome = recorder(data, value);

        Assert.False(outcome);

        Assert.Equal(value, data.Value);
    }

    [AssertionMethod]
    private static void TrueAndRecorded<T1>(T1 expected, object? value) where T1 : notnull
    {
        var recorder = Mapper<T1>.Target(Data<T1>.TrueRecorder);

        var data = new Data<T1>();

        var outcome = recorder(data, value);

        Assert.True(outcome);

        Assert.Equal(expected, data.Value);
    }

    [AssertionMethod]
    private static void FalseAndNotRecorded<T1>(object? value) where T1 : notnull
    {
        var recorder = Mapper<T1>.Target(Data<T1>.TrueRecorder);

        var data = new Data<T1>();

        var outcome = recorder(data, value);

        Assert.False(outcome);

        Assert.False(data.ValueRecorded);
    }

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used to expose static member of ASemanticAttributeMapper.")]
    private sealed class Mapper<T> : ASemanticAttributeMapper<Data<T>> where T : notnull
    {
        public static Func<Data<T>, object?, bool> Target(Func<Data<T>, T, bool> recorder) => Adapters.Simple.For(recorder).Invoke;
    }

    private sealed class Data<T>
    {
        public static Func<Data<T>, T, bool> TrueRecorder => (dataRecord, argument) =>
        {
            Recorder(dataRecord, argument);

            return true;
        };

        public static Func<Data<T>, T, bool> FalseRecorder => (dataRecord, argument) =>
        {
            Recorder(dataRecord, argument);

            return false;
        };

        private static void Recorder(Data<T> dataRecord, T argument)
        {
            dataRecord.Value = argument;
            dataRecord.ValueRecorded = true;
        }

        public T? Value { get; set; }
        public bool ValueRecorded { get; set; }
    }
}
