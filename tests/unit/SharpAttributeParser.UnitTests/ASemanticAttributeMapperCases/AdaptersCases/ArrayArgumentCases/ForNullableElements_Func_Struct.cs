﻿namespace SharpAttributeParser.ASemanticAttributeMapperCases.AdaptersCases.ArrayArgumentCases;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Xunit;

public sealed class ForNullableElements_Func_Struct
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
        var recorder = Mapper<int>.Target(Data<int?>.TrueRecorder);

        var exception = Record.Exception(() => recorder(null!, new[] { 3, 4 }));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void String_NotArrayType_FalseAndNotRecorded()
    {
        var value = "3";

        FalseAndNotRecorded<int>(value);
    }

    [Fact]
    public void Enum_SameType_TrueAndRecorded()
    {
        var value = new StringComparison?[] { StringComparison.CurrentCulture, null };

        TrueAndRecorded(value, value);
    }

    [Fact]
    public void Enum_NonNullSameType_TrueAndRecorded()
    {
        var value = new[] { StringComparison.CurrentCulture, StringComparison.InvariantCultureIgnoreCase };

        TrueAndRecorded(value.Select(static (value) => (StringComparison?)value), value);
    }

    [Fact]
    public void Enum_EnumsCastToObjects_TrueAndRecorded()
    {
        var value = new object?[] { StringComparison.CurrentCulture, null };

        TrueAndRecorded(value.Select(static (value) => (StringComparison?)value), value);
    }

    [Fact]
    public void Enum_DifferentEnumType_FalseAndNotRecorded()
    {
        var value = new StringSplitOptions?[] { StringSplitOptions.RemoveEmptyEntries, null };

        FalseAndNotRecorded<StringComparison>(value);
    }

    [Fact]
    public void Enum_NonNullDifferentEnumType_FalseAndNotRecorded()
    {
        var value = new[] { StringSplitOptions.RemoveEmptyEntries, StringSplitOptions.TrimEntries };

        FalseAndNotRecorded<StringComparison>(value);
    }

    [Fact]
    public void Enum_Int_FalseAndNotRecorded()
    {
        var value = new int?[] { 3, null };

        FalseAndNotRecorded<StringSplitOptions>(value);
    }

    [Fact]
    public void Int_SameType_TrueAndRecorded()
    {
        var value = new int?[] { 3, null };

        TrueAndRecorded(value, value);
    }

    [Fact]
    public void Int_NonNullable_TrueAndRecorded()
    {
        var value = new[] { 3, 4 };

        TrueAndRecorded(value.Select(static (value) => (int?)value), value);
    }

    [Fact]
    public void Int_Enum_FalseAndNotRecorded()
    {
        var value = new StringComparison?[] { StringComparison.CurrentCulture, null };

        FalseAndNotRecorded<int>(value);
    }

    [Fact]
    public void Int_Double_FalseAndNotRecorded()
    {
        var value = new double?[] { 3.14, null };

        FalseAndNotRecorded<int>(value);
    }

    [Fact]
    public void Int_String_FalseAndNotRecorded()
    {
        var value = new[] { "CurrentCulture", null };

        FalseAndNotRecorded<int>(value);
    }

    [Fact]
    public void Int_NullCollection_FalseAndNotRecorded()
    {
        IReadOnlyList<int>? value = null;

        FalseAndNotRecorded<int>(value);
    }

    [Fact]
    public void Double_Int_FalseAndNotRecorded()
    {
        var value = new int?[] { 3, null };

        FalseAndNotRecorded<double>(value);
    }

    [Fact]
    public void FalseReturningRecorder_FalseAndRecorded()
    {
        var recorder = Mapper<int>.Target(Data<int?>.FalseRecorder);

        var value = new int?[] { 3, 4 };

        Data<int?> data = new();

        var outcome = recorder(data, value);

        Assert.False(outcome);

        Assert.Equal(value, data.Value);
        Assert.True(data.ValueRecorded);
    }

    [AssertionMethod]
    private static void TrueAndRecorded<T>(IEnumerable<T?>? expected, object? value) where T : struct
    {
        var recorder = Mapper<T>.Target(Data<T?>.TrueRecorder);

        Data<T?> data = new();

        var outcome = recorder(data, value);

        Assert.True(outcome);

        Assert.Equal<IEnumerable<T?>>(expected, data.Value);
        Assert.True(data.ValueRecorded);
    }

    [AssertionMethod]
    private static void FalseAndNotRecorded<T>(object? value) where T : struct
    {
        var recorder = Mapper<T>.Target(Data<T?>.TrueRecorder);

        Data<T?> data = new();

        var outcome = recorder(data, value);

        Assert.False(outcome);

        Assert.False(data.ValueRecorded);
    }

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used to expose static member of ASemanticAttributeMapper.")]
    private sealed class Mapper<T> : ASemanticAttributeMapper<Data<T?>> where T : struct
    {
        public static Func<Data<T?>, object?, bool> Target(Func<Data<T?>, IReadOnlyList<T?>, bool> recorder) => Adapters.ArrayArgument.ForNullableElements(recorder).Invoke;
    }

    private sealed class Data<T>
    {
        public static Func<Data<T>, IReadOnlyList<T?>, bool> TrueRecorder => (dataRecord, argument) =>
        {
            Recorder(dataRecord, argument);

            return true;
        };

        public static Func<Data<T>, IReadOnlyList<T?>, bool> FalseRecorder => (dataRecord, argument) =>
        {
            Recorder(dataRecord, argument);

            return false;
        };

        private static void Recorder(Data<T> dataRecord, IReadOnlyList<T?>? argument)
        {
            dataRecord.Value = argument;
            dataRecord.ValueRecorded = true;
        }

        public IReadOnlyList<T?>? Value { get; set; }
        public bool ValueRecorded { get; set; }
    }
}