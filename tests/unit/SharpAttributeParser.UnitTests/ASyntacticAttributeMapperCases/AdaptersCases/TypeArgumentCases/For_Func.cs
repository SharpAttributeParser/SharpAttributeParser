namespace SharpAttributeParser.ASyntacticAttributeMapperCases.AdaptersCases.TypeArgumentCases;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
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

        var exception = Record.Exception(() => recorder(null!, SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidRecorder_NullSyntax_ArgumentNullExceptionWhenUsed()
    {
        var recorder = Mapper.Target(Data.TrueRecorder);

        var exception = Record.Exception(() => recorder(new Data(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidRecorder_UsesRecorderAndReturnsTrue()
    {
        var syntax = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);

        TrueAndRecorded(syntax);
    }

    [Fact]
    public void FalseReturningRecorder_FalseAndRecorded()
    {
        var recorder = Mapper.Target(Data.FalseRecorder);

        var data = new Data();
        var syntax = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);

        var outcome = recorder(data, syntax);

        Assert.False(outcome);

        Assert.Equal(syntax, data.Syntax);
        Assert.True(data.SyntaxRecorded);
    }

    [AssertionMethod]
    private static void TrueAndRecorded(ExpressionSyntax syntax)
    {
        var recorder = Mapper.Target(Data.TrueRecorder);

        var data = new Data();

        var outcome = recorder(data, syntax);

        Assert.True(outcome);

        Assert.Equal(syntax, data.Syntax);
        Assert.True(data.SyntaxRecorded);
    }

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used to expose static member of ASyntacticAttributeMapper.")]
    private sealed class Mapper : ASyntacticAttributeMapper<Data>
    {
        public static Func<Data, ExpressionSyntax, bool> Target(Func<Data, ExpressionSyntax, bool> recorder) => Adapters.TypeArgument.For(recorder).Invoke;
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
