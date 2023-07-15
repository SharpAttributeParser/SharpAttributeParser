namespace SharpAttributeParser.AAdaptiveAttributeMapperCases.AdaptersCases.TypeArgumentCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using System;
using System.Diagnostics.CodeAnalysis;

using Xunit;

public sealed class For_Func
{
    [Fact]
    public void NullSharedDelegate_ArgumentNullException()
    {
        var exception = Record.Exception(() => Mapper.Target(null!, Data.TrueSemanticRecorder));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullSemanticDelegate_ArgumentNullException()
    {
        var exception = Record.Exception(() => Mapper.Target(Data.TrueSharedRecorder, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidSharedRecorder_NullDataRecord_ArgumentNullExceptionWhenUsed()
    {
        var (recorder, _) = Mapper.Target(Data.TrueSharedRecorder, Data.TrueSemanticRecorder);

        var exception = Record.Exception(() => recorder(null!, Mock.Of<ITypeSymbol>(), ExpressionSyntaxFactory.Create()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidSharedRecorder_NullArgument_ArgumentNullExceptionWhenUsed()
    {
        var (recorder, _) = Mapper.Target(Data.TrueSharedRecorder, Data.TrueSemanticRecorder);

        var exception = Record.Exception(() => recorder(new Data(), null!, ExpressionSyntaxFactory.Create()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidSharedRecorder_NullSyntax_ArgumentNullExceptionWhenUsed()
    {
        var (recorder, _) = Mapper.Target(Data.TrueSharedRecorder, Data.TrueSemanticRecorder);

        var exception = Record.Exception(() => recorder(new Data(), Mock.Of<ITypeSymbol>(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidSemanticRecorder_NullDataRecord_ArgumentNullExceptionWhenUsed()
    {
        var (_, recorder) = Mapper.Target(Data.TrueSharedRecorder, Data.TrueSemanticRecorder);

        var exception = Record.Exception(() => recorder(null!, Mock.Of<ITypeSymbol>()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidSemanticRecorder_NullArgument_ArgumentNullExceptionWhenUsed()
    {
        var (_, recorder) = Mapper.Target(Data.TrueSharedRecorder, Data.TrueSemanticRecorder);

        var exception = Record.Exception(() => recorder(new Data(), null!));

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
        var (sharedRecorder, semanticRecorder) = Mapper.Target(Data.FalseSharedRecorder, Data.FalseSemanticRecorder);

        Data sharedData = new();
        Data semanticData = new();

        var argument = Mock.Of<ITypeSymbol>();
        var syntax = ExpressionSyntaxFactory.Create();

        var sharedOutcome = sharedRecorder(sharedData, argument, syntax);
        var semanticOutcome = semanticRecorder(semanticData, argument);

        Assert.False(sharedOutcome);
        Assert.False(semanticOutcome);

        Assert.Equal(argument, sharedData.Value);
        Assert.Equal(syntax, sharedData.ValueSyntax);
        Assert.True(sharedData.ValueRecorded);

        Assert.Equal(argument, semanticData.Value);
        Assert.True(semanticData.ValueRecorded);
    }

    [AssertionMethod]
    private static void TrueAndRecorded(ITypeSymbol argument, ExpressionSyntax syntax)
    {
        var (sharedRecorder, semanticRecorder) = Mapper.Target(Data.TrueSharedRecorder, Data.TrueSemanticRecorder);

        Data sharedData = new();
        Data semanticData = new();

        var sharedOutcome = sharedRecorder(sharedData, argument, syntax);
        var semanticOutcome = semanticRecorder(semanticData, argument);

        Assert.True(sharedOutcome);
        Assert.True(semanticOutcome);

        Assert.Equal(argument, sharedData.Value);
        Assert.Equal(syntax, sharedData.ValueSyntax);
        Assert.True(sharedData.ValueRecorded);

        Assert.Equal(argument, semanticData.Value);
        Assert.True(semanticData.ValueRecorded);
    }

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used to expose static member of ASyntacticAttributeMapper.")]
    private sealed class Mapper : AAdaptiveAttributeMapper<Data, Data>
    {
        public static (Func<Data, ITypeSymbol, ExpressionSyntax, bool>, Func<Data, ITypeSymbol, bool>) Target(Func<Data, ITypeSymbol, ExpressionSyntax, bool> sharedRecorder, Func<Data, ITypeSymbol, bool> semanticRecorder)
        {
            var recorders = Adapters.TypeArgument.For(sharedRecorder, semanticRecorder);

            return (recorders.Shared.Invoke, recorders.Semantic.Invoke);
        }
    }

    private sealed class Data
    {
        public static Func<Data, ITypeSymbol, ExpressionSyntax, bool> TrueSharedRecorder => (dataRecord, argument, syntax) =>
        {
            Recorder(dataRecord, argument, syntax);

            return true;
        };

        public static Func<Data, ITypeSymbol, ExpressionSyntax, bool> FalseSharedRecorder => (dataRecord, argument, syntax) =>
        {
            Recorder(dataRecord, argument, syntax);

            return false;
        };

        public static Func<Data, ITypeSymbol, bool> TrueSemanticRecorder => (dataRecord, argument) =>
        {
            Recorder(dataRecord, argument);

            return true;
        };

        public static Func<Data, ITypeSymbol, bool> FalseSemanticRecorder => (dataRecord, argument) =>
        {
            Recorder(dataRecord, argument);

            return false;
        };

        private static void Recorder(Data dataRecord, ITypeSymbol argument, ExpressionSyntax syntax)
        {
            dataRecord.Value = argument;
            dataRecord.ValueSyntax = syntax;
            dataRecord.ValueRecorded = true;
        }

        private static void Recorder(Data dataRecord, ITypeSymbol argument)
        {
            dataRecord.Value = argument;
            dataRecord.ValueRecorded = true;
        }

        public ITypeSymbol? Value { get; set; }
        public ExpressionSyntax? ValueSyntax { get; set; }
        public bool ValueRecorded { get; set; }
    }
}
