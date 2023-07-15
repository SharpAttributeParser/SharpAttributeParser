namespace SharpAttributeParser.ASyntacticAttributeMapperCases.AdaptersCases.ArrayArgumentCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Xunit;

public sealed class ForParams_Func
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
    public void ValidRecorder_NullSyntaxCollection_ArgumentExceptionWhenUsed()
    {
        var recorder = Mapper.Target(Data.TrueRecorder);

        var exception = Record.Exception(() => recorder(new Data(), OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(null!)));

        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public void Syntax_UsesRecorderAndReturnsTrue()
    {
        var syntax = ExpressionSyntaxFactory.Create();

        TrueAndRecorded(syntax);
    }

    [Fact]
    public void SyntaxCollection_UsesRecorderAndReturnsTrue()
    {
        var syntax = OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(ExpressionSyntaxFactory.CreateCollection());

        TrueAndRecorded(syntax);
    }

    [Fact]
    public void FalseReturningRecorder_FalseAndRecorded()
    {
        var recorder = Mapper.Target(Data.FalseRecorder);

        Data data = new();
        var syntax = OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(ExpressionSyntaxFactory.CreateCollection());

        var outcome = recorder(data, syntax);

        Assert.False(outcome);

        Assert.Equal(syntax, data.Syntax);
        Assert.True(data.SyntaxRecorded);
    }

    [AssertionMethod]
    private static void TrueAndRecorded(OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
    {
        var recorder = Mapper.Target(Data.TrueRecorder);

        Data data = new();

        var outcome = recorder(data, syntax);

        Assert.True(outcome);

        Assert.Equal(syntax, data.Syntax);
        Assert.True(data.SyntaxRecorded);
    }

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used to expose static member of ASyntacticAttributeMapper.")]
    private sealed class Mapper : ASyntacticAttributeMapper<Data>
    {
        public static Func<Data, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> Target(Func<Data, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> recorder) => Adapters.ArrayArgument.ForParams(recorder).Invoke;
    }

    private sealed class Data
    {
        public static Func<Data, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> TrueRecorder => (dataRecord, syntax) =>
        {
            Recorder(dataRecord, syntax);

            return true;
        };

        public static Func<Data, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> FalseRecorder => (dataRecord, syntax) =>
        {
            Recorder(dataRecord, syntax);

            return false;
        };

        private static void Recorder(Data dataRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
        {
            dataRecord.Syntax = syntax;
            dataRecord.SyntaxRecorded = true;
        }

        public OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> Syntax { get; set; }
        public bool SyntaxRecorded { get; set; }
    }
}
