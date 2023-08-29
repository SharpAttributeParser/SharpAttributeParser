namespace SharpAttributeParser.Mappers.Repositories.CombinedCases.MappedCombinedConstructorArgumentRecorderFactoryCases;

using Moq;

using SharpAttributeParser.Mappers.MappedRecorders;
using SharpAttributeParser.Mappers.Repositories.Combined;

using System;

using Xunit;

public sealed class Create
{
    private static IMappedCombinedConstructorArgumentRecorder Target<TRecord>(IMappedCombinedConstructorArgumentRecorderFactory factory, TRecord dataRecord, IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> detachedRecorder) => factory.Create(dataRecord, detachedRecorder);

    [Fact]
    public void NullDataRecord_ArgumentNullException()
    {
        var context = FactoryContext.Create();

        var exception = Record.Exception(() => Target(context.Factory, null!, Mock.Of<IDetachedMappedCombinedConstructorArgumentRecorder<object>>()));

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
        Mock<IDetachedMappedCombinedConstructorArgumentRecorder<object>> detachedRecorderMock = new();

        var context = FactoryContext.Create();

        var attachedRecorder = Target(context.Factory, dataRecord, detachedRecorderMock.Object);

        attachedRecorder.TryRecordArgument(argument, syntax);

        detachedRecorderMock.Verify((recorder) => recorder.TryRecordArgument(dataRecord, argument, syntax), Times.Once);
    }
}
