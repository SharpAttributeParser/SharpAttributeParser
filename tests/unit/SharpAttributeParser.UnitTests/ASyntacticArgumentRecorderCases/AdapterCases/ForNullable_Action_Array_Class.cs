namespace SharpAttributeParser.ASyntacticArgumentRecorderCases.AdapterCases;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

public sealed class ForNullable_Action_Array_Class
{
    private static DSyntacticArrayRecorder Target<T>(ISyntacticAdapterProvider adapters, Action<IReadOnlyList<T?>?, CollectionLocation> recorder) where T : class => adapters.ForNullable(recorder);

    [Fact]
    public void NullDelegate_ArgumentNullExceptionWhenUsed()
    {
        NullDelegateRecorder recorder = new();

        var exception = Record.Exception(() => RecordArgument(recorder, null, CollectionLocation.None));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void String_SameType_True_Recorded()
    {
        var value = new[] { "1", "2" };

        TrueAndRecorded(value, value);
    }

    [Fact]
    public void String_DifferentType_False_NotRecorded()
    {
        var value = new object[] { StringComparison.OrdinalIgnoreCase, "2" };

        FalseAndNotRecorded<string, object>(value);
    }

    [Fact]
    public void String_NullElement_True_Recorded()
    {
        var value = new[] { null, "2" };

        TrueAndRecorded(value, value);
    }

    [Fact]
    public void String_NullCollection_True_Recorded()
    {
        IReadOnlyList<string>? value = null;

        TrueAndRecorded(value, value);
    }

    [AssertionMethod]
    private static void TrueAndRecorded<T1, T2>(IEnumerable<T1?>? expected, IEnumerable<T2?>? value) where T1 : class
    {
        Recorder<T1> recorder = new();

        var location = CustomLocation.CreateCollection();

        var actual = RecordArgument(recorder, value?.Select(static (value) => (object?)value).ToList(), location);

        Assert.True(actual);

        Assert.Equal<IEnumerable<T1?>>(expected, recorder.Value);
        Assert.True(recorder.ValueRecorded);
        Assert.Equal(location, recorder.ValueLocation, ReferenceEqualityComparer.Instance);
    }

    [AssertionMethod]
    private static void FalseAndNotRecorded<T1, T2>(IEnumerable<T2?>? value) where T1 : class
    {
        Recorder<T1> recorder = new();

        var actual = RecordArgument(recorder, value?.Select(static (value) => (object?)value).ToList(), CollectionLocation.None);

        Assert.False(actual);

        Assert.False(recorder.ValueRecorded);
    }

    private static bool RecordArgument(ISyntacticArgumentRecorder recorder, IReadOnlyList<object?>? value, CollectionLocation location) => recorder.TryRecordNamedArgument(string.Empty, value, location);

    private sealed class NullDelegateRecorder : ASyntacticArgumentRecorder
    {
        protected override IEqualityComparer<string> Comparer { get; } = StringComparerMock.CreateComparer(true);

        protected override IEnumerable<(string, DSyntacticArrayRecorder)> AddArrayRecorders()
        {
            yield return (string.Empty, Target<string>(Adapters, null!));
        }
    }

    private sealed class Recorder<T> : ASyntacticArgumentRecorder where T : class
    {
        public IReadOnlyList<T?>? Value { get; private set; }
        public bool ValueRecorded { get; private set; }
        public CollectionLocation? ValueLocation { get; private set; }

        protected override IEqualityComparer<string> Comparer { get; } = StringComparerMock.CreateComparer(true);

        protected override IEnumerable<(string, DSyntacticArrayRecorder)> AddArrayRecorders()
        {
            yield return (string.Empty, Target<T>(Adapters, RecordValue));
        }

        private void RecordValue(IReadOnlyList<T?>? value, CollectionLocation location)
        {
            Value = value;
            ValueRecorded = true;
            ValueLocation = location;
        }
    }
}
