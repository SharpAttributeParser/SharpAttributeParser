namespace SharpAttributeParser.ASplitAttributeMapperCases.AdaptersCases.TypeArgumentCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using System;
using System.Diagnostics.CodeAnalysis;

using Xunit;

public sealed class For_Func
{
    [Fact]
    public void NullSemanticDelegate_ArgumentNullException()
    {
        var exception = Record.Exception(() => Mapper.Target(null!, SyntacticData.TrueRecorder));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullSyntacticDelegate_ArgumentNullException()
    {
        var exception = Record.Exception(() => Mapper.Target(SemanticData.TrueRecorder, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidSemanticRecorder_NullDataRecord_ArgumentNullExceptionWhenUsed()
    {
        var (recorder, _) = Mapper.Target(SemanticData.TrueRecorder, SyntacticData.TrueRecorder);

        var exception = Record.Exception(() => recorder(null!, Mock.Of<ITypeSymbol>()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidSemanticRecorder_NullArgument_ArgumentNullExceptionWhenUsed()
    {
        var (recorder, _) = Mapper.Target(SemanticData.TrueRecorder, SyntacticData.TrueRecorder);

        var exception = Record.Exception(() => recorder(new SemanticData(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidSyntacticRecorder_NullDataRecord_ArgumentNullExceptionWhenUsed()
    {
        var (_, recorder) = Mapper.Target(SemanticData.TrueRecorder, SyntacticData.TrueRecorder);

        var exception = Record.Exception(() => recorder(null!, ExpressionSyntaxFactory.Create()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidSyntacticRecorder_NullSyntax_ArgumentNullExceptionWhenUsed()
    {
        var (_, recorder) = Mapper.Target(SemanticData.TrueRecorder, SyntacticData.TrueRecorder);

        var exception = Record.Exception(() => recorder(new SyntacticData(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidRecorder_UsesRecorderAndReturnsTrue()
    {
        var argument = Mock.Of<ITypeSymbol>();
        var syntax = ExpressionSyntaxFactory.Create();

        TrueAndRecorded(argument, syntax);
    }

    [Fact]
    public void FalseReturningRecorder_FalseAndRecorded()
    {
        var (semanticRecorder, syntacticRecorder) = Mapper.Target(SemanticData.FalseRecorder, SyntacticData.FalseRecorder);

        SemanticData semanticData = new();
        SyntacticData syntacticData = new();

        var argument = Mock.Of<ITypeSymbol>();
        var syntax = ExpressionSyntaxFactory.Create();

        var semanticOutcome = semanticRecorder(semanticData, argument);
        var syntacticOutcome = syntacticRecorder(syntacticData, syntax);

        Assert.False(semanticOutcome);
        Assert.False(syntacticOutcome);

        Assert.Equal(argument, semanticData.Value);
        Assert.True(semanticData.ValueRecorded);

        Assert.Equal(syntax, syntacticData.ValueSyntax);
        Assert.True(syntacticData.ValueSyntaxRecorded);
    }

    [AssertionMethod]
    private static void TrueAndRecorded(ITypeSymbol argument, ExpressionSyntax syntax)
    {
        var (semanticRecorder, syntacticRecorder) = Mapper.Target(SemanticData.TrueRecorder, SyntacticData.TrueRecorder);

        SemanticData semanticData = new();
        SyntacticData syntacticData = new();

        var semanticOutcome = semanticRecorder(semanticData, argument);
        var syntacticOutcome = syntacticRecorder(syntacticData, syntax);

        Assert.True(semanticOutcome);
        Assert.True(syntacticOutcome);

        Assert.Equal(argument, semanticData.Value);
        Assert.True(semanticData.ValueRecorded);

        Assert.Equal(syntax, syntacticData.ValueSyntax);
        Assert.True(syntacticData.ValueSyntaxRecorded);
    }

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used to expose static member of ASyntacticAttributeMapper.")]
    private sealed class Mapper : ASplitAttributeMapper<SemanticData, SyntacticData>
    {
        public static (Func<SemanticData, ITypeSymbol, bool>, Func<SyntacticData, ExpressionSyntax, bool>) Target(Func<SemanticData, ITypeSymbol, bool> semanticRecorder, Func<SyntacticData, ExpressionSyntax, bool> syntacticRecorder)
        {
            var recorders = Adapters.TypeArgument.For(semanticRecorder, syntacticRecorder);

            return (recorders.Semantic.Invoke, recorders.Syntactic.Invoke);
        }
    }

    private sealed class SemanticData
    {
        public static Func<SemanticData, ITypeSymbol, bool> TrueRecorder => (dataRecord, argument) =>
        {
            Recorder(dataRecord, argument);

            return true;
        };

        public static Func<SemanticData, ITypeSymbol, bool> FalseRecorder => (dataRecord, argument) =>
        {
            Recorder(dataRecord, argument);

            return false;
        };

        private static void Recorder(SemanticData dataRecord, ITypeSymbol argument)
        {
            dataRecord.Value = argument;
            dataRecord.ValueRecorded = true;
        }

        public ITypeSymbol? Value { get; set; }
        public bool ValueRecorded { get; set; }
    }

    private sealed class SyntacticData
    {
        public static Func<SyntacticData, ExpressionSyntax, bool> TrueRecorder => (dataRecord, syntax) =>
        {
            Recorder(dataRecord, syntax);

            return true;
        };

        public static Func<SyntacticData, ExpressionSyntax, bool> FalseRecorder => (dataRecord, syntax) =>
        {
            Recorder(dataRecord, syntax);

            return false;
        };

        private static void Recorder(SyntacticData dataRecord, ExpressionSyntax syntax)
        {
            dataRecord.ValueSyntax = syntax;
            dataRecord.ValueSyntaxRecorded = true;
        }

        public ExpressionSyntax? ValueSyntax { get; set; }
        public bool ValueSyntaxRecorded { get; set; }
    }
}
