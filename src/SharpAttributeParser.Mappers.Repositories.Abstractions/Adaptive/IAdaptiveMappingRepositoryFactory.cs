namespace SharpAttributeParser.Mappers.Repositories.Adaptive;

/// <summary>Handles creation of <see cref="IAdaptiveMappingRepository{TCombinedRecord, TSemanticRecord}"/>.</summary>
/// <typeparam name="TCombinedRecord">The type to which arguments are recorded when parsed with syntactic conctext.</typeparam>
/// <typeparam name="TSemanticRecord">The type to which arguments are recorded when parsed without syntactic conctext.</typeparam>
public interface IAdaptiveMappingRepositoryFactory<TCombinedRecord, TSemanticRecord> : IMappingRepositoryFactory<IAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord>> { }
