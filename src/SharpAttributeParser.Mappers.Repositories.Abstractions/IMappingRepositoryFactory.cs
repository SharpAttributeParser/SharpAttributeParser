namespace SharpAttributeParser.Mappers.Repositories;

using System;
using System.Collections.Generic;

/// <summary>Handles creation of repositories.</summary>
/// <typeparam name="TRepository">The type of the creation repositories.</typeparam>
public interface IMappingRepositoryFactory<TRepository>
{
    /// <summary>Creates a repository.</summary>
    /// <param name="parameterNameComparer">Determines equality when comparing the names of parameters.</param>
    /// <param name="throwOnMultipleBuilds">Indicates whether the created repository should throw an <see cref="InvalidOperationException"/> if build is invoked more than once.</param>
    /// <returns>The created repository.</returns>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract TRepository Create(IEqualityComparer<string> parameterNameComparer, bool throwOnMultipleBuilds);

    /// <summary>Creates a repository.</summary>
    /// <param name="typeParameterNameComparer">Determines equality when comparing the names of type parameters.</param>
    /// <param name="constructorParameterNameComparer">Determines equality when comparing the names of constructor parameters.</param>
    /// <param name="namedParameterNameComparer">Determines equality when comparing the names of named parameters.</param>
    /// <param name="throwOnMultipleBuilds">Indicates whether the created repository should throw an <see cref="InvalidOperationException"/> if build is invoked more than once.</param>
    /// <returns>The created repository.</returns>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract TRepository Create(IEqualityComparer<string> typeParameterNameComparer, IEqualityComparer<string> constructorParameterNameComparer, IEqualityComparer<string> namedParameterNameComparer, bool throwOnMultipleBuilds);
}
