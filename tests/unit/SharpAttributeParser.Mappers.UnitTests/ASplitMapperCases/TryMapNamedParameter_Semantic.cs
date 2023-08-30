﻿namespace SharpAttributeParser.Mappers.ASplitMapperCases;

using Moq;

using SharpAttributeParser.Mappers.MappedRecorders;
using SharpAttributeParser.Mappers.Repositories;
using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Mappers.Repositories.Split;

using System;

using Xunit;

public sealed class TryMapNamedParameter_Semantic
{
    private static IMappedSemanticNamedArgumentRecorder? Target<TSemanticRecord, TSyntacticRecord>(ASplitMapper<TSemanticRecord, TSyntacticRecord> mapper, string parameterName, TSemanticRecord dataRecord) => mapper.TryMapNamedParameter(parameterName, dataRecord);

    [Fact]
    public void NullParameterName_ArgumentNullException()
    {
        var context = MapperContext<object, object>.Create();

        var exception = Record.Exception(() => Target(context.Mapper, null!, Mock.Of<object>()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullDataRecord_ArgumentNullException()
    {
        var context = MapperContext<object, object>.Create();

        var exception = Record.Exception(() => Target(context.Mapper, string.Empty, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Valid_InitializesMapper()
    {
        var context = MapperContext<object, object>.Create();

        Target(context.Mapper, string.Empty, Mock.Of<object>());

        context.AddMappingsDelegateMock.Verify(static (addMappings) => addMappings.Invoke(It.IsAny<IAppendableSplitMappingRepository<object, object>>()), Times.Once);
    }

    [Fact]
    public void Unmapped_ReturnsNullAndLogs()
    {
        var context = MapperContext<object, object>.Create();

        var recorder = Target(context.Mapper, string.Empty, Mock.Of<object>());

        Assert.Null(recorder);

        context.SemanticLoggerMock.Verify(static (logger) => logger.NamedParameter.FailedToMapNamedParameter(), Times.Once);
    }

    [Fact]
    public void Mapped_ReturnsRecorderAndUsesDependencies()
    {
        var parameterName = string.Empty;
        var dataRecord = Mock.Of<object>();

        var expectedRecorder = Mock.Of<IMappedSemanticNamedArgumentRecorder>();
        Mock<IDetachedMappedSplitNamedArgumentRecorderProvider<object, object>> detachedRecorderMock = new() { DefaultValue = DefaultValue.Mock };
        var detachedRecorder = detachedRecorderMock.Object;

        var context = MapperContext<object, object>.Create();

        context.SemanticRecorderFactoryMock.Setup(static (factory) => factory.NamedParameter.Create(It.IsAny<object>(), It.IsAny<IDetachedMappedSemanticNamedArgumentRecorder<object>>())).Returns(expectedRecorder);
        context.RepositoryFactoryMock.Setup((factory) => factory.Create(It.IsAny<IParameterComparer>(), It.IsAny<bool>()).Build().NamedParameters.Named.TryGetValue(It.IsAny<string>(), out detachedRecorder)).Returns(true);

        var actualRecorder = Target(context.Mapper, parameterName, dataRecord);

        Assert.Same(expectedRecorder, actualRecorder);

        context.SemanticRecorderFactoryMock.Verify((factory) => factory.NamedParameter.Create(dataRecord, detachedRecorder.Semantic), Times.Once);
        context.RepositoryFactoryMock.Verify((factory) => factory.Create(context.ParameterComparer, true).Build().NamedParameters.Named.TryGetValue(parameterName, out detachedRecorder), Times.Once);
    }
}
