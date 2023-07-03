namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>An abstract <see cref="ISemanticAttributeMapper{TRecorder}"/>, using mappings provided through the following methods:
/// <list type="bullet">
/// <item><see cref="GetIndexedTypeParameterMappings"/></item>
/// <item><see cref="GetNamedTypeParameterMappings"/></item>
/// <item><see cref="GetParameterMappings"/></item>
/// </list></summary>
public abstract class ASemanticAttributeMapper<TData> : ISemanticAttributeMapper<TData>
{
    /// <summary>Provides adapters that may be applied to parsed arguments before they are recorded.</summary>
    protected static ISemanticArgumentAdapter<TData> Adapters { get; } = new SemanticArgumentAdapter<TData>();

    private bool IsInitialized { get; set; }

    private IReadOnlyDictionary<int, DSemanticAttributeParameterMapping<TData>> IndexedTypeParameterMappings { get; set; } = null!;
    private IReadOnlyDictionary<string, DSemanticAttributeParameterMapping<TData>> NamedTypeParameterMappings { get; set; } = null!;
    private IReadOnlyDictionary<string, DSemanticAttributeParameterMapping<TData>> ParameterMappings { get; set; } = null!;

    private void InitializeMapper()
    {
        var comparer = Comparer ?? throw new InvalidOperationException($"The provided {nameof(IEqualityComparer<string>)} was null.");

        var indexedTypeParameterMappings = GetIndexedTypeParameterMappings() ?? throw new InvalidOperationException($"The provided collection of indexed type-parameter mappings was null.");
        var namedTypeParameterMappings = GetNamedTypeParameterMappings() ?? throw new InvalidOperationException($"The provided collection of named type-parameter mappings was null.");
        var parameterMappings = GetParameterMappings() ?? throw new InvalidOperationException($"The provided collection of parameter mappings was null.");

        Dictionary<int, DSemanticAttributeParameterMapping<TData>> indexedTypeParameterMappingsDictionary = new();
        Dictionary<string, DSemanticAttributeParameterMapping<TData>> namedTypeParameterMappingsDictionary = new(comparer);
        Dictionary<string, DSemanticAttributeParameterMapping<TData>> parameterMappingsDictionary = new(comparer);

        PopulateDictionary(indexedTypeParameterMappingsDictionary, indexedTypeParameterMappings);
        PopulateDictionary(namedTypeParameterMappingsDictionary, namedTypeParameterMappings);
        PopulateDictionary(parameterMappingsDictionary, parameterMappings);

        IndexedTypeParameterMappings = indexedTypeParameterMappingsDictionary;
        NamedTypeParameterMappings = namedTypeParameterMappingsDictionary;
        ParameterMappings = parameterMappingsDictionary;

        IsInitialized = true;
    }

    private static void PopulateDictionary<T>(IDictionary<string, T> dictionary, IEnumerable<(string, T)> mappings)
    {
        foreach (var (parameterName, mapping) in mappings)
        {
            if (parameterName is null)
            {
                throw new InvalidOperationException($"The name of a parameter in the provided collection of mappings was null.");
            }

            if (mapping is null)
            {
                throw new InvalidOperationException($"An element in the provided collection of mappings was null.");
            }

            try
            {
                dictionary.Add(parameterName, mapping);
            }
            catch (ArgumentException e)
            {
                throw new InvalidOperationException($"A mapping for a parameter with the provided name, \"{parameterName}\", has already been added.", e);
            }
        }
    }

    private static void PopulateDictionary<T>(IDictionary<int, T> dictionary, IEnumerable<(int, T)> mappings)
    {
        foreach (var (parameterIndex, mapping) in mappings)
        {
            if (parameterIndex < 0)
            {
                throw new InvalidOperationException($"The index of a parameter in the provided collection of mappings was negative.");
            }

            if (mapping is null)
            {
                throw new InvalidOperationException($"An element in the provided collection of mappings was null.");
            }

            try
            {
                dictionary.Add(parameterIndex, mapping);
            }
            catch (ArgumentException e)
            {
                throw new InvalidOperationException($"A mapping for a parameter with the provided index, \"{parameterIndex}\", has already been added.", e);
            }
        }
    }

    /// <summary>Determines how equality will be determined when comparing parameter names. The default value is <see cref="StringComparer.OrdinalIgnoreCase"/>.</summary>
    protected virtual IEqualityComparer<string> Comparer => StringComparer.OrdinalIgnoreCase;

    /// <summary>Maps the indices of type-parameters to recorders, responsible for recording the argument of the parameter.</summary>
    /// <returns>The mappings from type-parameter index to recorder.</returns>
    protected virtual IEnumerable<(int, DSemanticAttributeParameterMapping<TData>)> GetIndexedTypeParameterMappings() => Enumerable.Empty<(int, DSemanticAttributeParameterMapping<TData>)>();

    /// <summary>Maps the names of type-parameters to recorders, responsible for recording the argument of the parameter.</summary>
    /// <returns>The mappings from type-parameter name to recorder.</returns>
    protected virtual IEnumerable<(string, DSemanticAttributeParameterMapping<TData>)> GetNamedTypeParameterMappings() => Enumerable.Empty<(string, DSemanticAttributeParameterMapping<TData>)>();

    /// <summary>Maps the names of constructor or named parameters to recorders, responsible for recording the argument of the parameter.</summary>
    /// <returns>The mappings from parameter name to recorder.</returns>
    protected virtual IEnumerable<(string, DSemanticAttributeParameterMapping<TData>)> GetParameterMappings() => Enumerable.Empty<(string, DSemanticAttributeParameterMapping<TData>)>();

    /// <inheritdoc/>
    public DSemanticAttributeArgumentRecorder? TryMapTypeParameter(TData dataRecord, ITypeParameterSymbol parameter)
    {
        if (IsInitialized is false)
        {
            InitializeMapper();
        }

        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        var hasIndexedMapping = IndexedTypeParameterMappings.TryGetValue(parameter.Ordinal, out var indexedMapping);
        var hasNamedMapping = NamedTypeParameterMappings.TryGetValue(parameter.Name, out var namedMapping);

        return (hasIndexedMapping, hasNamedMapping) switch
        {
            (false, false) => null,
            (true, false) => indexedMapping(dataRecord),
            (false, true) => namedMapping(dataRecord),
            (true, true) => attemptToResolveConflictingMappings(dataRecord, indexedMapping, namedMapping)
        };

        static DSemanticAttributeArgumentRecorder? attemptToResolveConflictingMappings(TData dataRecord, DSemanticAttributeParameterMapping<TData> indexedMapping, DSemanticAttributeParameterMapping<TData> namedMapping)
        {
            if (Equals(indexedMapping, namedMapping))
            {
                return indexedMapping(dataRecord);
            }

            return null;
        }
    }

    /// <inheritdoc/>
    public DSemanticAttributeArgumentRecorder? TryMapConstructorParameter(TData dataRecord, IParameterSymbol parameter)
    {
        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        return TryMapNamedParameter(dataRecord, parameter.Name);
    }

    /// <inheritdoc/>
    public DSemanticAttributeArgumentRecorder? TryMapNamedParameter(TData dataRecord, string parameterName)
    {
        if (IsInitialized is false)
        {
            InitializeMapper();
        }

        if (parameterName is null)
        {
            throw new ArgumentNullException(nameof(parameterName));
        }

        if (ParameterMappings.TryGetValue(parameterName, out var mapping) is false)
        {
            return null;
        }

        return mapping(dataRecord);
    }
}
