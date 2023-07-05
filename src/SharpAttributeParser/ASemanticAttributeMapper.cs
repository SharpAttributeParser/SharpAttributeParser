namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using OneOf;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

/// <summary>An abstract <see cref="ISemanticAttributeMapper{TRecorder}"/>, using mappings provided through the following methods:
/// <list type="bullet">
/// <item><see cref="AddTypeParameterMappings"/></item>
/// <item><see cref="AddParameterMappings"/></item>
/// </list></summary>
/// <typeparam name="TData">The type to which the produced <see cref="SharpAttributeParser.DSemanticAttributeArgumentRecorder"/> records the arguments of attribute parameters.</typeparam>
public abstract class ASemanticAttributeMapper<TData> : ISemanticAttributeMapper<TData>
{
    /// <summary>Provides adapters that may be applied to parsed arguments before they are recorded.</summary>
    [SuppressMessage("Major Code Smell", "S2743: Static fields should not be used in generic types", Justification = "ISemanticArgumentAdapter uses TData.")]
    protected static ISemanticArgumentAdapterProvider Adapters { get; } = new SemanticArgumentAdapterProvider();

    private bool IsInitialized { get; set; }

    private IReadOnlyDictionary<int, DSemanticAttributeTypeArgumentRecorder> IndexedTypeParameterMappings { get; set; } = null!;
    private IReadOnlyDictionary<string, DSemanticAttributeTypeArgumentRecorder> NamedTypeParameterMappings { get; set; } = null!;
    private IReadOnlyDictionary<string, DSemanticAttributeArgumentRecorder> ParameterMappings { get; set; } = null!;

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

        Dictionary<int, DSemanticAttributeTypeArgumentRecorder> indexedTypeParameterMappingsDictionary = new();
        Dictionary<string, DSemanticAttributeTypeArgumentRecorder> namedTypeParameterMappingsDictionary = new(comparer);
        Dictionary<string, DSemanticAttributeArgumentRecorder> parameterMappingsDictionary = new(comparer);

        PopulateTypeMappingsDictionaries(indexedTypeParameterMappingsDictionary, namedTypeParameterMappingsDictionary, typeParameterMappings);
        PopulateMappingsDictionary(parameterMappingsDictionary, parameterMappings);

        IndexedTypeParameterMappings = indexedTypeParameterMappingsDictionary;
        NamedTypeParameterMappings = namedTypeParameterMappingsDictionary;
        ParameterMappings = parameterMappingsDictionary;

        IsInitialized = true;
    }

    private static void PopulateMappingsDictionary(IDictionary<string, DSemanticAttributeArgumentRecorder> dictionary, IEnumerable<(string, DSemanticAttributeArgumentRecorder)> mappings)
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

    private static void PopulateTypeMappingsDictionaries(IDictionary<int, DSemanticAttributeTypeArgumentRecorder> indexedDictionary, IDictionary<string, DSemanticAttributeTypeArgumentRecorder> namedDictionary, IEnumerable<(OneOf<int, string>, DSemanticAttributeTypeArgumentRecorder)> mappings)
    {
        foreach (var (parameter, mapping) in mappings)
        {
            if (mapping is null)
            {
                throw new InvalidOperationException("An element in the provided collection of mappings was null.");
            }

            var dictionaryDelegate = parameter.Match<Action<DSemanticAttributeTypeArgumentRecorder>>
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

    /// <summary>Determines equality when comparing parameter names. The default comparer is <see cref="StringComparer.OrdinalIgnoreCase"/>.</summary>
    protected virtual IEqualityComparer<string> GetComparer() => StringComparer.OrdinalIgnoreCase;

    /// <summary>Maps the indices of type-parameters to recorders, responsible for recording the argument of the parameter.</summary>
    /// <returns>The mappings from type-parameter index to recorder.</returns>
    protected virtual IEnumerable<(OneOf<int, string> Index, DSemanticAttributeTypeArgumentRecorder Mapping)> AddTypeParameterMappings() => Enumerable.Empty<(OneOf<int, string>, DSemanticAttributeTypeArgumentRecorder)>();

    /// <summary>Maps the names of constructor or named parameters to recorders, responsible for recording the argument of the parameter.</summary>
    /// <returns>The mappings from parameter name to recorder.</returns>
    protected virtual IEnumerable<(string Name, DSemanticAttributeArgumentRecorder Mapping)> AddParameterMappings() => Enumerable.Empty<(string, DSemanticAttributeArgumentRecorder)>();

    /// <inheritdoc/>
    public SharpAttributeParser.DSemanticAttributeArgumentRecorder? TryMapTypeParameter(TData dataRecord, ITypeParameterSymbol parameter)
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

        if (TryGetTypeParameterMapping(parameter) is not DSemanticAttributeTypeArgumentRecorder mapping)
        {
            return null;
        }

        return mappingWrapper;

        bool mappingWrapper(object? argument)
        {
            if (argument is not ITypeSymbol typeArgument)
            {
                return false;
            }

            return mapping(dataRecord, typeArgument);
        }
    }

    /// <inheritdoc/>
    public SharpAttributeParser.DSemanticAttributeArgumentRecorder? TryMapConstructorParameter(TData dataRecord, IParameterSymbol parameter)
    {
        if (dataRecord is null)
        {
            throw new ArgumentNullException(nameof(dataRecord));
        }

        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        return TryMapNamedParameter(dataRecord, parameter.Name);
    }

    /// <inheritdoc/>
    public SharpAttributeParser.DSemanticAttributeArgumentRecorder? TryMapNamedParameter(TData dataRecord, string parameterName)
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

        return mappingWrapper;

        bool mappingWrapper(object? argument) => mapping(dataRecord, argument);
    }

    private DSemanticAttributeTypeArgumentRecorder? TryGetTypeParameterMapping(ITypeParameterSymbol parameter)
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

        static DSemanticAttributeTypeArgumentRecorder? attemptToResolveConflictingMappings(DSemanticAttributeTypeArgumentRecorder indexedMapping, DSemanticAttributeTypeArgumentRecorder namedMapping)
        {
            if (Equals(indexedMapping, namedMapping))
            {
                return indexedMapping;
            }

            return null;
        }
    }

    /// <summary>Responsible for recording the semantically parsed argument of a type-parameter.</summary>
    /// <param name="dataRecord">The <typeparamref name="TData"/> to which the argument is recorded.</param>
    /// <param name="argument">The argument of the type-parameter.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    /// <exception cref="ArgumentNullException"/>
    protected delegate bool DSemanticAttributeTypeArgumentRecorder(TData dataRecord, ITypeSymbol argument);

    /// <summary>Responsible for recording the semantically parsed argument of a parameter.</summary>
    /// <param name="dataRecord">The <typeparamref name="TData"/> to which the argument is recorded.</param>
    /// <param name="argument">The argument of the parameter.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    /// <exception cref="ArgumentNullException"/>
    protected delegate bool DSemanticAttributeArgumentRecorder(TData dataRecord, object? argument);

    /// <summary>Provides adapters that may be applied to parsed attribute arguments before they are recorded.</summary>
    protected interface ISemanticArgumentAdapterProvider
    {
        /// <summary>Provides adapters related to type-arguments.</summary>
        public abstract ISemanticTypeArgumentAdapter Type { get; }

        /// <summary>Provides adapters related to simple, non-array valued arguments.</summary>
        public abstract ISemanticSimpleArgumentAdapter Simple { get; }

        /// <summary>Provides adapters related to array-valued arguments.</summary>
        public abstract ISemanticCollectionArgumentAdapter Collection { get; }
    }

    /// <summary>Provides adapters that may be applied to parsed attribute type-arguments before they are recorded.</summary>
    protected interface ISemanticTypeArgumentAdapter
    {
        /// <summary>Produces a type-argument recorder which wraps the provided recorder, and always returns <see langword="true"/>.</summary>
        /// <param name="recorder">Responsible for recording the argument of a type-parameter.</param>
        /// <returns>A recorder which wraps the provided recorder, and always returns <see langword="true"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DSemanticAttributeTypeArgumentRecorder For(Action<TData, ITypeSymbol> recorder);
    }

    /// <summary>Provides adapters that may be applied to parsed attribute arguments before they are recorded.</summary>
    protected interface ISemanticSimpleArgumentAdapter
    {
        /// <summary>Produces a recorder which ensures that the argument is of type <typeparamref name="T"/> before attempting to record it.</summary>
        /// <typeparam name="T">The expected type of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/>.</param>
        /// <returns>A mapping from some parameter to the produced recorder.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DSemanticAttributeArgumentRecorder For<T>(Func<TData, T, bool> recorder) where T : notnull;

        /// <summary>Produces a recorder which ensures that the argument is of type <typeparamref name="T"/> before attempting to record it - and which returns <see langword="true"/> if this is the case.</summary>
        /// <typeparam name="T">The expected type of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/>.</param>
        /// <returns>A mapping from some parameter to the produced recorder.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DSemanticAttributeArgumentRecorder For<T>(Action<TData, T> recorder) where T : notnull;

        /// <summary>Produces a recorder which ensures that the argument is of type <typeparamref name="T"/>, or <see langword="null"/>, before attempting to record it.</summary>
        /// <typeparam name="T">The expected type of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/> or <see langword="null"/>.</param>
        /// <returns>A mapping from some parameter to the produced recorder.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DSemanticAttributeArgumentRecorder ForNullable<T>(Func<TData, T?, bool> recorder) where T : class;

        /// <summary>Produces a recorder which ensures that the argument is of type <typeparamref name="T"/>, or <see langword="null"/>, before attempting to record it - and which returns <see langword="true"/> if this is the case.</summary>
        /// <typeparam name="T">The expected type of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/> or <see langword="null"/>.</param>
        /// <returns>A mapping from some parameter to the produced recorder.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DSemanticAttributeArgumentRecorder ForNullable<T>(Action<TData, T?> recorder) where T : class;

        /// <summary>Produces a recorder which ensures that the argument is of type <typeparamref name="T"/>, or <see langword="null"/>, before attempting to record it.</summary>
        /// <typeparam name="T">The expected type of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/> or <see langword="null"/>.</param>
        /// <returns>A mapping from some parameter to the produced recorder.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DSemanticAttributeArgumentRecorder ForNullable<T>(Func<TData, T?, bool> recorder) where T : struct;

        /// <summary>Produces a recorder which ensures that the argument is of type <typeparamref name="T"/>, or <see langword="null"/>, before attempting to record it - and which returns <see langword="true"/> if this is the case.</summary>
        /// <typeparam name="T">The expected type of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/> or <see langword="null"/>.</param>
        /// <returns>A mapping from some parameter to the produced recorder.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DSemanticAttributeArgumentRecorder ForNullable<T>(Action<TData, T?> recorder) where T : struct;
    }

    /// <summary>Provides adapters that may be applied to parsed array-valued attribute arguments before they are recorded.</summary>
    protected interface ISemanticCollectionArgumentAdapter
    {
        /// <summary>Produces a recorder which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before attempting to record it.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>A mapping from some parameter to the produced recorder.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DSemanticAttributeArgumentRecorder For<T>(Func<TData, IReadOnlyList<T>, bool> recorder) where T : notnull;

        /// <summary>Produces a recorder which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before attempting to record it - and which returns <see langword="true"/> if this is the case.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>A mapping from some parameter to the produced recorder.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DSemanticAttributeArgumentRecorder For<T>(Action<TData, IReadOnlyList<T>> recorder) where T : notnull;

        /// <summary>Produces a recorder which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before attempting to record it.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>A mapping from some parameter to the produced recorder.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DSemanticAttributeArgumentRecorder ForNullableCollection<T>(Func<TData, IReadOnlyList<T>?, bool> recorder) where T : notnull;

        /// <summary>Produces a recorder which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before attempting to record it - and which returns <see langword="true"/> if this is the case.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>A mapping from some parameter to the produced recorder.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DSemanticAttributeArgumentRecorder ForNullableCollection<T>(Action<TData, IReadOnlyList<T>?> recorder) where T : notnull;

        /// <summary>Produces a recorder which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before attempting to record it.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>A mapping from some parameter to the produced recorder.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DSemanticAttributeArgumentRecorder ForNullable<T>(Func<TData, IReadOnlyList<T?>?, bool> recorder) where T : class;

        /// <summary>Produces a recorder which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before attempting to record it - and which returns <see langword="true"/> if this is the case.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>A mapping from some parameter to the produced recorder.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DSemanticAttributeArgumentRecorder ForNullable<T>(Action<TData, IReadOnlyList<T?>?> recorder) where T : class;

        /// <summary>Produces a recorder which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before attempting to record it.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>A mapping from some parameter to the produced recorder.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DSemanticAttributeArgumentRecorder ForNullable<T>(Func<TData, IReadOnlyList<T?>?, bool> recorder) where T : struct;

        /// <summary>Produces a recorder which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before attempting to record it - and which returns <see langword="true"/> if this is the case.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>A mapping from some parameter to the produced recorder.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DSemanticAttributeArgumentRecorder ForNullable<T>(Action<TData, IReadOnlyList<T?>?> recorder) where T : struct;

        /// <summary>Produces a recorder which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before attempting to record it.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>A mapping from some parameter to the produced recorder.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DSemanticAttributeArgumentRecorder ForNullableElements<T>(Func<TData, IReadOnlyList<T?>, bool> recorder) where T : class;

        /// <summary>Produces a recorder which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before attempting to record it - and which returns <see langword="true"/> if this is the case.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>A mapping from some parameter to the produced recorder.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DSemanticAttributeArgumentRecorder ForNullableElements<T>(Action<TData, IReadOnlyList<T?>> recorder) where T : class;

        /// <summary>Produces a recorder which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before attempting to record it.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>A mapping from some parameter to the produced recorder.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DSemanticAttributeArgumentRecorder ForNullableElements<T>(Func<TData, IReadOnlyList<T?>, bool> recorder) where T : struct;

        /// <summary>Produces a recorder which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before attempting to record it - and which returns <see langword="true"/> if this is the case.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>A mapping from some parameter to the produced recorder.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DSemanticAttributeArgumentRecorder ForNullableElements<T>(Action<TData, IReadOnlyList<T?>> recorder) where T : struct;
    }

    private sealed class SemanticArgumentAdapterProvider : ISemanticArgumentAdapterProvider
    {
        ISemanticTypeArgumentAdapter ISemanticArgumentAdapterProvider.Type { get; } = new SemanticTypeArgumentAdapter();
        ISemanticSimpleArgumentAdapter ISemanticArgumentAdapterProvider.Simple { get; } = new SemanticSimpleArgumentAdapter();
        ISemanticCollectionArgumentAdapter ISemanticArgumentAdapterProvider.Collection { get; } = new SemanticCollectionArgumentAdapter();
    }

    private sealed class SemanticTypeArgumentAdapter : ISemanticTypeArgumentAdapter
    {
        DSemanticAttributeTypeArgumentRecorder ISemanticTypeArgumentAdapter.For(Action<TData, ITypeSymbol> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return wrapper;

            bool wrapper(TData data, ITypeSymbol argument)
            {
                recorder(data, argument);

                return true;
            }
        }
    }

    private sealed class SemanticSimpleArgumentAdapter : ISemanticSimpleArgumentAdapter
    {
        DSemanticAttributeArgumentRecorder ISemanticSimpleArgumentAdapter.For<T>(Func<TData, T, bool> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return For(recorder);
        }

        DSemanticAttributeArgumentRecorder ISemanticSimpleArgumentAdapter.For<T>(Action<TData, T> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return For<T>(wrapper);

            bool wrapper(TData data, T argument)
            {
                recorder(data, argument);

                return true;
            }
        }

        DSemanticAttributeArgumentRecorder ISemanticSimpleArgumentAdapter.ForNullable<T>(Func<TData, T?, bool> recorder) where T : class
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullable(recorder);
        }

        DSemanticAttributeArgumentRecorder ISemanticSimpleArgumentAdapter.ForNullable<T>(Action<TData, T?> recorder) where T : class
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullable<T>(wrapper);

            bool wrapper(TData data, T? argument)
            {
                recorder(data, argument);

                return true;
            }
        }

        DSemanticAttributeArgumentRecorder ISemanticSimpleArgumentAdapter.ForNullable<T>(Func<TData, T?, bool> recorder) where T : struct
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullable(recorder);
        }

        DSemanticAttributeArgumentRecorder ISemanticSimpleArgumentAdapter.ForNullable<T>(Action<TData, T?> recorder) where T : struct
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullable<T>(wrapper);

            bool wrapper(TData data, T? argument)
            {
                recorder(data, argument);

                return true;
            }
        }

        private static DSemanticAttributeArgumentRecorder For<T>(Func<TData, T, bool> recorder)
        {
            return wrapper;

            bool wrapper(TData data, object? argument)
            {
                if (argument is null)
                {
                    return false;
                }

                if (argument is not T tArgument)
                {
                    return false;
                }

                return recorder(data, tArgument);
            }
        }

        private static DSemanticAttributeArgumentRecorder ForNullable<T>(Func<TData, T?, bool> recorder) where T : class
        {
            return wrapper;

            bool wrapper(TData data, object? argument)
            {
                if (argument is null)
                {
                    return recorder(data, null);
                }

                if (argument is not T tArgument)
                {
                    return false;
                }

                return recorder(data, tArgument);
            }
        }

        private static DSemanticAttributeArgumentRecorder ForNullable<T>(Func<TData, T?, bool> recorder) where T : struct
        {
            return wrapper;

            bool wrapper(TData data, object? argument)
            {
                if (argument is null)
                {
                    return recorder(data, null);
                }

                if (argument is not T tArgument)
                {
                    return false;
                }

                return recorder(data, tArgument);
            }
        }
    }

    private sealed class SemanticCollectionArgumentAdapter : ISemanticCollectionArgumentAdapter
    {
        DSemanticAttributeArgumentRecorder ISemanticCollectionArgumentAdapter.For<T>(Func<TData, IReadOnlyList<T>, bool> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return For(recorder);
        }

        DSemanticAttributeArgumentRecorder ISemanticCollectionArgumentAdapter.For<T>(Action<TData, IReadOnlyList<T>> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return For<T>(wrapper);

            bool wrapper(TData data, IReadOnlyList<T> argument)
            {
                recorder(data, argument);

                return true;
            }
        }

        DSemanticAttributeArgumentRecorder ISemanticCollectionArgumentAdapter.ForNullable<T>(Func<TData, IReadOnlyList<T?>?, bool> recorder) where T : class
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullable(recorder);
        }

        DSemanticAttributeArgumentRecorder ISemanticCollectionArgumentAdapter.ForNullable<T>(Action<TData, IReadOnlyList<T?>?> recorder) where T : class
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullable<T>(wrapper);

            bool wrapper(TData data, IReadOnlyList<T?>? argument)
            {
                recorder(data, argument);

                return true;
            }
        }

        DSemanticAttributeArgumentRecorder ISemanticCollectionArgumentAdapter.ForNullable<T>(Func<TData, IReadOnlyList<T?>?, bool> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullable(recorder);
        }

        DSemanticAttributeArgumentRecorder ISemanticCollectionArgumentAdapter.ForNullable<T>(Action<TData, IReadOnlyList<T?>?> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullable<T>(wrapper);

            bool wrapper(TData data, IReadOnlyList<T?>? argument)
            {
                recorder(data, argument);

                return true;
            }
        }

        DSemanticAttributeArgumentRecorder ISemanticCollectionArgumentAdapter.ForNullableCollection<T>(Func<TData, IReadOnlyList<T>?, bool> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullableCollection(recorder);
        }

        DSemanticAttributeArgumentRecorder ISemanticCollectionArgumentAdapter.ForNullableCollection<T>(Action<TData, IReadOnlyList<T>?> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullableCollection<T>(wrapper);

            bool wrapper(TData data, IReadOnlyList<T>? argument)
            {
                recorder(data, argument);

                return true;
            }
        }

        DSemanticAttributeArgumentRecorder ISemanticCollectionArgumentAdapter.ForNullableElements<T>(Func<TData, IReadOnlyList<T?>, bool> recorder) where T : class
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullableElements(recorder);
        }

        DSemanticAttributeArgumentRecorder ISemanticCollectionArgumentAdapter.ForNullableElements<T>(Action<TData, IReadOnlyList<T?>> recorder) where T : class
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullableElements<T>(wrapper);

            bool wrapper(TData data, IReadOnlyList<T?> argument)
            {
                recorder(data, argument);

                return true;
            }
        }

        DSemanticAttributeArgumentRecorder ISemanticCollectionArgumentAdapter.ForNullableElements<T>(Func<TData, IReadOnlyList<T?>, bool> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullableElements(recorder);
        }

        DSemanticAttributeArgumentRecorder ISemanticCollectionArgumentAdapter.ForNullableElements<T>(Action<TData, IReadOnlyList<T?>> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullableElements<T>(wrapper);

            bool wrapper(TData data, IReadOnlyList<T?> argument)
            {
                recorder(data, argument);

                return true;
            }
        }

        private static DSemanticAttributeArgumentRecorder For<T>(Func<TData, IReadOnlyList<T>, bool> recorder)
        {
            return wrapper;

            bool wrapper(TData data, object? argument)
            {
                if (argument is null)
                {
                    return false;
                }

                if (argument is not IReadOnlyList<T> tListArgument)
                {
                    if (argument is not IReadOnlyList<object> objectListArgument)
                    {
                        return false;
                    }

                    var converted = new T[objectListArgument.Count];

                    for (var i = 0; i < objectListArgument.Count; i++)
                    {
                        if (objectListArgument[i] is not T tElement)
                        {
                            return false;
                        }

                        converted[i] = tElement;
                    }

                    return recorder(data, converted);
                }

                if (argument.GetType().GetElementType() != typeof(T))
                {
                    return false;
                }

                if (tListArgument.Any(static (argumentElement) => argumentElement is null))
                {
                    return false;
                }

                return recorder(data, tListArgument);
            }
        }

        private static DSemanticAttributeArgumentRecorder ForNullable<T>(Func<TData, IReadOnlyList<T?>?, bool> recorder) where T : class
        {
            return wrapper;

            bool wrapper(TData data, object? argument)
            {
                if (argument is null)
                {
                    return recorder(data, null);
                }

                if (argument is not IReadOnlyList<T> tListArgument)
                {
                    if (argument is not IReadOnlyList<object> objectListArgument)
                    {
                        return false;
                    }

                    var converted = new T?[objectListArgument.Count];

                    for (var i = 0; i < objectListArgument.Count; i++)
                    {
                        if (objectListArgument[i] is null)
                        {
                            converted[i] = null;

                            continue;
                        }

                        if (objectListArgument[i] is not T tElement)
                        {
                            return false;
                        }

                        converted[i] = tElement;
                    }

                    return recorder(data, converted);
                }

                return recorder(data, tListArgument);
            }
        }

        private static DSemanticAttributeArgumentRecorder ForNullable<T>(Func<TData, IReadOnlyList<T?>?, bool> recorder) where T : struct
        {
            return wrapper;

            bool wrapper(TData data, object? argument)
            {
                if (argument is null)
                {
                    return recorder(data, null);
                }

                if (argument is not IReadOnlyList<T?> tListArgument)
                {
                    if (argument is not IReadOnlyList<object> objectListArgument)
                    {
                        return false;
                    }

                    var converted = new T?[objectListArgument.Count];

                    for (var i = 0; i < objectListArgument.Count; i++)
                    {
                        if (objectListArgument[i] is null)
                        {
                            converted[i] = null;

                            continue;
                        }

                        if (objectListArgument[i] is not T tElement)
                        {
                            return false;
                        }

                        converted[i] = tElement;
                    }

                    return recorder(data, converted);
                }

                return recorder(data, tListArgument);
            }
        }

        private static DSemanticAttributeArgumentRecorder ForNullableElements<T>(Func<TData, IReadOnlyList<T?>, bool> recorder) where T : class
        {
            return ForNullable<T>(wrapper);

            bool wrapper(TData data, IReadOnlyList<T?>? argument)
            {
                if (argument is null)
                {
                    return false;
                }

                return recorder(data, argument);
            }
        }

        private static DSemanticAttributeArgumentRecorder ForNullableElements<T>(Func<TData, IReadOnlyList<T?>, bool> recorder) where T : struct
        {
            return ForNullable<T>(wrapper);

            bool wrapper(TData data, IReadOnlyList<T?>? argument)
            {
                if (argument is null)
                {
                    return false;
                }

                return recorder(data, argument);
            }
        }

        private static DSemanticAttributeArgumentRecorder ForNullableCollection<T>(Func<TData, IReadOnlyList<T>?, bool> recorder)
        {
            return wrapper;

            bool wrapper(TData data, object? argument)
            {
                if (argument is null)
                {
                    return recorder(data, null);
                }

                return For(recorder)(data, argument);
            }
        }
    }
}
