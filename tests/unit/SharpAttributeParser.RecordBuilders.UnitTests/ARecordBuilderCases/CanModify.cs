namespace SharpAttributeParser.Mappers.ARecordBuilderCases;

using Moq;

using Xunit;

public sealed class CanModify
{
    [Fact]
    public void Unbuilt_True()
    {
        RecordBuilder recordBuilder = new();

        var canModify = recordBuilder.InvokeCanModify();

        Assert.True(canModify);
    }

    [Fact]
    public void Built_False()
    {
        RecordBuilder recordBuilder = new();

        recordBuilder.Build();

        var canModify = recordBuilder.InvokeCanModify();

        Assert.False(canModify);
    }

    private sealed class RecordBuilder : ARecordBuilder<object>
    {
        public bool InvokeCanModify() => CanModify();

        protected override object GetRecord() => Mock.Of<object>();
    }
}
