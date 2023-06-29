namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>An abstract <see cref="ISemanticArgumentRecorder"/>, recording parsed attribute arguments using delegates accessed through <see cref="string"/>-dictionaries.</summary>
/// <remarks>Mappings from parameter names to delegates are added by overriding the following methods:
/// <list type="bullet">
/// <item><see cref="AddGenericRecorders"/></item>
/// <item><see cref="AddSingleRecorders"/></item>
/// <item><see cref="AddArrayRecorders"/></item>
/// </list></remarks>
public abstract class ASemanticArgumentRecorder : ISemanticArgumentRecorder
{
    /// <summary>Provides adapters that may be applied to parsed arguments before they are recorded.</summary>
    protected static ISemanticAdapterProvider Adapters { get; } = new SemanticAdapterProvider();

    private bool IsInitialized { get; set; }

    private IReadOnlyDictionary<string, DSemanticGenericRecorder> GenericRecorders { get; set; } = null!;
    private IReadOnlyDictionary<string, DSemanticSingleRecorder> SingleRecorders { get; set; } = null!;
    private IReadOnlyDictionary<string, DSemanticArrayRecorder> ArrayRecorders { get; set; } = null!;

    private void InitializeRecorder()
    {
        var comparer = Comparer ?? throw new InvalidOperationException($"The provided {nameof(IEqualityComparer<string>)} was null.");

        var genericRecorderMappings = AddGenericRecorders() ?? throw new InvalidOperationException($"The provided collection of type-parameter mappings was null.");
        var singleRecorderMappings = AddSingleRecorders() ?? throw new InvalidOperationException($"The provided collection of non-array-valued parameter mappings was null.");
        var arrayRecorderMappings = AddArrayRecorders() ?? throw new InvalidOperationException($"The provided collection of array-valued parameter mappings was null.");

        Dictionary<string, DSemanticGenericRecorder> genericRecorderDictionary = new(comparer);
        Dictionary<string, DSemanticSingleRecorder> singleRecorderDictionary = new(comparer);
        Dictionary<string, DSemanticArrayRecorder> arrayRecorderDictionary = new(comparer);

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
                throw new InvalidOperationException($"A {nameof(String)} in the provided collection of mappings was null.");
            }

            if (parameterName is "")
            {
                throw new InvalidOperationException($"A {nameof(String)} in the provided collection of mappings was empty.");
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

    /// <summary>Determines how equality will be determined when comparing parameter names. The default value is <see cref="StringComparer.OrdinalIgnoreCase"/>.</summary>
    protected virtual IEqualityComparer<string> Comparer => StringComparer.OrdinalIgnoreCase;

    /// <summary>Maps the names of type-parameters to recorders, responsible for recording the argument of the parameter.</summary>
    /// <returns>The mappings from parameter names to recorders.</returns>
    protected virtual IEnumerable<(string, DSemanticGenericRecorder)> AddGenericRecorders() => Enumerable.Empty<(string, DSemanticGenericRecorder)>();

    /// <summary>Maps the names of non-array-valued parameters to recorders, responsible for recording the argument of the parameter.</summary>
    /// <returns>The mappings from parameter names to recorders.</returns>
    protected virtual IEnumerable<(string, DSemanticSingleRecorder)> AddSingleRecorders() => Enumerable.Empty<(string, DSemanticSingleRecorder)>();

    /// <summary>Maps the names of array-valued parameters to recorders, responsible for recording the argument of the parameter.</summary>
    /// <returns>The mappings from parameter names to recorders.</returns>
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

        if (GenericRecorders.TryGetValue(parameter.Name, out var recorder) is false || recorder is null)
        {
            return false;
        }

        return recorder(value);
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

        var recorders = SingleRecorders;

        if (recorders is null || recorders.TryGetValue(parameterName, out var recorder) is false || recorder is null)
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

        var recorders = ArrayRecorders;

        if (recorders is null || recorders.TryGetValue(parameterName, out var recorder) is false || recorder is null)
        {
            return false;
        }

        return recorder(value);
    }
}
