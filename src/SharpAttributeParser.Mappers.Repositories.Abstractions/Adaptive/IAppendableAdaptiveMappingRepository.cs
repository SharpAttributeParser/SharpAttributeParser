namespace SharpAttributeParser.Mappers.Repositories.Adaptive;

/// <summary>A repository for mappings from parameters to recorders, to which new mappings can be appended.</summary>
/// <typeparam name="TCombinedRecord">The type to which arguments are recorded when parsed with syntactic conctext.</typeparam>
/// <typeparam name="TSemanticRecord">The type to which arguments are recorded when parsed without syntactic conctext.</typeparam>
public interface IAppendableAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord>
{
    /// <summary>The repository for mappings from type parameters to recorders.</summary>
    public abstract IAppendableTypeMappingRepository<IDetachedMappedAdaptiveTypeArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveTypeArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>> TypeParameters { get; }

    /// <summary>The repository for mappings from constructor parameters to recorders.</summary>
    public abstract IAppendableConstructorMappingRepository<IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>> ConstructorParameters { get; }

    /// <summary>The repository for mappings from named parameters to recorders.</summary>
    public abstract IAppendableNamedMappingRepository<IDetachedMappedAdaptiveNamedArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveNamedArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>> NamedParameters { get; }
}
