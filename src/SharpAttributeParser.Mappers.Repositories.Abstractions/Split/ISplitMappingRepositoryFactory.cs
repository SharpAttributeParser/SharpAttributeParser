namespace SharpAttributeParser.Mappers.Repositories.Split;

/// <summary>Handles creation of <see cref="ISplitMappingRepository{TSemanticRecord, TSyntacticRecord}"/>.</summary>
/// <typeparam name="TSemanticRecord">The type to which arguments are recorded.</typeparam>
/// <typeparam name="TSyntacticRecord">The type to which syntactic information about arguments is recorded.</typeparam>
public interface ISplitMappingRepositoryFactory<TSemanticRecord, TSyntacticRecord> : IMappingRepositoryFactory<ISplitMappingRepository<TSemanticRecord, TSyntacticRecord>> { }
