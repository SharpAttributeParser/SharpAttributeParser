namespace SharpAttributeParser.Mappers.Repositories.AdaptiveCases.AdaptiveMappingRepositoryFactoryCases;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Adaptive;

using System;

using Xunit;

public sealed class Create
{
    private static IAdaptiveMappingRepository<TAdaptiveRecord, TSemanticRecord> Target<TAdaptiveRecord, TSemanticRecord>(IAdaptiveMappingRepositoryFactory<TAdaptiveRecord, TSemanticRecord> factory, IParameterComparer comparer, bool throwOnMultipleBuilds) => factory.Create(comparer, throwOnMultipleBuilds);

    [Fact]
    public void NullParameterNameComparer_ArgumentNullException()
    {
        var context = FactoryContext<object, object>.Create();

        var exception = Record.Exception(() => Target(context.Factory, null!, true));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Valid_CreatesRepository()
    {
        Mock<IParameterComparer> comparerMock = new() { DefaultValue = DefaultValue.Mock };
        var throwOnMultipleBuilds = true;

        var context = FactoryContext<object, object>.Create();

        context.TypeRepositoryFactoryMock.Setup(static (factory) => factory.Create(It.IsAny<ITypeParameterComparer>(), It.IsAny<bool>())).Returns(Mock.Of<ITypeMappingRepository<IDetachedMappedAdaptiveTypeArgumentRecorderProvider<object, object>, IDetachedMappedAdaptiveTypeArgumentRecorderProviderFactory<object, object>>>());
        context.ConstructorRepositoryFactoryMock.Setup(static (factory) => factory.Create(It.IsAny<IConstructorParameterComparer>(), It.IsAny<bool>())).Returns(Mock.Of<IConstructorMappingRepository<IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<object, object>, IDetachedMappedAdaptiveConstructorArgumentRecorderProviderFactory<object, object>>>());
        context.NamedRepositoryFactoryMock.Setup(static (factory) => factory.Create(It.IsAny<INamedParameterComparer>(), It.IsAny<bool>())).Returns(Mock.Of<INamedMappingRepository<IDetachedMappedAdaptiveNamedArgumentRecorderProvider<object, object>, IDetachedMappedAdaptiveNamedArgumentRecorderProviderFactory<object, object>>>());

        var repository = Target(context.Factory, comparerMock.Object, throwOnMultipleBuilds);

        Assert.NotNull(repository);

        context.TypeRepositoryFactoryMock.Verify((factory) => factory.Create(comparerMock.Object.TypeParameter, throwOnMultipleBuilds), Times.Once);
        context.ConstructorRepositoryFactoryMock.Verify((factory) => factory.Create(comparerMock.Object.ConstructorParameter, throwOnMultipleBuilds), Times.Once);
        context.NamedRepositoryFactoryMock.Verify((factory) => factory.Create(comparerMock.Object.NamedParameter, throwOnMultipleBuilds), Times.Once);
    }
}
