namespace SharpAttributeParser.Mappers;

using SharpAttributeParser.Mappers.Logging;
using SharpAttributeParser.Mappers.Repositories;
using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Mappers.Repositories.Split;
using SharpAttributeParser.Mappers.Repositories.Syntactic;

using System;

/// <inheritdoc cref="ISplitMapperDependencyProvider{TSemanticRecord, TSyntacticRecord}"/>
public sealed class SplitMapperDependencyProvider<TSemanticRecord, TSyntacticRecord> : ISplitMapperDependencyProvider<TSemanticRecord, TSyntacticRecord>
{
    private static readonly Lazy<ISplitMappingRepositoryFactory<TSemanticRecord, TSyntacticRecord>> DefaultRepositoryFactory = new(DefaultRepositoryFactories.SplitFactory<TSemanticRecord, TSyntacticRecord>);

    private readonly IParameterComparer ParameterComparer;

    private readonly IMappedSemanticArgumentRecorderFactory SemanticRecorderFactory;
    private readonly IMappedSyntacticArgumentRecorderFactory SyntacticRecorderFactory;
    private readonly ISplitMappingRepositoryFactory<TSemanticRecord, TSyntacticRecord> RepositoryFactory;

    private readonly ISemanticMapperLogger<ASplitMapper<TSemanticRecord, TSyntacticRecord>> SemanticLogger;
    private readonly ISyntacticMapperLogger<ASplitMapper<TSemanticRecord, TSyntacticRecord>> SyntacticLogger;

    /// <summary>Instantiates a <see cref="SplitMapperDependencyProvider{TSemanticRecord, TSyntacticRecord}"/>, providing the dependencies of <see cref="ASplitMapper{TSemanticRecord, TSyntacticRecord}"/>.</summary>
    /// <param name="parameterComparer">Determines equality when comparing parameters.</param>
    /// <param name="semanticRecorderFactory">Handles creation of recorders responsible for recording arguments of some specific parameter.</param>
    /// <param name="syntacticRecorderFactory">Handles creation of recorders responsible for recording syntactic information about arguments of some specific parameter.</param>
    /// <param name="repositoryFactory">Handles creation of repositories.</param>
    /// <param name="semanticLogger">The logger used to log messages when acting as a semantic mapper.</param>
    /// <param name="syntacticLogger">The logger used to log messages when acting as a syntactic mapper.</param>
    public SplitMapperDependencyProvider(IParameterComparer? parameterComparer = null, IMappedSemanticArgumentRecorderFactory? semanticRecorderFactory = null, IMappedSyntacticArgumentRecorderFactory? syntacticRecorderFactory = null, ISplitMappingRepositoryFactory<TSemanticRecord, TSyntacticRecord>? repositoryFactory = null, ISemanticMapperLogger<ASplitMapper<TSemanticRecord, TSyntacticRecord>>? semanticLogger = null, ISyntacticMapperLogger<ASplitMapper<TSemanticRecord, TSyntacticRecord>>? syntacticLogger = null)
    {
        ParameterComparer = parameterComparer ?? DefaultParameterComparer.Comparer;

        SemanticRecorderFactory = semanticRecorderFactory ?? DefaultRecorderFactories.SemanticFactory();
        SyntacticRecorderFactory = syntacticRecorderFactory ?? DefaultRecorderFactories.SyntacticFactory();
        RepositoryFactory = repositoryFactory ?? DefaultRepositoryFactory.Value;

        SemanticLogger = semanticLogger ?? NullSemanticMapperLogger<ASplitMapper<TSemanticRecord, TSyntacticRecord>>.Singleton;
        SyntacticLogger = syntacticLogger ?? NullSyntacticMapperLogger<ASplitMapper<TSemanticRecord, TSyntacticRecord>>.Singleton;
    }

    IParameterComparer ISplitMapperDependencyProvider<TSemanticRecord, TSyntacticRecord>.ParameterComparer => ParameterComparer;

    IMappedSemanticArgumentRecorderFactory ISplitMapperDependencyProvider<TSemanticRecord, TSyntacticRecord>.SemanticRecorderFactory => SemanticRecorderFactory;
    IMappedSyntacticArgumentRecorderFactory ISplitMapperDependencyProvider<TSemanticRecord, TSyntacticRecord>.SyntacticRecorderFactory => SyntacticRecorderFactory;
    ISplitMappingRepositoryFactory<TSemanticRecord, TSyntacticRecord> ISplitMapperDependencyProvider<TSemanticRecord, TSyntacticRecord>.RepositoryFactory => RepositoryFactory;

    ISemanticMapperLogger<ASplitMapper<TSemanticRecord, TSyntacticRecord>> ISplitMapperDependencyProvider<TSemanticRecord, TSyntacticRecord>.SemanticLogger => SemanticLogger;
    ISyntacticMapperLogger<ASplitMapper<TSemanticRecord, TSyntacticRecord>> ISplitMapperDependencyProvider<TSemanticRecord, TSyntacticRecord>.SyntacticLogger => SyntacticLogger;
}
