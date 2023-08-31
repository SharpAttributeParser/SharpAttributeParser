namespace SharpAttributeParser.Mappers;

using Microsoft.CodeAnalysis;

using SharpAttributeParser.Mappers.MappedRecorders;

/// <summary>Maps attribute parameters to recorders, responsible for recording arguments, and syntactic information about the arguments, of that parameter.</summary>
/// <typeparam name="TRecord">The type to which arguments are recorded.</typeparam>
public interface ICombinedMapper<in TRecord>
{
    /// <summary>Attempts to map a type parameter to a recorder.</summary>
    /// <param name="parameter">The type parameter.</param>
    /// <param name="dataRecord">The record to which arguments are recorded by the mapped recorder.</param>
    /// <returns>The mapped recorder, or <see langword="null"/> if the attempt was unsuccessful.</returns>
    public abstract IMappedCombinedTypeArgumentRecorder? TryMapTypeParameter(ITypeParameterSymbol parameter, TRecord dataRecord);

    /// <summary>Attempts to map a constructor parameter to a recorder.</summary>
    /// <param name="parameter">The constructor parameter.</param>
    /// <param name="dataRecord">The record to which arguments are recorded by the mapped recorder.</param>
    /// <returns>The mapped recorder, or <see langword="null"/> if the attempt was unsuccessful.</returns>
    public abstract IMappedCombinedConstructorArgumentRecorder? TryMapConstructorParameter(IParameterSymbol parameter, TRecord dataRecord);

    /// <summary>Attempts to map a named parameter to a recorder.</summary>
    /// <param name="parameterName">The name of the named parameter.</param>
    /// <param name="dataRecord">The record to which arguments are recorded by the mapped recorder.</param>
    /// <returns>The mapped recorder, or <see langword="null"/> if the attempt was unsuccessful.</returns>
    public abstract IMappedCombinedNamedArgumentRecorder? TryMapNamedParameter(string parameterName, TRecord dataRecord);
}
