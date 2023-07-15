namespace SharpAttributeParser.AAdaptiveAttributeMapperCases;

using Microsoft.CodeAnalysis;

using Moq;

using System;

using Xunit;

public sealed class TryMapTypeParameter_Semantic
{
    private static ISemanticAttributeArgumentRecorder? Target<TData>(ISemanticAttributeMapper<TData> mapper, TData dataRecord, ITypeParameterSymbol parameter) => mapper.TryMapTypeParameter(parameter, dataRecord);

    [Fact]
    public void NullData_ArgumentNullException()
    {
        var exception = Record.Exception(() => Target(new Mapper(), null!, Mock.Of<ITypeParameterSymbol>()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullParameter_ArgumentNullException()
    {
        var exception = Record.Exception(() => Target(new Mapper(), new SemanticData(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NoMatchingParameter_ReturnsNull()
    {
        var recorder = Target(new Mapper(), new SemanticData(), Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == Mapper.TypeNameNone));

        Assert.Null(recorder);
    }

    [Fact]
    public void NullArgumentToRecorder_ReturnsFalse()
    {
        SemanticData dataRecord = new();

        var recorder = Target(new Mapper(), dataRecord, Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == Mapper.TypeNameT1));

        var outcome = recorder!.RecordArgument(null);

        Assert.False(outcome);

        IdenticalToExpected(new SemanticData(), dataRecord);
    }

    [Fact]
    public void NonITypeSymbolArgumentToRecorder_ReturnsFalse()
    {
        SemanticData dataRecord = new();

        var recorder = Target(new Mapper(), dataRecord, Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == Mapper.TypeNameT1));

        var outcome = recorder!.RecordArgument(string.Empty);

        Assert.False(outcome);

        IdenticalToExpected(new SemanticData(), dataRecord);
    }

    [Fact]
    public void T1ByIndex_RecorderMapsToT1()
    {
        SemanticData dataRecord = new();
        var argument = Mock.Of<ITypeSymbol>();

        var recorder = Target(new Mapper(), dataRecord, Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == Mapper.TypeNameNone && symbol.Ordinal == Mapper.IndexT1));

        var outcome = recorder!.RecordArgument(argument);

        Assert.True(outcome);

        IdenticalToExpected(new SemanticData() { T1 = argument, T1Recorded = true }, dataRecord);
    }

    [Fact]
    public void T1ByName_RecorderMapsToT1()
    {
        SemanticData dataRecord = new();
        var argument = Mock.Of<ITypeSymbol>();

        var recorder = Target(new Mapper(), dataRecord, Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == Mapper.TypeNameT1));

        var outcome = recorder!.RecordArgument(argument);

        Assert.True(outcome);

        IdenticalToExpected(new SemanticData() { T1 = argument, T1Recorded = true }, dataRecord);
    }

    [Fact]
    public void T1ByIndexAndName_RecorderMapsToT1()
    {
        SemanticData dataRecord = new();
        var argument = Mock.Of<ITypeSymbol>();

        var recorder = Target(new Mapper(), dataRecord, Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == Mapper.TypeNameT1 && symbol.Ordinal == Mapper.IndexT1));

        var outcome = recorder!.RecordArgument(argument);

        Assert.True(outcome);

        IdenticalToExpected(new SemanticData() { T1 = argument, T1Recorded = true }, dataRecord);
    }

    [Fact]
    public void T2ByIndex_RecorderMapsToT2()
    {
        SemanticData dataRecord = new();
        var argument = Mock.Of<ITypeSymbol>();

        var recorder = Target(new Mapper(), dataRecord, Mock.Of<ITypeParameterSymbol>((symol) => symol.Name == Mapper.TypeNameNone && symol.Ordinal == Mapper.IndexT2));

        var outcome = recorder!.RecordArgument(argument);

        Assert.True(outcome);

        IdenticalToExpected(new SemanticData() { T2 = argument, T2Recorded = true }, dataRecord);
    }

    [Fact]
    public void T2ByName_RecorderMapsToT2()
    {
        SemanticData dataRecord = new();
        var argument = Mock.Of<ITypeSymbol>();

        var recorder = Target(new Mapper(), dataRecord, Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == Mapper.TypeNameT2));

        var outcome = recorder!.RecordArgument(argument);

        Assert.True(outcome);

        IdenticalToExpected(new SemanticData() { T2 = argument, T2Recorded = true }, dataRecord);
    }

    [Fact]
    public void T1ByNameAndT2ByIndex_ReturnsNull()
    {
        var recorder = Target(new Mapper(), new SemanticData(), Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == Mapper.TypeNameT1 && symbol.Ordinal == Mapper.IndexT2));

        Assert.Null(recorder);
    }

    [AssertionMethod]
    private static void IdenticalToExpected(SemanticData expected, SemanticData actual)
    {
        Assert.Equal(expected.T1, actual.T1);
        Assert.Equal(expected.T2, actual.T2);

        Assert.Equal(expected.T1Recorded, actual.T1Recorded);
        Assert.Equal(expected.T2Recorded, actual.T2Recorded);
        Assert.False(actual.ValueARecorded);
        Assert.False(actual.ValueBRecorded);
    }
}
