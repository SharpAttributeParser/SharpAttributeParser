namespace SharpAttributeParser.Mappers.Repositories.Combined;

/// <summary>A repository for mappings from parameters to recorders, to which new mappings can be appended.</summary>
/// <typeparam name="TRecord">The type to which arguments are recorded.</typeparam>
public interface IAppendableCombinedMappingRepository<TRecord>
{
    /// <summary>The repository for mappings from type parameters to recorders.</summary>
    public abstract IAppendableTypeMappingRepository<IDetachedMappedCombinedTypeArgumentRecorder<TRecord>, IDetachedMappedCombinedTypeArgumentRecorderFactory<TRecord>> TypeParameters { get; }

    /// <summary>The repository for mappings from constructor parameters to recorders.</summary>
    public abstract IAppendableConstructorMappingRepository<IDetachedMappedCombinedConstructorArgumentRecorder<TRecord>, IDetachedMappedCombinedConstructorArgumentRecorderFactory<TRecord>> ConstructorParameters { get; }

    /// <summary>The repository for mappings from named parameters to recorders.</summary>
    public abstract IAppendableNamedMappingRepository<IDetachedMappedCombinedNamedArgumentRecorder<TRecord>, IDetachedMappedCombinedNamedArgumentRecorderFactory<TRecord>> NamedParameters { get; }
}
