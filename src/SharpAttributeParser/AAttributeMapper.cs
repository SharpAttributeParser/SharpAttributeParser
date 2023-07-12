namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>An abstract <see cref="IAttributeMapper{TRecorder}"/>, using mappings provided through the following methods:
/// <list type="bullet">
/// <item><see cref="AddTypeParameterMappings"/></item>
/// <item><see cref="AddParameterMappings"/></item>
/// </list></summary>
/// <typeparam name="TRecord">The type to which the arguments of the mapped parameters are recorded.</typeparam>
public abstract class AAttributeMapper<TRecord> : IAttributeMapper<TRecord>
{
    private bool IsInitialized { get; set; }

    private IReadOnlyDictionary<int, DTypeArgumentRecorder> IndexedTypeParameterMappings { get; set; } = null!;
    private IReadOnlyDictionary<string, DTypeArgumentRecorder> NamedTypeParameterMappings { get; set; } = null!;
    private IReadOnlyDictionary<string, DArgumentRecorder> ParameterMappings { get; set; } = null!;

    /// <summary>Initializes the mapper. If not invoked, initialization will occur when the mapper is first used.</summary>
    /// <exception cref="InvalidOperationException"/>
    protected void InitializeMapper()
    {
        if (IsInitialized)
        {
            return;
        }

        var comparer = GetComparer() ?? throw new InvalidOperationException("The provided string-equality comparer was null.");

        var typeParameterMappings = AddTypeParameterMappings() ?? throw new InvalidOperationException("The provided collection of type-parameter mappings was null.");
        var parameterMappings = AddParameterMappings() ?? throw new InvalidOperationException("The provided collection of parameter mappings was null.");

        Dictionary<int, DTypeArgumentRecorder> indexedTypeParameterMappingsDictionary = new();
        Dictionary<string, DTypeArgumentRecorder> namedTypeParameterMappingsDictionary = new(comparer);
        Dictionary<string, DArgumentRecorder> parameterMappingsDictionary = new(comparer);

        PopulateTypeMappingsDictionaries(indexedTypeParameterMappingsDictionary, namedTypeParameterMappingsDictionary, typeParameterMappings);
        PopulateMappingsDictionary(parameterMappingsDictionary, parameterMappings);

        IndexedTypeParameterMappings = indexedTypeParameterMappingsDictionary;
        NamedTypeParameterMappings = namedTypeParameterMappingsDictionary;
        ParameterMappings = parameterMappingsDictionary;

        IsInitialized = true;
    }

    private static void PopulateTypeMappingsDictionaries(IDictionary<int, DTypeArgumentRecorder> indexedDictionary, IDictionary<string, DTypeArgumentRecorder> namedDictionary, IEnumerable<(OneOf<int, string>, DTypeArgumentRecorder)> mappings)
    {
        foreach (var (parameter, mapping) in mappings)
        {
            if (mapping is null)
            {
                throw new InvalidOperationException("An element in the provided collection of mappings was null.");
            }

            var dictionaryDelegate = parameter.Match<Action<DTypeArgumentRecorder>>
            (
                (index) =>
                {
                    if (index < 0)
                    {
                        throw new InvalidOperationException("The index of a parameter in the provided collection of mappings was negative.");
                    }

                    return (value) => indexedDictionary.Add(index, value);
                },
                (name) =>
                {
                    if (name is null)
                    {
                        throw new InvalidOperationException("The name of a parameter in the provided collection of mappings was null.");
                    }

                    return (value) => namedDictionary.Add(name, value);
                }
            );

            try
            {
                dictionaryDelegate(mapping);
            }
            catch (ArgumentException e)
            {
                throw new InvalidOperationException($"A mapping for a parameter with the provided index, \"{parameter}\", has already been added.", e);
            }
        }
    }

    private static void PopulateMappingsDictionary(IDictionary<string, DArgumentRecorder> dictionary, IEnumerable<(string, DArgumentRecorder)> mappings)
    {
        foreach (var (parameterName, mapping) in mappings)
        {
            if (parameterName is null)
            {
                throw new InvalidOperationException("The name of a parameter in the provided collection of parameter mappings was null.");
            }

            if (mapping is null)
            {
                throw new InvalidOperationException("An element in the provided collection of parameter mappings was null.");
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

    /// <summary>Determines equality when comparing parameter names. The default comparer is <see cref="StringComparer.OrdinalIgnoreCase"/>.</summary>
    protected virtual IEqualityComparer<string> GetComparer() => StringComparer.OrdinalIgnoreCase;

    /// <summary>Maps the indices of type-parameters to recorders, responsible for recording the argument of the parameter.</summary>
    /// <returns>The mappings from type-parameter index to recorder.</returns>
    protected virtual IEnumerable<(OneOf<int, string> Index, DTypeArgumentRecorder Mapping)> AddTypeParameterMappings() => Enumerable.Empty<(OneOf<int, string>, DTypeArgumentRecorder)>();

    /// <summary>Maps the names of constructor or named parameters to recorders, responsible for recording the argument of the parameter.</summary>
    /// <returns>The mappings from parameter name to recorder.</returns>
    protected virtual IEnumerable<(string Name, DArgumentRecorder Mapping)> AddParameterMappings() => Enumerable.Empty<(string, DArgumentRecorder)>();

    /// <inheritdoc/>
    public IAttributeArgumentRecorder? TryMapTypeParameter(ITypeParameterSymbol parameter, TRecord dataRecord)
    {
        if (dataRecord is null)
        {
            throw new ArgumentNullException(nameof(dataRecord));
        }

        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        InitializeMapper();

        if (TryGetTypeParameterMapping(parameter) is not DTypeArgumentRecorder mapping)
        {
            return null;
        }

        return new AttributeTypeArgumentRecorder(dataRecord, mapping);
    }

    /// <inheritdoc/>
    public IAttributeConstructorArgumentRecorder? TryMapConstructorParameter(IParameterSymbol parameter, TRecord dataRecord)
    {
        if (dataRecord is null)
        {
            throw new ArgumentNullException(nameof(dataRecord));
        }

        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        InitializeMapper();

        if (ParameterMappings.TryGetValue(parameter.Name, out var mapping) is false)
        {
            return null;
        }

        return new AttributeConstructorArgumentRecorder(dataRecord, mapping);
    }

    /// <inheritdoc/>
    public IAttributeArgumentRecorder? TryMapNamedParameter(string parameterName, TRecord dataRecord)
    {
        if (dataRecord is null)
        {
            throw new ArgumentNullException(nameof(dataRecord));
        }

        if (parameterName is null)
        {
            throw new ArgumentNullException(nameof(parameterName));
        }

        InitializeMapper();

        if (ParameterMappings.TryGetValue(parameterName, out var mapping) is false)
        {
            return null;
        }

        return new AttributeArgumentRecorder(dataRecord, mapping);
    }

    private DTypeArgumentRecorder? TryGetTypeParameterMapping(ITypeParameterSymbol parameter)
    {
        var hasIndexedMapping = IndexedTypeParameterMappings.TryGetValue(parameter.Ordinal, out var indexedMapping);
        var hasNamedMapping = NamedTypeParameterMappings.TryGetValue(parameter.Name, out var namedMapping);

        return (hasIndexedMapping, hasNamedMapping) switch
        {
            (false, false) => null,
            (true, false) => indexedMapping,
            (false, true) => namedMapping,
            (true, true) => attemptToResolveConflictingMappings(indexedMapping, namedMapping)
        };

        static DTypeArgumentRecorder? attemptToResolveConflictingMappings(DTypeArgumentRecorder indexedMapping, DTypeArgumentRecorder namedMapping)
        {
            if (Equals(indexedMapping, namedMapping))
            {
                return indexedMapping;
            }

            return null;
        }
    }

    /// <summary>Responsible for recording the argument of a type-parameter.</summary>
    /// <param name="dataRecord">The <typeparamref name="TRecord"/> to which the argument is recorded.</param>
    /// <param name="argument">The argument of the type-parameter.</param>
    /// <param name="syntax">The syntactial information about the argument of the parameter.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    /// <exception cref="ArgumentNullException"/>
    protected delegate bool DTypeArgumentRecorder(TRecord dataRecord, ITypeSymbol argument, ExpressionSyntax syntax);

    /// <summary>Responsible for recording the argument of a parameter.</summary>
    /// <param name="dataRecord">The <typeparamref name="TRecord"/> to which the argument is recorded.</param>
    /// <param name="argument">The argument of the parameter.</param>
    /// <param name="syntax">The <see cref="ExpressionSyntax"/> syntactically describing the argument of the parameter, or each element if expressed as a <see langword="params"/>-array.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    /// <exception cref="ArgumentNullException"/>
    protected delegate bool DArgumentRecorder(TRecord dataRecord, object? argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax);

    private sealed class AttributeTypeArgumentRecorder : IAttributeArgumentRecorder
    {
        private TRecord DataRecord { get; }
        private DTypeArgumentRecorder Recorder { get; }

        public AttributeTypeArgumentRecorder(TRecord dataRecord, DTypeArgumentRecorder recorder)
        {
            DataRecord = dataRecord;
            Recorder = recorder;
        }

        bool IAttributeArgumentRecorder.RecordArgument(object? argument, ExpressionSyntax syntax)
        {
            if (argument is not ITypeSymbol typeArgument)
            {
                return false;
            }

            return Recorder(DataRecord, typeArgument, syntax);
        }
    }

    private sealed class AttributeArgumentRecorder : IAttributeArgumentRecorder
    {
        private TRecord DataRecord { get; }
        private DArgumentRecorder Recorder { get; }

        public AttributeArgumentRecorder(TRecord dataRecord, DArgumentRecorder recorder)
        {
            DataRecord = dataRecord;
            Recorder = recorder;
        }

        bool IAttributeArgumentRecorder.RecordArgument(object? argument, ExpressionSyntax syntax) => Recorder(DataRecord, argument, syntax);
    }

    private sealed class AttributeConstructorArgumentRecorder : IAttributeConstructorArgumentRecorder
    {
        private TRecord DataRecord { get; }
        private DArgumentRecorder Recorder { get; }

        public AttributeConstructorArgumentRecorder(TRecord dataRecord, DArgumentRecorder recorder)
        {
            DataRecord = dataRecord;
            Recorder = recorder;
        }

        bool IAttributeArgumentRecorder.RecordArgument(object? argument, ExpressionSyntax syntax) => Recorder(DataRecord, argument, syntax);
        bool IAttributeConstructorArgumentRecorder.RecordParamsArgument(object? argument, IReadOnlyList<ExpressionSyntax> elementSyntax) => Recorder(DataRecord, argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(elementSyntax));
    }
}
