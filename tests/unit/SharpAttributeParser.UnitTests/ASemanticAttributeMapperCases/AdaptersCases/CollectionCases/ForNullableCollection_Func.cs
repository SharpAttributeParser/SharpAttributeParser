namespace SharpAttributeParser.ASemanticAttributeMapperCases.AdaptersCases.CollectionCases;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Xunit;

public sealed class ForNullableCollection_Func
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
        var recorder = Mapper<int>.Target(Data<int>.TrueRecorder);

        var exception = Record.Exception(() => recorder(null!, new[] { 3, 4 }));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Enum_NotArrayType_FalseAndNotRecorded()
    {
        var value = "3";

        FalseAndNotRecorded<StringComparison>(value);
    }

    [Fact]
    public void Enum_SameType_TrueAndRecorded()
    {
        var value = new[] { StringComparison.CurrentCulture, StringComparison.InvariantCultureIgnoreCase };

        TrueAndRecorded(value, value);
    }

    [Fact]
    public void Enum_EnumsCastToObjects_TrueAndRecorded()
    {
        var value = new object[] { StringComparison.CurrentCulture, StringComparison.InvariantCultureIgnoreCase };

        TrueAndRecorded(value.Select(static (value) => (StringComparison)value), value);
    }

    [Fact]
    public void Enum_DifferentEnumType_FalseAndNotRecorded()
    {
        var value = new[] { StringSplitOptions.RemoveEmptyEntries, StringSplitOptions.TrimEntries };

        FalseAndNotRecorded<StringComparison>(value);
    }

    [Fact]
    public void Enum_Int_FalseAndNotRecorded()
    {
        var value = new[] { 3, 4 };

        FalseAndNotRecorded<StringSplitOptions>(value);
    }

    [Fact]
    public void Int_SameType_TrueAndRecorded()
    {
        var value = new[] { 3, 4 };

        TrueAndRecorded(value, value);
    }

    [Fact]
    public void Int_NullableintWithValues_FalseAndNotRecorded()
    {
        var value = new int?[] { 3, 4 };

        FalseAndNotRecorded<int>(value);
    }

    [Fact]
    public void Int_NullElement_FalseAndNotRecorded()
    {
        var value = new int?[] { 3, null };

        FalseAndNotRecorded<int>(value);
    }

    [Fact]
    public void Int_Enum_FalseAndNotRecorded()
    {
        var value = new[] { StringComparison.CurrentCulture, StringComparison.InvariantCultureIgnoreCase };

        FalseAndNotRecorded<int>(value);
    }

    [Fact]
    public void Int_Double_FalseAndNotRecorded()
    {
        var value = new[] { 3.14, 4.14 };

        FalseAndNotRecorded<int>(value);
    }

    [Fact]
    public void Int_String_FalseAndNotRecorded()
    {
        var value = new[] { "CurrentCulture", null };

        FalseAndNotRecorded<int>(value);
    }

    [Fact]
    public void Int_NullCollection_TrueAndRecorded()
    {
        IReadOnlyList<int>? value = null;

        TrueAndRecorded(value, value);
    }

    [Fact]
    public void Double_Int_FalseAndNotRecorded()
    {
        var value = new[] { 3, 4 };

        FalseAndNotRecorded<double>(value);
    }

    [Fact]
    public void String_SameType_TrueAndRecorded()
    {
        var value = new[] { "3", "4" };

        TrueAndRecorded(value, value);
    }

    [Fact]
    public void String_DifferentType_FalseAndNotRecorded()
    {
        var value = new object[] { "3", StringComparison.OrdinalIgnoreCase };

        FalseAndNotRecorded<string>(value);
    }

    [Fact]
    public void String_NullElement_FalseAndNotRecorded()
    {
        var value = new[] { "3", null };

        FalseAndNotRecorded<string>(value);
    }

    [Fact]
    public void String_NullCollection_TrueAndRecorded()
    {
        IReadOnlyList<string>? value = null;

        TrueAndRecorded(value, value);
    }

    [Fact]
    public void FalseReturningRecorder_FalseAndRecorded()
    {
        var recorder = Mapper<int>.Target(Data<int>.FalseRecorder);

        var value = new[] { 3, 4 };

        var data = new Data<int>();

        var outcome = recorder(data, value);

        Assert.False(outcome);

        Assert.Equal(value, data.Value);
        Assert.True(data.ValueRecorded);
    }

    [AssertionMethod]
    private static void TrueAndRecorded<T1>(IEnumerable<T1>? expected, object? value) where T1 : notnull
    {
        var recorder = Mapper<T1>.Target(Data<T1>.TrueRecorder);

        var data = new Data<T1>();

        var outcome = recorder(data, value);

        Assert.True(outcome);

        Assert.Equal<IEnumerable<T1>>(expected, data.Value);
        Assert.True(data.ValueRecorded);
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
        public static Func<Data<T>, object?, bool> Target(Func<Data<T>, IReadOnlyList<T>?, bool> recorder) => Adapters.Collection.ForNullableCollection(recorder).Invoke;
    }

    private sealed class Data<T>
    {
        public static Func<Data<T>, IReadOnlyList<T>?, bool> TrueRecorder => (dataRecord, argument) =>
        {
            Recorder(dataRecord, argument);

            return true;
        };

        public static Func<Data<T>, IReadOnlyList<T>?, bool> FalseRecorder => (dataRecord, argument) =>
        {
            Recorder(dataRecord, argument);

            return false;
        };

        private static void Recorder(Data<T> dataRecord, IReadOnlyList<T>? argument)
        {
            dataRecord.Value = argument;
            dataRecord.ValueRecorded = true;
        }

        public IReadOnlyList<T>? Value { get; set; }
        public bool ValueRecorded { get; set; }
    }
}
