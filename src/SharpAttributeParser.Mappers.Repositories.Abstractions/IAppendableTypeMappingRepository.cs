namespace SharpAttributeParser.Mappers.Repositories;

using System;

/// <summary>A repository for mappings from type parameters to recorders, to which new mappings can be appended.</summary>
/// <typeparam name="TRecorder">The type of the mapped recorders.</typeparam>
/// <typeparam name="TRecorderFactory">The type handling creation of recorders.</typeparam>
public interface IAppendableTypeMappingRepository<TRecorder, TRecorderFactory>
{
    /// <summary>Adds a mapping from a type parameter, identified by an index, to a recorder.</summary>
    /// <param name="parameterIndex">The zero-based index of the type parameter.</param>
    /// <param name="recorder">The recorder responsible for recording arguments of the type parameter.</param>
    public abstract void AddIndexedMapping(int parameterIndex, TRecorder recorder);

    /// <summary>Adds a mapping from a type parameter, identified by an index, to a recorder.</summary>
    /// <param name="parameterIndex">The zero-based index of the type parameter.</param>
    /// <param name="recorderDelegate">Creates the recorder responsible for recording arguments of the type parameter.</param>
    public abstract void AddIndexedMapping(int parameterIndex, Func<TRecorderFactory, TRecorder> recorderDelegate);

    /// <summary>Adds a mapping from a type parameter, identified by a name, to a recorder.</summary>
    /// <param name="parameterName">The name of the type parameter.</param>
    /// <param name="recorder">The recorder responsible for recording arguments of the type parameter.</param>
    public abstract void AddNamedMapping(string parameterName, TRecorder recorder);

    /// <summary>Adds a mapping from a type parameter, identified by a name, to a recorder.</summary>
    /// <param name="parameterName">The name of the type parameter.</param>
    /// <param name="recorderDelegate">Creates the recorder responsible for recording arguments of the type parameter.</param>
    public abstract void AddNamedMapping(string parameterName, Func<TRecorderFactory, TRecorder> recorderDelegate);
}
