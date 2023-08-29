namespace SharpAttributeParser.Mappers.Repositories.AdaptiveCases.DetachedMappedAdaptiveNamedArgumentRecorderFactoryCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Adaptive;
using SharpAttributeParser.Mappers.Repositories.Combined;
using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Patterns;

using System;

using Xunit;

public sealed class Create_PatternDelegate_Action
{
    private static IDetachedMappedAdaptiveNamedArgumentRecorderProvider<TCombinedRecord, TSemanticRecord> Target<TCombinedRecord, TSemanticRecord, TArgument>(IDetachedMappedAdaptiveNamedArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> factory, Func<IArgumentPatternFactory, IArgumentPattern<TArgument>> patternDelegate, Action<TCombinedRecord, TArgument, ExpressionSyntax> combinedRecorder, Action<TSemanticRecord, TArgument> semanticRecorder) => factory.Create(patternDelegate, combinedRecorder, semanticRecorder);

    [Fact]
    public void NullPatternDelegate_ArgumentNullException()
    {
        var context = FactoryContext<object, object>.Create();

        var exception = Record.Exception(() => Target<object, object, object>(context.Factory, null!, static (dataRecord, argument, syntax) => { }, static (dataRecord, argument) => { }));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullCombinedRecorder_ArgumentNullException()
    {
        var context = FactoryContext<object, object>.Create();

        var exception = Record.Exception(() => Target(context.Factory, static (factory) => Mock.Of<IArgumentPattern<object>>(), null!, static (dataRecord, argument) => { }));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullSemanticRecorder_ArgumentNullException()
    {
        var context = FactoryContext<object, object>.Create();

        var exception = Record.Exception(() => Target(context.Factory, static (factory) => Mock.Of<IArgumentPattern<object>>(), static (dataRecord, argument, syntax) => { }, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidPatternAndRecorders_UsesCombinedAndSemanticFactories()
    {
        var patternDelegate = Mock.Of<Func<IArgumentPatternFactory, IArgumentPattern<object>>>();

        var combinedRecorder = Mock.Of<IDetachedMappedCombinedNamedArgumentRecorder<object>>();
        var semanticRecorder = Mock.Of<IDetachedMappedSemanticNamedArgumentRecorder<object>>();

        var combinedRecorderDelegate = Mock.Of<Action<object, object, ExpressionSyntax>>();
        var semanticReecorderDelegate = Mock.Of<Action<object, object>>();

        var context = FactoryContext<object, object>.Create();

        context.CombinedFactoryMock.Setup(static (factory) => factory.Create(It.IsAny<Func<IArgumentPatternFactory, IArgumentPattern<object>>>(), It.IsAny<Action<object, object, ExpressionSyntax>>())).Returns(combinedRecorder);
        context.SemanticFactoryMock.Setup(static (factory) => factory.Create(It.IsAny<Func<IArgumentPatternFactory, IArgumentPattern<object>>>(), It.IsAny<Action<object, object>>())).Returns(semanticRecorder);

        var recorderProvider = Target(context.Factory, patternDelegate, combinedRecorderDelegate, semanticReecorderDelegate);

        Assert.Same(combinedRecorder, recorderProvider.Combined);
        Assert.Same(semanticRecorder, recorderProvider.Semantic);

        context.CombinedFactoryMock.Verify((factory) => factory.Create(patternDelegate, combinedRecorderDelegate), Times.Once);
        context.SemanticFactoryMock.Verify((factory) => factory.Create(patternDelegate, semanticReecorderDelegate), Times.Once);
    }
}
