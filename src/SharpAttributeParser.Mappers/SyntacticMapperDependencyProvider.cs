namespace SharpAttributeParser.Mappers;

using SharpAttributeParser.Mappers.Logging;
using SharpAttributeParser.Mappers.Repositories;
using SharpAttributeParser.Mappers.Repositories.Syntactic;

using System;

/// <inheritdoc cref="ISyntacticMapperDependencyProvider{TRecord}"/>
public sealed class SyntacticMapperDependencyProvider<TRecord> : ISyntacticMapperDependencyProvider<TRecord>
{
    private static Lazy<ISyntacticMappingRepositoryFactory<TRecord>> DefaultRepositoryFactory { get; } = new(DefaultRepositoryFactories.SyntacticFactory<TRecord>);

    private IParameterComparer ParameterComparer { get; }

    private IMappedSyntacticArgumentRecorderFactory RecorderFactory { get; }
    private ISyntacticMappingRepositoryFactory<TRecord> RepositoryFactory { get; }

    private ISyntacticMapperLogger<ASyntacticMapper<TRecord>> Logger { get; }

    /// <summary>Instantiates a <see cref="SyntacticMapperDependencyProvider{TRecord}"/>, providing the dependencies of <see cref="ASyntacticMapper{TRecord}"/>.</summary>
    /// <param name="parameterComparer">Determines equality when comparing parameters.</param>
    /// <param name="recorderFactory">Handles creation of recorders responsible for recording syntactic information about arguments of some specific parameter.</param>
    /// <param name="repositoryFactory">Handles creation of repositories.</param>
    /// <param name="logger">The logger used to log messages.</param>
    public SyntacticMapperDependencyProvider(IParameterComparer? parameterComparer = null, IMappedSyntacticArgumentRecorderFactory? recorderFactory = null, ISyntacticMappingRepositoryFactory<TRecord>? repositoryFactory = null, ISyntacticMapperLogger<ASyntacticMapper<TRecord>>? logger = null)
    {
        ParameterComparer = parameterComparer ?? DefaultParameterComparer.Comparer;

        RecorderFactory = recorderFactory ?? DefaultRecorderFactories.SyntacticFactory();
        RepositoryFactory = repositoryFactory ?? DefaultRepositoryFactory.Value;

        Logger = logger ?? NullSyntacticMapperLogger<ASyntacticMapper<TRecord>>.Singleton;
    }

    IParameterComparer ISyntacticMapperDependencyProvider<TRecord>.ParameterComparer => ParameterComparer;

    IMappedSyntacticArgumentRecorderFactory ISyntacticMapperDependencyProvider<TRecord>.RecorderFactory => RecorderFactory;
    ISyntacticMappingRepositoryFactory<TRecord> ISyntacticMapperDependencyProvider<TRecord>.RepositoryFactory => RepositoryFactory;

    ISyntacticMapperLogger<ASyntacticMapper<TRecord>> ISyntacticMapperDependencyProvider<TRecord>.Logger => Logger;
}
