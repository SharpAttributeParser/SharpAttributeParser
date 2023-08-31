namespace SharpAttributeParser.Mappers.Repositories;

using System;

/// <summary>Handles creation of repositories.</summary>
/// <typeparam name="TRepository">The type of the creation repositories.</typeparam>
public interface IMappingRepositoryFactory<out TRepository>
{
    /// <summary>Creates a repository.</summary>
    /// <param name="comparer">Determines equality when comparing parameters.</param>
    /// <param name="throwOnMultipleBuilds">Indicates whether the created repository should throw an <see cref="InvalidOperationException"/> if build is invoked more than once.</param>
    /// <returns>The created repository.</returns>
    public abstract TRepository Create(IParameterComparer comparer, bool throwOnMultipleBuilds);
}
