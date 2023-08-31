namespace SharpAttributeParser.Mappers.Logging.CombinedMapperComponents;

using Microsoft.CodeAnalysis;

using SharpAttributeParser.Mappers.Repositories;

using System;

/// <summary>Handles logging for <see cref="ICombinedMapper{TRecord}"/> when related to constructor parameters.</summary>
public interface IConstructorParameterLogger
{
    /// <summary>Begins a log scope describing an attempt to map a constructor parameter to a recorder.</summary>
    /// <typeparam name="TRecorder">The type of the mapped recorders.</typeparam>
    /// <param name="parameter">The constructor parameter.</param>
    /// <param name="mappingRepository">The repository for mappings from constructor parameters to recorders.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    public abstract IDisposable? BeginScopeMappingConstructorParameter<TRecorder>(IParameterSymbol parameter, IBuiltConstructorMappingRepository<TRecorder> mappingRepository);

    /// <summary>Logs a message describing a failed attempt to map a constructor parameter to a recorder.</summary>
    public abstract void FailedToMapConstructorParameter();
}
