namespace SharpAttributeParser.Mappers;

using SharpAttributeParser.Mappers.Logging;
using SharpAttributeParser.Mappers.Repositories;
using SharpAttributeParser.Mappers.Repositories.Semantic;

using System;

/// <inheritdoc cref="ISemanticMapperDependencyProvider{TRecord}"/>
public sealed class SemanticMapperDependencyProvider<TRecord> : ISemanticMapperDependencyProvider<TRecord>
{
    private static readonly Lazy<ISemanticMappingRepositoryFactory<TRecord>> DefaultRepositoryFactory = new(DefaultRepositoryFactories.SemanticFactory<TRecord>);

    private readonly IParameterComparer ParameterComparer;

    private readonly IMappedSemanticArgumentRecorderFactory RecorderFactory;
    private readonly ISemanticMappingRepositoryFactory<TRecord> RepositoryFactory;

    private readonly ISemanticMapperLogger<ASemanticMapper<TRecord>> Logger;

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
