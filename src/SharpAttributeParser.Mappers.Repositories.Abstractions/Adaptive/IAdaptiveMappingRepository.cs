namespace SharpAttributeParser.Mappers.Repositories.Adaptive;

/// <summary>A repository for mappings from parameters to recorders.</summary>
/// <typeparam name="TCombinedRecord">The type to which arguments are recorded when parsed with syntactic conctext.</typeparam>
/// <typeparam name="TSemanticRecord">The type to which arguments are recorded when parsed without syntactic conctext.</typeparam>
public interface IAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord> : IAppendableAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord>, IBuildableMappingRepository<IBuiltAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord>>
{
    /// <summary>The repository for mappings from type parameters to recorders.</summary>
    new public abstract ITypeMappingRepository<IDetachedMappedAdaptiveTypeArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveTypeArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>> TypeParameters { get; }

    /// <summary>The repository for mappings from constructor parameters to recorders.</summary>
    new public abstract IConstructorMappingRepository<IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>> ConstructorParameters { get; }

    /// <summary>The repository for mappings from named parameters to recorders.</summary>
    new public abstract INamedMappingRepository<IDetachedMappedAdaptiveNamedArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveNamedArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>> NamedParameters { get; }
}
