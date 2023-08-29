namespace SharpAttributeParser.Mappers.ARecordBuilderCases;

using Moq;

using System;

using Xunit;

public sealed class Build
{
    private static TRecord Target<TRecord>(ARecordBuilder<TRecord> recordBuilder) => recordBuilder.Build();

    [Fact]
    public void NullReturningGetTarget_InvalidOperationException()
    {
        RecordBuilder recordBuilder = new(null, false, true);

        var exception = Record.Exception(() => Target(recordBuilder));

        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void FalseReturningCheckFullyConstructed_InvalidOperationException()
    {
        RecordBuilder recordBuilder = new(Mock.Of<object>(), false, false);

        var exception = Record.Exception(() => Target(recordBuilder));

        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void MultipleInvokationsOfBuild_ThrowOnMultipleBuildsEnabled_InvalidOperationExceptionOnSecondInvokation()
    {
        RecordBuilder recordBuilder = new(Mock.Of<object>(), true, true);

        recordBuilder.Build();

        var exception = Record.Exception(() => Target(recordBuilder));

        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void MultipleInvokationsOfBuild_ThrowOnMultipleBuildsDisabled_ReturnsTargetEveryTime()
    {
        var buildTarget = Mock.Of<object>();

        RecordBuilder recordBuilder = new(buildTarget, false, true);

        var firstBuildResult = recordBuilder.Build();
        var secondBuildResult = recordBuilder.Build();

        Assert.Same(buildTarget, firstBuildResult);
        Assert.Same(buildTarget, secondBuildResult);
    }

    private sealed class RecordBuilder : ARecordBuilder<object>
    {
        private object? BuildTarget { get; }
        private bool CheckFullyConstructedReturnValue { get; }

        public RecordBuilder(object? buildTarget, bool throwOnMultipleBuilds, bool checkFullyConstructedReturnValue) : base(throwOnMultipleBuilds)
        {
            BuildTarget = buildTarget;
            CheckFullyConstructedReturnValue = checkFullyConstructedReturnValue;
        }

        protected override object GetRecord() => BuildTarget!;
        protected override bool CanBuildRecord() => CheckFullyConstructedReturnValue;
    }
}
