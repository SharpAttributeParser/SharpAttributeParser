namespace SharpAttributeParser.ASemanticAttributeMapperCases.AdaptersCases.TypeArgumentCases;

using Microsoft.CodeAnalysis;

using Moq;

using System;
using System.Collections.Generic;
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

        var exception = Record.Exception(() => recorder(null!, Mock.Of<ITypeSymbol>()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidRecorder_NullArgument_ArgumentNullExceptionWhenUsed()
    {
        var recorder = Mapper.Target(Data.Recorder);

        var exception = Record.Exception(() => recorder(new Data(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidRecorder_UsesRecorderAndReturnsTrue()
    {
        var recorder = Mapper.Target(Data.Recorder);

        var data = new Data();
        var argument = Mock.Of<ITypeSymbol>();

        var outcome = recorder(data, argument);

        Assert.True(outcome);

        Assert.Equal(argument, data.Value, ReferenceEqualityComparer.Instance);
        Assert.True(data.ValueRecorded);
    }

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used to expose static member of ASemanticAttributeMapper.")]
    private sealed class Mapper : ASemanticAttributeMapper<Data>
    {
        public static Func<Data, ITypeSymbol, bool> Target(Action<Data, ITypeSymbol> recorder) => Adapters.TypeArgument.For(recorder).Invoke;
    }

    private sealed class Data
    {
        public static Action<Data, ITypeSymbol> Recorder => (dataRecord, argument) =>
        {
            dataRecord.Value = argument;
            dataRecord.ValueRecorded = true;
        };

        public ITypeSymbol? Value { get; set; }
        public bool ValueRecorded { get; set; }
    }
}
