namespace SharpAttributeParser.ASyntacticAttributeMapperCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class TryMapConstructorParameter
{
    private static ISyntacticAttributeArgumentRecorder? Target<TData>(ISyntacticAttributeMapper<TData> mapper, TData dataRecord, IParameterSymbol parameter) => mapper.TryMapConstructorParameter(parameter, dataRecord);

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
        var syntax = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);

        var recorder = Target(new Mapper(), dataRecord, Mock.Of<IParameterSymbol>((symbol) => symbol.Name == Mapper.NameValueA));

        var outcome = recorder!.RecordArgumentSyntax(syntax);

        Assert.True(outcome);

        Assert.False(dataRecord.T1SyntaxRecorded);
        Assert.False(dataRecord.T2SyntaxRecorded);
        Assert.Equal(syntax, dataRecord.ValueASyntax.AsT0, ReferenceEqualityComparer.Instance);
        Assert.False(dataRecord.ValueBSyntaxRecorded);
    }

    [Fact]
    public void ValueA_Params_RecorderMapsToValueA()
    {
        Data dataRecord = new();
        var syntax = new[] { SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression) };

        var recorder = Target(new Mapper(), dataRecord, Mock.Of<IParameterSymbol>((symbol) => symbol.Name == Mapper.NameValueA));

        var outcome = recorder!.RecordParamsArgumentSyntax(syntax);

        Assert.True(outcome);

        Assert.False(dataRecord.T1SyntaxRecorded);
        Assert.False(dataRecord.T2SyntaxRecorded);
        Assert.Equal<IReadOnlyList<ExpressionSyntax>>(syntax, dataRecord.ValueASyntax.AsT1, ReferenceEqualityComparer.Instance);
        Assert.False(dataRecord.ValueBSyntaxRecorded);
    }

    [Fact]
    public void ValueB_RecorderMapsToValueB()
    {
        Data dataRecord = new();
        var syntax = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);

        var recorder = Target(new Mapper(), dataRecord, Mock.Of<IParameterSymbol>((parameter) => parameter.Name == Mapper.NameValueB));

        var outcome = recorder!.RecordArgumentSyntax(syntax);

        Assert.True(outcome);

        Assert.False(dataRecord.T1SyntaxRecorded);
        Assert.False(dataRecord.T2SyntaxRecorded);
        Assert.False(dataRecord.ValueASyntaxRecorded);
        Assert.Equal(syntax, dataRecord.ValueBSyntax.AsT0, ReferenceEqualityComparer.Instance);
    }
}
