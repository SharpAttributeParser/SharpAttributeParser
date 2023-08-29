namespace SharpAttributeParser.Mappers.Repositories.SplitCases.DetachedMappedSplitTypeArgumentRecorderFactoryCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Mappers.Repositories.Split;
using SharpAttributeParser.Mappers.Repositories.Syntactic;

using System;

using Xunit;

public sealed class Create_Action
{
    private static IDetachedMappedSplitTypeArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord> Target<TSemanticRecord, TSyntacticRecord>(IDetachedMappedSplitTypeArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> factory, Action<TSemanticRecord, ITypeSymbol> semanticRecorder, Action<TSyntacticRecord, ExpressionSyntax> syntacticRecorder) => factory.Create(semanticRecorder, syntacticRecorder);

    [Fact]
    public void NullSemanticRecorder_ArgumentNullException()
    {
        var context = FactoryContext<object, object>.Create();

        var exception = Record.Exception(() => Target(context.Factory, null!, Mock.Of<Action<object, ExpressionSyntax>>()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullSyntacticRecorder_ArgumentNullException()
    {
        var context = FactoryContext<object, object>.Create();

        var exception = Record.Exception(() => Target(context.Factory, Mock.Of<Action<object, ITypeSymbol>>(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidRecorder_UsesSemanticAndSyntacticFactories()
    {
        var semanticRecorder = Mock.Of<IDetachedMappedSemanticTypeArgumentRecorder<object>>();
        var syntacticRecorder = Mock.Of<IDetachedMappedSyntacticTypeArgumentRecorder<object>>();

        var semanticRecorderDelegate = Mock.Of<Action<object, ITypeSymbol>>();
        var syntacticReecorderDelegate = Mock.Of<Action<object, ExpressionSyntax>>();

        var context = FactoryContext<object, object>.Create();

        context.SemanticFactoryMock.Setup(static (factory) => factory.Create(It.IsAny<Action<object, ITypeSymbol>>())).Returns(semanticRecorder);
        context.SyntacticFactoryMock.Setup(static (factory) => factory.Create(It.IsAny<Action<object, ExpressionSyntax>>())).Returns(syntacticRecorder);

        var recorderProvider = Target(context.Factory, semanticRecorderDelegate, syntacticReecorderDelegate);

        Assert.Same(semanticRecorder, recorderProvider.Semantic);
        Assert.Same(syntacticRecorder, recorderProvider.Syntactic);

        context.SemanticFactoryMock.Verify((factory) => factory.Create(semanticRecorderDelegate), Times.Once);
        context.SyntacticFactoryMock.Verify((factory) => factory.Create(syntacticReecorderDelegate), Times.Once);
    }
}
