namespace SharpAttributeParser.Mappers.Repositories.SplitCases.DetachedMappedSplitNamedArgumentRecorderFactoryCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Mappers.Repositories.Split;
using SharpAttributeParser.Mappers.Repositories.Syntactic;

using System;

using Xunit;

public sealed class Create_Func
{
    private static IDetachedMappedSplitNamedArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord> Target<TSemanticRecord, TSyntacticRecord>(IDetachedMappedSplitNamedArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> factory, Func<TSemanticRecord, object?, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder) => factory.Create(semanticRecorder, syntacticRecorder);

    [Fact]
    public void NullSemanticRecorder_ArgumentNullException()
    {
        var context = FactoryContext<object, object>.Create();

        var exception = Record.Exception(() => Target(context.Factory, null!, Mock.Of<Func<object, ExpressionSyntax, bool>>()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullSyntacticRecorder_ArgumentNullException()
    {
        var context = FactoryContext<object, object>.Create();

        var exception = Record.Exception(() => Target(context.Factory, Mock.Of<Func<object, object?, bool>>(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidRecorders_UsesSemanticAndSyntacticFactories()
    {
        var semanticRecorder = Mock.Of<IDetachedMappedSemanticNamedArgumentRecorder<object>>();
        var syntacticRecorder = Mock.Of<IDetachedMappedSyntacticNamedArgumentRecorder<object>>();

        var semanticRecorderDelegate = Mock.Of<Func<object, object?, bool>>();
        var syntacticReecorderDelegate = Mock.Of<Func<object, ExpressionSyntax, bool>>();

        var context = FactoryContext<object, object>.Create();

        context.SemanticFactoryMock.Setup(static (factory) => factory.Create(It.IsAny<Func<object, object?, bool>>())).Returns(semanticRecorder);
        context.SyntacticFactoryMock.Setup(static (factory) => factory.Create(It.IsAny<Func<object, ExpressionSyntax, bool>>())).Returns(syntacticRecorder);

        var recorderProvider = Target(context.Factory, semanticRecorderDelegate, syntacticReecorderDelegate);

        Assert.Same(semanticRecorder, recorderProvider.Semantic);
        Assert.Same(syntacticRecorder, recorderProvider.Syntactic);

        context.SemanticFactoryMock.Verify((factory) => factory.Create(semanticRecorderDelegate), Times.Once);
        context.SyntacticFactoryMock.Verify((factory) => factory.Create(syntacticReecorderDelegate), Times.Once);
    }
}
