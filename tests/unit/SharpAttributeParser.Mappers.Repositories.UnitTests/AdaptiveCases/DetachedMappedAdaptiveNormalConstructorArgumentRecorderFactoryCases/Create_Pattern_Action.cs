namespace SharpAttributeParser.Mappers.Repositories.AdaptiveCases.DetachedMappedAdaptiveNormalConstructorArgumentRecorderFactoryCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Adaptive;
using SharpAttributeParser.Mappers.Repositories.Combined;
using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Patterns;

using System;

using Xunit;

public sealed class Create_Pattern_Action
{
    private static IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord> Target<TCombinedRecord, TSemanticRecord, TArgument>(IDetachedMappedAdaptiveNormalConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> factory, IArgumentPattern<TArgument> pattern, Action<TCombinedRecord, TArgument, ExpressionSyntax> combinedRecorder, Action<TSemanticRecord, TArgument> semanticRecorder) => factory.Create(pattern, combinedRecorder, semanticRecorder);

    [Fact]
    public void NullPattern_ArgumentNullException()
    {
        var context = FactoryContext<object, object>.Create();

        var exception = Record.Exception(() => Target<object, object, object>(context.Factory, null!, static (dataRecord, argument, syntax) => { }, static (dataRecord, argument) => { }));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullCombinedRecorder_ArgumentNullException()
    {
        var context = FactoryContext<object, object>.Create();

        var exception = Record.Exception(() => Target(context.Factory, Mock.Of<IArgumentPattern<object>>(), null!, static (dataRecord, argument) => { }));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullSemanticRecorder_ArgumentNullException()
    {
        var context = FactoryContext<object, object>.Create();

        var exception = Record.Exception(() => Target(context.Factory, Mock.Of<IArgumentPattern<object>>(), static (dataRecord, argument, syntax) => { }, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidPatternAndRecorders_UsesCombinedAndSemanticFactories()
    {
        var pattern = Mock.Of<IArgumentPattern<object>>();

        var combinedRecorder = Mock.Of<IDetachedMappedCombinedConstructorArgumentRecorder<object>>();
        var semanticRecorder = Mock.Of<IDetachedMappedSemanticConstructorArgumentRecorder<object>>();

        var combinedRecorderDelegate = Mock.Of<Action<object, object?, ExpressionSyntax>>();
        var semanticReecorderDelegate = Mock.Of<Action<object, object?>>();

        var context = FactoryContext<object, object>.Create();

        context.CombinedFactoryMock.Setup(static (factory) => factory.Create(It.IsAny<IArgumentPattern<object>>(), It.IsAny<Action<object, object, ExpressionSyntax>>())).Returns(combinedRecorder);
        context.SemanticFactoryMock.Setup(static (factory) => factory.Create(It.IsAny<IArgumentPattern<object>>(), It.IsAny<Action<object, object>>())).Returns(semanticRecorder);

        var recorderProvider = Target(context.Factory, pattern, combinedRecorderDelegate, semanticReecorderDelegate);

        Assert.Same(combinedRecorder, recorderProvider.Combined);
        Assert.Same(semanticRecorder, recorderProvider.Semantic);

        context.CombinedFactoryMock.Verify((factory) => factory.Create(pattern, combinedRecorderDelegate), Times.Once);
        context.SemanticFactoryMock.Verify((factory) => factory.Create(pattern, semanticReecorderDelegate), Times.Once);
    }
}
