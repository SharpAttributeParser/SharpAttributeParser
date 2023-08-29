namespace SharpAttributeParser.Mappers.Repositories.Semantic;

/// <summary>A repository for mappings from parameters to recorders, to which new mappings can be appended.</summary>
/// <typeparam name="TRecord">The type to which arguments are recorded.</typeparam>
public interface IAppendableSemanticMappingRepository<TRecord>
{
    /// <summary>The repository for mappings from type parameters to recorders.</summary>
    public abstract IAppendableTypeMappingRepository<IDetachedMappedSemanticTypeArgumentRecorder<TRecord>, IDetachedMappedSemanticTypeArgumentRecorderFactory<TRecord>> TypeParameters { get; }

    /// <summary>The repository for mappings from constructor parameters to recorders.</summary>
    public abstract IAppendableConstructorMappingRepository<IDetachedMappedSemanticConstructorArgumentRecorder<TRecord>, IDetachedMappedSemanticConstructorArgumentRecorderFactory<TRecord>> ConstructorParameters { get; }

    /// <summary>The repository for mappings from named parameters to recorders.</summary>
    public abstract IAppendableNamedMappingRepository<IDetachedMappedSemanticNamedArgumentRecorder<TRecord>, IDetachedMappedSemanticNamedArgumentRecorderFactory<TRecord>> NamedParameters { get; }
}
