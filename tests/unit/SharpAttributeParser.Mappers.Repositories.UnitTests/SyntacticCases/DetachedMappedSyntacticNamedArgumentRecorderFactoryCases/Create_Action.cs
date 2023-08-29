namespace SharpAttributeParser.Mappers.Repositories.SyntacticCases.DetachedMappedSyntacticNamedArgumentRecorderFactoryCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Syntactic;

using System;

using Xunit;

public sealed class Create_Action
{
    private static IDetachedMappedSyntacticNamedArgumentRecorder<TRecord> Target<TRecord>(IDetachedMappedSyntacticNamedArgumentRecorderFactory<TRecord> factory, Action<TRecord, ExpressionSyntax> recorder) => factory.Create(recorder);

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
        var syntax = ExpressionSyntaxFactory.Create();

        var context = FactoryContext<object>.Create();

        Mock<Action<object, ExpressionSyntax>> recorderMock = new();

        var recorder = Target(context.Factory, recorderMock.Object);

        var outcome = recorder.TryRecordArgument(dataRecord, syntax);

        Assert.True(outcome);

        recorderMock.Verify((recorder) => recorder.Invoke(dataRecord, syntax), Times.Once);
    }
}
