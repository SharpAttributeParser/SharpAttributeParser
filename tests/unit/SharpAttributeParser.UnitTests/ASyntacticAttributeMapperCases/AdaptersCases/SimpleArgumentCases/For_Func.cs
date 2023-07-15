﻿namespace SharpAttributeParser.ASyntacticAttributeMapperCases.AdaptersCases.SimpleArgumentCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Xunit;

public sealed class For_Func
{
    [Fact]
    public void NullDelegate_ArgumentNullException()
    {
        var exception = Record.Exception(() => Mapper.Target(null!));

        Assert.IsType<ArgumentNullException>(exception);
    }
    [Fact]
    public void ValidRecorder_NullDataRecord_ArgumentNullExceptionWhenUsed()
    {
        var recorder = Mapper.Target(Data.TrueRecorder);

        var exception = Record.Exception(() => recorder(null!, ExpressionSyntaxFactory.Create()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidRecorder_NullSyntax_ArgumentExceptionWhenUsed()
    {
        var recorder = Mapper.Target(Data.TrueRecorder);

        var exception = Record.Exception(() => recorder(new Data(), (ExpressionSyntax)null!));

        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public void ValidRecorder_NullSyntaxCollection_FalseAndNotRecorded()
    {
        var syntax = OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(null!);

        FalseAndNotRecorded(syntax);
    }

    [Fact]
    public void Syntax_UsesRecorderAndReturnsTrue()
    {
        var syntax = ExpressionSyntaxFactory.Create();

        TrueAndRecorded(syntax);
    }

    [Fact]
    public void SyntaxCollection_FalseAndNotRecorded()
    {
        var syntax = OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(ExpressionSyntaxFactory.CreateCollection());

        FalseAndNotRecorded(syntax);
    }

    [Fact]
    public void FalseReturningRecorder_FalseAndRecorded()
    {
        var recorder = Mapper.Target(Data.FalseRecorder);
        Data data = new();

        var syntax = ExpressionSyntaxFactory.Create();

        var outcome = recorder(data, syntax);

        Assert.False(outcome);

        Assert.Equal(syntax, data.Syntax);
        Assert.True(data.SyntaxRecorded);
    }

    [AssertionMethod]
    private static void TrueAndRecorded(ExpressionSyntax syntax)
    {
        var recorder = Mapper.Target(Data.TrueRecorder);

        Data data = new();

        var outcome = recorder(data, syntax);

        Assert.True(outcome);

        Assert.Equal(syntax, data.Syntax);
        Assert.True(data.SyntaxRecorded);
    }

    [AssertionMethod]
    private static void FalseAndNotRecorded(OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
    {
        var recorder = Mapper.Target(Data.TrueRecorder);

        Data data = new();

        var outcome = recorder(data, syntax);

        Assert.False(outcome);

        Assert.False(data.SyntaxRecorded);
    }

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used to expose static member of ASyntacticAttributeMapper.")]
    private sealed class Mapper : ASyntacticAttributeMapper<Data>
    {
        public static Func<Data, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> Target(Func<Data, ExpressionSyntax, bool> recorder) => Adapters.SimpleArgument.For(recorder).Invoke;
    }

    private sealed class Data
    {
        public static Func<Data, ExpressionSyntax, bool> TrueRecorder => (dataRecord, syntax) =>
        {
            Recorder(dataRecord, syntax);

            return true;
        };

        public static Func<Data, ExpressionSyntax, bool> FalseRecorder => (dataRecord, syntax) =>
        {
            Recorder(dataRecord, syntax);

            return false;
        };

        private static void Recorder(Data dataRecord, ExpressionSyntax syntax)
        {
            dataRecord.Syntax = syntax;
            dataRecord.SyntaxRecorded = true;
        }

        public ExpressionSyntax? Syntax { get; set; }
        public bool SyntaxRecorded { get; set; }
    }
}
