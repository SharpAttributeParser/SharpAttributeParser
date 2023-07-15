namespace SharpAttributeParser.ASemanticAttributeMapperCases.AdaptersCases.TypeArgumentCases;

using Microsoft.CodeAnalysis;

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

        var exception = Record.Exception(() => recorder(null!, Mock.Of<ITypeSymbol>()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidRecorder_NullArgument_ArgumentNullExceptionWhenUsed()
    {
        var recorder = Mapper.Target(Data.TrueRecorder);

        var exception = Record.Exception(() => recorder(new Data(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidRecorder_UsesRecorderAndReturnsTrue()
    {
        var recorder = Mapper.Target(Data.TrueRecorder);

        Data data = new();
        var argument = Mock.Of<ITypeSymbol>();

        var outcome = recorder(data, argument);

        Assert.True(outcome);

        Assert.Equal(argument, data.Value);
        Assert.True(data.ValueRecorded);
    }

    [Fact]
    public void FalseReturningRecorder_FalseAndRecorded()
    {
        var recorder = Mapper.Target(Data.FalseRecorder);

        Data data = new();
        var argument = Mock.Of<ITypeSymbol>();

        var outcome = recorder(data, argument);

        Assert.False(outcome);

        Assert.Equal(argument, data.Value);
        Assert.True(data.ValueRecorded);
    }

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used to expose static member of ASemanticAttributeMapper.")]
    private sealed class Mapper : ASemanticAttributeMapper<Data>
    {
        public static Func<Data, ITypeSymbol, bool> Target(Func<Data, ITypeSymbol, bool> recorder) => Adapters.TypeArgument.For(recorder).Invoke;
    }

    private sealed class Data
    {
        public static Func<Data, ITypeSymbol, bool> TrueRecorder => (dataRecord, argument) =>
        {
            Recorder(dataRecord, argument);

            return true;
        };

        public static Func<Data, ITypeSymbol, bool> FalseRecorder => (dataRecord, argument) =>
        {
            Recorder(dataRecord, argument);

            return false;
        };

        private static void Recorder(Data dataRecord, ITypeSymbol argument)
        {
            dataRecord.Value = argument;
            dataRecord.ValueRecorded = true;
        }

        public ITypeSymbol? Value { get; set; }
        public bool ValueRecorded { get; set; }
    }
}
