namespace SharpAttributeParser.Mappers.Repositories.AdaptiveCases.DetachedMappedAdaptiveParamsConstructorArgumentRecorderFactoryCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using OneOf;

using SharpAttributeParser.Mappers.Repositories.Adaptive;
using SharpAttributeParser.Mappers.Repositories.Combined;
using SharpAttributeParser.Mappers.Repositories.Semantic;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class Create_Func
{
    private static IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord> Target<TCombinedRecord, TSemanticRecord>(IDetachedMappedAdaptiveParamsConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> factory, Func<TCombinedRecord, object?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> combinedRecorder, Func<TSemanticRecord, object?, bool> semanticRecorder) => factory.Create(combinedRecorder, semanticRecorder);

    [Fact]
    public void NullCombinedRecorder_ArgumentNullException()
    {
        var context = FactoryContext<object, object>.Create();

        var exception = Record.Exception(() => Target(context.Factory, null!, Mock.Of<Func<object, object?, bool>>()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullSemanticRecorder_ArgumentNullException()
    {
        var context = FactoryContext<object, object>.Create();

        var exception = Record.Exception(() => Target(context.Factory, Mock.Of<Func<object, object?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool>>(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidRecorders_UsesCombinedAndSemanticFactories()
    {
        var combinedRecorder = Mock.Of<IDetachedMappedCombinedConstructorArgumentRecorder<object>>();
        var semanticRecorder = Mock.Of<IDetachedMappedSemanticConstructorArgumentRecorder<object>>();

        var combinedRecorderDelegate = Mock.Of<Func<object, object?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool>>();
        var semanticReecorderDelegate = Mock.Of<Func<object, object?, bool>>();

        var context = FactoryContext<object, object>.Create();

        context.CombinedFactoryMock.Setup(static (factory) => factory.Create(It.IsAny<Func<object, object?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool>>())).Returns(combinedRecorder);
        context.SemanticFactoryMock.Setup(static (factory) => factory.Create(It.IsAny<Func<object, object?, bool>>())).Returns(semanticRecorder);

        var recorderProvider = Target(context.Factory, combinedRecorderDelegate, semanticReecorderDelegate);

        Assert.Same(combinedRecorder, recorderProvider.Combined);
        Assert.Same(semanticRecorder, recorderProvider.Semantic);

        context.CombinedFactoryMock.Verify((factory) => factory.Create(combinedRecorderDelegate), Times.Once);
        context.SemanticFactoryMock.Verify((factory) => factory.Create(semanticReecorderDelegate), Times.Once);
    }
}
