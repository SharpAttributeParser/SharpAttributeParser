namespace SharpAttributeParser.Mappers;

using SharpAttributeParser.Mappers.Logging;
using SharpAttributeParser.Mappers.Repositories;
using SharpAttributeParser.Mappers.Repositories.Adaptive;
using SharpAttributeParser.Mappers.Repositories.Combined;
using SharpAttributeParser.Mappers.Repositories.Semantic;

using System;

/// <inheritdoc cref="IAdaptiveMapperDependencyProvider{TCombinedRecord, TSemanticRecord}"/>
public sealed class AdaptiveMapperDependencyProvider<TCombinedRecord, TSemanticRecord> : IAdaptiveMapperDependencyProvider<TCombinedRecord, TSemanticRecord>
{
    private static readonly Lazy<IAdaptiveMappingRepositoryFactory<TCombinedRecord, TSemanticRecord>> DefaultRepositoryFactory = new(DefaultRepositoryFactories.AdaptiveFactory<TCombinedRecord, TSemanticRecord>);

    private readonly IParameterComparer ParameterComparer;

    private readonly IMappedCombinedArgumentRecorderFactory CombinedRecorderFactory;
    private readonly IMappedSemanticArgumentRecorderFactory SemanticRecorderFactory;
    private readonly IAdaptiveMappingRepositoryFactory<TCombinedRecord, TSemanticRecord> RepositoryFactory;

    private readonly ICombinedMapperLogger<AAdaptiveMapper<TCombinedRecord, TSemanticRecord>> CombinedLogger;
    private readonly ISemanticMapperLogger<AAdaptiveMapper<TCombinedRecord, TSemanticRecord>> SemanticLogger;

    /// <summary>Instantiates a <see cref="AdaptiveMapperDependencyProvider{TCombinedRecord, TSemanticRecord}"/>, providing the dependencies of <see cref="AAdaptiveMapper{TCombinedRecord, TSemanticRecord}"/>.</summary>
    /// <param name="parameterComparer">Determines equality when comparing parameters.</param>
    /// <param name="combinedRecorderFactory">Handles creation of recorders responsible for recording arguments, together with syntactic information about arguments, of some specific parameter.</param>
    /// <param name="semanticRecorderFactory">Handles creation of recorders responsible for recording arguments of some specific parameter.</param>
    /// <param name="repositoryFactory">Handles creation of repositories.</param>
    /// <param name="combinedLogger">The logger used to log messages when acting as a combined mapper.</param>
    /// <param name="semanticLogger">The logger used to log messages when acting as a semantic mapper.</param>
    public AdaptiveMapperDependencyProvider(IParameterComparer? parameterComparer = null, IMappedCombinedArgumentRecorderFactory? combinedRecorderFactory = null, IMappedSemanticArgumentRecorderFactory? semanticRecorderFactory = null, IAdaptiveMappingRepositoryFactory<TCombinedRecord, TSemanticRecord>? repositoryFactory = null, ICombinedMapperLogger<AAdaptiveMapper<TCombinedRecord, TSemanticRecord>>? combinedLogger = null, ISemanticMapperLogger<AAdaptiveMapper<TCombinedRecord, TSemanticRecord>>? semanticLogger = null)
    {
        ParameterComparer = parameterComparer ?? DefaultParameterComparer.Comparer;

        CombinedRecorderFactory = combinedRecorderFactory ?? DefaultRecorderFactories.CombinedFactory();
        SemanticRecorderFactory = semanticRecorderFactory ?? DefaultRecorderFactories.SemanticFactory();
        RepositoryFactory = repositoryFactory ?? DefaultRepositoryFactory.Value;

        CombinedLogger = combinedLogger ?? NullCombinedMapperLogger<AAdaptiveMapper<TCombinedRecord, TSemanticRecord>>.Singleton;
        SemanticLogger = semanticLogger ?? NullSemanticMapperLogger<AAdaptiveMapper<TCombinedRecord, TSemanticRecord>>.Singleton;
    }

    IParameterComparer IAdaptiveMapperDependencyProvider<TCombinedRecord, TSemanticRecord>.ParameterComparer => ParameterComparer;

    IMappedCombinedArgumentRecorderFactory IAdaptiveMapperDependencyProvider<TCombinedRecord, TSemanticRecord>.CombinedRecorderFactory => CombinedRecorderFactory;
    IMappedSemanticArgumentRecorderFactory IAdaptiveMapperDependencyProvider<TCombinedRecord, TSemanticRecord>.SemanticRecorderFactory => SemanticRecorderFactory;
    IAdaptiveMappingRepositoryFactory<TCombinedRecord, TSemanticRecord> IAdaptiveMapperDependencyProvider<TCombinedRecord, TSemanticRecord>.RepositoryFactory => RepositoryFactory;

    ICombinedMapperLogger<AAdaptiveMapper<TCombinedRecord, TSemanticRecord>> IAdaptiveMapperDependencyProvider<TCombinedRecord, TSemanticRecord>.CombinedLogger => CombinedLogger;
    ISemanticMapperLogger<AAdaptiveMapper<TCombinedRecord, TSemanticRecord>> IAdaptiveMapperDependencyProvider<TCombinedRecord, TSemanticRecord>.SemanticLogger => SemanticLogger;
}
