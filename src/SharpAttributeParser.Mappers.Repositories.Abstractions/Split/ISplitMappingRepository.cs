namespace SharpAttributeParser.Mappers.Repositories.Split;

/// <summary>A repository for mappings from parameters to recorders.</summary>
/// <typeparam name="TSemanticRecord">The type to which arguments are recorded.</typeparam>
/// <typeparam name="TSyntacticRecord">The type to which syntactic information about arguments is recorded.</typeparam>
public interface ISplitMappingRepository<TSemanticRecord, TSyntacticRecord> : IAppendableSplitMappingRepository<TSemanticRecord, TSyntacticRecord>, IBuildableMappingRepository<IBuiltSplitMappingRepository<TSemanticRecord, TSyntacticRecord>>
{
    /// <summary>The repository for mappings from type parameters to recorders.</summary>
    new public abstract ITypeMappingRepository<IDetachedMappedSplitTypeArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitTypeArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>> TypeParameters { get; }

    /// <summary>The repository for mappings from constructor parameters to recorders.</summary>
    new public abstract IConstructorMappingRepository<IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>> ConstructorParameters { get; }

    /// <summary>The repository for mappings from named parameters to recorders.</summary>
    new public abstract INamedMappingRepository<IDetachedMappedSplitNamedArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitNamedArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>> NamedParameters { get; }
}
