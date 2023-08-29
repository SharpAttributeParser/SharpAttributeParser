namespace SharpAttributeParser.Mappers.Repositories.Combined;

/// <summary>Handles creation of <see cref="ICombinedMappingRepository{TRecord}"/>.</summary>
/// <typeparam name="TRecord">The type to which arguments are recorded.</typeparam>
public interface ICombinedMappingRepositoryFactory<TRecord> : IMappingRepositoryFactory<ICombinedMappingRepository<TRecord>> { }
