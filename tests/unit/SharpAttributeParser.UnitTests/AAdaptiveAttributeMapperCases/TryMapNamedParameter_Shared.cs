namespace SharpAttributeParser.AAdaptiveAttributeMapperCases;

using Moq;

using System;

using Xunit;

public sealed class TryMapNamedParameter_Shared
{
    private static IAttributeArgumentRecorder? Target<TData>(IAttributeMapper<TData> mapper, TData dataRecord, string parameterName) => mapper.TryMapNamedParameter(parameterName, dataRecord);

    [Fact]
    public void NullData_ArgumentNullException()
    {
        var exception = Record.Exception(() => Target(new Mapper(), null!, string.Empty));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullParameterName_ArgumentNullException()
    {
        var exception = Record.Exception(() => Target(new Mapper(), new SharedData(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NoMatchingParameter_ReturnsNull()
    {
        var recorder = Target(new Mapper(), new SharedData(), Mapper.NameNone);

        Assert.Null(recorder);
    }

    [Fact]
    public void ValueA_RecorderMapsToValueA()
    {
        SharedData dataRecord = new();
        var argument = Mock.Of<object>();
        var syntax = ExpressionSyntaxFactory.Create();

        var recorder = Target(new Mapper(), dataRecord, Mapper.NameValueA);

        var outcome = recorder!.RecordArgument(argument, syntax);

        Assert.True(outcome);

        Assert.False(dataRecord.T1Recorded);
        Assert.False(dataRecord.T2Recorded);
        Assert.Equal(argument, dataRecord.ValueA);
        Assert.Equal(syntax, dataRecord.ValueASyntax);
        Assert.False(dataRecord.ValueBRecorded);
    }

    [Fact]
    public void ValueB_RecorderMapsToValueB()
    {
        SharedData dataRecord = new();
        var argument = Mock.Of<object>();
        var syntax = ExpressionSyntaxFactory.Create();

        var recorder = Target(new Mapper(), dataRecord, Mapper.NameValueB);

        var outcome = recorder!.RecordArgument(argument, syntax);

        Assert.True(outcome);

        Assert.False(dataRecord.T1Recorded);
        Assert.False(dataRecord.T2Recorded);
        Assert.False(dataRecord.ValueARecorded);
        Assert.Equal(argument, dataRecord.ValueB);
        Assert.Equal(syntax, dataRecord.ValueBSyntax);
    }

    [Fact]
    public void NullSyntaxToMappedRecorder_ArgumentNullExceptionWhenUsed()
    {
        SharedData dataRecord = new();

        var recorder = Target(new Mapper(), dataRecord, Mapper.NameValueA);

        var exception = Record.Exception(() => recorder!.RecordArgument(null, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }
}
