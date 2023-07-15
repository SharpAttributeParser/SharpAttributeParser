namespace SharpAttributeParser.ASemanticArgumentRecorderCases.AdapterCases;

using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

public sealed class ForNullableElements_Action_Struct
{
    private static DSemanticArrayRecorder Target<T>(ISemanticAdapterProvider adapters, Action<IReadOnlyList<T?>> recorder) where T : struct => adapters.ForNullableElements(recorder);

    [Fact]
    public void NullDelegate_ArgumentNullExceptionWhenUsed()
    {
        NullDelegateRecorder recorder = new();

        var exception = Record.Exception(() => RecordArgument(recorder, null));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Enum_SameType_True_Recorded()
    {
        var value = new StringComparison?[] { StringComparison.CurrentCulture, StringComparison.InvariantCultureIgnoreCase };

        TrueAndRecorded(value, value);
    }

    [Fact]
    public void Enum_DifferentEnumType_False_NotRecorded()
    {
        var value = new[] { StringSplitOptions.RemoveEmptyEntries, StringSplitOptions.TrimEntries };

        FalseAndNotRecorded<StringComparison, StringSplitOptions>(value);
    }

    [Fact]
    public void Enum_Int_False_NotRecorded()
    {
        var value = new[] { 2, 3 };

        FalseAndNotRecorded<StringSplitOptions, int>(value);
    }

    [Fact]
    public void Int_SameType_True_Recorded()
    {
        var value = new int?[] { 1, 2 };

        TrueAndRecorded(value, value);
    }

    [Fact]
    public void Int_NullElement_True_Recorded()
    {
        var value = new int?[] { null, 2 };

        TrueAndRecorded(value, value);
    }

    [Fact]
    public void Int_Enum_False_NotRecorded()
    {
        var value = new[] { StringComparison.CurrentCulture, StringComparison.InvariantCultureIgnoreCase };

        FalseAndNotRecorded<int, StringComparison>(value);
    }

    [Fact]
    public void Int_Double_False_NotRecorded()
    {
        var value = new[] { 2.718, 3.14 };

        FalseAndNotRecorded<int, double>(value);
    }

    [Fact]
    public void Int_String_False_NotRecorded()
    {
        var value = new[] { "CurrentCulture", "InvariantCultureIgnoreCase" };

        FalseAndNotRecorded<int, string>(value);
    }

    [Fact]
    public void Int_NullCollection_False_NotRecorded()
    {
        IReadOnlyList<int>? value = null;

        FalseAndNotRecorded<int, int>(value);
    }

    [Fact]
    public void Double_Int_False_NotRecorded()
    {
        var value = new[] { 2, 3 };

        FalseAndNotRecorded<double, int>(value);
    }

    [AssertionMethod]
    private static void TrueAndRecorded<T1, T2>(IEnumerable<T1?> expected, IEnumerable<T2?> value) where T1 : struct
    {
        Recorder<T1> recorder = new();

        var actual = RecordArgument(recorder, value.Select(static (value) => (object?)value).ToList());

        Assert.True(actual);

        Assert.Equal<IEnumerable<T1?>>(expected, recorder.Value);
        Assert.True(recorder.ValueRecorded);
    }

    [AssertionMethod]
    private static void FalseAndNotRecorded<T1, T2>(IEnumerable<T2?>? value) where T1 : struct
    {
        Recorder<T1> recorder = new();

        var actual = RecordArgument(recorder, value?.Select(static (value) => (object?)value).ToList());

        Assert.False(actual);

        Assert.False(recorder.ValueRecorded);
    }

    private static bool RecordArgument(ISemanticArgumentRecorder recorder, IReadOnlyList<object?>? value) => recorder.TryRecordNamedArgument(string.Empty, value);

    private sealed class NullDelegateRecorder : ASemanticArgumentRecorder
    {
        protected override IEqualityComparer<string> Comparer { get; } = StringComparerMock.CreateComparer(true);

        protected override IEnumerable<(string, DSemanticArrayRecorder)> AddArrayRecorders()
        {
            yield return (string.Empty, Target<int>(Adapters, null!));
        }
    }

    private sealed class Recorder<T> : ASemanticArgumentRecorder where T : struct
    {
        public IReadOnlyList<T?>? Value { get; private set; }
        public bool ValueRecorded { get; private set; }

        protected override IEqualityComparer<string> Comparer { get; } = StringComparerMock.CreateComparer(true);

        protected override IEnumerable<(string, DSemanticArrayRecorder)> AddArrayRecorders()
        {
            yield return (string.Empty, Target<T>(Adapters, RecordValue));
        }

        private void RecordValue(IReadOnlyList<T?> value)
        {
            Value = value;
            ValueRecorded = true;
        }
    }
}
