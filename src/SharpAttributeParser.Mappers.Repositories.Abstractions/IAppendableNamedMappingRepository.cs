namespace SharpAttributeParser.Mappers.Repositories;

using System;

/// <summary>A repository for mappings from named parameters to recorders, to which new mappings can be appended.</summary>
/// <typeparam name="TRecorder">The type of the mapped recorders.</typeparam>
/// <typeparam name="TRecorderFactory">The type handling creation of recorders.</typeparam>
public interface IAppendableNamedMappingRepository<in TRecorder, out TRecorderFactory>
{
    /// <summary>Adds a mapping from a named parameter to a recorder.</summary>
    /// <param name="parameterName">The name of the named parameter.</param>
    /// <param name="recorder">The recorder responsible for recording arguments of the named parameter.</param>
    public abstract void AddNamedMapping(string parameterName, TRecorder recorder);

    /// <summary>Adds a mapping from a named parameter to a recorder.</summary>
    /// <param name="parameterName">The name of the named parameter.</param>
    /// <param name="recorderDelegate">Creates the recorder responsible for recording arguments of the named parameter.</param>
    public abstract void AddNamedMapping(string parameterName, Func<TRecorderFactory, TRecorder> recorderDelegate);
}
