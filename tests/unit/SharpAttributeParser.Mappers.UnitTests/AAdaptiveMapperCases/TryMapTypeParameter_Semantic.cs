namespace SharpAttributeParser.Mappers.AAdaptiveMapperCases;

using Microsoft.CodeAnalysis;

using Moq;

using SharpAttributeParser.Mappers.MappedRecorders;
using SharpAttributeParser.Mappers.Repositories;
using SharpAttributeParser.Mappers.Repositories.Adaptive;
using SharpAttributeParser.Mappers.Repositories.Semantic;

using System;

using Xunit;

public sealed class TryMapTypeParameter_Semantic
{
    private static IMappedSemanticTypeArgumentRecorder? Target<TCombinedRecord, TSemanticRecord>(AAdaptiveMapper<TCombinedRecord, TSemanticRecord> mapper, ITypeParameterSymbol parameter, TSemanticRecord dataRecord) => mapper.TryMapTypeParameter(parameter, dataRecord);

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

        context.SemanticLoggerMock.Verify(static (logger) => logger.TypeParameter.FailedToMapTypeParameter(), Times.Once);
    }

    [Fact]
    public void MappedByName_ReturnsRecorderAndUsesDependencies()
    {
        var parameterName = string.Empty;
        var parameter = Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == parameterName);
        var dataRecord = Mock.Of<object>();

        var expectedRecorder = Mock.Of<IMappedSemanticTypeArgumentRecorder>();
        Mock<IDetachedMappedAdaptiveTypeArgumentRecorderProvider<object, object>> detachedRecorderMock = new() { DefaultValue = DefaultValue.Mock };
        var detachedRecorder = detachedRecorderMock.Object;

        var context = MapperContext<object, object>.Create();

        context.SemanticRecorderFactoryMock.Setup(static (factory) => factory.TypeParameter.Create(It.IsAny<object>(), It.IsAny<IDetachedMappedSemanticTypeArgumentRecorder<object>>())).Returns(expectedRecorder);
        context.RepositoryFactoryMock.Setup((factory) => factory.Create(It.IsAny<IParameterComparer>(), It.IsAny<bool>()).Build().TypeParameters.Named.TryGetValue(It.IsAny<string>(), out detachedRecorder)).Returns(true);

        var actualRecorder = Target(context.Mapper, parameter, dataRecord);

        Assert.Same(expectedRecorder, actualRecorder);

        context.SemanticRecorderFactoryMock.Verify((factory) => factory.TypeParameter.Create(dataRecord, detachedRecorder.Semantic), Times.Once);
        context.RepositoryFactoryMock.Verify((factory) => factory.Create(context.ParameterComparer, true).Build().TypeParameters.Named.TryGetValue(parameterName, out detachedRecorder), Times.Once);
    }

    [Fact]
    public void MappedByIndex_ReturnsRecorderAndUsesDependencies()
    {
        var parameterIndex = 0;
        var parameter = Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Ordinal == parameterIndex);
        var dataRecord = Mock.Of<object>();

        var expectedRecorder = Mock.Of<IMappedSemanticTypeArgumentRecorder>();
        Mock<IDetachedMappedAdaptiveTypeArgumentRecorderProvider<object, object>> detachedRecorderMock = new() { DefaultValue = DefaultValue.Mock };
        var detachedRecorder = detachedRecorderMock.Object;

        var context = MapperContext<object, object>.Create();

        context.SemanticRecorderFactoryMock.Setup(static (factory) => factory.TypeParameter.Create(It.IsAny<object>(), It.IsAny<IDetachedMappedSemanticTypeArgumentRecorder<object>>())).Returns(expectedRecorder);
        context.RepositoryFactoryMock.Setup((factory) => factory.Create(It.IsAny<IParameterComparer>(), It.IsAny<bool>()).Build().TypeParameters.Indexed.TryGetValue(It.IsAny<int>(), out detachedRecorder)).Returns(true);

        var actualRecorder = Target(context.Mapper, parameter, dataRecord);

        Assert.Same(expectedRecorder, actualRecorder);

        context.SemanticRecorderFactoryMock.Verify((factory) => factory.TypeParameter.Create(dataRecord, detachedRecorder.Semantic), Times.Once);
        context.RepositoryFactoryMock.Verify((factory) => factory.Create(context.ParameterComparer, true).Build().TypeParameters.Indexed.TryGetValue(parameterIndex, out detachedRecorder), Times.Once);
    }
}
