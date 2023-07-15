namespace SharpAttributeParser.ASplitAttributeMapperCases.AdaptersCases.TypeArgumentCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using System;
using System.Diagnostics.CodeAnalysis;

using Xunit;

public sealed class For_Action
{
    [Fact]
    public void NullSemanticDelegate_ArgumentNullException()
    {
        var exception = Record.Exception(() => Mapper.Target(null!, SyntacticData.Recorder));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullSyntacticDelegate_ArgumentNullException()
    {
        var exception = Record.Exception(() => Mapper.Target(SemanticData.Recorder, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidSemanticRecorder_NullDataRecord_ArgumentNullExceptionWhenUsed()
    {
        var (recorder, _) = Mapper.Target(SemanticData.Recorder, SyntacticData.Recorder);

        var exception = Record.Exception(() => recorder(null!, Mock.Of<ITypeSymbol>()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidSemanticRecorder_NullArgument_ArgumentNullExceptionWhenUsed()
    {
        var (recorder, _) = Mapper.Target(SemanticData.Recorder, SyntacticData.Recorder);

        var exception = Record.Exception(() => recorder(new SemanticData(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidSyntacticRecorder_NullDataRecord_ArgumentNullExceptionWhenUsed()
    {
        var (_, recorder) = Mapper.Target(SemanticData.Recorder, SyntacticData.Recorder);

        var exception = Record.Exception(() => recorder(null!, ExpressionSyntaxFactory.Create()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidSyntacticRecorder_NullSyntax_ArgumentNullExceptionWhenUsed()
    {
        var (_, recorder) = Mapper.Target(SemanticData.Recorder, SyntacticData.Recorder);

        var exception = Record.Exception(() => recorder(new SyntacticData(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidRecorder_UsesRecorderAndReturnsTrue()
    {
        var (semanticRecorder, syntacticRecorder) = Mapper.Target(SemanticData.Recorder, SyntacticData.Recorder);

        SemanticData semanticData = new();
        SyntacticData syntacticData = new();

        var argument = Mock.Of<ITypeSymbol>();
        var syntax = ExpressionSyntaxFactory.Create();

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
        public static (Func<SemanticData, ITypeSymbol, bool>, Func<SyntacticData, ExpressionSyntax, bool>) Target(Action<SemanticData, ITypeSymbol> semanticRecorder, Action<SyntacticData, ExpressionSyntax> syntacticRecorder)
        {
            var recorders = Adapters.TypeArgument.For(semanticRecorder, syntacticRecorder);

            return (recorders.Semantic.Invoke, recorders.Syntactic.Invoke);
        }
    }

    private sealed class SemanticData
    {
        public static Action<SemanticData, ITypeSymbol> Recorder => (dataRecord, argument) =>
        {
            dataRecord.Value = argument;
            dataRecord.ValueRecorded = true;
        };

        public ITypeSymbol? Value { get; set; }
        public bool ValueRecorded { get; set; }
    }

    private sealed class SyntacticData
    {
        public static Action<SyntacticData, ExpressionSyntax> Recorder => (dataRecord, syntax) =>
        {
            dataRecord.ValueSyntax = syntax;
            dataRecord.ValueSyntaxRecorded = true;
        };

        public ExpressionSyntax? ValueSyntax { get; set; }
        public bool ValueSyntaxRecorded { get; set; }
    }
}
