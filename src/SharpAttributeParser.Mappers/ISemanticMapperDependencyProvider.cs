namespace SharpAttributeParser.Mappers;

using SharpAttributeParser.Mappers.Logging;
using SharpAttributeParser.Mappers.Repositories;
using SharpAttributeParser.Mappers.Repositories.Semantic;

/// <summary>Provides the dependencies of <see cref="ASemanticMapper{TRecord}"/>.</summary>
/// <typeparam name="TRecord">The type to which arguments are recorded.</typeparam>
public interface ISemanticMapperDependencyProvider<TRecord>
{
    /// <summary>Determines equality when comparing parameters.</summary>
    public abstract IParameterComparer ParameterComparer { get; }

    /// <summary>Handles creation of recorders responsible for recording arguments of some specific parameter.</summary>
    public abstract IMappedSemanticArgumentRecorderFactory RecorderFactory { get; }

    /// <summary>Handles creation of repositories.</summary>
    public abstract ISemanticMappingRepositoryFactory<TRecord> RepositoryFactory { get; }

    /// <summary>The logger used to log messages.</summary>
    public abstract ISemanticMapperLogger<ASemanticMapper<TRecord>> Logger { get; }
}
