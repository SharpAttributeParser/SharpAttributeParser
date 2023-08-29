namespace SharpAttributeParser.Mappers.Repositories;

using System;

/// <summary>A repository for mappings from parameters to recorders, which can be built.</summary>
/// <typeparam name="TBuiltRepository">The type of the built repository.</typeparam>
public interface IBuildableMappingRepository<out TBuiltRepository>
{
    /// <summary>Builds the repository.</summary>
    /// <returns>The built repository.</returns>
    /// <exception cref="InvalidOperationException"/>
    public abstract TBuiltRepository Build();
}
