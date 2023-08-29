namespace SharpAttributeParser.Mappers.Repositories.Adaptive;

/// <summary>A built repository for mappings from parameters to recorders.</summary>
/// <typeparam name="TCombinedRecord">The type to which arguments are recorded when parsed with syntactic conctext.</typeparam>
/// <typeparam name="TSemanticRecord">The type to which arguments are recorded when parsed without syntactic conctext.</typeparam>
public interface IBuiltAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord>
{
    /// <summary>The repository for mappings from type parameters to recorders.</summary>
    public abstract IBuiltTypeMappingRepository<IDetachedMappedAdaptiveTypeArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>> TypeParameters { get; }

    /// <summary>The repository for mappings from constructor parameters to recorders.</summary>
    public abstract IBuiltConstructorMappingRepository<IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>> ConstructorParameters { get; }

    /// <summary>The repository for mappings from named parameters to recorders.</summary>
    public abstract IBuiltNamedMappingRepository<IDetachedMappedAdaptiveNamedArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>> NamedParameters { get; }
}
