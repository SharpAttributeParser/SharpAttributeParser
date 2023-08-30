namespace SharpAttributeParser.Mappers;

using SharpAttributeParser.Mappers.Logging;
using SharpAttributeParser.Mappers.Repositories;
using SharpAttributeParser.Mappers.Repositories.Semantic;

using System;

/// <inheritdoc cref="ISemanticMapperDependencyProvider{TRecord}"/>
public sealed class SemanticMapperDependencyProvider<TRecord> : ISemanticMapperDependencyProvider<TRecord>
{
    private static Lazy<ISemanticMappingRepositoryFactory<TRecord>> DefaultRepositoryFactory { get; } = new(DefaultRepositoryFactories.SemanticFactory<TRecord>);

    private IParameterComparer ParameterComparer { get; }

    private IMappedSemanticArgumentRecorderFactory RecorderFactory { get; }
    private ISemanticMappingRepositoryFactory<TRecord> RepositoryFactory { get; }

    private ISemanticMapperLogger<ASemanticMapper<TRecord>> Logger { get; }

    /// <summary>Instantiates a <see cref="SemanticMapperDependencyProvider{TRecord}"/>, providing the dependencies of <see cref="ASemanticMapper{TRecord}"/>.</summary>
    /// <param name="parameterComparer">Determines equality when comparing parameters.</param>
    /// <param name="recorderFactory">Handles creation of recorders responsible for recording arguments of some specific parameter.</param>
    /// <param name="repositoryFactory">Handles creation of repositories.</param>
    /// <param name="logger">The logger used to log messages.</param>
    public SemanticMapperDependencyProvider(IParameterComparer? parameterComparer = null, IMappedSemanticArgumentRecorderFactory? recorderFactory = null, ISemanticMappingRepositoryFactory<TRecord>? repositoryFactory = null, ISemanticMapperLogger<ASemanticMapper<TRecord>>? logger = null)
    {
        ParameterComparer = parameterComparer ?? DefaultParameterComparer.Comparer;

        RecorderFactory = recorderFactory ?? DefaultRecorderFactories.SemanticFactory();
        RepositoryFactory = repositoryFactory ?? DefaultRepositoryFactory.Value;

        Logger = logger ?? NullSemanticMapperLogger<ASemanticMapper<TRecord>>.Singleton;
    }

    IParameterComparer ISemanticMapperDependencyProvider<TRecord>.ParameterComparer => ParameterComparer;

    IMappedSemanticArgumentRecorderFactory ISemanticMapperDependencyProvider<TRecord>.RecorderFactory => RecorderFactory;
    ISemanticMappingRepositoryFactory<TRecord> ISemanticMapperDependencyProvider<TRecord>.RepositoryFactory => RepositoryFactory;

    ISemanticMapperLogger<ASemanticMapper<TRecord>> ISemanticMapperDependencyProvider<TRecord>.Logger => Logger;
}
