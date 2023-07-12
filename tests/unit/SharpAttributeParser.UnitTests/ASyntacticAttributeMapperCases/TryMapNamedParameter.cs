namespace SharpAttributeParser.ASyntacticAttributeMapperCases;

using Microsoft.CodeAnalysis.CSharp;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class TryMapNamedParameter
{
    private static ISyntacticAttributeArgumentRecorder? Target<TData>(ISyntacticAttributeMapper<TData> mapper, TData dataRecord, string parameterName) => mapper.TryMapNamedParameter(parameterName, dataRecord);

    [Fact]
    public void NullData_ArgumentNullException()
    {
        var exception = Record.Exception(() => Target(new Mapper(), null!, string.Empty));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullParameterName_ArgumentNullException()
    {
        var exception = Record.Exception(() => Target(new Mapper(), new Data(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NoMatchingParameter_ReturnsNull()
    {
        var recorder = Target(new Mapper(), new Data(), Mapper.NameNone);

        Assert.Null(recorder);
    }

    [Fact]
    public void ValueA_RecorderMapsToValueA()
    {
        Data dataRecord = new();
        var syntax = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);

        var recorder = Target(new Mapper(), dataRecord, Mapper.NameValueA);

        var outcome = recorder!.RecordArgumentSyntax(syntax);

        Assert.True(outcome);

        Assert.False(dataRecord.T1SyntaxRecorded);
        Assert.False(dataRecord.T2SyntaxRecorded);
        Assert.Equal(syntax, dataRecord.ValueASyntax.AsT0, ReferenceEqualityComparer.Instance);
        Assert.False(dataRecord.ValueBSyntaxRecorded);
    }

    [Fact]
    public void ValueB_RecorderMapsToValueB()
    {
        Data dataRecord = new();
        var syntax = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);

        var recorder = Target(new Mapper(), dataRecord, Mapper.NameValueB);

        var outcome = recorder!.RecordArgumentSyntax(syntax);

        Assert.True(outcome);

        Assert.False(dataRecord.T1SyntaxRecorded);
        Assert.False(dataRecord.T2SyntaxRecorded);
        Assert.False(dataRecord.ValueASyntaxRecorded);
        Assert.Equal(syntax, dataRecord.ValueBSyntax.AsT0, ReferenceEqualityComparer.Instance);
    }
}
