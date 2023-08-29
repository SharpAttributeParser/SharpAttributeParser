namespace SharpAttributeParser.Mappers.Repositories.Semantic;

/// <summary>Handles creation of <see cref="ISemanticMappingRepository{TRecord}"/>.</summary>
/// <typeparam name="TRecord">The type to which arguments are recorded.</typeparam>
public interface ISemanticMappingRepositoryFactory<TRecord> : IMappingRepositoryFactory<ISemanticMappingRepository<TRecord>> { }
