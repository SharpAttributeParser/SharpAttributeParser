namespace SharpAttributeParser.Mappers.Repositories.SemanticCases.DetachedMappedSemanticTypeArgumentRecorderFactoryCases;

using Microsoft.CodeAnalysis;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Semantic;

using System;

using Xunit;

public sealed class Create_Action
{
    private static IDetachedMappedSemanticTypeArgumentRecorder<TRecord> Target<TRecord>(IDetachedMappedSemanticTypeArgumentRecorderFactory<TRecord> factory, Action<TRecord, ITypeSymbol> recorder) => factory.Create(recorder);

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
        var argument = Mock.Of<ITypeSymbol>();

        var context = FactoryContext<object>.Create();

        Mock<Action<object, ITypeSymbol>> recorderMock = new();

        var recorder = Target(context.Factory, recorderMock.Object);

        var outcome = recorder.TryRecordArgument(dataRecord, argument);

        Assert.True(outcome);

        recorderMock.Verify((recorder) => recorder.Invoke(dataRecord, argument), Times.Once);
    }
}
