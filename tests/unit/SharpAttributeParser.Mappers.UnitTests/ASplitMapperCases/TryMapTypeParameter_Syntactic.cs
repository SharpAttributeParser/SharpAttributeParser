namespace SharpAttributeParser.Mappers.ASplitMapperCases;

using Microsoft.CodeAnalysis;

using Moq;

using SharpAttributeParser.Mappers.MappedRecorders;
using SharpAttributeParser.Mappers.Repositories.Split;
using SharpAttributeParser.Mappers.Repositories.Syntactic;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class TryMapTypeParameter_Syntactic
{
    private static IMappedSyntacticTypeArgumentRecorder? Target<TSemanticRecord, TSyntacticRecord>(ASplitMapper<TSemanticRecord, TSyntacticRecord> mapper, ITypeParameterSymbol parameter, TSyntacticRecord dataRecord) => mapper.TryMapTypeParameter(parameter, dataRecord);

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

        context.AddMappingsDelegateMock.Verify(static (addMappings) => addMappings.Invoke(It.IsAny<IAppendableSplitMappingRepository<object, object>>()), Times.Once);
    }

    [Fact]
    public void Unmapped_ReturnsNullAndLogs()
    {
        var context = MapperContext<object, object>.Create();

        var recorder = Target(context.Mapper, Mock.Of<ITypeParameterSymbol>(), Mock.Of<object>());

        Assert.Null(recorder);

        context.SyntacticLoggerMock.Verify(static (logger) => logger.TypeParameter.FailedToMapTypeParameter(), Times.Once);
    }

    [Fact]
    public void MappedByName_ReturnsRecorderAndUsesDependencies()
    {
        var parameterName = string.Empty;
        var parameter = Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == parameterName);
        var dataRecord = Mock.Of<object>();

        var expectedRecorder = Mock.Of<IMappedSyntacticTypeArgumentRecorder>();
        Mock<IDetachedMappedSplitTypeArgumentRecorderProvider<object, object>> detachedRecorderMock = new() { DefaultValue = DefaultValue.Mock };
        var detachedRecorder = detachedRecorderMock.Object;

        var context = MapperContext<object, object>.Create();

        context.SyntacticRecorderFactoryMock.Setup(static (factory) => factory.TypeParameter.Create(It.IsAny<object>(), It.IsAny<IDetachedMappedSyntacticTypeArgumentRecorder<object>>())).Returns(expectedRecorder);
        context.RepositoryFactoryMock.Setup((factory) => factory.Create(It.IsAny<IEqualityComparer<string>>(), It.IsAny<bool>()).Build().TypeParameters.Named.TryGetValue(It.IsAny<string>(), out detachedRecorder)).Returns(true);

        var actualRecorder = Target(context.Mapper, parameter, dataRecord);

        Assert.Same(expectedRecorder, actualRecorder);

        context.SyntacticRecorderFactoryMock.Verify((factory) => factory.TypeParameter.Create(dataRecord, detachedRecorder.Syntactic), Times.Once);
        context.RepositoryFactoryMock.Verify((factory) => factory.Create(context.ParameterNameComparer, true).Build().TypeParameters.Named.TryGetValue(parameterName, out detachedRecorder), Times.Once);
    }

    [Fact]
    public void MappedByIndex_ReturnsRecorderAndUsesDependencies()
    {
        var parameterIndex = 0;
        var parameter = Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Ordinal == parameterIndex);
        var dataRecord = Mock.Of<object>();

        var expectedRecorder = Mock.Of<IMappedSyntacticTypeArgumentRecorder>();
        Mock<IDetachedMappedSplitTypeArgumentRecorderProvider<object, object>> detachedRecorderMock = new() { DefaultValue = DefaultValue.Mock };
        var detachedRecorder = detachedRecorderMock.Object;

        var context = MapperContext<object, object>.Create();

        context.SyntacticRecorderFactoryMock.Setup(static (factory) => factory.TypeParameter.Create(It.IsAny<object>(), It.IsAny<IDetachedMappedSyntacticTypeArgumentRecorder<object>>())).Returns(expectedRecorder);
        context.RepositoryFactoryMock.Setup((factory) => factory.Create(It.IsAny<IEqualityComparer<string>>(), It.IsAny<bool>()).Build().TypeParameters.Indexed.TryGetValue(It.IsAny<int>(), out detachedRecorder)).Returns(true);

        var actualRecorder = Target(context.Mapper, parameter, dataRecord);

        Assert.Same(expectedRecorder, actualRecorder);

        context.SyntacticRecorderFactoryMock.Verify((factory) => factory.TypeParameter.Create(dataRecord, detachedRecorder.Syntactic), Times.Once);
        context.RepositoryFactoryMock.Verify((factory) => factory.Create(context.ParameterNameComparer, true).Build().TypeParameters.Indexed.TryGetValue(parameterIndex, out detachedRecorder), Times.Once);
    }
}
