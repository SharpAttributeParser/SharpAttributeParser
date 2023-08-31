namespace SharpAttributeParser.Mappers.Logging.SyntacticMapperComponents;

using Microsoft.CodeAnalysis;

using SharpAttributeParser.Mappers.Repositories;

using System;

/// <summary>Handles logging for <see cref="ISyntacticMapper{TRecord}"/> when related to type parameters.</summary>
public interface ITypeParameterLogger
{
    /// <summary>Begins a log scope describing an attempt to map a type parameter to a recorder.</summary>
    /// <typeparam name="TRecorder">The type of the mapped recorders.</typeparam>
    /// <param name="parameter">The type parameter.</param>
    /// <param name="mappingRepository">The repository for mappings from type parameters to recorders.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    public abstract IDisposable? BeginScopeMappingTypeParameter<TRecorder>(ITypeParameterSymbol parameter, IBuiltTypeMappingRepository<TRecorder> mappingRepository);

    /// <summary>Logs a message describing a failed attempt to map a type parameter to a recorder.</summary>
    public abstract void FailedToMapTypeParameter();
}
