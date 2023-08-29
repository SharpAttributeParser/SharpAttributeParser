﻿namespace SharpAttributeParser.Mappers.AAdaptiveMapperCases;

using Microsoft.CodeAnalysis;

using Moq;

using SharpAttributeParser.Mappers.MappedRecorders;
using SharpAttributeParser.Mappers.Repositories.Adaptive;
using SharpAttributeParser.Mappers.Repositories.Combined;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class TryMapTypeParameter_Combined
{
    private static IMappedCombinedTypeArgumentRecorder? Target<TCombinedRecord, TSemanticRecord>(AAdaptiveMapper<TCombinedRecord, TSemanticRecord> mapper, ITypeParameterSymbol parameter, TCombinedRecord dataRecord) => mapper.TryMapTypeParameter(parameter, dataRecord);

    [Fact]
    public void NullParameter_ArgumentNullException()
    {
        var context = MapperContext<object, object>.Create();

        var exception = Record.Exception(() => Target(context.Mapper, null!, Mock.Of<object>()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullDataRecord_ArgumentNullException()
    {
        var context = MapperContext<object, object>.Create();

        var exception = Record.Exception(() => Target(context.Mapper, Mock.Of<ITypeParameterSymbol>(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Valid_InitializesMapper()
    {
        var context = MapperContext<object, object>.Create();

        Target(context.Mapper, Mock.Of<ITypeParameterSymbol>(), Mock.Of<object>());

        context.AddMappingsDelegateMock.Verify(static (addMappings) => addMappings.Invoke(It.IsAny<IAppendableAdaptiveMappingRepository<object, object>>()), Times.Once);
    }

    [Fact]
    public void Unmapped_ReturnsNullAndLogs()
    {
        var context = MapperContext<object, object>.Create();

        var recorder = Target(context.Mapper, Mock.Of<ITypeParameterSymbol>(), Mock.Of<object>());

        Assert.Null(recorder);

        context.CombinedLoggerMock.Verify(static (logger) => logger.TypeParameter.FailedToMapTypeParameter(), Times.Once);
    }

    [Fact]
    public void MappedByName_ReturnsRecorderAndUsesDependencies()
    {
        var parameterName = string.Empty;
        var parameter = Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == parameterName);
        var dataRecord = Mock.Of<object>();

        var expectedRecorder = Mock.Of<IMappedCombinedTypeArgumentRecorder>();
        Mock<IDetachedMappedAdaptiveTypeArgumentRecorderProvider<object, object>> detachedRecorderMock = new() { DefaultValue = DefaultValue.Mock };
        var detachedRecorder = detachedRecorderMock.Object;

        var context = MapperContext<object, object>.Create();

        context.CombinedRecorderFactoryMock.Setup(static (factory) => factory.TypeParameter.Create(It.IsAny<object>(), It.IsAny<IDetachedMappedCombinedTypeArgumentRecorder<object>>())).Returns(expectedRecorder);
        context.RepositoryFactoryMock.Setup((factory) => factory.Create(It.IsAny<IEqualityComparer<string>>(), It.IsAny<bool>()).Build().TypeParameters.Named.TryGetValue(It.IsAny<string>(), out detachedRecorder)).Returns(true);

        var actualRecorder = Target(context.Mapper, parameter, dataRecord);

        Assert.Same(expectedRecorder, actualRecorder);

        context.CombinedRecorderFactoryMock.Verify((factory) => factory.TypeParameter.Create(dataRecord, detachedRecorder.Combined), Times.Once);
        context.RepositoryFactoryMock.Verify((factory) => factory.Create(context.ParameterNameComparer, true).Build().TypeParameters.Named.TryGetValue(parameterName, out detachedRecorder), Times.Once);
    }

    [Fact]
    public void MappedByIndex_ReturnsRecorderAndUsesDependencies()
    {
        var parameterIndex = 0;
        var parameter = Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Ordinal == parameterIndex);
        var dataRecord = Mock.Of<object>();

        var expectedRecorder = Mock.Of<IMappedCombinedTypeArgumentRecorder>();
        Mock<IDetachedMappedAdaptiveTypeArgumentRecorderProvider<object, object>> detachedRecorderMock = new() { DefaultValue = DefaultValue.Mock };
        var detachedRecorder = detachedRecorderMock.Object;

        var context = MapperContext<object, object>.Create();

        context.CombinedRecorderFactoryMock.Setup(static (factory) => factory.TypeParameter.Create(It.IsAny<object>(), It.IsAny<IDetachedMappedCombinedTypeArgumentRecorder<object>>())).Returns(expectedRecorder);
        context.RepositoryFactoryMock.Setup((factory) => factory.Create(It.IsAny<IEqualityComparer<string>>(), It.IsAny<bool>()).Build().TypeParameters.Indexed.TryGetValue(It.IsAny<int>(), out detachedRecorder)).Returns(true);

        var actualRecorder = Target(context.Mapper, parameter, dataRecord);

        Assert.Same(expectedRecorder, actualRecorder);

        context.CombinedRecorderFactoryMock.Verify((factory) => factory.TypeParameter.Create(dataRecord, detachedRecorder.Combined), Times.Once);
        context.RepositoryFactoryMock.Verify((factory) => factory.Create(context.ParameterNameComparer, true).Build().TypeParameters.Indexed.TryGetValue(parameterIndex, out detachedRecorder), Times.Once);
    }
}