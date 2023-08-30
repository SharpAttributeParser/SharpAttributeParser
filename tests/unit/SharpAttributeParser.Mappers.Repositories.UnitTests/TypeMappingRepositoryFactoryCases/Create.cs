namespace SharpAttributeParser.Mappers.Repositories.TypeMappingRepositoryFactoryCases;

using Moq;

using System;

using Xunit;

public sealed class Create
{
    private static ITypeMappingRepository<TRecorder, TRecorderFactory> Target<TRecorder, TRecorderFactory>(TypeMappingRepositoryFactory<TRecorder, TRecorderFactory> factory, ITypeParameterComparer comparer, bool throwOnMultipleBuilds) => ((ITypeMappingRepositoryFactory<TRecorder, TRecorderFactory>)factory).Create(comparer, throwOnMultipleBuilds);

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

        var repository = Target(context.Factory, Mock.Of<ITypeParameterComparer>(), true);

        Assert.NotNull(repository);
    }
}
