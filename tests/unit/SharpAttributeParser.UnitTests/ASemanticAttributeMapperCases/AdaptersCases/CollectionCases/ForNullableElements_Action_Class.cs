namespace SharpAttributeParser.ASemanticAttributeMapperCases.AdaptersCases.CollectionCases;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Xunit;

public sealed class ForNullableElements_Action_Class
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

        var exception = Record.Exception(() => recorder(null!, new[] { "3", "4" }));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void String_NotArrayType_FalseAndNotRecorded()
    {
        var value = "3";

        FalseAndNotRecorded<string>(value);
    }

    [Fact]
    public void String_SameType_TrueAndRecorded()
    {
        var value = new[] { "3", null };

        TrueAndRecorded(value, value);
    }

    [Fact]
    public void String_StringsCastToObjects_TrueAndRecorded()
    {
        var value = new object?[] { "1", null };

        TrueAndRecorded(value.Select(static (value) => (string?)value), value);
    }

    [Fact]
    public void String_DifferentType_FalseAndNotRecorded()
    {
        var value = new object[] { StringComparison.OrdinalIgnoreCase, "2" };

        FalseAndNotRecorded<string>(value);
    }

    [Fact]
    public void String_NullCollection_FalseAndNotRecorded()
    {
        IReadOnlyList<string>? value = null;

        FalseAndNotRecorded<string>(value);
    }

    [AssertionMethod]
    private static void TrueAndRecorded<T1>(IEnumerable<T1?>? expected, object? value) where T1 : class
    {
        var recorder = Mapper<T1>.Target(Data<T1>.Recorder);

        var data = new Data<T1>();

        var outcome = recorder(data, value);

        Assert.True(outcome);

        Assert.Equal<IEnumerable<T1?>>(expected, data.Value);
        Assert.True(data.ValueRecorded);
    }

    [AssertionMethod]
    private static void FalseAndNotRecorded<T1>(object? value) where T1 : class
    {
        var recorder = Mapper<T1>.Target(Data<T1>.Recorder);

        var data = new Data<T1>();

        var outcome = recorder(data, value);

        Assert.False(outcome);

        Assert.False(data.ValueRecorded);
    }

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used to expose static member of ASemanticAttributeMapper.")]
    private sealed class Mapper<T> : ASemanticAttributeMapper<Data<T>> where T : class
    {
        public static Func<Data<T>, object?, bool> Target(Action<Data<T>, IReadOnlyList<T?>> recorder) => Adapters.Collection.ForNullableElements(recorder).Invoke;
    }

    private sealed class Data<T>
    {
        public static Action<Data<T>, IReadOnlyList<T?>> Recorder => (dataRecord, argument) =>
        {
            dataRecord.Value = argument;
            dataRecord.ValueRecorded = true;
        };

        public IReadOnlyList<T?>? Value { get; set; }
        public bool ValueRecorded { get; set; }
    }
}
