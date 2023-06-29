namespace SharpAttributeParser.Tests.ASemanticArgumentRecorderCases;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;

using Xunit;

public class TryRecordConstructorArgument_Array
{
    private static bool Target(ASemanticArgumentRecorder recorder, IParameterSymbol parameter, IReadOnlyList<object?>? value) => recorder.TryRecordConstructorArgument(parameter, value);

    [Fact]
    public void NullParameterName_ArgumentNullException()
    {
        SemanticArgumentRecorder recorder = new(StringComparer.OrdinalIgnoreCase);

        var parameter = Datasets.GetNullParameter();
        var value = Datasets.GetValidArrayArgument();

        var exception = Record.Exception(() => Target(recorder, parameter, value));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullValue_True_ArgumentRecorded()
    {
        var comparerMock = ComparerMock.GetComparer(true);

        SemanticArgumentRecorder recorder = new(comparerMock.Object);

        var parameter = Datasets.GetMockedParameter();
        var value = Datasets.GetNullArrayArgument();

        var actual = Target(recorder, parameter, value);

        Assert.True(actual);

        Assert.Null(recorder.Values);
        Assert.True(recorder.ValuesRecorded);
    }

    [Fact]
    public void Matching_True_ArgumentRecorded()
    {
        var comparerMock = ComparerMock.GetComparer(true);

        SemanticArgumentRecorder recorder = new(comparerMock.Object);

        var parameter = Datasets.GetMockedParameter();
        var value = Datasets.GetValidArrayArgument();

        var actual = Target(recorder, parameter, value);

        Assert.True(actual);

        Assert.Equal(value, recorder.Values);
        Assert.True(recorder.ValuesRecorded);
    }

    [Fact]
    public void NotMatching_False_ArgumentNotRecorded()
    {
        var comparerMock = ComparerMock.GetComparer(false);

        SemanticArgumentRecorder recorder = new(comparerMock.Object);

        var parameter = Datasets.GetMockedParameter();
        var value = Datasets.GetValidArrayArgument();

        var actual = Target(recorder, parameter, value);

        Assert.False(actual);

        Assert.Null(recorder.Values);
        Assert.False(recorder.ValuesRecorded);
    }
}
