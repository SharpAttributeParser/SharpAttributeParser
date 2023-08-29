namespace SharpAttributeParser.Mappers.Repositories;

using System;
using System.Collections.Generic;

/// <summary>Handles creation of <see cref="ITypeMappingRepository{TRecorder, TRecorderFactory}"/>.</summary>
/// <typeparam name="TRecorder">The type of the recorders mapped by the created repositories.</typeparam>
/// <typeparam name="TRecorderFactory">The type handling creation of recorders for the created repositories.</typeparam>
public interface ITypeMappingRepositoryFactory<TRecorder, TRecorderFactory>
{
    /// <summary>Creates a repository for mappings from type parameters to recorders.</summary>
    /// <param name="parameterNameComparer">Determines equality when comparing the names of parameters.</param>
    /// <param name="throwOnMultipleBuilds">Indicates whether the created repository should throw an <see cref="InvalidOperationException"/> if build is invoked more than once.</param>
    /// <returns>The created repository.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract ITypeMappingRepository<TRecorder, TRecorderFactory> Create(IEqualityComparer<string> parameterNameComparer, bool throwOnMultipleBuilds);
}
