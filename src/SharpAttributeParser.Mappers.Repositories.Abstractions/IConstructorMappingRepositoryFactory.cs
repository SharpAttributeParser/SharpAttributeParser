namespace SharpAttributeParser.Mappers.Repositories;

using System;

/// <summary>Handles creation of <see cref="IConstructorMappingRepository{TRecorder, TRecorderFactory}"/>.</summary>
/// <typeparam name="TRecorder">The type of the recorders mapped by the created repositories.</typeparam>
/// <typeparam name="TRecorderFactory">The type handling creation of recorders for the created repositories.</typeparam>
public interface IConstructorMappingRepositoryFactory<TRecorder, TRecorderFactory>
{
    /// <summary>Creates a repository for mappings from constructor parameters to recorders.</summary>
    /// <param name="comparer">Determines equality when comparing parameters.</param>
    /// <param name="throwOnMultipleBuilds">Indicates whether the created repository should throw an <see cref="InvalidOperationException"/> if build is invoked more than once.</param>
    /// <returns>The created repository.</returns>
    public abstract IConstructorMappingRepository<TRecorder, TRecorderFactory> Create(IConstructorParameterComparer comparer, bool throwOnMultipleBuilds);
}
