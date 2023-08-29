namespace SharpAttributeParser.Mappers.Repositories.Syntactic;

/// <summary>A repository for mappings from parameters to recorders, to which new mappings can be appended.</summary>
/// <typeparam name="TRecord">The type to which syntactic information about arguments is recorded.</typeparam>
public interface IAppendableSyntacticMappingRepository<TRecord>
{
    /// <summary>The repository for mappings from type parameters to recorders.</summary>
    public abstract IAppendableTypeMappingRepository<IDetachedMappedSyntacticTypeArgumentRecorder<TRecord>, IDetachedMappedSyntacticTypeArgumentRecorderFactory<TRecord>> TypeParameters { get; }

    /// <summary>The repository for mappings from constructor parameters to recorders.</summary>
    public abstract IAppendableConstructorMappingRepository<IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord>, IDetachedMappedSyntacticConstructorArgumentRecorderFactory<TRecord>> ConstructorParameters { get; }

    /// <summary>The repository for mappings from named parameters to recorders.</summary>
    public abstract IAppendableNamedMappingRepository<IDetachedMappedSyntacticNamedArgumentRecorder<TRecord>, IDetachedMappedSyntacticNamedArgumentRecorderFactory<TRecord>> NamedParameters { get; }
}
