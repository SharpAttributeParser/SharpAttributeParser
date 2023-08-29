namespace SharpAttributeParser.Mappers.Repositories.Semantic;

/// <summary>A built repository for mappings from parameters to recorders.</summary>
/// <typeparam name="TRecord">The type to which arguments are recorded.</typeparam>
public interface IBuiltSemanticMappingRepository<TRecord>
{
    /// <summary>The repository for mappings from type parameters to recorders.</summary>
    public abstract IBuiltTypeMappingRepository<IDetachedMappedSemanticTypeArgumentRecorder<TRecord>> TypeParameters { get; }

    /// <summary>The repository for mappings from constructor parameters to recorders.</summary>
    public abstract IBuiltConstructorMappingRepository<IDetachedMappedSemanticConstructorArgumentRecorder<TRecord>> ConstructorParameters { get; }

    /// <summary>The repository for mappings from named parameters to recorders.</summary>
    public abstract IBuiltNamedMappingRepository<IDetachedMappedSemanticNamedArgumentRecorder<TRecord>> NamedParameters { get; }
}
