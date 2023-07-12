namespace SharpAttributeParser.ASyntacticAttributeMapperCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using Moq;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class TryMapTypeParameter
{
    private static ISyntacticAttributeArgumentRecorder? Target<TData>(ISyntacticAttributeMapper<TData> mapper, TData dataRecord, ITypeParameterSymbol parameter) => mapper.TryMapTypeParameter(parameter, dataRecord);

    [Fact]
    public void NullData_ArgumentNullException()
    {
        var exception = Record.Exception(() => Target(new Mapper(), null!, Mock.Of<ITypeParameterSymbol>()));

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
        var recorder = Target(new Mapper(), new Data(), Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == Mapper.TypeNameNone));

        Assert.Null(recorder);
    }

    [Fact]
    public void T1ByIndex_RecorderMapsToT1()
    {
        Data dataRecord = new();
        var syntax = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);

        var recorder = Target(new Mapper(), dataRecord, Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == Mapper.TypeNameNone && symbol.Ordinal == Mapper.IndexT1));

        var outcome = recorder!.RecordArgumentSyntax(syntax);

        Assert.True(outcome);

        IdenticalToExpected(new Data() { T1Syntax = syntax, T1SyntaxRecorded = true }, dataRecord);
    }

    [Fact]
    public void T1ByName_RecorderMapsToT1()
    {
        Data dataRecord = new();
        var syntax = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);

        var recorder = Target(new Mapper(), dataRecord, Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == Mapper.TypeNameT1));

        var outcome = recorder!.RecordArgumentSyntax(syntax);

        Assert.True(outcome);

        IdenticalToExpected(new Data() { T1Syntax = syntax, T1SyntaxRecorded = true }, dataRecord);
    }

    [Fact]
    public void T1ByIndexAndName_RecorderMapsToT1()
    {
        Data dataRecord = new();
        var syntax = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);

        var recorder = Target(new Mapper(), dataRecord, Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == Mapper.TypeNameT1 && symbol.Ordinal == Mapper.IndexT1));

        var outcome = recorder!.RecordArgumentSyntax(syntax);

        Assert.True(outcome);

        IdenticalToExpected(new Data() { T1Syntax = syntax, T1SyntaxRecorded = true }, dataRecord);
    }

    [Fact]
    public void T2ByIndex_RecorderMapsToT2()
    {
        Data dataRecord = new();
        var syntax = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);

        var recorder = Target(new Mapper(), dataRecord, Mock.Of<ITypeParameterSymbol>((symol) => symol.Name == Mapper.TypeNameNone && symol.Ordinal == Mapper.IndexT2));

        var outcome = recorder!.RecordArgumentSyntax(syntax);

        Assert.True(outcome);

        IdenticalToExpected(new Data() { T2Syntax = syntax, T2SyntaxRecorded = true }, dataRecord);
    }

    [Fact]
    public void T2ByName_RecorderMapsToT2()
    {
        Data dataRecord = new();
        var syntax = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);

        var recorder = Target(new Mapper(), dataRecord, Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == Mapper.TypeNameT2));

        var outcome = recorder!.RecordArgumentSyntax(syntax);

        Assert.True(outcome);

        IdenticalToExpected(new Data() { T2Syntax = syntax, T2SyntaxRecorded = true }, dataRecord);
    }

    [Fact]
    public void T1ByNameAndT2ByIndex_ReturnsNull()
    {
        var recorder = Target(new Mapper(), new Data(), Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == Mapper.TypeNameT1 && symbol.Ordinal == Mapper.IndexT2));

        Assert.Null(recorder);
    }

    [AssertionMethod]
    private static void IdenticalToExpected(Data expected, Data actual)
    {
        Assert.Equal(expected.T1Syntax, actual.T1Syntax, ReferenceEqualityComparer.Instance);
        Assert.Equal(expected.T2Syntax, actual.T2Syntax, ReferenceEqualityComparer.Instance);

        expected.ValueASyntax.Switch
        (
            (syntax) => Assert.Equal(syntax, actual.ValueASyntax.AsT0, ReferenceEqualityComparer.Instance),
            (elementSyntax) => Assert.Equal(elementSyntax, actual.ValueASyntax.AsT1, ReferenceEqualityComparer.Instance)
        );

        expected.ValueBSyntax.Switch
        (
            (syntax) => Assert.Equal(syntax, actual.ValueBSyntax.AsT0, ReferenceEqualityComparer.Instance),
            (elementSyntax) => Assert.Equal(elementSyntax, actual.ValueBSyntax.AsT1, ReferenceEqualityComparer.Instance)
        );

        Assert.Equal(expected.T1SyntaxRecorded, actual.T1SyntaxRecorded);
        Assert.Equal(expected.T2SyntaxRecorded, actual.T2SyntaxRecorded);
        Assert.Equal(expected.ValueASyntaxRecorded, actual.ValueASyntaxRecorded);
        Assert.Equal(expected.ValueBSyntaxRecorded, actual.ValueBSyntaxRecorded);
    }
}
