namespace SharpAttributeParser.Mappers.Repositories.SemanticCases.MappedSemanticTypeArgumentRecorderFactoryCases;

using Microsoft.CodeAnalysis;

using Moq;

using SharpAttributeParser.Mappers.MappedRecorders;
using SharpAttributeParser.Mappers.Repositories.Semantic;

using System;

using Xunit;

public sealed class Create
{
    private static IMappedSemanticTypeArgumentRecorder Target<TRecord>(IMappedSemanticTypeArgumentRecorderFactory factory, TRecord dataRecord, IDetachedMappedSemanticTypeArgumentRecorder<TRecord> detachedRecorder) => factory.Create(dataRecord, detachedRecorder);

    [Fact]
    public void NullDataRecord_ArgumentNullException()
    {
        var context = FactoryContext.Create();

        var exception = Record.Exception(() => Target(context.Factory, null!, Mock.Of<IDetachedMappedSemanticTypeArgumentRecorder<object>>()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullDetachedRecorder_ArgumentNullException()
    {
        var context = FactoryContext.Create();

        var exception = Record.Exception(() => Target(context.Factory, Mock.Of<object>(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Valid_CreatedRecorderUsesProvidedArguments()
    {
        var argument = Mock.Of<ITypeSymbol>();
        var dataRecord = Mock.Of<object>();
        Mock<IDetachedMappedSemanticTypeArgumentRecorder<object>> detachedRecorderMock = new();

        var context = FactoryContext.Create();

        var attachedRecorder = Target(context.Factory, dataRecord, detachedRecorderMock.Object);

        attachedRecorder.TryRecordArgument(argument);

        detachedRecorderMock.Verify((recorder) => recorder.TryRecordArgument(dataRecord, argument), Times.Once);
    }
}
