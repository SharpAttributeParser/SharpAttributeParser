namespace SharpAttributeParser.Mappers.Repositories;

/// <summary>A repository for mappings from type parameters to recorders.</summary>
/// <typeparam name="TRecorder">The type of the mapped recorders.</typeparam>
/// <typeparam name="TRecorderFactory">The type handling creation of recorders.</typeparam>
public interface ITypeMappingRepository<TRecorder, TRecorderFactory> : IAppendableTypeMappingRepository<TRecorder, TRecorderFactory>, IBuildableMappingRepository<IBuiltTypeMappingRepository<TRecorder>> { }
