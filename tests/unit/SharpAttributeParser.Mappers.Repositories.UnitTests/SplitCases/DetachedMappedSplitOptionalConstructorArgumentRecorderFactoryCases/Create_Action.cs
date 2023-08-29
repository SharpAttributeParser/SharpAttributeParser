﻿namespace SharpAttributeParser.Mappers.Repositories.SplitCases.DetachedMappedSplitOptionalConstructorArgumentRecorderFactoryCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using OneOf;
using OneOf.Types;

using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Mappers.Repositories.Split;
using SharpAttributeParser.Mappers.Repositories.Syntactic;

using System;

using Xunit;

public sealed class Create_Action
{
    private static IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord> Target<TSemanticRecord, TSyntacticRecord>(IDetachedMappedSplitOptionalConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> factory, Action<TSemanticRecord, object?> semanticRecorder, Action<TSyntacticRecord, OneOf<None, ExpressionSyntax>> syntacticRecorder) => factory.Create(semanticRecorder, syntacticRecorder);

    [Fact]
    public void NullSemanticRecorder_ArgumentNullException()
    {
        var context = FactoryContext<object, object>.Create();

        var exception = Record.Exception(() => Target(context.Factory, null!, Mock.Of<Action<object, OneOf<None, ExpressionSyntax>>>()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullSyntacticRecorder_ArgumentNullException()
    {
        var context = FactoryContext<object, object>.Create();

        var exception = Record.Exception(() => Target(context.Factory, Mock.Of<Action<object, object?>>(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidRecorders_UsesSemanticAndSyntacticFactories()
    {
        var semanticRecorder = Mock.Of<IDetachedMappedSemanticConstructorArgumentRecorder<object>>();
        var syntacticRecorder = Mock.Of<IDetachedMappedSyntacticConstructorArgumentRecorder<object>>();

        var semanticRecorderDelegate = Mock.Of<Action<object, object?>>();
        var syntacticReecorderDelegate = Mock.Of<Action<object, OneOf<None, ExpressionSyntax>>>();

        var context = FactoryContext<object, object>.Create();

        context.SemanticFactoryMock.Setup(static (factory) => factory.Create(It.IsAny<Action<object, object?>>())).Returns(semanticRecorder);
        context.SyntacticFactoryMock.Setup(static (factory) => factory.Create(It.IsAny<Action<object, OneOf<None, ExpressionSyntax>>>())).Returns(syntacticRecorder);

        var recorderProvider = Target(context.Factory, semanticRecorderDelegate, syntacticReecorderDelegate);

        Assert.Same(semanticRecorder, recorderProvider.Semantic);
        Assert.Same(syntacticRecorder, recorderProvider.Syntactic);

        context.SemanticFactoryMock.Verify((factory) => factory.Create(semanticRecorderDelegate), Times.Once);
        context.SyntacticFactoryMock.Verify((factory) => factory.Create(syntacticReecorderDelegate), Times.Once);
    }
}
