namespace SharpAttributeParser.Tests.ASemanticArgumentRecorderCases;

using Microsoft.CodeAnalysis;

using System;

using Xunit;

public class TryRecordGenericArgument
{
    private static bool Target(ASemanticArgumentRecorder recorder, string parameterName, ITypeSymbol value) => recorder.TryRecordGenericArgument(parameterName, value);

    [Fact]
    public void NullParameterName_ArgumentNullException()
    {
        SemanticArgumentRecorder recorder = new(StringComparer.OrdinalIgnoreCase);

        var parameterName = Datasets.GetNullParameterName();
        var value = Datasets.GetValidTypeSymbol();

        var exception = Record.Exception(() => Target(recorder, parameterName, value));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullValue_ArgumentNullException()
    {
        SemanticArgumentRecorder recorder = new(StringComparer.OrdinalIgnoreCase);

        var parameterName = Datasets.GetValidParameterName();
        var value = Datasets.GetNullTypeSymbol();

        var exception = Record.Exception(() => Target(recorder, parameterName, value));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Matching_True_ArgumentRecorded()
    {
        var comparerMock = ComparerMock.GetComparer(true);

        SemanticArgumentRecorder recorder = new(comparerMock.Object);

        var parameterName = Datasets.GetValidParameterName();
        var value = Datasets.GetValidTypeSymbol();

        var actual = Target(recorder, parameterName, value);

        Assert.True(actual);

        Assert.Equal(value, recorder.TGeneric);
    }

    [Fact]
    public void NotMatching_False_ArgumentNotRecorded()
    {
        var comparerMock = ComparerMock.GetComparer(false);

        SemanticArgumentRecorder recorder = new(comparerMock.Object);

        var parameterName = Datasets.GetValidParameterName();
        var value = Datasets.GetValidTypeSymbol();

        var actual = Target(recorder, parameterName, value);

        Assert.False(actual);

        Assert.Null(recorder.TGeneric);
    }
}
