﻿namespace SharpAttributeParser.Mappers.Repositories.CombinedCases.DetachedMappedCombinedParamsConstructorArgumentRecorderFactoryCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using OneOf;

using SharpAttributeParser.Mappers.Repositories.Combined;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class Create_Action
{
    private static IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> Target<TRecord>(IDetachedMappedCombinedParamsConstructorArgumentRecorderFactory<TRecord> factory, Action<TRecord, object?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> recorder) => factory.Create(recorder);

    [Fact]
    public void NullRecorder_ArgumentNullException()
    {
        var context = FactoryContext<object>.Create();

        var exception = Record.Exception(() => Target(context.Factory, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidRecorder_ConstructedRecorderUsesDelegateAndReturnsTrue()
    {
        var dataRecord = Mock.Of<object>();
        var argument = Mock.Of<object>();
        var syntax = ExpressionSyntaxFactory.Create();

        var context = FactoryContext<object>.Create();

        Mock<Action<object, object?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>>> recorderMock = new();

        var recorder = Target(context.Factory, recorderMock.Object);

        var outcome = recorder.TryRecordArgument(dataRecord, argument, syntax);

        Assert.True(outcome);

        recorderMock.Verify((recorder) => recorder.Invoke(dataRecord, argument, syntax), Times.Once);
    }
}
