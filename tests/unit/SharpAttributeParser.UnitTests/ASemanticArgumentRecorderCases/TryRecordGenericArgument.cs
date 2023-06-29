namespace SharpAttributeParser.Tests.ASemanticArgumentRecorderCases;

using Microsoft.CodeAnalysis;

using System;

using Xunit;

public class TryRecordGenericArgument
{
    private static bool Target(ASemanticArgumentRecorder recorder, ITypeParameterSymbol parameter, ITypeSymbol value) => recorder.TryRecordGenericArgument(parameter, value);

    [Fact]
    public void NullParameter_ArgumentNullException()
    {
        SemanticArgumentRecorder recorder = new(StringComparer.OrdinalIgnoreCase);

        var parameter = Datasets.GetNullTypeParameter();
        var value = Datasets.GetValidTypeSymbol();

        var exception = Record.Exception(() => Target(recorder, parameter, value));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullValue_ArgumentNullException()
    {
        SemanticArgumentRecorder recorder = new(StringComparer.OrdinalIgnoreCase);

        var parameter = Datasets.GetMockedTypeParameter();
        var value = Datasets.GetNullTypeSymbol();

        var exception = Record.Exception(() => Target(recorder, parameter, value));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Matching_True_ArgumentRecorded()
    {
        var comparerMock = ComparerMock.GetComparer(true);

        SemanticArgumentRecorder recorder = new(comparerMock.Object);

        var parameter = Datasets.GetMockedTypeParameter();
        var value = Datasets.GetValidTypeSymbol();

        var actual = Target(recorder, parameter, value);

        Assert.True(actual);

        Assert.Equal(value, recorder.TGeneric);
    }

    [Fact]
    public void NotMatching_False_ArgumentNotRecorded()
    {
        var comparerMock = ComparerMock.GetComparer(false);

        SemanticArgumentRecorder recorder = new(comparerMock.Object);

        var parameter = Datasets.GetMockedTypeParameter();
        var value = Datasets.GetValidTypeSymbol();

        var actual = Target(recorder, parameter, value);

        Assert.False(actual);

        Assert.Null(recorder.TGeneric);
    }
}
