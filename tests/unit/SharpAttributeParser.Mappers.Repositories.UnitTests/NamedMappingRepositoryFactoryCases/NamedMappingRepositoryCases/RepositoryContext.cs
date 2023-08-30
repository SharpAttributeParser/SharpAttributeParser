namespace SharpAttributeParser.Mappers.Repositories.NamedMappingRepositoryFactoryCases.NamedMappingRepositoryCases;

using Moq;

using System;

internal sealed class RepositoryContext<TRecorder, TRecorderFactory> where TRecorderFactory : class
{
    public static RepositoryContext<TRecorder, TRecorderFactory> CreateWithOrdinalComparer(bool throwOnMultipleBuilds = false)
    {
        var factoryContext = FactoryContext<TRecorder, TRecorderFactory>.Create();

        Mock<INamedParameterComparer> comparerMock = new();

        comparerMock.Setup(static (comparer) => comparer.Name.Equals(It.IsAny<string>(), It.IsAny<string>())).Returns<string, string>(static (x, y) => StringComparer.Ordinal.Equals(x, y));

        var repository = ((INamedMappingRepositoryFactory<TRecorder, TRecorderFactory>)factoryContext.Factory).Create(comparerMock.Object, throwOnMultipleBuilds);

        return new(repository, comparerMock, factoryContext);
    }

    public static RepositoryContext<TRecorder, TRecorderFactory> Create(bool throwOnMultipleBuilds = false)
    {
        var factoryContext = FactoryContext<TRecorder, TRecorderFactory>.Create();

        Mock<INamedParameterComparer> comparerMock = new() { DefaultValue = DefaultValue.Mock };

        var repository = ((INamedMappingRepositoryFactory<TRecorder, TRecorderFactory>)factoryContext.Factory).Create(comparerMock.Object, throwOnMultipleBuilds);

        return new(repository, comparerMock, factoryContext);
    }

    public INamedMappingRepository<TRecorder, TRecorderFactory> Repository { get; }

    public Mock<INamedParameterComparer> ComparerMock { get; }

    public FactoryContext<TRecorder, TRecorderFactory> FactoryContext { get; }

    private RepositoryContext(INamedMappingRepository<TRecorder, TRecorderFactory> repository, Mock<INamedParameterComparer> comparerMock, FactoryContext<TRecorder, TRecorderFactory> factoryContext)
    {
        Repository = repository;

        ComparerMock = comparerMock;

        FactoryContext = factoryContext;
    }
}
