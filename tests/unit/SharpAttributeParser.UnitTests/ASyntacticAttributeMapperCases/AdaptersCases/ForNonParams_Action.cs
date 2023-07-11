namespace SharpAttributeParser.ASyntacticAttributeMapperCases.AdaptersCases;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Xunit;

public sealed class ForNonParams_Action
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
    public void ValidRecorder_NullSyntax_ArgumentExceptionWhenUsed()
    {
        var recorder = Mapper.Target(Data.Recorder);

        var exception = Record.Exception(() => recorder(new Data(), (ExpressionSyntax)null!));

        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public void ValidRecorder_NullElementSyntax_FalseAndNotRecorded()
    {
        var syntax = OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(null!);

        FalseAndNotRecorded(syntax);
    }

    [Fact]
    public void Syntax_UsesRecorderAndReturnsTrue()
    {
        var syntax = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);

        TrueAndRecorded(syntax);
    }

    [Fact]
    public void ElementSyntax_FalseAndNotRecorded()
    {
        var syntax = OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(new[] { SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression) });

        FalseAndNotRecorded(syntax);
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

    [AssertionMethod]
    private static void FalseAndNotRecorded(OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
    {
        var recorder = Mapper.Target(Data.Recorder);

        var data = new Data();

        var outcome = recorder(data, syntax);

        Assert.False(outcome);

        Assert.False(data.SyntaxRecorded);
    }

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used to expose static member of ASyntacticAttributeMapper.")]
    private sealed class Mapper : ASyntacticAttributeMapper<Data>
    {
        public static Func<Data, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> Target(Action<Data, ExpressionSyntax> recorder) => Adapters.ForNonParams(recorder).Invoke;
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
