namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

/// <summary>An abstract <see cref="ISyntacticAttributeMapper{TRecorder}"/>, using mappings provided through the following methods:
/// <list type="bullet">
/// <item><see cref="AddTypeParameterMappings"/></item>
/// <item><see cref="AddParameterMappings"/></item>
/// </list></summary>
/// <typeparam name="TRecord">The type to which the syntactical information about arguments of the mapped parameters are recorded.</typeparam>
public abstract class ASyntacticAttributeMapper<TRecord> : ISyntacticAttributeMapper<TRecord>
{
    /// <summary>Provides adapters that may be applied to syntactical information before it is recorded.</summary>
    [SuppressMessage("Major Code Smell", "S2743: Static fields should not be used in generic types", Justification = "Type uses TRecord.")]
    protected static ISyntaxAdapter Adapters { get; } = new SyntaxAdapter();

    private bool IsInitialized { get; set; }

    private IReadOnlyDictionary<int, DTypeArgumentSyntaxRecorder> IndexedTypeParameterMappings { get; set; } = null!;
    private IReadOnlyDictionary<string, DTypeArgumentSyntaxRecorder> NamedTypeParameterMappings { get; set; } = null!;
    private IReadOnlyDictionary<string, DArgumentSyntaxRecorder> ParameterMappings { get; set; } = null!;

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

        Dictionary<int, DTypeArgumentSyntaxRecorder> indexedTypeParameterMappingsDictionary = new();
        Dictionary<string, DTypeArgumentSyntaxRecorder> namedTypeParameterMappingsDictionary = new(comparer);
        Dictionary<string, DArgumentSyntaxRecorder> parameterMappingsDictionary = new(comparer);

        PopulateTypeMappingsDictionaries(indexedTypeParameterMappingsDictionary, namedTypeParameterMappingsDictionary, typeParameterMappings);
        PopulateMappingsDictionary(parameterMappingsDictionary, parameterMappings);

        IndexedTypeParameterMappings = indexedTypeParameterMappingsDictionary;
        NamedTypeParameterMappings = namedTypeParameterMappingsDictionary;
        ParameterMappings = parameterMappingsDictionary;

        IsInitialized = true;
    }

    private static void PopulateTypeMappingsDictionaries(IDictionary<int, DTypeArgumentSyntaxRecorder> indexedDictionary, IDictionary<string, DTypeArgumentSyntaxRecorder> namedDictionary, IEnumerable<(OneOf<int, string>, DTypeArgumentSyntaxRecorder)> mappings)
    {
        foreach (var (parameter, mapping) in mappings)
        {
            if (mapping is null)
            {
                throw new InvalidOperationException("An element in the provided collection of mappings was null.");
            }

            var dictionaryDelegate = parameter.Match<Action<DTypeArgumentSyntaxRecorder>>
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

    private static void PopulateMappingsDictionary(IDictionary<string, DArgumentSyntaxRecorder> dictionary, IEnumerable<(string, DArgumentSyntaxRecorder)> mappings)
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
    protected virtual IEnumerable<(OneOf<int, string> Index, DTypeArgumentSyntaxRecorder Mapping)> AddTypeParameterMappings() => Enumerable.Empty<(OneOf<int, string>, DTypeArgumentSyntaxRecorder)>();

    /// <summary>Maps the names of constructor or named parameters to recorders, responsible for recording the argument of the parameter.</summary>
    /// <returns>The mappings from parameter name to recorder.</returns>
    protected virtual IEnumerable<(string Name, DArgumentSyntaxRecorder Mapping)> AddParameterMappings() => Enumerable.Empty<(string, DArgumentSyntaxRecorder)>();

    /// <inheritdoc/>
    public ISyntacticAttributeArgumentRecorder? TryMapTypeParameter(ITypeParameterSymbol parameter, TRecord dataRecord)
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

        if (TryGetTypeParameterMapping(parameter) is not DTypeArgumentSyntaxRecorder mapping)
        {
            return null;
        }

        return new AttributeTypeArgumentRecorder(dataRecord, mapping);
    }

    /// <inheritdoc/>
    public ISyntacticAttributeArgumentRecorder? TryMapConstructorParameter(IParameterSymbol parameter, TRecord dataRecord)
    {
        if (dataRecord is null)
        {
            throw new ArgumentNullException(nameof(dataRecord));
        }

        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        return TryMapNamedParameter(parameter.Name, dataRecord);
    }

    /// <inheritdoc/>
    public ISyntacticAttributeArgumentRecorder? TryMapNamedParameter(string parameterName, TRecord dataRecord)
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

    private DTypeArgumentSyntaxRecorder? TryGetTypeParameterMapping(ITypeParameterSymbol parameter)
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

        static DTypeArgumentSyntaxRecorder? attemptToResolveConflictingMappings(DTypeArgumentSyntaxRecorder indexedMapping, DTypeArgumentSyntaxRecorder namedMapping)
        {
            if (Equals(indexedMapping, namedMapping))
            {
                return indexedMapping;
            }

            return null;
        }
    }

    /// <summary>Responsible for recording the syntactical information about the argument of a type-parameter.</summary>
    /// <param name="dataRecord">The <typeparamref name="TRecord"/> to which the argument is recorded.</param>
    /// <param name="syntax">The syntactial information about the argument of the parameter.</param>
    /// <returns>A <see cref="bool"/> indicating whether the syntactical information was successfully recorded.</returns>
    /// <exception cref="ArgumentNullException"/>
    protected delegate bool DTypeArgumentSyntaxRecorder(TRecord dataRecord, ExpressionSyntax syntax);

    /// <summary>Responsible for recording the syntactical information about the argument of a parameter, which may have been expressed as a <see langword="params"/>-array.</summary>
    /// <param name="dataRecord">The <typeparamref name="TRecord"/> to which the argument is recorded.</param>
    /// <param name="syntax">The syntactial information about the argument of the parameter - either of a single expression or of each element in a <see langword="params"/>-array.</param>
    /// <returns>A <see cref="bool"/> indicating whether the syntactical information was successfully recorded.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    protected delegate bool DArgumentSyntaxRecorder(TRecord dataRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax);

    /// <summary>Provides adapters that may be applied to syntactial information before it is recorded.</summary>
    protected interface ISyntaxAdapter
    {
        /// <summary>Produces a recorder which wraps the provided recorder, and always returns <see langword="true"/>.</summary>
        /// <param name="recorder">Responsible for recording the syntactical information about the argument of a type-parameter.</param>
        /// <returns>A recorder which wraps the provided recorder, and always returns <see langword="true"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DTypeArgumentSyntaxRecorder ForType(Action<TRecord, ExpressionSyntax> recorder);

        /// <summary>Produces a recorder which wraps the provided recorder, and always returns <see langword="true"/>.</summary>
        /// <param name="recorder">Responsible for recording the syntactical information about the argument of a parameter.</param>
        /// <returns>A recorder which wraps the provided recorder, and always returns <see langword="true"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentSyntaxRecorder ForParams(Action<TRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> recorder);

        /// <summary>Produces a recorder which wraps the provided recorder, and returns <see langword="false"/> for <see langword="params"/>-arguments.</summary>
        /// <param name="recorder">Responsible for recording the syntactical information about the argument of a non-<see langword="params"/> parameter.</param>
        /// <returns>A recorder which wraps the provided recorder, and returns <see langword="false"/> for <see langword="params"/>-arguments.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentSyntaxRecorder ForNonParams(Func<TRecord, ExpressionSyntax, bool> recorder);

        /// <summary>Produces a recorder which wraps the provided recorder, and returns <see langword="false"/> for <see langword="params"/>-arguments - and <see langword="true"/> otherwise.</summary>
        /// <param name="recorder">Responsible for recording the syntactical information about the argument of a non-<see langword="params"/> parameter.</param>
        /// <returns>A recorder which wraps the provided recorder, and always returns <see langword="true"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentSyntaxRecorder ForNonParams(Action<TRecord, ExpressionSyntax> recorder);
    }

    private sealed class AttributeTypeArgumentRecorder : ISyntacticAttributeArgumentRecorder
    {
        private TRecord DataRecord { get; }
        private DTypeArgumentSyntaxRecorder Recorder { get; }

        public AttributeTypeArgumentRecorder(TRecord dataRecord, DTypeArgumentSyntaxRecorder recorder)
        {
            DataRecord = dataRecord;
            Recorder = recorder;
        }

        bool ISyntacticAttributeArgumentRecorder.RecordArgumentSyntax(ExpressionSyntax syntax) => Recorder(DataRecord, syntax);
        bool ISyntacticAttributeArgumentRecorder.RecordParamsArgumentSyntax(IReadOnlyList<ExpressionSyntax> elementSyntax) => false;
    }

    private sealed class AttributeArgumentRecorder : ISyntacticAttributeArgumentRecorder
    {
        private TRecord DataRecord { get; }
        private DArgumentSyntaxRecorder Recorder { get; }

        public AttributeArgumentRecorder(TRecord dataRecord, DArgumentSyntaxRecorder recorder)
        {
            DataRecord = dataRecord;
            Recorder = recorder;
        }

        bool ISyntacticAttributeArgumentRecorder.RecordArgumentSyntax(ExpressionSyntax syntax) => Recorder(DataRecord, syntax);
        bool ISyntacticAttributeArgumentRecorder.RecordParamsArgumentSyntax(IReadOnlyList<ExpressionSyntax> elementSyntax) => Recorder(DataRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(elementSyntax));
    }

    private sealed class SyntaxAdapter : ISyntaxAdapter
    {
        DTypeArgumentSyntaxRecorder ASyntacticAttributeMapper<TRecord>.ISyntaxAdapter.ForType(Action<TRecord, ExpressionSyntax> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return wrapper;

            bool wrapper(TRecord dataRecord, ExpressionSyntax syntax)
            {
                if (dataRecord is null)
                {
                    throw new ArgumentNullException(nameof(dataRecord));
                }

                if (syntax is null)
                {
                    throw new ArgumentNullException(nameof(syntax));
                }

                recorder(dataRecord, syntax);

                return true;
            }
        }

        DArgumentSyntaxRecorder ASyntacticAttributeMapper<TRecord>.ISyntaxAdapter.ForParams(Action<TRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return wrapper;

            bool wrapper(TRecord dataRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
            {
                if (dataRecord is null)
                {
                    throw new ArgumentNullException(nameof(dataRecord));
                }

                var syntaxParameterName = nameof(syntax);

                syntax.Switch
                (
                    (syntax) =>
                    {
                        if (syntax is null)
                        {
                            throw new ArgumentException($"The {nameof(ExpressionSyntax)} of the provided {nameof(OneOf<object, object>)} was null.", syntaxParameterName);
                        }
                    },
                    (elementSyntax) =>
                    {
                        if (elementSyntax is null)
                        {
                            throw new ArgumentException($"The {nameof(ExpressionSyntax)}-collection of the provided {nameof(OneOf<object, object>)} was null.", syntaxParameterName);
                        }
                    }
                );

                recorder(dataRecord, syntax);

                return true;
            }
        }

        DArgumentSyntaxRecorder ASyntacticAttributeMapper<TRecord>.ISyntaxAdapter.ForNonParams(Func<TRecord, ExpressionSyntax, bool> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNonParams(recorder);
        }

        DArgumentSyntaxRecorder ASyntacticAttributeMapper<TRecord>.ISyntaxAdapter.ForNonParams(Action<TRecord, ExpressionSyntax> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNonParams(wrapper);

            bool wrapper(TRecord dataRecord, ExpressionSyntax syntax)
            {
                recorder(dataRecord, syntax);

                return true;
            }
        }

        private static DArgumentSyntaxRecorder ForNonParams(Func<TRecord, ExpressionSyntax, bool> recorder)
        {
            return wrapper;

            bool wrapper(TRecord dataRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
            {
                if (dataRecord is null)
                {
                    throw new ArgumentNullException(nameof(dataRecord));
                }

                if (syntax.IsT0 && syntax.AsT0 is null)
                {
                    throw new ArgumentException($"The {nameof(ExpressionSyntax)} of the provided {nameof(OneOf<object, object>)} was null.", nameof(syntax));
                }

                if (syntax.IsT1)
                {
                    return false;
                }

                return recorder(dataRecord, syntax.AsT0);
            }
        }
    }
}
