namespace SharpAttributeParser.ASemanticAttributeMapperCases;

using Microsoft.CodeAnalysis;

using Moq;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class TryMapConstructorParameter
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
        var exception = Record.Exception(() => Target(new Mapper(), new Data(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NoMatchingParameter_ReturnsNull()
    {
        var recorder = Target(new Mapper(), new Data(), Mock.Of<IParameterSymbol>((symbol) => symbol.Name == Mapper.NameNone));

        Assert.Null(recorder);
    }

    [Fact]
    public void ValueA_RecorderMapsToValueA()
    {
        Data dataRecord = new();
        var argument = Mock.Of<object>();

        var recorder = Target(new Mapper(), dataRecord, Mock.Of<IParameterSymbol>((symbol) => symbol.Name == Mapper.NameValueA));

        var outcome = recorder!.RecordArgument(argument);

        Assert.True(outcome);

        Assert.False(dataRecord.T1Recorded);
        Assert.False(dataRecord.T2Recorded);
        Assert.Equal(argument, dataRecord.ValueA, ReferenceEqualityComparer.Instance);
        Assert.False(dataRecord.ValueBRecorded);
    }

    [Fact]
    public void ValueB_RecorderMapsToValueB()
    {
        Data dataRecord = new();
        var argument = Mock.Of<object>();

        var recorder = Target(new Mapper(), dataRecord, Mock.Of<IParameterSymbol>((parameter) => parameter.Name == Mapper.NameValueB));

        var outcome = recorder!.RecordArgument(argument);

        Assert.True(outcome);

        Assert.False(dataRecord.T1Recorded);
        Assert.False(dataRecord.T2Recorded);
        Assert.False(dataRecord.ValueARecorded);
        Assert.Equal(argument, dataRecord.ValueB, ReferenceEqualityComparer.Instance);
    }
}
