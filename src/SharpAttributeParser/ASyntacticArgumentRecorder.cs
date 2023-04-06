namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;

/// <summary>An abstract <see cref="ISyntacticArgumentRecorder"/>, recording parsed attribute arguments using delegates accessed through <see cref="string"/>-dictionaries.</summary>
/// <remarks>Mappings from parameter names to delegates are added by overriding the following methods:
/// <list type="bullet">
/// <item><see cref="AddGenericRecorders"/></item>
/// <item><see cref="AddSingleRecorders"/></item>
/// <item><see cref="AddArrayRecorders"/></item>
/// </list></remarks>
public abstract class ASyntacticArgumentRecorder : ISyntacticArgumentRecorder
{
    /// <summary>Provides adapters that may be applied to parsed arguments before they are recorded.</summary>
    protected static ISyntacticAdapterProvider Adapters { get; } = new SyntacticAdapterProvider(new SemanticAdapterProvider());

    private bool IsInitialized { get; set; }

    private IReadOnlyDictionary<string, DSyntacticGenericRecorder> GenericRecorders { get; set; } = null!;
    private IReadOnlyDictionary<string, DSyntacticSingleRecorder> SingleRecorders { get; set; } = null!;
    private IReadOnlyDictionary<string, DSyntacticArrayRecorder> ArrayRecorders { get; set; } = null!;

    private void InitializeRecorder()
    {
        var comparer = Comparer ?? throw new InvalidOperationException($"The provided {nameof(IEqualityComparer<string>)} was null.");

        var genericRecorderMappings = AddGenericRecorders() ?? throw new InvalidOperationException($"The provided collection of {nameof(String)}-{nameof(DSyntacticGenericRecorder)} mappings was null.");
        var singleRecorderMappings = AddSingleRecorders() ?? throw new InvalidOperationException($"The provided collection of {nameof(String)}-{nameof(DSyntacticSingleRecorder)} mappings was null.");
        var arrayRecorderMappings = AddArrayRecorders() ?? throw new InvalidOperationException($"The provided collection of {nameof(String)}-{nameof(DSyntacticArrayRecorder)} mappings was null.");

        Dictionary<string, DSyntacticGenericRecorder> genericRecorderDictionary = new(comparer);
        Dictionary<string, DSyntacticSingleRecorder> singleRecorderDictionary = new(comparer);
        Dictionary<string, DSyntacticArrayRecorder> arrayRecorderDictionary = new(comparer);

        PopulateDictionary(genericRecorderDictionary, genericRecorderMappings);
        PopulateDictionary(singleRecorderDictionary, singleRecorderMappings);
        PopulateDictionary(arrayRecorderDictionary, arrayRecorderMappings);

        GenericRecorders = genericRecorderDictionary;
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
                throw new InvalidOperationException($"A {nameof(String)} in the provided collection of {nameof(String)}-{typeof(T).Name} mappings was null.");
            }

            if (parameterName is "")
            {
                throw new InvalidOperationException($"A {nameof(String)} in the provided collection of {nameof(String)}-{typeof(T).Name} mappings was null.");
            }

            if (recorder is null)
            {
                throw new InvalidOperationException($"A {typeof(T).Name} in the provided collection of mappings was null.");
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

    /// <summary>Determines how equality will be determined when comparing parameter names. The default value is <see cref="StringComparer.OrdinalIgnoreCase"/>.</summary>
    protected virtual IEqualityComparer<string> Comparer => StringComparer.OrdinalIgnoreCase;

    /// <summary>Adds mappings from the names of type parameters to <see cref="DSyntacticGenericRecorder"/>, responsible for recording the argument of the parameter.</summary>
    /// <returns>The mappings from parameter names to <see cref="DSyntacticGenericRecorder"/>.</returns>
    /// <exception cref="ArgumentNullException"/>
    protected virtual IEnumerable<(string, DSyntacticGenericRecorder)> AddGenericRecorders() => Array.Empty<(string, DSyntacticGenericRecorder)>();

    /// <summary>Adds mappings from the names of non-array-valued parameters to <see cref="DSyntacticSingleRecorder"/>, responsible for recording the argument of the parameter.</summary>
    /// <returns>The mappings from parameter names to <see cref="DSyntacticSingleRecorder"/>.</returns>
    /// <exception cref="ArgumentNullException"/>
    protected virtual IEnumerable<(string, DSyntacticSingleRecorder)> AddSingleRecorders() => Array.Empty<(string, DSyntacticSingleRecorder)>();

    /// <summary>Adds mappings from the names of array-valued parameters to <see cref="DSyntacticArrayRecorder"/>, responsible for recording the argument of the parameter.</summary>
    /// <returns>The mappings from parameter names to <see cref="DSyntacticArrayRecorder"/>.</returns>
    /// <exception cref="ArgumentNullException"/>
    protected virtual IEnumerable<(string, DSyntacticArrayRecorder)> AddArrayRecorders() => Array.Empty<(string, DSyntacticArrayRecorder)>();

    /// <inheritdoc/>
    public bool TryRecordGenericArgument(string parameterName, ITypeSymbol value, Location location)
    {
        if (IsInitialized is false)
        {
            InitializeRecorder();
        }

        if (parameterName is null)
        {
            throw new ArgumentNullException(nameof(parameterName));
        }

        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (location is null)
        {
            throw new ArgumentNullException(nameof(location));
        }

        var recorders = GenericRecorders;

        if (recorders is null || recorders.TryGetValue(parameterName, out var recorder) is false || recorder is null)
        {
            return false;
        }

        return recorder(value, location);
    }

    /// <inheritdoc/>
    public bool TryRecordConstructorArgument(string parameterName, object? value, Location location)
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

        var recorders = SingleRecorders;

        if (recorders is null || recorders.TryGetValue(parameterName, out var recorder) is false || recorder is null)
        {
            return false;
        }

        return recorder(value, location);
    }

    /// <inheritdoc/>
    public bool TryRecordConstructorArgument(string parameterName, IReadOnlyList<object?>? value, Location collectionLocation, IReadOnlyList<Location> elementLocations)
    {
        if (IsInitialized is false)
        {
            InitializeRecorder();
        }

        if (parameterName is null)
        {
            throw new ArgumentNullException(nameof(parameterName));
        }

        if (collectionLocation is null)
        {
            throw new ArgumentNullException(nameof(collectionLocation));
        }

        if (elementLocations is null)
        {
            throw new ArgumentNullException(nameof(elementLocations));
        }

        var recorders = ArrayRecorders;

        if (recorders is null || recorders.TryGetValue(parameterName, out var recorder) is false || recorder is null)
        {
            return false;
        }

        return recorder(value, collectionLocation, elementLocations);
    }

    /// <inheritdoc/>
    public bool TryRecordNamedArgument(string parameterName, object? value, Location location) => TryRecordConstructorArgument(parameterName, value, location);

    /// <inheritdoc/>
    public bool TryRecordNamedArgument(string parameterName, IReadOnlyList<object?>? value, Location collectionLocation, IReadOnlyList<Location> elementLocations) => TryRecordConstructorArgument(parameterName, value, collectionLocation, elementLocations);
}
