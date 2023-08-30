namespace SharpAttributeParser.Mappers;

using SharpAttributeParser.Mappers.Logging;
using SharpAttributeParser.Mappers.Repositories;
using SharpAttributeParser.Mappers.Repositories.Syntactic;

/// <summary>Provides the dependencies of <see cref="ASyntacticMapper{TRecord}"/>.</summary>
/// <typeparam name="TRecord">The type to which arguments are recorded.</typeparam>
public interface ISyntacticMapperDependencyProvider<TRecord>
{
    /// <summary>Determines equality when comparing parameters.</summary>
    public abstract IParameterComparer ParameterComparer { get; }

    /// <summary>Handles creation of recorders responsible for recording syntactic information about arguments of some specific parameter.</summary>
    public abstract IMappedSyntacticArgumentRecorderFactory RecorderFactory { get; }

    /// <summary>Handles creation of repositories.</summary>
    public abstract ISyntacticMappingRepositoryFactory<TRecord> RepositoryFactory { get; }

    /// <summary>The logger used to log messages.</summary>
    public abstract ISyntacticMapperLogger<ASyntacticMapper<TRecord>> Logger { get; }
}
