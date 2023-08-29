namespace SharpAttributeParser.Mappers.Repositories.Syntactic;

/// <summary>A built repository for mappings from parameters to recorders.</summary>
/// <typeparam name="TRecord">The type to which syntactic information about arguments is recorded.</typeparam>
public interface IBuiltSyntacticMappingRepository<TRecord>
{
    /// <summary>The repository for mappings from type parameters to recorders.</summary>
    public abstract IBuiltTypeMappingRepository<IDetachedMappedSyntacticTypeArgumentRecorder<TRecord>> TypeParameters { get; }

    /// <summary>The repository for mappings from constructor parameters to recorders.</summary>
    public abstract IBuiltConstructorMappingRepository<IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord>> ConstructorParameters { get; }

    /// <summary>The repository for mappings from named parameters to recorders.</summary>
    public abstract IBuiltNamedMappingRepository<IDetachedMappedSyntacticNamedArgumentRecorder<TRecord>> NamedParameters { get; }
}
