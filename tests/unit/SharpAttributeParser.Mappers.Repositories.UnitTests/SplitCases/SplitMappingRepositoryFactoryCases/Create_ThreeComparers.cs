namespace SharpAttributeParser.Mappers.Repositories.SplitCases.SplitMappingRepositoryFactoryCases;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Split;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class Create_ThreeComparer
{
    private static ISplitMappingRepository<TSplitRecord, TSyntacticRecord> Target<TSplitRecord, TSyntacticRecord>(ISplitMappingRepositoryFactory<TSplitRecord, TSyntacticRecord> factory, IEqualityComparer<string> typeParameterNameComparer, IEqualityComparer<string> constructorParameterNameComparer, IEqualityComparer<string> namedParameterNameComparer, bool throwOnMultipleBuilds) => factory.Create(typeParameterNameComparer, constructorParameterNameComparer, namedParameterNameComparer, throwOnMultipleBuilds);

    [Fact]
    public void NullTypeParameterNameComparer_ArgumentNullException()
    {
        var context = FactoryContext<object, object>.Create();

        var exception = Record.Exception(() => Target(context.Factory, null!, Mock.Of<IEqualityComparer<string>>(), Mock.Of<IEqualityComparer<string>>(), true));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullConstructorParameterNameComparer_ArgumentNullException()
    {
        var context = FactoryContext<object, object>.Create();

        var exception = Record.Exception(() => Target(context.Factory, Mock.Of<IEqualityComparer<string>>(), null!, Mock.Of<IEqualityComparer<string>>(), true));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullNamedParameterNameComparer_ArgumentNullException()
    {
        var context = FactoryContext<object, object>.Create();

        var exception = Record.Exception(() => Target(context.Factory, Mock.Of<IEqualityComparer<string>>(), Mock.Of<IEqualityComparer<string>>(), null!, true));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Valid_CreatesRepository()
    {
        var typeParameterNameComparer = Mock.Of<IEqualityComparer<string>>();
        var constructorParameterNameComparer = Mock.Of<IEqualityComparer<string>>();
        var namedParameterNameComparer = Mock.Of<IEqualityComparer<string>>();

        var throwOnMultipleBuilds = true;

        var context = FactoryContext<object, object>.Create();

        context.TypeRepositoryFactoryMock.Setup(static (factory) => factory.Create(It.IsAny<IEqualityComparer<string>>(), It.IsAny<bool>())).Returns(Mock.Of<ITypeMappingRepository<IDetachedMappedSplitTypeArgumentRecorderProvider<object, object>, IDetachedMappedSplitTypeArgumentRecorderProviderFactory<object, object>>>());
        context.ConstructorRepositoryFactoryMock.Setup(static (factory) => factory.Create(It.IsAny<IEqualityComparer<string>>(), It.IsAny<bool>())).Returns(Mock.Of<IConstructorMappingRepository<IDetachedMappedSplitConstructorArgumentRecorderProvider<object, object>, IDetachedMappedSplitConstructorArgumentRecorderProviderFactory<object, object>>>());
        context.NamedRepositoryFactoryMock.Setup(static (factory) => factory.Create(It.IsAny<IEqualityComparer<string>>(), It.IsAny<bool>())).Returns(Mock.Of<INamedMappingRepository<IDetachedMappedSplitNamedArgumentRecorderProvider<object, object>, IDetachedMappedSplitNamedArgumentRecorderProviderFactory<object, object>>>());

        var repository = Target(context.Factory, typeParameterNameComparer, constructorParameterNameComparer, namedParameterNameComparer, throwOnMultipleBuilds);

        Assert.NotNull(repository);

        context.TypeRepositoryFactoryMock.Verify((factory) => factory.Create(typeParameterNameComparer, throwOnMultipleBuilds), Times.Once);
        context.ConstructorRepositoryFactoryMock.Verify((factory) => factory.Create(constructorParameterNameComparer, throwOnMultipleBuilds), Times.Once);
        context.NamedRepositoryFactoryMock.Verify((factory) => factory.Create(namedParameterNameComparer, throwOnMultipleBuilds), Times.Once);
    }
}
