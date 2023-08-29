namespace SharpAttributeParser.Mappers.Repositories.Syntactic;

/// <summary>A repository for mappings from parameters to recorders.</summary>
/// <typeparam name="TRecord">The type to which syntactic information about arguments is recorded.</typeparam>
public interface ISyntacticMappingRepository<TRecord> : IAppendableSyntacticMappingRepository<TRecord>, IBuildableMappingRepository<IBuiltSyntacticMappingRepository<TRecord>>
{
    /// <summary>The repository for mappings from type parameters to recorders.</summary>
    new public abstract ITypeMappingRepository<IDetachedMappedSyntacticTypeArgumentRecorder<TRecord>, IDetachedMappedSyntacticTypeArgumentRecorderFactory<TRecord>> TypeParameters { get; }

    /// <summary>The repository for mappings from constructor parameters to recorders.</summary>
    new public abstract IConstructorMappingRepository<IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord>, IDetachedMappedSyntacticConstructorArgumentRecorderFactory<TRecord>> ConstructorParameters { get; }

    /// <summary>The repository for mappings from named parameters to recorders.</summary>
    new public abstract INamedMappingRepository<IDetachedMappedSyntacticNamedArgumentRecorder<TRecord>, IDetachedMappedSyntacticNamedArgumentRecorderFactory<TRecord>> NamedParameters { get; }
}
