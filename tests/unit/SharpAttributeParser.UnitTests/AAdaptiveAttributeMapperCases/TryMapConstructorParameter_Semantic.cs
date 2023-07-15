namespace SharpAttributeParser.AAdaptiveAttributeMapperCases;

using Microsoft.CodeAnalysis;

using Moq;

using System;

using Xunit;

public sealed class TryMapConstructorParameter_Semantic
{
    private static ISemanticAttributeArgumentRecorder? Target<TData>(ISemanticAttributeMapper<TData> mapper, TData dataRecord, IParameterSymbol parameter) => mapper.TryMapConstructorParameter(parameter, dataRecord);

    [Fact]
    public void NullData_ArgumentNullException()
    {
        var exception = Record.Exception(() => Target(new Mapper(), null!, Mock.Of<IParameterSymbol>()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullParameter_ArgumentNullException()
    {
        var exception = Record.Exception(() => Target(new Mapper(), new SemanticData(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NoMatchingParameter_ReturnsNull()
    {
        var recorder = Target(new Mapper(), new SemanticData(), Mock.Of<IParameterSymbol>((symbol) => symbol.Name == Mapper.NameNone));

        Assert.Null(recorder);
    }

    [Fact]
    public void ValueA_RecorderMapsToValueA()
    {
        SemanticData dataRecord = new();
        var argument = Mock.Of<object>();

        var recorder = Target(new Mapper(), dataRecord, Mock.Of<IParameterSymbol>((symbol) => symbol.Name == Mapper.NameValueA));

        var outcome = recorder!.RecordArgument(argument);

        Assert.True(outcome);

        Assert.False(dataRecord.T1Recorded);
        Assert.False(dataRecord.T2Recorded);
        Assert.Equal(argument, dataRecord.ValueA);
        Assert.False(dataRecord.ValueBRecorded);
    }

    [Fact]
    public void ValueB_RecorderMapsToValueB()
    {
        SemanticData dataRecord = new();
        var argument = Mock.Of<object>();

        var recorder = Target(new Mapper(), dataRecord, Mock.Of<IParameterSymbol>((parameter) => parameter.Name == Mapper.NameValueB));

        var outcome = recorder!.RecordArgument(argument);

        Assert.True(outcome);

        Assert.False(dataRecord.T1Recorded);
        Assert.False(dataRecord.T2Recorded);
        Assert.False(dataRecord.ValueARecorded);
        Assert.Equal(argument, dataRecord.ValueB);
    }
}
