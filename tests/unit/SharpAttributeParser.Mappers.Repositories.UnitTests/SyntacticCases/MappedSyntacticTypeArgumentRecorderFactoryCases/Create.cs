namespace SharpAttributeParser.Mappers.Repositories.SyntacticCases.MappedSyntacticTypeArgumentRecorderFactoryCases;

using Moq;

using SharpAttributeParser.Mappers.MappedRecorders;
using SharpAttributeParser.Mappers.Repositories.Syntactic;

using System;

using Xunit;

public sealed class Create
{
    private static IMappedSyntacticTypeArgumentRecorder Target<TRecord>(IMappedSyntacticTypeArgumentRecorderFactory factory, TRecord dataRecord, IDetachedMappedSyntacticTypeArgumentRecorder<TRecord> detachedRecorder) => factory.Create(dataRecord, detachedRecorder);

    [Fact]
    public void NullDataRecord_ArgumentNullException()
    {
        var context = FactoryContext.Create();

        var exception = Record.Exception(() => Target(context.Factory, null!, Mock.Of<IDetachedMappedSyntacticTypeArgumentRecorder<object>>()));

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
        var syntax = ExpressionSyntaxFactory.Create();
        var dataRecord = Mock.Of<object>();
        Mock<IDetachedMappedSyntacticTypeArgumentRecorder<object>> detachedRecorderMock = new();

        var context = FactoryContext.Create();

        var attachedRecorder = Target(context.Factory, dataRecord, detachedRecorderMock.Object);

        attachedRecorder.TryRecordArgument(syntax);

        detachedRecorderMock.Verify((recorder) => recorder.TryRecordArgument(dataRecord, syntax), Times.Once);
    }
}
