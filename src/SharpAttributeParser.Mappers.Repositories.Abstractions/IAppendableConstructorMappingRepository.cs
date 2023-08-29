namespace SharpAttributeParser.Mappers.Repositories;

using System;

/// <summary>A repository for mappings from constructor parameters to recorders, to which new mappings can be appended.</summary>
/// <typeparam name="TRecorder">The type of the mapped recorders.</typeparam>
/// <typeparam name="TRecorderFactory">The type handling creation of recorders.</typeparam>
public interface IAppendableConstructorMappingRepository<in TRecorder, out TRecorderFactory>
{
    /// <summary>Adds a mapping from a constructor parameter, identified by a name, to a recorder.</summary>
    /// <param name="parameterName">The name of the constructor parameter.</param>
    /// <param name="recorder">The recorder responsible for recording arguments of the constructor parameter.</param>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract void AddNamedMapping(string parameterName, TRecorder recorder);

    /// <summary>Adds a mapping from a constructor parameter, identified by a name, to a recorder.</summary>
    /// <param name="parameterName">The name of the constructor parameter.</param>
    /// <param name="recorderDelegate">Creates the recorder responsible for recording arguments of the constructor parameter.</param>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="ArgumentOutOfRangeException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract void AddNamedMapping(string parameterName, Func<TRecorderFactory, TRecorder> recorderDelegate);
}
