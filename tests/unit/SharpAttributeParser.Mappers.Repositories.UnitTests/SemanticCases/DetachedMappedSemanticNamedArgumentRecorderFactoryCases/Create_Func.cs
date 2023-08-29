namespace SharpAttributeParser.Mappers.Repositories.SemanticCases.DetachedMappedSemanticNamedArgumentRecorderFactoryCases;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Semantic;

using System;

using Xunit;

public sealed class Create_Func
{
    private static IDetachedMappedSemanticNamedArgumentRecorder<TRecord> Target<TRecord>(IDetachedMappedSemanticNamedArgumentRecorderFactory<TRecord> factory, Func<TRecord, object?, bool> recorder) => factory.Create(recorder);

    [Fact]
    public void NullRecorder_ArgumentNullException()
    {
        var context = FactoryContext<object>.Create();

        var exception = Record.Exception(() => Target(context.Factory, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidRecorder_ConstructedRecorderUsesDelegate()
    {
        var dataRecord = Mock.Of<object>();
        var argument = Mock.Of<object>();

        var context = FactoryContext<object>.Create();

        Mock<Func<object, object?, bool>> recorderMock = new();

        var recorder = Target(context.Factory, recorderMock.Object);

        recorder.TryRecordArgument(dataRecord, argument);

        recorderMock.Verify((recorder) => recorder.Invoke(dataRecord, argument), Times.Once);
    }
}
