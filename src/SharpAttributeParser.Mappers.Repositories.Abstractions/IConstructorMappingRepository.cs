namespace SharpAttributeParser.Mappers.Repositories;

/// <summary>A repository for mappings from constructor parameters to recorders.</summary>
/// <typeparam name="TRecorder">The type of the mapped recorders.</typeparam>
/// <typeparam name="TRecorderFactory">The type handling creation of recorders.</typeparam>
public interface IConstructorMappingRepository<TRecorder, TRecorderFactory> : IAppendableConstructorMappingRepository<TRecorder, TRecorderFactory>, IBuildableMappingRepository<IBuiltConstructorMappingRepository<TRecorder>> { }
