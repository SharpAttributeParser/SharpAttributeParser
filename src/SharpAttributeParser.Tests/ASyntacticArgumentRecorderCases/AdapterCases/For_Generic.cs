namespace SharpAttributeParser.Tests.ASyntacticArgumentRecorderCases.AdapterCases;

using Microsoft.CodeAnalysis;

using Moq;

using SharpAttributeParser;

using System.Collections.Generic;

using Xunit;

public class For_Generic
{
    private static bool TryRecordGenericArgument(ASyntacticArgumentRecorder recorder, string parameterName, ITypeSymbol value)
    {
        Mock<ITypeParameterSymbol> parameterMock = new();

        parameterMock.SetupGet(static (parameter) => parameter.Name).Returns(parameterName);

        return recorder.TryRecordGenericArgument(parameterMock.Object, value, Location.None);
    }

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

    private sealed class Recorder : ASyntacticArgumentRecorder
    {
        public ITypeSymbol? Value { get; private set; }

        protected override IEnumerable<(string, DSyntacticGenericRecorder)> AddGenericRecorders()
        {
            yield return ("Value", Adapters.For(RecordValue));
        }

        private void RecordValue(ITypeSymbol value, Location location)
        {
            Value = value;
        }
    }
}
