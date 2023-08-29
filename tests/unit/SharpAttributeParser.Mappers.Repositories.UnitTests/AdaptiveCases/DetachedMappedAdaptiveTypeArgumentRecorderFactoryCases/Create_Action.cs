namespace SharpAttributeParser.Mappers.Repositories.AdaptiveCases.DetachedMappedAdaptiveTypeArgumentRecorderFactoryCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Adaptive;
using SharpAttributeParser.Mappers.Repositories.Combined;
using SharpAttributeParser.Mappers.Repositories.Semantic;

using System;

using Xunit;

public sealed class Create_Action
{
    private static IDetachedMappedAdaptiveTypeArgumentRecorderProvider<TCombinedRecord, TSemanticRecord> Target<TCombinedRecord, TSemanticRecord>(IDetachedMappedAdaptiveTypeArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> factory, Action<TCombinedRecord, ITypeSymbol, ExpressionSyntax> combinedRecorder, Action<TSemanticRecord, ITypeSymbol> semanticRecorder) => factory.Create(combinedRecorder, semanticRecorder);

    [Fact]
    public void NullCombinedRecorder_ArgumentNullException()
    {
        var context = FactoryContext<object, object>.Create();

        var exception = Record.Exception(() => Target(context.Factory, null!, Mock.Of<Action<object, ITypeSymbol>>()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullSemanticRecorder_ArgumentNullException()
    {
        var context = FactoryContext<object, object>.Create();

        var exception = Record.Exception(() => Target(context.Factory, Mock.Of<Action<object, ITypeSymbol, ExpressionSyntax>>(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidRecorder_UsesCombinedAndSemanticFactories()
    {
        var combinedRecorder = Mock.Of<IDetachedMappedCombinedTypeArgumentRecorder<object>>();
        var semanticRecorder = Mock.Of<IDetachedMappedSemanticTypeArgumentRecorder<object>>();

        var combinedRecorderDelegate = Mock.Of<Action<object, ITypeSymbol, ExpressionSyntax>>();
        var semanticReecorderDelegate = Mock.Of<Action<object, ITypeSymbol>>();

        var context = FactoryContext<object, object>.Create();

        context.CombinedFactoryMock.Setup(static (factory) => factory.Create(It.IsAny<Action<object, ITypeSymbol, ExpressionSyntax>>())).Returns(combinedRecorder);
        context.SemanticFactoryMock.Setup(static (factory) => factory.Create(It.IsAny<Action<object, ITypeSymbol>>())).Returns(semanticRecorder);

        var recorderProvider = Target(context.Factory, combinedRecorderDelegate, semanticReecorderDelegate);

        Assert.Same(combinedRecorder, recorderProvider.Combined);
        Assert.Same(semanticRecorder, recorderProvider.Semantic);

        context.CombinedFactoryMock.Verify((factory) => factory.Create(combinedRecorderDelegate), Times.Once);
        context.SemanticFactoryMock.Verify((factory) => factory.Create(semanticReecorderDelegate), Times.Once);
    }
}
