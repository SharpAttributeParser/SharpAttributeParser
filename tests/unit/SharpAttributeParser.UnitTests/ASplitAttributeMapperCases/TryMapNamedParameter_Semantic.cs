namespace SharpAttributeParser.ASplitAttributeMapperCases;

using Moq;

using System;

using Xunit;

public sealed class TryMapNamedParameter_Semantic
{
    private static ISemanticAttributeArgumentRecorder? Target<TData>(ISemanticAttributeMapper<TData> mapper, TData dataRecord, string parameterName) => mapper.TryMapNamedParameter(parameterName, dataRecord);

    [Fact]
    public void NullData_ArgumentNullException()
    {
        var exception = Record.Exception(() => Target(new Mapper(), null!, string.Empty));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullParameterName_ArgumentNullException()
    {
        var exception = Record.Exception(() => Target(new Mapper(), new SemanticData(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NoMatchingParameter_ReturnsNull()
    {
        var recorder = Target(new Mapper(), new SemanticData(), Mapper.NameNone);

        Assert.Null(recorder);
    }

    [Fact]
    public void ValueA_RecorderMapsToValueA()
    {
        SemanticData dataRecord = new();
        var argument = Mock.Of<object>();

        var recorder = Target(new Mapper(), dataRecord, Mapper.NameValueA);

        var outcome = recorder!.RecordArgument(argument);

        Assert.True(outcome);

        Assert.False(dataRecord.T1Recorded);
        Assert.False(dataRecord.T2Recorded);
        Assert.Equal(argument, dataRecord.ValueA);
        Assert.False(dataRecord.ValueBRecorded);
    }

    [Fact]
    public void ValueB_RecorderMapsToValueB()
    {
        SemanticData dataRecord = new();
        var argument = Mock.Of<object>();

        var recorder = Target(new Mapper(), dataRecord, Mapper.NameValueB);

        var outcome = recorder!.RecordArgument(argument);

        Assert.True(outcome);

        Assert.False(dataRecord.T1Recorded);
        Assert.False(dataRecord.T2Recorded);
        Assert.False(dataRecord.ValueARecorded);
        Assert.Equal(argument, dataRecord.ValueB);
    }
}
