namespace SharpAttributeParser.ASyntacticArgumentRecorderCases.AdapterCases;

using Microsoft.CodeAnalysis;

using Moq;

using SharpAttributeParser;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class For_ITypeSymbol
{
    private static DSyntacticGenericRecorder Target(ISyntacticAdapterProvider adapters, Action<ITypeSymbol, Location> recorder) => adapters.For(recorder);

    [Fact]
    public void NullDelegate_ArgumentNullExceptionWhenUsed()
    {
        NullDelegateRecorder recorder = new();

        var exception = Record.Exception(() => RecordArgument(recorder, Mock.Of<ITypeSymbol>(), Location.None));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void True_Recorded()
    {
        Recorder recorder = new();

        var value = Mock.Of<ITypeSymbol>();

        var location = CustomLocation.Create();

        var actual = RecordArgument(recorder, value, location);

        Assert.True(actual);

        Assert.Equal(value, recorder.Value, ReferenceEqualityComparer.Instance);
        Assert.True(recorder.ValueRecorded);
        Assert.Equal(location, recorder.ValueLocation, ReferenceEqualityComparer.Instance);
    }

    private static bool RecordArgument(ASyntacticArgumentRecorder recorder, ITypeSymbol value, Location location) => recorder.TryRecordGenericArgument(Mock.Of<ITypeParameterSymbol>((parameter) => parameter.Name == string.Empty), value, location);

    private sealed class NullDelegateRecorder : ASyntacticArgumentRecorder
    {
        protected override IEqualityComparer<string> Comparer { get; } = StringComparerMock.CreateComparer(true);

        protected override IEnumerable<(string, DSyntacticGenericRecorder)> AddNamedGenericRecorders()
        {
            yield return (string.Empty, Target(Adapters, null!));
        }
    }

    private sealed class Recorder : ASyntacticArgumentRecorder
    {
        public ITypeSymbol? Value { get; private set; }
        public bool ValueRecorded { get; private set; }
        public Location? ValueLocation { get; private set; }

        protected override IEqualityComparer<string> Comparer { get; } = StringComparerMock.CreateComparer(true);

        protected override IEnumerable<(string, DSyntacticGenericRecorder)> AddNamedGenericRecorders()
        {
            yield return (string.Empty, Target(Adapters, RecordValue));
        }

        private void RecordValue(ITypeSymbol value, Location location)
        {
            Value = value;
            ValueRecorded = true;
            ValueLocation = location;
        }
    }
}
