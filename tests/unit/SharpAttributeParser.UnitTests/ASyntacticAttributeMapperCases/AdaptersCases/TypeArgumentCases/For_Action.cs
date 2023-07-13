namespace SharpAttributeParser.ASyntacticAttributeMapperCases.AdaptersCases.TypeArgumentCases;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Diagnostics.CodeAnalysis;

using Xunit;

public sealed class For_Action
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
        var recorder = Mapper.Target(Data.Recorder);

        var exception = Record.Exception(() => recorder(null!, SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidRecorder_NullSyntax_ArgumentNullExceptionWhenUsed()
    {
        var recorder = Mapper.Target(Data.Recorder);

        var exception = Record.Exception(() => recorder(new Data(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidRecorder_UsesRecorderAndReturnsTrue()
    {
        var syntax = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);

        TrueAndRecorded(syntax);
    }

    [AssertionMethod]
    private static void TrueAndRecorded(ExpressionSyntax syntax)
    {
        var recorder = Mapper.Target(Data.Recorder);

        var data = new Data();

        var outcome = recorder(data, syntax);

        Assert.True(outcome);

        Assert.Equal(syntax, data.Syntax);
        Assert.True(data.SyntaxRecorded);
    }

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used to expose static member of ASyntacticAttributeMapper.")]
    private sealed class Mapper : ASyntacticAttributeMapper<Data>
    {
        public static Func<Data, ExpressionSyntax, bool> Target(Action<Data, ExpressionSyntax> recorder) => Adapters.TypeArgument.For(recorder).Invoke;
    }

    private sealed class Data
    {
        public static Action<Data, ExpressionSyntax> Recorder => (dataRecord, syntax) =>
        {
            dataRecord.Syntax = syntax;
            dataRecord.SyntaxRecorded = true;
        };

        public ExpressionSyntax? Syntax { get; set; }
        public bool SyntaxRecorded { get; set; }
    }
}
