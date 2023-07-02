namespace SharpAttributeParser.ASemanticArgumentRecorderCases.AdapterCases;

using Microsoft.CodeAnalysis;

using Moq;

using SharpAttributeParser;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class For_ITypeSymbol
{
    private static DSemanticGenericRecorder Target(ISemanticAdapterProvider adapters, Action<ITypeSymbol> recorder) => adapters.For(recorder);

    [Fact]
    public void NullDelegate_ArgumentNullExceptionWhenUsed()
    {
        NullDelegateRecorder recorder = new();

        var exception = Record.Exception(() => RecordArgument(recorder, Mock.Of<ITypeSymbol>()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void True_Recorded()
    {
        Recorder recorder = new();

        var value = Mock.Of<ITypeSymbol>();

        var actual = RecordArgument(recorder, value);

        Assert.True(actual);

        Assert.Equal(value, recorder.Value, ReferenceEqualityComparer.Instance);
        Assert.True(recorder.ValueRecorded);
    }

    private static bool RecordArgument(ASemanticArgumentRecorder recorder, ITypeSymbol value) => recorder.TryRecordGenericArgument(Mock.Of<ITypeParameterSymbol>((parameter) => parameter.Name == string.Empty), value);

    private sealed class NullDelegateRecorder : ASemanticArgumentRecorder
    {
        protected override IEqualityComparer<string> Comparer { get; } = StringComparerMock.CreateComparer(true);

        protected override IEnumerable<(string, DSemanticGenericRecorder)> AddNamedGenericRecorders()
        {
            yield return (string.Empty, Target(Adapters, null!));
        }
    }

    private sealed class Recorder : ASemanticArgumentRecorder
    {
        public ITypeSymbol? Value { get; private set; }
        public bool ValueRecorded { get; private set; }

        protected override IEqualityComparer<string> Comparer { get; } = StringComparerMock.CreateComparer(true);

        protected override IEnumerable<(string, DSemanticGenericRecorder)> AddNamedGenericRecorders()
        {
            yield return (string.Empty, Target(Adapters, RecordValue));
        }

        private void RecordValue(ITypeSymbol value)
        {
            Value = value;
            ValueRecorded = true;
        }
    }
}
