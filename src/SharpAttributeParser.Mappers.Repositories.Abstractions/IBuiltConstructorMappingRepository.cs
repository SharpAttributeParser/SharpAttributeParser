namespace SharpAttributeParser.Mappers.Repositories;

using System.Collections.Generic;

/// <summary>A built repository for mappings from constructor parameters to recorders.</summary>
/// <typeparam name="TRecorder">The type of the mapped recorders.</typeparam>
public interface IBuiltConstructorMappingRepository<TRecorder>
{
    /// <summary>The mappings from the names of constructor parameters to recorders.</summary>
    public abstract IReadOnlyDictionary<string, TRecorder> Named { get; }
}
