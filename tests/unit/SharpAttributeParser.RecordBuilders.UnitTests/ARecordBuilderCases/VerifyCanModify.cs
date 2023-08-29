namespace SharpAttributeParser.Mappers.ARecordBuilderCases;

using Moq;

using System;

using Xunit;

public sealed class VerifyCanModify
{
    [Fact]
    public void Unbuilt_NoException()
    {
        RecordBuilder recordBuilder = new();

        var exception = Record.Exception(recordBuilder.InvokeVerifyCanModify);

        Assert.Null(exception);
    }

    [Fact]
    public void Built_InvalidOperationException()
    {
        RecordBuilder recordBuilder = new();

        recordBuilder.Build();

        var exception = Record.Exception(recordBuilder.InvokeVerifyCanModify);

        Assert.IsType<InvalidOperationException>(exception);
    }

    private sealed class RecordBuilder : ARecordBuilder<object>
    {
        public void InvokeVerifyCanModify() => VerifyCanModify();

        protected override object GetRecord() => Mock.Of<object>();
    }
}
