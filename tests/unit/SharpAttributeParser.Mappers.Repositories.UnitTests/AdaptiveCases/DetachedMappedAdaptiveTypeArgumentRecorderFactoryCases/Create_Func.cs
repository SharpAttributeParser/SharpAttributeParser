namespace SharpAttributeParser.Mappers.Repositories.AdaptiveCases.DetachedMappedAdaptiveTypeArgumentRecorderFactoryCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Adaptive;
using SharpAttributeParser.Mappers.Repositories.Combined;
using SharpAttributeParser.Mappers.Repositories.Semantic;

using System;

using Xunit;

public sealed class Create_Func
{
    private static IDetachedMappedAdaptiveTypeArgumentRecorderProvider<TCombinedRecord, TSemanticRecord> Target<TCombinedRecord, TSemanticRecord>(IDetachedMappedAdaptiveTypeArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> factory, Func<TCombinedRecord, ITypeSymbol, ExpressionSyntax, bool> combinedRecorder, Func<TSemanticRecord, ITypeSymbol, bool> semanticRecorder) => factory.Create(combinedRecorder, semanticRecorder);

    [Fact]
    public void NullCombinedRecorder_ArgumentNullException()
    {
        var context = FactoryContext<object, object>.Create();

        var exception = Record.Exception(() => Target(context.Factory, null!, Mock.Of<Func<object, ITypeSymbol, bool>>()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullSemanticRecorder_ArgumentNullException()
    {
        var context = FactoryContext<object, object>.Create();

        var exception = Record.Exception(() => Target(context.Factory, Mock.Of<Func<object, ITypeSymbol, ExpressionSyntax, bool>>(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidRecorder_UsesCombinedAndSemanticFactories()
    {
        var combinedRecorder = Mock.Of<IDetachedMappedCombinedTypeArgumentRecorder<object>>();
        var semanticRecorder = Mock.Of<IDetachedMappedSemanticTypeArgumentRecorder<object>>();

        var combinedRecorderDelegate = Mock.Of<Func<object, ITypeSymbol, ExpressionSyntax, bool>>();
        var semanticReecorderDelegate = Mock.Of<Func<object, ITypeSymbol, bool>>();

        var context = FactoryContext<object, object>.Create();

        context.CombinedFactoryMock.Setup(static (factory) => factory.Create(It.IsAny<Func<object, ITypeSymbol, ExpressionSyntax, bool>>())).Returns(combinedRecorder);
        context.SemanticFactoryMock.Setup(static (factory) => factory.Create(It.IsAny<Func<object, ITypeSymbol, bool>>())).Returns(semanticRecorder);

        var recorderProvider = Target(context.Factory, combinedRecorderDelegate, semanticReecorderDelegate);

        Assert.Same(combinedRecorder, recorderProvider.Combined);
        Assert.Same(semanticRecorder, recorderProvider.Semantic);

        context.CombinedFactoryMock.Verify((factory) => factory.Create(combinedRecorderDelegate), Times.Once);
        context.SemanticFactoryMock.Verify((factory) => factory.Create(semanticReecorderDelegate), Times.Once);
    }
}
