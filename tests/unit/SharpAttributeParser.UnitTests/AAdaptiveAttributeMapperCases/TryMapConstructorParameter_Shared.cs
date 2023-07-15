namespace SharpAttributeParser.AAdaptiveAttributeMapperCases;

using Microsoft.CodeAnalysis;

using Moq;

using System;

using Xunit;

public sealed class TryMapConstructorParameter_Shared
{
    private static IAttributeConstructorArgumentRecorder? Target<TData>(IAttributeMapper<TData> mapper, TData dataRecord, IParameterSymbol parameter) => mapper.TryMapConstructorParameter(parameter, dataRecord);

    [Fact]
    public void NullData_ArgumentNullException()
    {
        var exception = Record.Exception(() => Target(new Mapper(), null!, Mock.Of<IParameterSymbol>()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullParameter_ArgumentNullException()
    {
        var exception = Record.Exception(() => Target(new Mapper(), new SharedData(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NoMatchingParameter_ReturnsNull()
    {
        var recorder = Target(new Mapper(), new SharedData(), Mock.Of<IParameterSymbol>((symbol) => symbol.Name == Mapper.NameNone));

        Assert.Null(recorder);
    }

    [Fact]
    public void ValueA_RecorderMapsToValueA()
    {
        SharedData dataRecord = new();
        var argument = Mock.Of<object>();
        var syntax = ExpressionSyntaxFactory.Create();

        var recorder = Target(new Mapper(), dataRecord, Mock.Of<IParameterSymbol>((symbol) => symbol.Name == Mapper.NameValueA));

        var outcome = recorder!.RecordArgument(argument, syntax);

        Assert.True(outcome);

        Assert.False(dataRecord.T1Recorded);
        Assert.False(dataRecord.T2Recorded);
        Assert.Equal(argument, dataRecord.ValueA);
        Assert.Equal(syntax, dataRecord.ValueASyntax);
        Assert.False(dataRecord.ValueBRecorded);
    }

    [Fact]
    public void ValueA_Params_RecorderMapsToValueA()
    {
        SharedData dataRecord = new();
        var argument = Mock.Of<object>();
        var syntax = ExpressionSyntaxFactory.CreateCollection();

        var recorder = Target(new Mapper(), dataRecord, Mock.Of<IParameterSymbol>((symbol) => symbol.Name == Mapper.NameValueA));

        var outcome = recorder!.RecordParamsArgument(argument, syntax);

        Assert.True(outcome);

        Assert.False(dataRecord.T1Recorded);
        Assert.False(dataRecord.T2Recorded);
        Assert.Equal(argument, dataRecord.ValueA);
        Assert.Equal(syntax, dataRecord.ValueASyntax.AsT1);
        Assert.False(dataRecord.ValueBRecorded);
    }

    [Fact]
    public void ValueB_RecorderMapsToValueB()
    {
        SharedData dataRecord = new();
        var argument = Mock.Of<object>();
        var syntax = ExpressionSyntaxFactory.Create();

        var recorder = Target(new Mapper(), dataRecord, Mock.Of<IParameterSymbol>((parameter) => parameter.Name == Mapper.NameValueB));

        var outcome = recorder!.RecordArgument(argument, syntax);

        Assert.True(outcome);

        Assert.False(dataRecord.T1Recorded);
        Assert.False(dataRecord.T2Recorded);
        Assert.False(dataRecord.ValueARecorded);
        Assert.Equal(argument, dataRecord.ValueB);
        Assert.Equal(syntax, dataRecord.ValueBSyntax);
    }

    [Fact]
    public void NullSyntaxToMappedRecorder_ArgumentNullExceptionWhenUsed()
    {
        SharedData dataRecord = new();

        var recorder = Target(new Mapper(), dataRecord, Mock.Of<IParameterSymbol>((symbol) => symbol.Name == Mapper.NameValueA));

        var exception = Record.Exception(() => recorder!.RecordArgument(null, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullSyntaxCollectionToMappedRecorder_ArgumentNullExceptionWhenUsed()
    {
        SharedData dataRecord = new();

        var recorder = Target(new Mapper(), dataRecord, Mock.Of<IParameterSymbol>((symbol) => symbol.Name == Mapper.NameValueA));

        var exception = Record.Exception(() => recorder!.RecordParamsArgument(null, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }
}
