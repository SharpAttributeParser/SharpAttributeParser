namespace SharpAttributeParser.Mappers;

using SharpAttributeParser.Mappers.Logging;
using SharpAttributeParser.Mappers.Repositories;
using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Mappers.Repositories.Split;
using SharpAttributeParser.Mappers.Repositories.Syntactic;

/// <summary>Provides the dependencies of <see cref="ASplitMapper{TSemanticRecord, TSyntacticRecord}"/>.</summary>
/// <typeparam name="TSemanticRecord">The type to which arguments are recorded.</typeparam>
/// <typeparam name="TSyntacticRecord">The type to which syntactic information about arguments is recorded.</typeparam>
public interface ISplitMapperDependencyProvider<TSemanticRecord, TSyntacticRecord>
{
    /// <summary>Determines equality when comparing parameters.</summary>
    public abstract IParameterComparer ParameterComparer { get; }

    /// <summary>Handles creation of recorders responsible for recording arguments of some specific parameter.</summary>
    public abstract IMappedSemanticArgumentRecorderFactory SemanticRecorderFactory { get; }

    /// <summary>Handles creation of recorders responsible for recording syntactic information about arguments of some specific parameter.</summary>
    public abstract IMappedSyntacticArgumentRecorderFactory SyntacticRecorderFactory { get; }

    /// <summary>Handles creation of repositories.</summary>
    public abstract ISplitMappingRepositoryFactory<TSemanticRecord, TSyntacticRecord> RepositoryFactory { get; }

    /// <summary>The logger used to log messages when acting as a semantic mapper.</summary>
    public abstract ISemanticMapperLogger<ASplitMapper<TSemanticRecord, TSyntacticRecord>> SemanticLogger { get; }

    /// <summary>The logger used to log messages when acting as a syntactic mapper.</summary>
    public abstract ISyntacticMapperLogger<ASplitMapper<TSemanticRecord, TSyntacticRecord>> SyntacticLogger { get; }
}
