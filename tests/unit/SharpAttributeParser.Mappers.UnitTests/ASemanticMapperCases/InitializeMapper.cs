namespace SharpAttributeParser.Mappers.ASemanticMapperCases;

using Moq;

using SharpAttributeParser.Mappers.Repositories;
using SharpAttributeParser.Mappers.Repositories.Semantic;

using System;

using Xunit;

public sealed class InitializeMapper
{
    [Fact]
    public void NullReturningRepositoryFactory_InvalidOperationException()
    {
        var context = MapperContext<object>.Create();

        context.RepositoryFactoryMock.Setup((factory) => factory.Create(It.IsAny<IParameterComparer>(), It.IsAny<bool>())).Returns((ISemanticMappingRepository<object>)null!);

        var exception = Record.Exception(context.Mapper.Initialize);

        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void RepositoryPassedToAddMappingsIsProducedByFactory()
    {
        var repository = Mock.Of<ISemanticMappingRepository<object>>();

        var context = MapperContext<object>.Create();

        context.RepositoryFactoryMock.Setup((factory) => factory.Create(It.IsAny<IParameterComparer>(), It.IsAny<bool>())).Returns(repository);

        context.Mapper.Initialize();

        context.AddMappingsDelegateMock.Verify((addMappings) => addMappings.Invoke(repository), Times.Once);
    }

    [Fact]
    public void MultipleInvokations_AddMappingsOnlyInvokedOnce()
    {
        var context = MapperContext<object>.Create();

        context.Mapper.Initialize();
        context.Mapper.Initialize();

        context.AddMappingsDelegateMock.Verify((addMappings) => addMappings.Invoke(It.IsAny<IAppendableSemanticMappingRepository<object>>()), Times.Once);
    }
}
