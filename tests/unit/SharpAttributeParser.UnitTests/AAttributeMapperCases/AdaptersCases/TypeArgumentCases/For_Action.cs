namespace SharpAttributeParser.AAttributeMapperCases.AdaptersCases.TypeArgumentCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

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

        var exception = Record.Exception(() => recorder(null!, Mock.Of<ITypeSymbol>(), ExpressionSyntaxFactory.Create()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidRecorder_NullArgument_ArgumentNullExceptionWhenUsed()
    {
        var recorder = Mapper.Target(Data.Recorder);

        var exception = Record.Exception(() => recorder(new Data(), null!, ExpressionSyntaxFactory.Create()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidRecorder_NullSyntax_ArgumentNullExceptionWhenUsed()
    {
        var recorder = Mapper.Target(Data.Recorder);

        var exception = Record.Exception(() => recorder(new Data(), Mock.Of<ITypeSymbol>(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidRecorder_UsesRecorderAndReturnsTrue()
    {
        var recorder = Mapper.Target(Data.Recorder);

        Data data = new();
        var argument = Mock.Of<ITypeSymbol>();
        var syntax = ExpressionSyntaxFactory.Create();

        var outcome = recorder(data, argument, syntax);

        Assert.True(outcome);

        Assert.Equal(argument, data.Value);
        Assert.Equal(syntax, data.ValueSyntax);
        Assert.True(data.ValueRecorded);
    }

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used to expose static member of ASemanticAttributeMapper.")]
    private sealed class Mapper : AAttributeMapper<Data>
    {
        public static Func<Data, ITypeSymbol, ExpressionSyntax, bool> Target(Action<Data, ITypeSymbol, ExpressionSyntax> recorder) => Adapters.TypeArgument.For(recorder).Invoke;
    }

    private sealed class Data
    {
        public static Action<Data, ITypeSymbol, ExpressionSyntax> Recorder => (dataRecord, argument, syntax) =>
        {
            dataRecord.Value = argument;
            dataRecord.ValueSyntax = syntax;
            dataRecord.ValueRecorded = true;
        };

        public ITypeSymbol? Value { get; set; }
        public ExpressionSyntax? ValueSyntax { get; set; }
        public bool ValueRecorded { get; set; }
    }
}
