﻿namespace SharpAttributeParser.Mappers.ASemanticMapperCases;

using Microsoft.CodeAnalysis;

using Moq;

using SharpAttributeParser.Mappers.MappedRecorders;
using SharpAttributeParser.Mappers.Repositories.Semantic;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class TryMapConstructorParameter
{
    private static IMappedSemanticConstructorArgumentRecorder? Target<TRecord>(ASemanticMapper<TRecord> mapper, IParameterSymbol parameter, TRecord dataRecord) => mapper.TryMapConstructorParameter(parameter, dataRecord);

    [Fact]
    public void NullParameter_ArgumentNullException()
    {
        var context = MapperContext<object>.Create();

        var exception = Record.Exception(() => Target(context.Mapper, null!, Mock.Of<object>()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullDataRecord_ArgumentNullException()
    {
        var context = MapperContext<object>.Create();

        var exception = Record.Exception(() => Target(context.Mapper, Mock.Of<IParameterSymbol>(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Valid_InitializesMapper()
    {
        var context = MapperContext<object>.Create();

        Target(context.Mapper, Mock.Of<IParameterSymbol>(), Mock.Of<object>());

        context.AddMappingsDelegateMock.Verify(static (addMappings) => addMappings.Invoke(It.IsAny<IAppendableSemanticMappingRepository<object>>()), Times.Once);
    }

    [Fact]
    public void Unmapped_ReturnsNullAndLogs()
    {
        var context = MapperContext<object>.Create();

        var recorder = Target(context.Mapper, Mock.Of<IParameterSymbol>(), Mock.Of<object>());

        Assert.Null(recorder);

        context.LoggerMock.Verify(static (logger) => logger.ConstructorParameter.FailedToMapConstructorParameter(), Times.Once);
    }

    [Fact]
    public void Mapped_ReturnsRecorderAndUsesDependencies()
    {
        var parameterName = string.Empty;
        var parameter = Mock.Of<IParameterSymbol>((symbol) => symbol.Name == parameterName);
        var dataRecord = Mock.Of<object>();

        var expectedRecorder = Mock.Of<IMappedSemanticConstructorArgumentRecorder>();
        var detachedRecorder = Mock.Of<IDetachedMappedSemanticConstructorArgumentRecorder<object>>();

        var context = MapperContext<object>.Create();

        context.RecorderFactoryMock.Setup(static (factory) => factory.ConstructorParameter.Create(It.IsAny<object>(), It.IsAny<IDetachedMappedSemanticConstructorArgumentRecorder<object>>())).Returns(expectedRecorder);
        context.RepositoryFactoryMock.Setup((factory) => factory.Create(It.IsAny<IEqualityComparer<string>>(), It.IsAny<bool>()).Build().ConstructorParameters.Named.TryGetValue(It.IsAny<string>(), out detachedRecorder)).Returns(true);

        var actualRecorder = Target(context.Mapper, parameter, dataRecord);

        Assert.Same(expectedRecorder, actualRecorder);

        context.RecorderFactoryMock.Verify((factory) => factory.ConstructorParameter.Create(dataRecord, detachedRecorder), Times.Once);
        context.RepositoryFactoryMock.Verify((factory) => factory.Create(context.ParameterNameComparer, true).Build().ConstructorParameters.Named.TryGetValue(parameterName, out detachedRecorder), Times.Once);
    }
}