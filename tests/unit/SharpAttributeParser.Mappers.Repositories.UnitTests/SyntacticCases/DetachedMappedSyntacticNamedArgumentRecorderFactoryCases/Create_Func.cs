namespace SharpAttributeParser.Mappers.Repositories.SyntacticCases.DetachedMappedSyntacticNamedArgumentRecorderFactoryCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Syntactic;

using System;

using Xunit;

public sealed class Create_Func
{
    private static IDetachedMappedSyntacticNamedArgumentRecorder<TRecord> Target<TRecord>(IDetachedMappedSyntacticNamedArgumentRecorderFactory<TRecord> factory, Func<TRecord, ExpressionSyntax, bool> recorder) => factory.Create(recorder);

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
        var syntax = ExpressionSyntaxFactory.Create();

        var context = FactoryContext<object>.Create();

        Mock<Func<object, ExpressionSyntax, bool>> recorderMock = new();

        var recorder = Target(context.Factory, recorderMock.Object);

        recorder.TryRecordArgument(dataRecord, syntax);

        recorderMock.Verify((recorder) => recorder.Invoke(dataRecord, syntax), Times.Once);
    }
}
