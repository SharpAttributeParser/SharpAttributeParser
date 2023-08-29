namespace SharpAttributeParser.Mappers.Repositories.Combined;

/// <summary>A built repository for mappings from parameters to recorders.</summary>
/// <typeparam name="TRecord">The type to which arguments are recorded.</typeparam>
public interface IBuiltCombinedMappingRepository<TRecord>
{
    /// <summary>The repository for mappings from type parameters to recorders.</summary>
    public abstract IBuiltTypeMappingRepository<IDetachedMappedCombinedTypeArgumentRecorder<TRecord>> TypeParameters { get; }

    /// <summary>The repository for mappings from constructor parameters to recorders.</summary>
    public abstract IBuiltConstructorMappingRepository<IDetachedMappedCombinedConstructorArgumentRecorder<TRecord>> ConstructorParameters { get; }

    /// <summary>The repository for mappings from named parameters to recorders.</summary>
    public abstract IBuiltNamedMappingRepository<IDetachedMappedCombinedNamedArgumentRecorder<TRecord>> NamedParameters { get; }
}
