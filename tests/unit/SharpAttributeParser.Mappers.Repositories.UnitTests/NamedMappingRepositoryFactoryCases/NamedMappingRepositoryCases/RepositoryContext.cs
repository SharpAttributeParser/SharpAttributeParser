namespace SharpAttributeParser.Mappers.Repositories.NamedMappingRepositoryFactoryCases.NamedMappingRepositoryCases;

using Moq;

using System;
using System.Collections.Generic;

internal sealed class RepositoryContext<TRecorder, TRecorderFactory> where TRecorderFactory : class
{
    public static RepositoryContext<TRecorder, TRecorderFactory> CreateWithOrdinalComparer(bool throwOnMultipleBuilds = false)
    {
        var factoryContext = FactoryContext<TRecorder, TRecorderFactory>.Create();

        Mock<IEqualityComparer<string>> parameterNameComparerMock = new();

        parameterNameComparerMock.Setup(static (comparer) => comparer.Equals(It.IsAny<string>(), It.IsAny<string>())).Returns<string, string>(static (x, y) => StringComparer.Ordinal.Equals(x, y));

        var repository = ((INamedMappingRepositoryFactory<TRecorder, TRecorderFactory>)factoryContext.Factory).Create(parameterNameComparerMock.Object, throwOnMultipleBuilds);

        return new(repository, parameterNameComparerMock, factoryContext);
    }

    public static RepositoryContext<TRecorder, TRecorderFactory> Create(bool throwOnMultipleBuilds = false)
    {
        var factoryContext = FactoryContext<TRecorder, TRecorderFactory>.Create();

        Mock<IEqualityComparer<string>> parameterNameComparerMock = new();

        var repository = ((INamedMappingRepositoryFactory<TRecorder, TRecorderFactory>)factoryContext.Factory).Create(parameterNameComparerMock.Object, throwOnMultipleBuilds);

        return new(repository, parameterNameComparerMock, factoryContext);
    }

    public INamedMappingRepository<TRecorder, TRecorderFactory> Repository { get; }

    public Mock<IEqualityComparer<string>> ParameterNameComparerMock { get; }

    public FactoryContext<TRecorder, TRecorderFactory> FactoryContext { get; }

    private RepositoryContext(INamedMappingRepository<TRecorder, TRecorderFactory> repository, Mock<IEqualityComparer<string>> parameterNameComparerMock, FactoryContext<TRecorder, TRecorderFactory> factoryContext)
    {
        Repository = repository;

        ParameterNameComparerMock = parameterNameComparerMock;

        FactoryContext = factoryContext;
    }
}
