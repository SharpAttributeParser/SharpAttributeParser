namespace SharpAttributeParser.Mappers.Repositories.Combined;

/// <summary>A repository for mappings from parameters to recorders.</summary>
/// <typeparam name="TRecord">The type to which arguments are recorded.</typeparam>
public interface ICombinedMappingRepository<TRecord> : IAppendableCombinedMappingRepository<TRecord>, IBuildableMappingRepository<IBuiltCombinedMappingRepository<TRecord>>
{
    /// <summary>The repository for mappings from type parameters to recorders.</summary>
    new public abstract ITypeMappingRepository<IDetachedMappedCombinedTypeArgumentRecorder<TRecord>, IDetachedMappedCombinedTypeArgumentRecorderFactory<TRecord>> TypeParameters { get; }

    /// <summary>The repository for mappings from constructor parameters to recorders.</summary>
    new public abstract IConstructorMappingRepository<IDetachedMappedCombinedConstructorArgumentRecorder<TRecord>, IDetachedMappedCombinedConstructorArgumentRecorderFactory<TRecord>> ConstructorParameters { get; }

    /// <summary>The repository for mappings from named parameters to recorder.</summary>
    new public abstract INamedMappingRepository<IDetachedMappedCombinedNamedArgumentRecorder<TRecord>, IDetachedMappedCombinedNamedArgumentRecorderFactory<TRecord>> NamedParameters { get; }
}
