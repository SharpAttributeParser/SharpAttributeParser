namespace SharpAttributeParser.Mappers.Logging.CombinedMapperComponents;

using SharpAttributeParser.Mappers.Repositories;

using System;

/// <summary>Handles logging for <see cref="ICombinedMapper{TRecord}"/> when related to named parameters.</summary>
public interface INamedParameterLogger
{
    /// <summary>Begins a log scope describing an attempt to map a named parameter to a recorder.</summary>
    /// <typeparam name="TRecorder">The type of the mapped recorders.</typeparam>
    /// <param name="parameterName">The name of the named parameter.</param>
    /// <param name="mappingRepository">The repository for mappings from named parameters to recorders.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    public abstract IDisposable? BeginScopeMappingNamedParameter<TRecorder>(string parameterName, IBuiltNamedMappingRepository<TRecorder> mappingRepository);

    /// <summary>Logs a message describing a failed attempt to map a named parameter to a recorder.</summary>
    public abstract void FailedToMapNamedParameter();
}
