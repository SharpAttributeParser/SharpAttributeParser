﻿namespace SharpAttributeParser.ASemanticAttributeMapperCases.AdaptersCases.SimpleCases;

using System;
using System.Diagnostics.CodeAnalysis;

using Xunit;

public sealed class ForNullable_Action_Struct
{
    [Fact]
    public void NullDelegate_ArgumentNullExceptionWhenUsed()
    {
        var exception = Record.Exception(() => Mapper<int>.Target(null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidRecorder_NullDataRecord_ArgumentNullExceptionWhenUsed()
    {
        var recorder = Mapper<int>.Target(Data<int?>.Recorder);

        var exception = Record.Exception(() => recorder(null!, 3));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Enum_SameType_TrueAndRecorded()
    {
        var value = StringComparison.OrdinalIgnoreCase;

        TrueAndRecorded<StringComparison>(value, value);
    }

    [Fact]
    public void Enum_DifferentEnumType_FalseAndNotRecorded()
    {
        var value = StringSplitOptions.TrimEntries;

        FalseAndNotRecorded<StringComparison>(value);
    }

    [Fact]
    public void Enum_Null_TrueAndRecorded()
    {
        StringComparison? value = null;

        TrueAndRecorded(value, value);
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

        TrueAndRecorded<int>(value, value);
    }

    [Fact]
    public void Int_IntCastToObject_TrueAndRecorded()
    {
        object value = 3;

        TrueAndRecorded<int>((int)value, value);
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
    public void NullableInt_Null_TrueAndRecorded()
    {
        int? value = null;

        TrueAndRecorded(value, value);
    }

    [AssertionMethod]
    private static void TrueAndRecorded<T1>(T1? expected, object? value) where T1 : struct
    {
        var recorder = Mapper<T1>.Target(Data<T1?>.Recorder);

        var data = new Data<T1?>();

        var outcome = recorder(data, value);

        Assert.True(outcome);

        Assert.Equal(expected, data.Value);
        Assert.True(data.ValueRecorded);
    }

    [AssertionMethod]
    private static void FalseAndNotRecorded<T1>(object? value) where T1 : struct
    {
        var recorder = Mapper<T1>.Target(Data<T1?>.Recorder);

        var data = new Data<T1?>();

        var outcome = recorder(data, value);

        Assert.False(outcome);

        Assert.False(data.ValueRecorded);
    }

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used to expose static member of ASemanticAttributeMapper.")]
    private sealed class Mapper<T> : ASemanticAttributeMapper<Data<T?>> where T : struct
    {
        public static Func<Data<T?>, object?, bool> Target(Action<Data<T?>, T?> recorder) => Adapters.Simple.ForNullable(recorder).Invoke;
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
