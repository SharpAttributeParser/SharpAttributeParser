namespace SharpAttributeParser.Mappers.Repositories.SplitCases.SplitMappingRepositoryFactoryCases;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Split;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class Create_OneComparer
{
    private static ISplitMappingRepository<TSplitRecord, TSyntacticRecord> Target<TSplitRecord, TSyntacticRecord>(ISplitMappingRepositoryFactory<TSplitRecord, TSyntacticRecord> factory, IEqualityComparer<string> parameterNameComparer, bool throwOnMultipleBuilds) => factory.Create(parameterNameComparer, throwOnMultipleBuilds);

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
        var parameterNameComparer = Mock.Of<IEqualityComparer<string>>();
        var throwOnMultipleBuilds = true;

        var context = FactoryContext<object, object>.Create();

        context.TypeRepositoryFactoryMock.Setup(static (factory) => factory.Create(It.IsAny<IEqualityComparer<string>>(), It.IsAny<bool>())).Returns(Mock.Of<ITypeMappingRepository<IDetachedMappedSplitTypeArgumentRecorderProvider<object, object>, IDetachedMappedSplitTypeArgumentRecorderProviderFactory<object, object>>>());
        context.ConstructorRepositoryFactoryMock.Setup(static (factory) => factory.Create(It.IsAny<IEqualityComparer<string>>(), It.IsAny<bool>())).Returns(Mock.Of<IConstructorMappingRepository<IDetachedMappedSplitConstructorArgumentRecorderProvider<object, object>, IDetachedMappedSplitConstructorArgumentRecorderProviderFactory<object, object>>>());
        context.NamedRepositoryFactoryMock.Setup(static (factory) => factory.Create(It.IsAny<IEqualityComparer<string>>(), It.IsAny<bool>())).Returns(Mock.Of<INamedMappingRepository<IDetachedMappedSplitNamedArgumentRecorderProvider<object, object>, IDetachedMappedSplitNamedArgumentRecorderProviderFactory<object, object>>>());

        var repository = Target(context.Factory, parameterNameComparer, throwOnMultipleBuilds);

        Assert.NotNull(repository);

        context.TypeRepositoryFactoryMock.Verify((factory) => factory.Create(parameterNameComparer, throwOnMultipleBuilds), Times.Once);
        context.ConstructorRepositoryFactoryMock.Verify((factory) => factory.Create(parameterNameComparer, throwOnMultipleBuilds), Times.Once);
        context.NamedRepositoryFactoryMock.Verify((factory) => factory.Create(parameterNameComparer, throwOnMultipleBuilds), Times.Once);
    }
}
