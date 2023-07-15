namespace SharpAttributeParser.AAttributeMapperCases.AdaptersCases.TypeArgumentCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

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

        var exception = Record.Exception(() => recorder(null!, Mock.Of<ITypeSymbol>(), ExpressionSyntaxFactory.Create()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidRecorder_NullArgument_ArgumentNullExceptionWhenUsed()
    {
        var recorder = Mapper.Target(Data.TrueRecorder);

        var exception = Record.Exception(() => recorder(new Data(), null!, ExpressionSyntaxFactory.Create()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidRecorder_NullSyntax_ArgumentNullExceptionWhenUsed()
    {
        var recorder = Mapper.Target(Data.TrueRecorder);

        var exception = Record.Exception(() => recorder(new Data(), Mock.Of<ITypeSymbol>(), null!));

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
        var recorder = Mapper.Target(Data.FalseRecorder);

        Data data = new();
        var argument = Mock.Of<ITypeSymbol>();
        var syntax = ExpressionSyntaxFactory.Create();

        var outcome = recorder(data, argument, syntax);

        Assert.False(outcome);

        Assert.Equal(argument, data.Value);
        Assert.Equal(syntax, data.Syntax);
        Assert.True(data.ValueRecorded);
    }

    [AssertionMethod]
    private static void TrueAndRecorded(ITypeSymbol argument, ExpressionSyntax syntax)
    {
        var recorder = Mapper.Target(Data.TrueRecorder);

        Data data = new();

        var outcome = recorder(data, argument, syntax);

        Assert.True(outcome);

        Assert.Equal(argument, data.Value);
        Assert.Equal(syntax, data.Syntax);
        Assert.True(data.ValueRecorded);
    }

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used to expose static member of ASyntacticAttributeMapper.")]
    private sealed class Mapper : AAttributeMapper<Data>
    {
        public static Func<Data, ITypeSymbol, ExpressionSyntax, bool> Target(Func<Data, ITypeSymbol, ExpressionSyntax, bool> recorder) => Adapters.TypeArgument.For(recorder).Invoke;
    }

    private sealed class Data
    {
        public static Func<Data, ITypeSymbol, ExpressionSyntax, bool> TrueRecorder => (dataRecord, argument, syntax) =>
        {
            Recorder(dataRecord, argument, syntax);

            return true;
        };

        public static Func<Data, ITypeSymbol, ExpressionSyntax, bool> FalseRecorder => (dataRecord, argument, syntax) =>
        {
            Recorder(dataRecord, argument, syntax);

            return false;
        };

        private static void Recorder(Data dataRecord, ITypeSymbol argument, ExpressionSyntax syntax)
        {
            dataRecord.Value = argument;
            dataRecord.Syntax = syntax;
            dataRecord.ValueRecorded = true;
        }

        public ITypeSymbol? Value { get; set; }
        public ExpressionSyntax? Syntax { get; set; }
        public bool ValueRecorded { get; set; }
    }
}
