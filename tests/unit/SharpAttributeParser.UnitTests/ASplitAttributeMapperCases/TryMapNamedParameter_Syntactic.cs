﻿namespace SharpAttributeParser.ASplitAttributeMapperCases;

using System;

using Xunit;

public sealed class TryMapNamedParameter_Syntactic
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
        var exception = Record.Exception(() => Target(new Mapper(), new SyntacticData(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NoMatchingParameter_ReturnsNull()
    {
        var recorder = Target(new Mapper(), new SyntacticData(), Mapper.NameNone);

        Assert.Null(recorder);
    }

    [Fact]
    public void ValueA_RecorderMapsToValueA()
    {
        SyntacticData dataRecord = new();
        var syntax = ExpressionSyntaxFactory.Create();

        var recorder = Target(new Mapper(), dataRecord, Mapper.NameValueA);

        var outcome = recorder!.RecordArgumentSyntax(syntax);

        Assert.True(outcome);

        Assert.False(dataRecord.T1SyntaxRecorded);
        Assert.False(dataRecord.T2SyntaxRecorded);
        Assert.Equal(syntax, dataRecord.ValueASyntax);
        Assert.False(dataRecord.ValueBSyntaxRecorded);
    }

    [Fact]
    public void ValueB_RecorderMapsToValueB()
    {
        SyntacticData dataRecord = new();
        var syntax = ExpressionSyntaxFactory.Create();

        var recorder = Target(new Mapper(), dataRecord, Mapper.NameValueB);

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
        SyntacticData dataRecord = new();

        var recorder = Target(new Mapper(), dataRecord, Mapper.NameValueA);

        var exception = Record.Exception(() => recorder!.RecordArgumentSyntax(null!));

        Assert.IsType<ArgumentNullException>(exception);
    }
}