namespace SharpAttributeParser.Mappers.ASplitMapperCases;

using Moq;

using SharpAttributeParser.Mappers.Repositories;
using SharpAttributeParser.Mappers.Repositories.Split;

using Xunit;

public sealed class InitializeMapper
{
    [Fact]
    public void RepositoryPassedToAddMappingsIsProducedByFactory()
    {
        var repository = Mock.Of<ISplitMappingRepository<object, object>>();

        var context = MapperContext<object, object>.Create();

        context.RepositoryFactoryMock.Setup((factory) => factory.Create(It.IsAny<IParameterComparer>(), It.IsAny<bool>())).Returns(repository);

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
