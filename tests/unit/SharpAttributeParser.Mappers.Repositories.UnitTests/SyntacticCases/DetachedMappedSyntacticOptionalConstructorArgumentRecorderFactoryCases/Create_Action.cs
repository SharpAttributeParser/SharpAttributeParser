namespace SharpAttributeParser.Mappers.Repositories.SyntacticCases.DetachedMappedSyntacticOptionalConstructorArgumentRecorderFactoryCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using OneOf;
using OneOf.Types;

using SharpAttributeParser.Mappers.Repositories.Syntactic;

using System;

using Xunit;

public sealed class Create_Action
{
    private static IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord> Target<TRecord>(IDetachedMappedSyntacticOptionalConstructorArgumentRecorderFactory<TRecord> factory, Action<TRecord, OneOf<None, ExpressionSyntax>> recorder) => factory.Create(recorder);

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

        Mock<Action<object, OneOf<None, ExpressionSyntax>>> recorderMock = new();

        var recorder = Target(context.Factory, recorderMock.Object);

        var outcome = recorder.TryRecordArgument(dataRecord, syntax);

        Assert.True(outcome);

        recorderMock.Verify((recorder) => recorder.Invoke(dataRecord, syntax), Times.Once);
    }
}
