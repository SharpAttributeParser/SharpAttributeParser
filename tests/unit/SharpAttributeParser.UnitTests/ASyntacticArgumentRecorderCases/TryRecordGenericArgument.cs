namespace SharpAttributeParser.ASyntacticArgumentRecorderCases;

using Microsoft.CodeAnalysis;

using Moq;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class TryRecordGenericArgument
{
    private static bool Target(ISyntacticArgumentRecorder recorder, ITypeParameterSymbol parameter, ITypeSymbol value, Location location) => recorder.TryRecordGenericArgument(parameter, value, location);

    [Fact]
    public void NullParameter_ArgumentNullException()
    {
        SyntacticArgumentRecorder recorder = new(StringComparer.OrdinalIgnoreCase);

        var exception = Record.Exception(() => Target(recorder, null!, Mock.Of<ITypeSymbol>(), Location.None));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullValue_ArgumentNullException()
    {
        SyntacticArgumentRecorder recorder = new(StringComparer.OrdinalIgnoreCase);

        var exception = Record.Exception(() => Target(recorder, Mock.Of<ITypeParameterSymbol>(), null!, Location.None));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullLocation_ArgumentNullException()
    {
        SyntacticArgumentRecorder recorder = new(StringComparer.OrdinalIgnoreCase);

        var exception = Record.Exception(() => Target(recorder, Mock.Of<ITypeParameterSymbol>(), Mock.Of<ITypeSymbol>(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void MatchingName_True_Recorded()
    {
        var parameter = Mock.Of<ITypeParameterSymbol>(static (symbol) => symbol.Name == string.Empty && symbol.Ordinal == -1);

        TrueAndRecorded(StringComparerMock.CreateComparer(true), parameter);
    }

    [Fact]
    public void MatchingIndex_True_Recorded()
    {
        var parameter = Mock.Of<ITypeParameterSymbol>(static (symbol) => symbol.Name == string.Empty && symbol.Ordinal == 0);

        TrueAndRecorded(StringComparerMock.CreateComparer(false), parameter);
    }

    [Fact]
    public void MatchingNameAndIndexSameRecorder_True_Recorded()
    {
        var parameter = Mock.Of<ITypeParameterSymbol>(static (symbol) => symbol.Name == string.Empty && symbol.Ordinal == 0);

        TrueAndRecorded(StringComparerMock.CreateComparer(true), parameter);
    }

    [Fact]
    public void MatchingNameAndIndexDifferentRecorder_False_NotRecorded()
    {
        MultipleMatchingGenericRecorder recorder = new();

        var parameter = Mock.Of<ITypeParameterSymbol>(static (symbol) => symbol.Name == string.Empty && symbol.Ordinal == 0);

        var value = Mock.Of<ITypeSymbol>();

        var actual = Target(recorder, parameter, value, Location.None);

        Assert.False(actual);

        Assert.False(recorder.TRecorded);
    }

    [Fact]
    public void NotMatching_False_NotRecorded()
    {
        SyntacticArgumentRecorder recorder = new(StringComparerMock.CreateComparer(false));

        var parameter = Mock.Of<ITypeParameterSymbol>(static (symbol) => symbol.Name == string.Empty && symbol.Ordinal == -1);

        var actual = Target(recorder, parameter, Mock.Of<ITypeSymbol>(), Location.None);

        Assert.False(actual);

        Assert.False(recorder.TRecorded);
    }

    [AssertionMethod]
    private static void TrueAndRecorded(IEqualityComparer<string> comparer, ITypeParameterSymbol parameter)
    {
        SyntacticArgumentRecorder recorder = new(comparer);

        var value = Mock.Of<ITypeSymbol>();

        var location = CustomLocation.Create();

        var actual = Target(recorder, parameter, value, location);

        Assert.True(actual);

        Assert.Equal(value, recorder.T, ReferenceEqualityComparer.Instance);
        Assert.True(recorder.TRecorded);
        Assert.Equal(location, recorder.TLocation, ReferenceEqualityComparer.Instance);
    }
}
