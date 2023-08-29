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

public sealed class Create_Pattern_Action
{
    private static IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord> Target<TSemanticRecord, TSyntacticRecord, TArgument>(IDetachedMappedSplitParamsConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> factory, IArgumentPattern<TArgument> pattern, Action<TSemanticRecord, TArgument> semanticRecorder, Action<TSyntacticRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> syntacticRecorder) => factory.Create(pattern, semanticRecorder, syntacticRecorder);

    [Fact]
    public void NullPattern_ArgumentNullException()
    {
        var context = FactoryContext<object, object>.Create();

        var exception = Record.Exception(() => Target<object, object, object>(context.Factory, null!, static (dataRecord, argument) => { }, static (dataRecord, syntax) => { }));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullSemanticRecorder_ArgumentNullException()
    {
        var context = FactoryContext<object, object>.Create();

        var exception = Record.Exception(() => Target(context.Factory, Mock.Of<IArgumentPattern<object>>(), null!, static (dataRecord, syntax) => { }));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullSyntacticRecorder_ArgumentNullException()
    {
        var context = FactoryContext<object, object>.Create();

        var exception = Record.Exception(() => Target(context.Factory, Mock.Of<IArgumentPattern<object>>(), static (dataRecord, argument) => { }, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidPatternAndRecorders_UsesSemanticAndSyntacticFactories()
    {
        var pattern = Mock.Of<IArgumentPattern<object>>();

        var semanticRecorder = Mock.Of<IDetachedMappedSemanticConstructorArgumentRecorder<object>>();
        var syntacticRecorder = Mock.Of<IDetachedMappedSyntacticConstructorArgumentRecorder<object>>();

        var semanticRecorderDelegate = Mock.Of<Action<object, object?>>();
        var syntacticReecorderDelegate = Mock.Of<Action<object, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>>>();

        var context = FactoryContext<object, object>.Create();

        context.SemanticFactoryMock.Setup(static (factory) => factory.Create(It.IsAny<IArgumentPattern<object>>(), It.IsAny<Action<object, object>>())).Returns(semanticRecorder);
        context.SyntacticFactoryMock.Setup(static (factory) => factory.Create(It.IsAny<Action<object, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>>>())).Returns(syntacticRecorder);

        var recorderProvider = Target(context.Factory, pattern, semanticRecorderDelegate, syntacticReecorderDelegate);

        Assert.Same(semanticRecorder, recorderProvider.Semantic);
        Assert.Same(syntacticRecorder, recorderProvider.Syntactic);

        context.SemanticFactoryMock.Verify((factory) => factory.Create(pattern, semanticRecorderDelegate), Times.Once);
        context.SyntacticFactoryMock.Verify((factory) => factory.Create(syntacticReecorderDelegate), Times.Once);
    }
}
