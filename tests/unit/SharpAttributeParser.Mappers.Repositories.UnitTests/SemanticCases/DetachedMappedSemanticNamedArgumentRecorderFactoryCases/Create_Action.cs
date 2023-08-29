namespace SharpAttributeParser.Mappers.Repositories.SemanticCases.DetachedMappedSemanticNamedArgumentRecorderFactoryCases;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Semantic;

using System;

using Xunit;

public sealed class Create_Action
{
    private static IDetachedMappedSemanticNamedArgumentRecorder<TRecord> Target<TRecord>(IDetachedMappedSemanticNamedArgumentRecorderFactory<TRecord> factory, Action<TRecord, object?> recorder) => factory.Create(recorder);

    [Fact]
    public void NullRecorder_ArgumentNullException()
    {
        var context = FactoryContext<object>.Create();

        var exception = Record.Exception(() => Target(context.Factory, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidRecorder_ConstructedRecorderUsesDelegateAndReturnsTrue()
    {
        var dataRecord = Mock.Of<object>();
        var argument = Mock.Of<object>();

        var context = FactoryContext<object>.Create();

        Mock<Action<object, object?>> recorderMock = new();

        var recorder = Target(context.Factory, recorderMock.Object);

        var outcome = recorder.TryRecordArgument(dataRecord, argument);

        Assert.True(outcome);

        recorderMock.Verify((recorder) => recorder.Invoke(dataRecord, argument), Times.Once);
    }
}
