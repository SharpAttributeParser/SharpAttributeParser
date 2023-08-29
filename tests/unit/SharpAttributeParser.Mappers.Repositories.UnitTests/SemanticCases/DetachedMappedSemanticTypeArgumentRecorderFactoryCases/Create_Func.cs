namespace SharpAttributeParser.Mappers.Repositories.SemanticCases.DetachedMappedSemanticTypeArgumentRecorderFactoryCases;

using Microsoft.CodeAnalysis;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Semantic;

using System;

using Xunit;

public sealed class Create_Func
{
    private static IDetachedMappedSemanticTypeArgumentRecorder<TRecord> Target<TRecord>(IDetachedMappedSemanticTypeArgumentRecorderFactory<TRecord> factory, Func<TRecord, ITypeSymbol, bool> recorder) => factory.Create(recorder);

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
        var argument = Mock.Of<ITypeSymbol>();

        var context = FactoryContext<object>.Create();

        Mock<Func<object, ITypeSymbol, bool>> recorderMock = new();

        var recorder = Target(context.Factory, recorderMock.Object);

        recorder.TryRecordArgument(dataRecord, argument);

        recorderMock.Verify((recorder) => recorder.Invoke(dataRecord, argument), Times.Once);
    }
}
