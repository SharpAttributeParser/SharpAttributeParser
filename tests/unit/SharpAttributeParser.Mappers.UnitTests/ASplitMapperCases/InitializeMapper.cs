namespace SharpAttributeParser.Mappers.ASplitMapperCases;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Split;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class InitializeMapper
{
    [Fact]
    public void NullReturningRepositoryFactory_InvalidOperationException()
    {
        var context = MapperContext<object, object>.Create();

        context.RepositoryFactoryMock.Setup((factory) => factory.Create(It.IsAny<IEqualityComparer<string>>(), It.IsAny<bool>())).Returns((ISplitMappingRepository<object, object>)null!);

        var exception = Record.Exception(context.Mapper.Initialize);

        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void RepositoryPassedToAddMappingsIsProducedByFactory()
    {
        var repository = Mock.Of<ISplitMappingRepository<object, object>>();

        var context = MapperContext<object, object>.Create();

        context.RepositoryFactoryMock.Setup((factory) => factory.Create(It.IsAny<IEqualityComparer<string>>(), It.IsAny<bool>())).Returns(repository);

        context.Mapper.Initialize();

        context.AddMappingsDelegateMock.Verify((addMappings) => addMappings.Invoke(repository), Times.Once);
    }

    [Fact]
    public void MultipleInvokations_AddMappingsOnlyInvokedOnce()
    {
        var context = MapperContext<object, object>.Create();

        context.Mapper.Initialize();
        context.Mapper.Initialize();

        context.AddMappingsDelegateMock.Verify((addMappings) => addMappings.Invoke(It.IsAny<IAppendableSplitMappingRepository<object, object>>()), Times.Once);
    }
}
