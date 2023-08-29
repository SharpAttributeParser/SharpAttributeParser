namespace SharpAttributeParser.Mappers.Repositories.Syntactic;

/// <summary>Handles creation of <see cref="ISyntacticMappingRepository{TRecord}"/>.</summary>
/// <typeparam name="TRecord">The type to which syntactic information about arguments is recorded.</typeparam>
public interface ISyntacticMappingRepositoryFactory<TRecord> : IMappingRepositoryFactory<ISyntacticMappingRepository<TRecord>> { }
