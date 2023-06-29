namespace SharpAttributeParser.Tests.ASemanticArgumentRecorderCases;

using System;
using System.Collections.Generic;

using Xunit;

public class TryRecordNamedArgument_Array
{
    private static bool Target(ASemanticArgumentRecorder recorder, string parameterName, IReadOnlyList<object?>? value) => recorder.TryRecordNamedArgument(parameterName, value);

    [Fact]
    public void NullParameterName_ArgumentNullException()
    {
        SemanticArgumentRecorder recorder = new(StringComparer.OrdinalIgnoreCase);

        var parameterName = Datasets.GetNullParameterName();
        var value = Datasets.GetValidArrayArgument();

        var exception = Record.Exception(() => Target(recorder, parameterName, value));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullValue_True_ArgumentRecorded()
    {
        var comparerMock = ComparerMock.GetComparer(true);

        SemanticArgumentRecorder recorder = new(comparerMock.Object);

        var parameterName = Datasets.GetValidParameterName();
        var value = Datasets.GetNullArrayArgument();

        var actual = Target(recorder, parameterName, value);

        Assert.True(actual);

        Assert.Null(recorder.Values);
        Assert.True(recorder.ValuesRecorded);
    }

    [Fact]
    public void Matching_True_ArgumentRecorded()
    {
        var comparerMock = ComparerMock.GetComparer(true);

        SemanticArgumentRecorder recorder = new(comparerMock.Object);

        var parameterName = Datasets.GetValidParameterName();
        var value = Datasets.GetValidArrayArgument();

        var actual = Target(recorder, parameterName, value);

        Assert.True(actual);

        Assert.Equal(value, recorder.Values);
        Assert.True(recorder.ValuesRecorded);
    }

    [Fact]
    public void NotMatching_False_ArgumentNotRecorded()
    {
        var comparerMock = ComparerMock.GetComparer(false);

        SemanticArgumentRecorder recorder = new(comparerMock.Object);

        var parameterName = Datasets.GetValidParameterName();
        var value = Datasets.GetValidArrayArgument();

        var actual = Target(recorder, parameterName, value);

        Assert.False(actual);

        Assert.Null(recorder.Values);
        Assert.False(recorder.ValuesRecorded);
    }
}
