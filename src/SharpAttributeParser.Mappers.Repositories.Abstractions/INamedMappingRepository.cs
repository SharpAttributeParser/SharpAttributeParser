namespace SharpAttributeParser.Mappers.Repositories;

/// <summary>A repository for mappings from named parameters to recorders.</summary>
/// <typeparam name="TRecorder">The type of the mapped recorders.</typeparam>
/// <typeparam name="TRecorderFactory">The type handling creation of recorders.</typeparam>
public interface INamedMappingRepository<TRecorder, TRecorderFactory> : IAppendableNamedMappingRepository<TRecorder, TRecorderFactory>, IBuildableMappingRepository<IBuiltNamedMappingRepository<TRecorder>> { }
