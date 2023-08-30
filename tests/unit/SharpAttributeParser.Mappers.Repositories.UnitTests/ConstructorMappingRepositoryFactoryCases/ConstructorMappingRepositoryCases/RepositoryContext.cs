namespace SharpAttributeParser.Mappers.Repositories.ConstructorMappingRepositoryFactoryCases.ConstructorMappingRepositoryCases;

using Moq;

using System;

internal sealed class RepositoryContext<TRecorder, TRecorderFactory> where TRecorderFactory : class
{
    public static RepositoryContext<TRecorder, TRecorderFactory> CreateWithOrdinalComparer(bool throwOnMultipleBuilds = false)
    {
        var factoryContext = FactoryContext<TRecorder, TRecorderFactory>.Create();

        Mock<IConstructorParameterComparer> comparerMock = new();

        comparerMock.Setup(static (comparer) => comparer.Name.Equals(It.IsAny<string>(), It.IsAny<string>())).Returns<string, string>(static (x, y) => StringComparer.Ordinal.Equals(x, y));

        var repository = ((IConstructorMappingRepositoryFactory<TRecorder, TRecorderFactory>)factoryContext.Factory).Create(comparerMock.Object, throwOnMultipleBuilds);

        return new(repository, comparerMock, factoryContext);
    }

    public static RepositoryContext<TRecorder, TRecorderFactory> Create(bool throwOnMultipleBuilds = false)
    {
        var factoryContext = FactoryContext<TRecorder, TRecorderFactory>.Create();

        Mock<IConstructorParameterComparer> comparerMock = new() { DefaultValue = DefaultValue.Mock };

        var repository = ((IConstructorMappingRepositoryFactory<TRecorder, TRecorderFactory>)factoryContext.Factory).Create(comparerMock.Object, throwOnMultipleBuilds);

        return new(repository, comparerMock, factoryContext);
    }

    public IConstructorMappingRepository<TRecorder, TRecorderFactory> Repository { get; }

    public Mock<IConstructorParameterComparer> ComparerMock { get; }

    public FactoryContext<TRecorder, TRecorderFactory> FactoryContext { get; }

    private RepositoryContext(IConstructorMappingRepository<TRecorder, TRecorderFactory> repository, Mock<IConstructorParameterComparer> comparerMock, FactoryContext<TRecorder, TRecorderFactory> factoryContext)
    {
        Repository = repository;

        ComparerMock = comparerMock;

        FactoryContext = factoryContext;
    }
}
