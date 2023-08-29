namespace SharpAttributeParser.Mappers.Repositories.Semantic;

/// <summary>A repository for mappings from parameters to recorders.</summary>
/// <typeparam name="TRecord">The type to which arguments are recorded.</typeparam>
public interface ISemanticMappingRepository<TRecord> : IAppendableSemanticMappingRepository<TRecord>, IBuildableMappingRepository<IBuiltSemanticMappingRepository<TRecord>>
{
    /// <summary>The repository for mappings from type parameters to recorders.</summary>
    new public abstract ITypeMappingRepository<IDetachedMappedSemanticTypeArgumentRecorder<TRecord>, IDetachedMappedSemanticTypeArgumentRecorderFactory<TRecord>> TypeParameters { get; }

    /// <summary>The repository for mappings from constructor parameters to recorders.</summary>
    new public abstract IConstructorMappingRepository<IDetachedMappedSemanticConstructorArgumentRecorder<TRecord>, IDetachedMappedSemanticConstructorArgumentRecorderFactory<TRecord>> ConstructorParameters { get; }

    /// <summary>The repository for mappings from named parameters to recorders.</summary>
    new public abstract INamedMappingRepository<IDetachedMappedSemanticNamedArgumentRecorder<TRecord>, IDetachedMappedSemanticNamedArgumentRecorderFactory<TRecord>> NamedParameters { get; }
}
