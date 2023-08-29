namespace SharpAttributeParser.Mappers.Repositories.ConstructorMappingRepositoryFactoryCases;

using Moq;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class Create
{
    private static IConstructorMappingRepository<TRecorder, TRecorderFactory> Target<TRecorder, TRecorderFactory>(ConstructorMappingRepositoryFactory<TRecorder, TRecorderFactory> factory, IEqualityComparer<string> parameterNameComparer, bool throwOnMultipleBuilds) => ((IConstructorMappingRepositoryFactory<TRecorder, TRecorderFactory>)factory).Create(parameterNameComparer, throwOnMultipleBuilds);

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
        var context = FactoryContext<object, object>.Create();

        var repository = Target(context.Factory, Mock.Of<IEqualityComparer<string>>(), true);

        Assert.NotNull(repository);
    }
}
