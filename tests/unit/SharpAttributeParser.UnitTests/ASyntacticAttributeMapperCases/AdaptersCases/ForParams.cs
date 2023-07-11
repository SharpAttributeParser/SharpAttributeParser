namespace SharpAttributeParser.ASyntacticAttributeMapperCases.AdaptersCases;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Xunit;

public sealed class ForParams
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
    public void ValidRecorder_NullElementSyntax_ArgumentExceptionWhenUsed()
    {
        var recorder = Mapper.Target(Data.Recorder);

        var exception = Record.Exception(() => recorder(new Data(), OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(null!)));

        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public void Syntax_UsesRecorderAndReturnsTrue()
    {
        var syntax = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);

        TrueAndRecorded(syntax);
    }

    [Fact]
    public void ElementSyntax_UsesRecorderAndReturnsTrue()
    {
        var syntax = OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(new[] { SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression) });

        TrueAndRecorded(syntax);
    }

    [AssertionMethod]
    private static void TrueAndRecorded(OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
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
        public static Func<Data, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> Target(Action<Data, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> recorder) => Adapters.ForParams(recorder).Invoke;
    }

    private sealed class Data
    {
        public static Action<Data, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> Recorder => (dataRecord, syntax) =>
        {
            dataRecord.Syntax = syntax;
            dataRecord.SyntaxRecorded = true;
        };

        public OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> Syntax { get; set; }
        public bool SyntaxRecorded { get; set; }
    }
}
