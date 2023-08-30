namespace SharpAttributeParser.Mappers;

using SharpAttributeParser.Mappers.Logging;
using SharpAttributeParser.Mappers.Repositories;
using SharpAttributeParser.Mappers.Repositories.Combined;

/// <summary>Provides the dependencies of <see cref="ACombinedMapper{TRecord}"/>.</summary>
/// <typeparam name="TRecord">The type to which arguments are recorded.</typeparam>
public interface ICombinedMapperDependencyProvider<TRecord>
{
    /// <summary>Determines equality when comparing parameters.</summary>
    public abstract IParameterComparer ParameterComparer { get; }

    /// <summary>Handles creation of recorders responsible for recording arguments, together with syntactic information about arguments, of some specific parameter.</summary>
    public abstract IMappedCombinedArgumentRecorderFactory RecorderFactory { get; }

    /// <summary>Handles creation of repositories.</summary>
    public abstract ICombinedMappingRepositoryFactory<TRecord> RepositoryFactory { get; }

    /// <summary>The logger used to log messages.</summary>
    public abstract ICombinedMapperLogger<ACombinedMapper<TRecord>> Logger { get; }
}
