﻿namespace SharpAttributeParser.Mappers.Repositories.SemanticCases.MappedSemanticNamedArgumentRecorderFactoryCases;

using Microsoft.CodeAnalysis;

using Moq;

using SharpAttributeParser.Mappers.MappedRecorders;
using SharpAttributeParser.Mappers.Repositories.Semantic;

using System;

using Xunit;

public sealed class Create
{
    private static IMappedSemanticNamedArgumentRecorder Target<TRecord>(IMappedSemanticNamedArgumentRecorderFactory factory, TRecord dataRecord, IDetachedMappedSemanticNamedArgumentRecorder<TRecord> detachedRecorder) => factory.Create(dataRecord, detachedRecorder);

    [Fact]
    public void NullDataRecord_ArgumentNullException()
    {
        var context = FactoryContext.Create();

        var exception = Record.Exception(() => Target(context.Factory, null!, Mock.Of<IDetachedMappedSemanticNamedArgumentRecorder<object>>()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullDetachedRecorder_ArgumentNullException()
    {
        var context = FactoryContext.Create();

        var exception = Record.Exception(() => Target(context.Factory, Mock.Of<object>(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Valid_CreatedRecorderUsesProvidedArguments()
    {
        var argument = Mock.Of<ITypeSymbol>();
        var dataRecord = Mock.Of<object>();
        Mock<IDetachedMappedSemanticNamedArgumentRecorder<object>> detachedRecorderMock = new();

        var context = FactoryContext.Create();

        var attachedRecorder = Target(context.Factory, dataRecord, detachedRecorderMock.Object);

        attachedRecorder.TryRecordArgument(argument);

        detachedRecorderMock.Verify((recorder) => recorder.TryRecordArgument(dataRecord, argument), Times.Once);
    }
}