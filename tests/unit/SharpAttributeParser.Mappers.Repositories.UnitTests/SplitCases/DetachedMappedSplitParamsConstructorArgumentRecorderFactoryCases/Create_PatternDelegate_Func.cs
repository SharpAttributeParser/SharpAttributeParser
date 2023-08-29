namespace SharpAttributeParser.Mappers.Repositories.SplitCases.DetachedMappedSplitParamsConstructorArgumentRecorderFactoryCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using OneOf;

using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Mappers.Repositories.Split;
using SharpAttributeParser.Mappers.Repositories.Syntactic;
using SharpAttributeParser.Patterns;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class Create_PatternDelegate_Func
{
    private static IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord> Target<TSemanticRecord, TSyntacticRecord, TArgument>(IDetachedMappedSplitParamsConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> factory, Func<IArgumentPatternFactory, IArgumentPattern<TArgument>> patternDelegate, Func<TSemanticRecord, TArgument, bool> semanticRecorder, Func<TSyntacticRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> syntacticRecorder) => factory.Create(patternDelegate, semanticRecorder, syntacticRecorder);

    [Fact]
    public void NullPatternDelegate_ArgumentNullException()
    {
        var context = FactoryContext<object, object>.Create();

        var exception = Record.Exception(() => Target<object, object, object>(context.Factory, null!, static (dataRecord, argument) => true, static (dataRecord, syntax) => true));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullSemanticRecorder_ArgumentNullException()
    {
        var context = FactoryContext<object, object>.Create();

        var exception = Record.Exception(() => Target(context.Factory, static (factory) => Mock.Of<IArgumentPattern<object>>(), null!, static (dataRecord, syntax) => true));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullSyntacticRecorder_ArgumentNullException()
    {
        var context = FactoryContext<object, object>.Create();

        var exception = Record.Exception(() => Target(context.Factory, static (factory) => Mock.Of<IArgumentPattern<object>>(), static (dataRecord, argument) => true, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidPatternAndRecorders_UsesSemanticAndSyntacticFactories()
    {
        var patternDelegate = Mock.Of<Func<IArgumentPatternFactory, IArgumentPattern<object>>>();

        var semanticRecorder = Mock.Of<IDetachedMappedSemanticConstructorArgumentRecorder<object>>();
        var syntacticRecorder = Mock.Of<IDetachedMappedSyntacticConstructorArgumentRecorder<object>>();

        var semanticRecorderDelegate = Mock.Of<Func<object, object, bool>>();
        var syntacticReecorderDelegate = Mock.Of<Func<object, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool>>();

        var context = FactoryContext<object, object>.Create();

        context.SemanticFactoryMock.Setup(static (factory) => factory.Create(It.IsAny<Func<IArgumentPatternFactory, IArgumentPattern<object>>>(), It.IsAny<Func<object, object, bool>>())).Returns(semanticRecorder);
        context.SyntacticFactoryMock.Setup(static (factory) => factory.Create(It.IsAny<Func<object, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool>>())).Returns(syntacticRecorder);

        var recorderProvider = Target(context.Factory, patternDelegate, semanticRecorderDelegate, syntacticReecorderDelegate);

        Assert.Same(semanticRecorder, recorderProvider.Semantic);
        Assert.Same(syntacticRecorder, recorderProvider.Syntactic);

        context.SemanticFactoryMock.Verify((factory) => factory.Create(patternDelegate, semanticRecorderDelegate), Times.Once);
        context.SyntacticFactoryMock.Verify((factory) => factory.Create(syntacticReecorderDelegate), Times.Once);
    }
}
