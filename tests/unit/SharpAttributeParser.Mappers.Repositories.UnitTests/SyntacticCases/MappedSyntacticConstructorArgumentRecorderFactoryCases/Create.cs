namespace SharpAttributeParser.Mappers.Repositories.SyntacticCases.MappedSyntacticConstructorArgumentRecorderFactoryCases;

using Moq;

using SharpAttributeParser.Mappers.MappedRecorders;
using SharpAttributeParser.Mappers.Repositories.Syntactic;

using System;

using Xunit;

public sealed class Create
{
    private static IMappedSyntacticConstructorArgumentRecorder Target<TRecord>(IMappedSyntacticConstructorArgumentRecorderFactory factory, TRecord dataRecord, IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord> detachedRecorder) => factory.Create(dataRecord, detachedRecorder);

    [Fact]
    public void NullDataRecord_ArgumentNullException()
    {
        var context = FactoryContext.Create();

        var exception = Record.Exception(() => Target(context.Factory, null!, Mock.Of<IDetachedMappedSyntacticConstructorArgumentRecorder<object>>()));

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
        Mock<IDetachedMappedSyntacticConstructorArgumentRecorder<object>> detachedRecorderMock = new();

        var context = FactoryContext.Create();

        var attachedRecorder = Target(context.Factory, dataRecord, detachedRecorderMock.Object);

        attachedRecorder.TryRecordArgument(syntax);

        detachedRecorderMock.Verify((recorder) => recorder.TryRecordArgument(dataRecord, syntax), Times.Once);
    }
}
