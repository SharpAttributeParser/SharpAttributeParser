namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>An abstract <see cref="ISyntacticArgumentRecorder"/>, recording syntactically parsed attribute arguments using recorders provided through the following methods:
/// <list type="bullet">
/// <item><see cref="AddIndexedGenericRecorders"/></item>
/// <item><see cref="AddNamedGenericRecorders"/></item>
/// <item><see cref="AddSingleRecorders"/></item>
/// <item><see cref="AddArrayRecorders"/></item>
/// </list></summary>
public abstract class ASyntacticArgumentRecorder : ISyntacticArgumentRecorder
{
    /// <summary>Provides adapters that may be applied to parsed arguments before they are recorded.</summary>
    protected static ISyntacticAdapterProvider Adapters { get; } = new SyntacticAdapterProvider(new SemanticAdapterProvider());

    private bool IsInitialized { get; set; }

    private IReadOnlyDictionary<int, DSyntacticGenericRecorder> IndexedGenericRecorders { get; set; } = null!;
    private IReadOnlyDictionary<string, DSyntacticGenericRecorder> NamedGenericRecorders { get; set; } = null!;
    private IReadOnlyDictionary<string, DSyntacticSingleRecorder> SingleRecorders { get; set; } = null!;
    private IReadOnlyDictionary<string, DSyntacticArrayRecorder> ArrayRecorders { get; set; } = null!;

    private void InitializeRecorder()
    {
        var comparer = Comparer ?? throw new InvalidOperationException($"The provided {nameof(IEqualityComparer<string>)} was null.");

        var indexedGenericRecorderMappings = AddIndexedGenericRecorders() ?? throw new InvalidOperationException($"The provided collection of indexed type-parameter mappings was null.");
        var namedGenericRecorderMappings = AddNamedGenericRecorders() ?? throw new InvalidOperationException($"The provided collection of named type-parameter mappings was null.");
        var singleRecorderMappings = AddSingleRecorders() ?? throw new InvalidOperationException($"The provided collection of non-array-valued parameter mappings was null.");
        var arrayRecorderMappings = AddArrayRecorders() ?? throw new InvalidOperationException($"The provided collection of array-valued parameter mappings was null.");

        Dictionary<int, DSyntacticGenericRecorder> indexedGenericRecorderDictionary = new();
        Dictionary<string, DSyntacticGenericRecorder> namedGenericRecorderDictionary = new(comparer);
        Dictionary<string, DSyntacticSingleRecorder> singleRecorderDictionary = new(comparer);
        Dictionary<string, DSyntacticArrayRecorder> arrayRecorderDictionary = new(comparer);

        PopulateDictionary(indexedGenericRecorderDictionary, indexedGenericRecorderMappings);
        PopulateDictionary(namedGenericRecorderDictionary, namedGenericRecorderMappings);
        PopulateDictionary(singleRecorderDictionary, singleRecorderMappings);
        PopulateDictionary(arrayRecorderDictionary, arrayRecorderMappings);

        IndexedGenericRecorders = indexedGenericRecorderDictionary;
        NamedGenericRecorders = namedGenericRecorderDictionary;
        SingleRecorders = singleRecorderDictionary;
        ArrayRecorders = arrayRecorderDictionary;

        IsInitialized = true;
    }

    private static void PopulateDictionary<T>(IDictionary<string, T> dictionary, IEnumerable<(string, T)> mappings)
    {
        foreach (var (parameterName, recorder) in mappings)
        {
            if (parameterName is null)
            {
                throw new InvalidOperationException($"The name of a parameter in the provided collection of mappings was null.");
            }

            if (recorder is null)
            {
                throw new InvalidOperationException($"A recorder in the provided collection of mappings was null.");
            }

            try
            {
                dictionary.Add(parameterName, recorder);
            }
            catch (ArgumentException e)
            {
                throw new InvalidOperationException($"A parameter with the provided name, \"{parameterName}\", has already been added.", e);
            }
        }
    }

    private static void PopulateDictionary<T>(IDictionary<int, T> dictionary, IEnumerable<(int, T)> mappings)
    {
        foreach (var (parameterIndex, recorder) in mappings)
        {
            if (parameterIndex < 0)
            {
                throw new InvalidOperationException($"The index of a parameter in the provided collection of mappings was negative.");
            }

            if (recorder is null)
            {
                throw new InvalidOperationException($"A recorder in the provided collection of mappings was null.");
            }

            try
            {
                dictionary.Add(parameterIndex, recorder);
            }
            catch (ArgumentException e)
            {
                throw new InvalidOperationException($"A parameter with the provided index, \"{parameterIndex}\", has already been added.", e);
            }
        }
    }

    /// <summary>Determines how equality will be determined when comparing parameter names. The default value is <see cref="StringComparer.OrdinalIgnoreCase"/>.</summary>
    protected virtual IEqualityComparer<string> Comparer => StringComparer.OrdinalIgnoreCase;

    /// <summary>Maps the indices of type-parameters to recorders, responsible for recording the argument of the parameter.</summary>
    /// <returns>The mappings from type-parameter index to recorder.</returns>
    protected virtual IEnumerable<(int, DSyntacticGenericRecorder)> AddIndexedGenericRecorders() => Enumerable.Empty<(int, DSyntacticGenericRecorder)>();

    /// <summary>Maps the names of type-parameters to recorders, responsible for recording the argument of the parameter.</summary>
    /// <returns>The mappings from type-parameter name to recorder.</returns>
    protected virtual IEnumerable<(string, DSyntacticGenericRecorder)> AddNamedGenericRecorders() => Enumerable.Empty<(string, DSyntacticGenericRecorder)>();

    /// <summary>Maps the names of non-array-valued parameters to recorders, responsible for recording the argument of the parameter.</summary>
    /// <returns>The mappings from parameter name to recorder.</returns>
    protected virtual IEnumerable<(string, DSyntacticSingleRecorder)> AddSingleRecorders() => Enumerable.Empty<(string, DSyntacticSingleRecorder)>();

    /// <summary>Maps the names of array-valued parameters to recorders, responsible for recording the argument of the parameter.</summary>
    /// <returns>The mappings from parameter name to recorder.</returns>
    protected virtual IEnumerable<(string, DSyntacticArrayRecorder)> AddArrayRecorders() => Enumerable.Empty<(string, DSyntacticArrayRecorder)>();

    /// <inheritdoc/>
    public bool TryRecordGenericArgument(ITypeParameterSymbol parameter, ITypeSymbol value, Location location)
    {
        if (IsInitialized is false)
        {
            InitializeRecorder();
        }

        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (location is null)
        {
            throw new ArgumentNullException(nameof(location));
        }

        var hasIndexedRecorder = IndexedGenericRecorders.TryGetValue(parameter.Ordinal, out var indexedRecorder);
        var hasNamedRecorder = NamedGenericRecorders.TryGetValue(parameter.Name, out var namedRecorder);

        return (hasIndexedRecorder, hasNamedRecorder) switch
        {
            (false, false) => false,
            (true, false) => indexedRecorder(value, location),
            (false, true) => namedRecorder(value, location),
            (true, true) => attemptToResolveOverlappingRecorders(indexedRecorder, namedRecorder, value, location)
        };

        static bool attemptToResolveOverlappingRecorders(DSyntacticGenericRecorder indexedRecorder, DSyntacticGenericRecorder namedRecorder, ITypeSymbol value, Location location)
        {
            if (Equals(indexedRecorder, namedRecorder))
            {
                return indexedRecorder(value, location);
            }

            return false;
        }
    }

    /// <inheritdoc/>
    public bool TryRecordConstructorArgument(IParameterSymbol parameter, object? value, Location location)
    {
        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        return TryRecordNamedArgument(parameter.Name, value, location);
    }

    /// <inheritdoc/>
    public bool TryRecordConstructorArgument(IParameterSymbol parameter, IReadOnlyList<object?>? value, CollectionLocation location)
    {
        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        return TryRecordNamedArgument(parameter.Name, value, location);
    }

    /// <inheritdoc/>
    public bool TryRecordNamedArgument(string parameterName, object? value, Location location)
    {
        if (IsInitialized is false)
        {
            InitializeRecorder();
        }

        if (parameterName is null)
        {
            throw new ArgumentNullException(nameof(parameterName));
        }

        if (location is null)
        {
            throw new ArgumentNullException(nameof(location));
        }

        if (SingleRecorders.TryGetValue(parameterName, out var recorder) is false)
        {
            return false;
        }

        return recorder(value, location);
    }

    /// <inheritdoc/>
    public bool TryRecordNamedArgument(string parameterName, IReadOnlyList<object?>? value, CollectionLocation location)
    {
        if (IsInitialized is false)
        {
            InitializeRecorder();
        }

        if (parameterName is null)
        {
            throw new ArgumentNullException(nameof(parameterName));
        }

        if (location is null)
        {
            throw new ArgumentNullException(nameof(location));
        }

        if (ArrayRecorders.TryGetValue(parameterName, out var recorder) is false)
        {
            return false;
        }

        return recorder(value, location);
    }
}
