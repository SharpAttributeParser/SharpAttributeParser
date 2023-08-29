namespace SharpAttributeParser.Mappers.Repositories.CombinedCases.MappedCombinedNamedArgumentRecorderFactoryCases;

using Moq;

using SharpAttributeParser.Mappers.MappedRecorders;
using SharpAttributeParser.Mappers.Repositories.Combined;

using System;

using Xunit;

public sealed class Create
{
    private static IMappedCombinedNamedArgumentRecorder Target<TRecord>(IMappedCombinedNamedArgumentRecorderFactory factory, TRecord dataRecord, IDetachedMappedCombinedNamedArgumentRecorder<TRecord> detachedRecorder) => factory.Create(dataRecord, detachedRecorder);

    [Fact]
    public void NullDataRecord_ArgumentNullException()
    {
        var context = FactoryContext.Create();

        var exception = Record.Exception(() => Target(context.Factory, null!, Mock.Of<IDetachedMappedCombinedNamedArgumentRecorder<object>>()));

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
        var argument = Mock.Of<object>();
        var syntax = ExpressionSyntaxFactory.Create();
        var dataRecord = Mock.Of<object>();
        Mock<IDetachedMappedCombinedNamedArgumentRecorder<object>> detachedRecorderMock = new();

        var context = FactoryContext.Create();

        var attachedRecorder = Target(context.Factory, dataRecord, detachedRecorderMock.Object);

        attachedRecorder.TryRecordArgument(argument, syntax);

        detachedRecorderMock.Verify((recorder) => recorder.TryRecordArgument(dataRecord, argument, syntax), Times.Once);
    }
}
