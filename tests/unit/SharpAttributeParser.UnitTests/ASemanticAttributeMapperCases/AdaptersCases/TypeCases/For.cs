namespace SharpAttributeParser.ASemanticAttributeMapperCases.AdaptersCases.TypeCases;

using Microsoft.CodeAnalysis;

using Moq;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Xunit;

[SuppressMessage("Naming", "CA1716: Identifiers should not match keywords", Justification = "Type should not be used.")]
public sealed class For
{
    [Fact]
    public void NullDelegate_ArgumentNullExceptionWhenUsed()
    {
        var exception = Record.Exception(() => Mapper.Target(null!));

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
    }

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used to expose static member of ASemanticAttributeMapper.")]
    private sealed class Mapper : ASemanticAttributeMapper<Data>
    {
        public static Func<Data, ITypeSymbol, bool> Target(Action<Data, ITypeSymbol> recorder) => Adapters.Type.For(recorder).Invoke;
    }

    private sealed class Data
    {
        public static Action<Data, ITypeSymbol> Recorder => (dataRecord, argument) => dataRecord.Value = argument;

        public ITypeSymbol? Value { get; set; }
    }
}
