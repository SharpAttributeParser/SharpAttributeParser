namespace SharpAttributeParser.Mappers.Repositories;

using System.Collections.Generic;

/// <summary>A built repository for mappings from type parameters to recorders.</summary>
/// <typeparam name="TRecorder">The type of the mapped recorders.</typeparam>
public interface IBuiltTypeMappingRepository<TRecorder>
{
    /// <summary>The mappings from the indices of type parameters to recorders.</summary>
    public abstract IReadOnlyDictionary<int, TRecorder> Indexed { get; }

    /// <summary>The mappings from the names of type parameters to recorders.</summary>
    public abstract IReadOnlyDictionary<string, TRecorder> Named { get; }
}
