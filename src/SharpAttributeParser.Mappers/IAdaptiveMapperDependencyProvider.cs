namespace SharpAttributeParser.Mappers;

using SharpAttributeParser.Mappers.Logging;
using SharpAttributeParser.Mappers.Repositories;
using SharpAttributeParser.Mappers.Repositories.Adaptive;
using SharpAttributeParser.Mappers.Repositories.Combined;
using SharpAttributeParser.Mappers.Repositories.Semantic;

/// <summary>Provides the dependencies of <see cref="AAdaptiveMapper{TCombinedRecord, TSemanticRecord}"/>.</summary>
/// <typeparam name="TCombinedRecord">The type to which arguments are recorded when parsed with syntactic context.</typeparam>
/// <typeparam name="TSemanticRecord">The type to which arguments are recorded when parsed without syntactic context.</typeparam>
public interface IAdaptiveMapperDependencyProvider<TCombinedRecord, TSemanticRecord>
{
    /// <summary>Determines equality when comparing parameters.</summary>
    public abstract IParameterComparer ParameterComparer { get; }

    /// <summary>Handles creation of recorders responsible for recording arguments, together with syntactic information about arguments, of some specific parameter.</summary>
    public abstract IMappedCombinedArgumentRecorderFactory CombinedRecorderFactory { get; }

    /// <summary>Handles creation of recorders responsible for recording arguments of some specific parameter.</summary>
    public abstract IMappedSemanticArgumentRecorderFactory SemanticRecorderFactory { get; }

    /// <summary>Handles creation of repositories.</summary>
    public abstract IAdaptiveMappingRepositoryFactory<TCombinedRecord, TSemanticRecord> RepositoryFactory { get; }

    /// <summary>The logger used to log messages when acting as a combined mapper.</summary>
    public abstract ICombinedMapperLogger<AAdaptiveMapper<TCombinedRecord, TSemanticRecord>> CombinedLogger { get; }

    /// <summary>The logger used to log messages when acting as a semantic mapper.</summary>
    public abstract ISemanticMapperLogger<AAdaptiveMapper<TCombinedRecord, TSemanticRecord>> SemanticLogger { get; }
}
