namespace SharpAttributeParser.Mappers.Repositories.Split;

/// <summary>A built repository for mappings from parameters to recorders.</summary>
/// <typeparam name="TSemanticRecord">The type to which arguments are recorded.</typeparam>
/// <typeparam name="TSyntacticRecord">The type to which syntactic information about arguments is recorded.</typeparam>
public interface IBuiltSplitMappingRepository<TSemanticRecord, TSyntacticRecord>
{
    /// <summary>The repository for mappings from type parameters to recorders.</summary>
    public abstract IBuiltTypeMappingRepository<IDetachedMappedSplitTypeArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>> TypeParameters { get; }

    /// <summary>The repository for mappings from constructor parameters to recorders.</summary>
    public abstract IBuiltConstructorMappingRepository<IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>> ConstructorParameters { get; }

    /// <summary>The repository for mappings from named parameters to recorders.</summary>
    public abstract IBuiltNamedMappingRepository<IDetachedMappedSplitNamedArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>> NamedParameters { get; }
}
