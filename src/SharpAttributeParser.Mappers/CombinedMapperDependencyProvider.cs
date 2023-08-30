namespace SharpAttributeParser.Mappers;

using SharpAttributeParser.Mappers.Logging;
using SharpAttributeParser.Mappers.Repositories;
using SharpAttributeParser.Mappers.Repositories.Combined;

using System;

/// <inheritdoc cref="ICombinedMapperDependencyProvider{TRecord}"/>
public sealed class CombinedMapperDependencyProvider<TRecord> : ICombinedMapperDependencyProvider<TRecord>
{
    private static Lazy<ICombinedMappingRepositoryFactory<TRecord>> DefaultRepositoryFactory { get; } = new(DefaultRepositoryFactories.CombinedFactory<TRecord>);

    private IParameterComparer ParameterComparer { get; }

    private IMappedCombinedArgumentRecorderFactory RecorderFactory { get; }
    private ICombinedMappingRepositoryFactory<TRecord> RepositoryFactory { get; }

    private ICombinedMapperLogger<ACombinedMapper<TRecord>> Logger { get; }

    /// <summary>Instantiates a <see cref="CombinedMapperDependencyProvider{TRecord}"/>, providing the dependencies of <see cref="ACombinedMapper{TRecord}"/>.</summary>
    /// <param name="parameterComparer">Determines equality when comparing parameters.</param>
    /// <param name="recorderFactory">Handles creation of recorders responsible for recording arguments, together with syntactic information about arguments, of some specific parameter.</param>
    /// <param name="repositoryFactory">Handles creation of repositories.</param>
    /// <param name="logger">The logger used to log messages.</param>
    public CombinedMapperDependencyProvider(IParameterComparer? parameterComparer = null, IMappedCombinedArgumentRecorderFactory? recorderFactory = null, ICombinedMappingRepositoryFactory<TRecord>? repositoryFactory = null, ICombinedMapperLogger<ACombinedMapper<TRecord>>? logger = null)
    {
        ParameterComparer = parameterComparer ?? DefaultParameterComparer.Comparer;

        RecorderFactory = recorderFactory ?? DefaultRecorderFactories.CombinedFactory();
        RepositoryFactory = repositoryFactory ?? DefaultRepositoryFactory.Value;

        Logger = logger ?? NullCombinedMapperLogger<ACombinedMapper<TRecord>>.Singleton;
    }

    IParameterComparer ICombinedMapperDependencyProvider<TRecord>.ParameterComparer => ParameterComparer;

    IMappedCombinedArgumentRecorderFactory ICombinedMapperDependencyProvider<TRecord>.RecorderFactory => RecorderFactory;
    ICombinedMappingRepositoryFactory<TRecord> ICombinedMapperDependencyProvider<TRecord>.RepositoryFactory => RepositoryFactory;

    ICombinedMapperLogger<ACombinedMapper<TRecord>> ICombinedMapperDependencyProvider<TRecord>.Logger => Logger;
}
