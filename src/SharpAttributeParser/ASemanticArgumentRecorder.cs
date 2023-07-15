namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>An abstract <see cref="ISemanticArgumentRecorder"/>, recording semantically parsed attribute arguments using recorders provided through the following methods:
/// <list type="bullet">
/// <item><see cref="AddIndexedGenericRecorders"/></item>
/// <item><see cref="AddNamedGenericRecorders"/></item>
/// <item><see cref="AddSingleRecorders"/></item>
/// <item><see cref="AddArrayRecorders"/></item>
/// </list></summary>
public abstract class ASemanticArgumentRecorder : ISemanticArgumentRecorder
{
    /// <summary>Provides adapters that may be applied to parsed arguments before they are recorded.</summary>
    protected static ISemanticAdapterProvider Adapters { get; } = new SemanticAdapterProvider();

    private bool IsInitialized { get; set; }

    private IReadOnlyDictionary<int, DSemanticGenericRecorder> IndexedGenericRecorders { get; set; } = null!;
    private IReadOnlyDictionary<string, DSemanticGenericRecorder> NamedGenericRecorders { get; set; } = null!;
    private IReadOnlyDictionary<string, DSemanticSingleRecorder> SingleRecorders { get; set; } = null!;
    private IReadOnlyDictionary<string, DSemanticArrayRecorder> ArrayRecorders { get; set; } = null!;

    private void InitializeRecorder()
    {
        var comparer = Comparer ?? throw new InvalidOperationException($"The provided {nameof(IEqualityComparer<string>)} was null.");

        var indexedGenericRecorderMappings = AddIndexedGenericRecorders() ?? throw new InvalidOperationException($"The provided collection of indexed type-parameter mappings was null.");
        var namedGenericRecorderMappings = AddNamedGenericRecorders() ?? throw new InvalidOperationException($"The provided collection of named type-parameter mappings was null.");
        var singleRecorderMappings = AddSingleRecorders() ?? throw new InvalidOperationException($"The provided collection of non-array-valued parameter mappings was null.");
        var arrayRecorderMappings = AddArrayRecorders() ?? throw new InvalidOperationException($"The provided collection of array-valued parameter mappings was null.");

        Dictionary<int, DSemanticGenericRecorder> indexedGenericRecorderDictionary = new();
        Dictionary<string, DSemanticGenericRecorder> namedGenericRecorderDictionary = new(comparer);
        Dictionary<string, DSemanticSingleRecorder> singleRecorderDictionary = new(comparer);
        Dictionary<string, DSemanticArrayRecorder> arrayRecorderDictionary = new(comparer);

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
    protected virtual IEnumerable<(int, DSemanticGenericRecorder)> AddIndexedGenericRecorders() => Enumerable.Empty<(int, DSemanticGenericRecorder)>();

    /// <summary>Maps the names of type-parameters to recorders, responsible for recording the argument of the parameter.</summary>
    /// <returns>The mappings from type-parameter name to recorder.</returns>
    protected virtual IEnumerable<(string, DSemanticGenericRecorder)> AddNamedGenericRecorders() => Enumerable.Empty<(string, DSemanticGenericRecorder)>();

    /// <summary>Maps the names of non-array-valued parameters to recorders, responsible for recording the argument of the parameter.</summary>
    /// <returns>The mappings from parameter name to recorder.</returns>
    protected virtual IEnumerable<(string, DSemanticSingleRecorder)> AddSingleRecorders() => Enumerable.Empty<(string, DSemanticSingleRecorder)>();

    /// <summary>Maps the names of array-valued parameters to recorders, responsible for recording the argument of the parameter.</summary>
    /// <returns>The mappings from parameter name to recorder.</returns>
    protected virtual IEnumerable<(string, DSemanticArrayRecorder)> AddArrayRecorders() => Enumerable.Empty<(string, DSemanticArrayRecorder)>();

    /// <inheritdoc/>
    public bool TryRecordGenericArgument(ITypeParameterSymbol parameter, ITypeSymbol value)
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

        var hasIndexedRecorder = IndexedGenericRecorders.TryGetValue(parameter.Ordinal, out var indexedRecorder);
        var hasNamedRecorder = NamedGenericRecorders.TryGetValue(parameter.Name, out var namedRecorder);

        return (hasIndexedRecorder, hasNamedRecorder) switch
        {
            (false, false) => false,
            (true, false) => indexedRecorder(value),
            (false, true) => namedRecorder(value),
            (true, true) => attemptToResolveOverlappingRecorders(indexedRecorder, namedRecorder, value)
        };

        static bool attemptToResolveOverlappingRecorders(DSemanticGenericRecorder indexedRecorder, DSemanticGenericRecorder namedRecorder, ITypeSymbol value)
        {
            if (Equals(indexedRecorder, namedRecorder))
            {
                return indexedRecorder(value);
            }

            return false;
        }
    }

    /// <inheritdoc/>
    public bool TryRecordConstructorArgument(IParameterSymbol parameter, object? value)
    {
        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        return TryRecordNamedArgument(parameter.Name, value);
    }

    /// <inheritdoc/>
    public bool TryRecordConstructorArgument(IParameterSymbol parameter, IReadOnlyList<object?>? value)
    {
        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        return TryRecordNamedArgument(parameter.Name, value);
    }

    /// <inheritdoc/>
    public bool TryRecordNamedArgument(string parameterName, object? value)
    {
        if (IsInitialized is false)
        {
            InitializeRecorder();
        }

        if (parameterName is null)
        {
            throw new ArgumentNullException(nameof(parameterName));
        }

        if (SingleRecorders.TryGetValue(parameterName, out var recorder) is false)
        {
            return false;
        }

        return recorder(value);
    }

    /// <inheritdoc/>
    public bool TryRecordNamedArgument(string parameterName, IReadOnlyList<object?>? value)
    {
        if (IsInitialized is false)
        {
            InitializeRecorder();
        }

        if (parameterName is null)
        {
            throw new ArgumentNullException(nameof(parameterName));
        }

        if (ArrayRecorders.TryGetValue(parameterName, out var recorder) is false)
        {
            return false;
        }

        return recorder(value);
    }
}
