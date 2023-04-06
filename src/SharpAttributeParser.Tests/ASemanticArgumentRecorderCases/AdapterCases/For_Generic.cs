namespace SharpAttributeParser.Tests.ASemanticArgumentRecorderCases.AdapterCases;

using Microsoft.CodeAnalysis;

using Moq;

using SharpAttributeParser;

using System.Collections.Generic;

using Xunit;

public class For_Generic
{
    private static bool TryRecordGenericArgument(ASemanticArgumentRecorder recorder, string parameterName, ITypeSymbol value) => recorder.TryRecordGenericArgument(parameterName, value);

    [Fact]
    public void True_RecordedPopulated()
    {
        Recorder recorder = new();

        var parameterName = "Value";
        Mock<ITypeSymbol> symbolMock = new();

        var actual = TryRecordGenericArgument(recorder, parameterName, symbolMock.Object);

        Assert.True(actual);

        Assert.Equal(symbolMock.Object, recorder.Value);
    }

    private sealed class Recorder : ASemanticArgumentRecorder
    {
        public ITypeSymbol? Value { get; private set; }

        protected override IEnumerable<(string, DSemanticGenericRecorder)> AddGenericRecorders()
        {
            yield return ("Value", Adapters.For(RecordValue));
        }

        private void RecordValue(ITypeSymbol value)
        {
            Value = value;
        }
    }
}
