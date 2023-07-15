namespace SharpAttributeParser.ASyntacticAttributeMapperCases;

using Microsoft.CodeAnalysis;

using Moq;

using System;

using Xunit;

public sealed class TryMapConstructorParameter
{
    private static ISyntacticAttributeConstructorArgumentRecorder? Target<TData>(ISyntacticAttributeMapper<TData> mapper, TData dataRecord, IParameterSymbol parameter) => mapper.TryMapConstructorParameter(parameter, dataRecord);

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
        var syntax = ExpressionSyntaxFactory.Create();

        var recorder = Target(new Mapper(), dataRecord, Mock.Of<IParameterSymbol>((symbol) => symbol.Name == Mapper.NameValueA));

        var outcome = recorder!.RecordArgumentSyntax(syntax);

        Assert.True(outcome);

        Assert.False(dataRecord.T1SyntaxRecorded);
        Assert.False(dataRecord.T2SyntaxRecorded);
        Assert.Equal(syntax, dataRecord.ValueASyntax);
        Assert.False(dataRecord.ValueBSyntaxRecorded);
    }

    [Fact]
    public void ValueA_Params_RecorderMapsToValueA()
    {
        Data dataRecord = new();
        var syntax = ExpressionSyntaxFactory.CreateCollection();

        var recorder = Target(new Mapper(), dataRecord, Mock.Of<IParameterSymbol>((symbol) => symbol.Name == Mapper.NameValueA));

        var outcome = recorder!.RecordParamsArgumentSyntax(syntax);

        Assert.True(outcome);

        Assert.False(dataRecord.T1SyntaxRecorded);
        Assert.False(dataRecord.T2SyntaxRecorded);
        Assert.Equal(syntax, dataRecord.ValueASyntax.AsT1);
        Assert.False(dataRecord.ValueBSyntaxRecorded);
    }

    [Fact]
    public void ValueB_RecorderMapsToValueB()
    {
        Data dataRecord = new();
        var syntax = ExpressionSyntaxFactory.Create();

        var recorder = Target(new Mapper(), dataRecord, Mock.Of<IParameterSymbol>((parameter) => parameter.Name == Mapper.NameValueB));

        var outcome = recorder!.RecordArgumentSyntax(syntax);

        Assert.True(outcome);

        Assert.False(dataRecord.T1SyntaxRecorded);
        Assert.False(dataRecord.T2SyntaxRecorded);
        Assert.False(dataRecord.ValueASyntaxRecorded);
        Assert.Equal(syntax, dataRecord.ValueBSyntax);
    }

    [Fact]
    public void NullSyntaxToMappedRecorder_ArgumentNullExceptionWhenUsed()
    {
        Data dataRecord = new();

        var recorder = Target(new Mapper(), dataRecord, Mock.Of<IParameterSymbol>((symbol) => symbol.Name == Mapper.NameValueA));

        var exception = Record.Exception(() => recorder!.RecordArgumentSyntax(null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullSyntaxCollectionToMappedRecorder_ArgumentNullExceptionWhenUsed()
    {
        Data dataRecord = new();

        var recorder = Target(new Mapper(), dataRecord, Mock.Of<IParameterSymbol>((symbol) => symbol.Name == Mapper.NameValueA));

        var exception = Record.Exception(() => recorder!.RecordParamsArgumentSyntax(null!));

        Assert.IsType<ArgumentNullException>(exception);
    }
}
