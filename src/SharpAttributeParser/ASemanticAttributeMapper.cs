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
/// <typeparam name="TRecord">The type to which the arguments of the mapped parameters are recorded.</typeparam>
public abstract class ASemanticAttributeMapper<TRecord> : ISemanticAttributeMapper<TRecord>
{
    /// <summary>Provides adapters that may be applied to parsed arguments before they are recorded.</summary>
    [SuppressMessage("Major Code Smell", "S2743: Static fields should not be used in generic types", Justification = "Type uses TRecord.")]
    protected static IArgumentAdapterProvider Adapters { get; } = new ArgumentAdapterProvider();

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
    public ISemanticAttributeArgumentRecorder? TryMapTypeParameter(ITypeParameterSymbol parameter, TRecord dataRecord)
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

        return new AttributeArgumentRecorder(dataRecord, typeCheckedMapping);

        bool typeCheckedMapping(TRecord dataRecord, object? argument)
        {
            if (argument is not ITypeSymbol typeArgument)
            {
                return false;
            }

            return mapping(dataRecord, typeArgument);
        }
    }

    /// <inheritdoc/>
    public ISemanticAttributeArgumentRecorder? TryMapConstructorParameter(IParameterSymbol parameter, TRecord dataRecord)
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
    public ISemanticAttributeArgumentRecorder? TryMapNamedParameter(string parameterName, TRecord dataRecord)
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
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    /// <exception cref="ArgumentNullException"/>
    protected delegate bool DTypeArgumentRecorder(TRecord dataRecord, ITypeSymbol argument);

    /// <summary>Responsible for recording the argument of a parameter.</summary>
    /// <param name="dataRecord">The <typeparamref name="TRecord"/> to which the argument is recorded.</param>
    /// <param name="argument">The argument of the parameter.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    /// <exception cref="ArgumentNullException"/>
    protected delegate bool DArgumentRecorder(TRecord dataRecord, object? argument);

    /// <summary>Provides adapters that may be applied to parsed attribute arguments before they are recorded.</summary>
    protected interface IArgumentAdapterProvider
    {
        /// <summary>Provides adapters related to type-arguments.</summary>
        public abstract ITypeArgumentAdapter Type { get; }

        /// <summary>Provides adapters related to simple, non-array valued arguments.</summary>
        public abstract ISimpleArgumentAdapter Simple { get; }

        /// <summary>Provides adapters related to array-valued arguments.</summary>
        public abstract ICollectionArgumentAdapter Collection { get; }
    }

    /// <summary>Provides adapters that may be applied to parsed attribute type-arguments before they are recorded.</summary>
    protected interface ITypeArgumentAdapter
    {
        /// <summary>Produces a type-argument recorder which wraps the provided recorder, and always returns <see langword="true"/>.</summary>
        /// <param name="recorder">Responsible for recording the argument of a type-parameter.</param>
        /// <returns>A recorder which wraps the provided recorder, and always returns <see langword="true"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DTypeArgumentRecorder For(Action<TRecord, ITypeSymbol> recorder);
    }

    /// <summary>Provides adapters that may be applied to parsed attribute arguments before they are recorded.</summary>
    protected interface ISimpleArgumentAdapter
    {
        /// <summary>Produces a recorder which ensures that the argument is of type <typeparamref name="T"/> before attempting to record it.</summary>
        /// <typeparam name="T">The expected type of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/>.</param>
        /// <returns>A mapping from some parameter to the produced recorder.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder For<T>(Func<TRecord, T, bool> recorder) where T : notnull;

        /// <summary>Produces a recorder which ensures that the argument is of type <typeparamref name="T"/> before attempting to record it - and which returns <see langword="true"/> if this is the case.</summary>
        /// <typeparam name="T">The expected type of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/>.</param>
        /// <returns>A mapping from some parameter to the produced recorder.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder For<T>(Action<TRecord, T> recorder) where T : notnull;

        /// <summary>Produces a recorder which ensures that the argument is of type <typeparamref name="T"/>, or <see langword="null"/>, before attempting to record it.</summary>
        /// <typeparam name="T">The expected type of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/> or <see langword="null"/>.</param>
        /// <returns>A mapping from some parameter to the produced recorder.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder ForNullable<T>(Func<TRecord, T?, bool> recorder) where T : class;

        /// <summary>Produces a recorder which ensures that the argument is of type <typeparamref name="T"/>, or <see langword="null"/>, before attempting to record it - and which returns <see langword="true"/> if this is the case.</summary>
        /// <typeparam name="T">The expected type of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/> or <see langword="null"/>.</param>
        /// <returns>A mapping from some parameter to the produced recorder.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder ForNullable<T>(Action<TRecord, T?> recorder) where T : class;

        /// <summary>Produces a recorder which ensures that the argument is of type <typeparamref name="T"/>, or <see langword="null"/>, before attempting to record it.</summary>
        /// <typeparam name="T">The expected type of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/> or <see langword="null"/>.</param>
        /// <returns>A mapping from some parameter to the produced recorder.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder ForNullable<T>(Func<TRecord, T?, bool> recorder) where T : struct;

        /// <summary>Produces a recorder which ensures that the argument is of type <typeparamref name="T"/>, or <see langword="null"/>, before attempting to record it - and which returns <see langword="true"/> if this is the case.</summary>
        /// <typeparam name="T">The expected type of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/> or <see langword="null"/>.</param>
        /// <returns>A mapping from some parameter to the produced recorder.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder ForNullable<T>(Action<TRecord, T?> recorder) where T : struct;
    }

    /// <summary>Provides adapters that may be applied to parsed array-valued attribute arguments before they are recorded.</summary>
    protected interface ICollectionArgumentAdapter
    {
        /// <summary>Produces a recorder which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before attempting to record it.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>A mapping from some parameter to the produced recorder.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder For<T>(Func<TRecord, IReadOnlyList<T>, bool> recorder) where T : notnull;

        /// <summary>Produces a recorder which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before attempting to record it - and which returns <see langword="true"/> if this is the case.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>A mapping from some parameter to the produced recorder.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder For<T>(Action<TRecord, IReadOnlyList<T>> recorder) where T : notnull;

        /// <summary>Produces a recorder which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before attempting to record it.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>A mapping from some parameter to the produced recorder.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder ForNullableCollection<T>(Func<TRecord, IReadOnlyList<T>?, bool> recorder) where T : notnull;

        /// <summary>Produces a recorder which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before attempting to record it - and which returns <see langword="true"/> if this is the case.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>A mapping from some parameter to the produced recorder.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder ForNullableCollection<T>(Action<TRecord, IReadOnlyList<T>?> recorder) where T : notnull;

        /// <summary>Produces a recorder which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before attempting to record it.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>A mapping from some parameter to the produced recorder.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder ForNullable<T>(Func<TRecord, IReadOnlyList<T?>?, bool> recorder) where T : class;

        /// <summary>Produces a recorder which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before attempting to record it - and which returns <see langword="true"/> if this is the case.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>A mapping from some parameter to the produced recorder.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder ForNullable<T>(Action<TRecord, IReadOnlyList<T?>?> recorder) where T : class;

        /// <summary>Produces a recorder which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before attempting to record it.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>A mapping from some parameter to the produced recorder.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder ForNullable<T>(Func<TRecord, IReadOnlyList<T?>?, bool> recorder) where T : struct;

        /// <summary>Produces a recorder which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before attempting to record it - and which returns <see langword="true"/> if this is the case.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>A mapping from some parameter to the produced recorder.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder ForNullable<T>(Action<TRecord, IReadOnlyList<T?>?> recorder) where T : struct;

        /// <summary>Produces a recorder which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before attempting to record it.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>A mapping from some parameter to the produced recorder.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder ForNullableElements<T>(Func<TRecord, IReadOnlyList<T?>, bool> recorder) where T : class;

        /// <summary>Produces a recorder which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before attempting to record it - and which returns <see langword="true"/> if this is the case.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>A mapping from some parameter to the produced recorder.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder ForNullableElements<T>(Action<TRecord, IReadOnlyList<T?>> recorder) where T : class;

        /// <summary>Produces a recorder which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before attempting to record it.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>A mapping from some parameter to the produced recorder.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder ForNullableElements<T>(Func<TRecord, IReadOnlyList<T?>, bool> recorder) where T : struct;

        /// <summary>Produces a recorder which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before attempting to record it - and which returns <see langword="true"/> if this is the case.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>A mapping from some parameter to the produced recorder.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder ForNullableElements<T>(Action<TRecord, IReadOnlyList<T?>> recorder) where T : struct;
    }

    private sealed class AttributeArgumentRecorder : ISemanticAttributeArgumentRecorder
    {
        private TRecord DataRecord { get; }
        private DArgumentRecorder Recorder { get; }

        public AttributeArgumentRecorder(TRecord dataRecord, DArgumentRecorder recorder)
        {
            DataRecord = dataRecord;
            Recorder = recorder;
        }

        bool ISemanticAttributeArgumentRecorder.RecordArgument(object? argument) => Recorder(DataRecord, argument);
    }

    private sealed class ArgumentAdapterProvider : IArgumentAdapterProvider
    {
        ITypeArgumentAdapter IArgumentAdapterProvider.Type { get; } = new TypeArgumentAdapter();
        ISimpleArgumentAdapter IArgumentAdapterProvider.Simple { get; } = new SimpleArgumentAdapter();
        ICollectionArgumentAdapter IArgumentAdapterProvider.Collection { get; } = new CollectionArgumentAdapter();
    }

    private sealed class TypeArgumentAdapter : ITypeArgumentAdapter
    {
        DTypeArgumentRecorder ITypeArgumentAdapter.For(Action<TRecord, ITypeSymbol> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return wrapper;

            bool wrapper(TRecord dataRecord, ITypeSymbol argument)
            {
                if (dataRecord is null)
                {
                    throw new ArgumentNullException(nameof(dataRecord));
                }

                if (argument is null)
                {
                    throw new ArgumentNullException(nameof(argument));
                }

                recorder(dataRecord, argument);

                return true;
            }
        }
    }

    private sealed class SimpleArgumentAdapter : ISimpleArgumentAdapter
    {
        DArgumentRecorder ISimpleArgumentAdapter.For<T>(Func<TRecord, T, bool> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return For(recorder);
        }

        DArgumentRecorder ISimpleArgumentAdapter.For<T>(Action<TRecord, T> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return For<T>(wrapper);

            bool wrapper(TRecord dataRecord, T argument)
            {
                recorder(dataRecord, argument);

                return true;
            }
        }

        DArgumentRecorder ISimpleArgumentAdapter.ForNullable<T>(Func<TRecord, T?, bool> recorder) where T : class
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullable(recorder);
        }

        DArgumentRecorder ISimpleArgumentAdapter.ForNullable<T>(Action<TRecord, T?> recorder) where T : class
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullable<T>(wrapper);

            bool wrapper(TRecord dataRecord, T? argument)
            {
                recorder(dataRecord, argument);

                return true;
            }
        }

        DArgumentRecorder ISimpleArgumentAdapter.ForNullable<T>(Func<TRecord, T?, bool> recorder) where T : struct
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullable(recorder);
        }

        DArgumentRecorder ISimpleArgumentAdapter.ForNullable<T>(Action<TRecord, T?> recorder) where T : struct
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullable<T>(wrapper);

            bool wrapper(TRecord dataRecord, T? argument)
            {
                recorder(dataRecord, argument);

                return true;
            }
        }

        private static DArgumentRecorder For<T>(Func<TRecord, T, bool> recorder)
        {
            return wrapper;

            bool wrapper(TRecord dataRecord, object? argument)
            {
                if (dataRecord is null)
                {
                    throw new ArgumentNullException(nameof(dataRecord));
                }

                if (argument is null)
                {
                    return false;
                }

                if (argument is not T tArgument)
                {
                    return false;
                }

                return recorder(dataRecord, tArgument);
            }
        }

        private static DArgumentRecorder ForNullable<T>(Func<TRecord, T?, bool> recorder) where T : class
        {
            return wrapper;

            bool wrapper(TRecord dataRecord, object? argument)
            {
                if (dataRecord is null)
                {
                    throw new ArgumentNullException(nameof(dataRecord));
                }

                if (argument is null)
                {
                    return recorder(dataRecord, null);
                }

                if (argument is not T tArgument)
                {
                    return false;
                }

                return recorder(dataRecord, tArgument);
            }
        }

        private static DArgumentRecorder ForNullable<T>(Func<TRecord, T?, bool> recorder) where T : struct
        {
            return wrapper;

            bool wrapper(TRecord dataRecord, object? argument)
            {
                if (dataRecord is null)
                {
                    throw new ArgumentNullException(nameof(dataRecord));
                }

                if (argument is null)
                {
                    return recorder(dataRecord, null);
                }

                if (argument is not T tArgument)
                {
                    return false;
                }

                return recorder(dataRecord, tArgument);
            }
        }
    }

    private sealed class CollectionArgumentAdapter : ICollectionArgumentAdapter
    {
        DArgumentRecorder ICollectionArgumentAdapter.For<T>(Func<TRecord, IReadOnlyList<T>, bool> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return For(recorder);
        }

        DArgumentRecorder ICollectionArgumentAdapter.For<T>(Action<TRecord, IReadOnlyList<T>> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return For<T>(wrapper);

            bool wrapper(TRecord dataRecord, IReadOnlyList<T> argument)
            {
                recorder(dataRecord, argument);

                return true;
            }
        }

        DArgumentRecorder ICollectionArgumentAdapter.ForNullable<T>(Func<TRecord, IReadOnlyList<T?>?, bool> recorder) where T : class
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullable(recorder);
        }

        DArgumentRecorder ICollectionArgumentAdapter.ForNullable<T>(Action<TRecord, IReadOnlyList<T?>?> recorder) where T : class
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullable<T>(wrapper);

            bool wrapper(TRecord dataRecord, IReadOnlyList<T?>? argument)
            {
                recorder(dataRecord, argument);

                return true;
            }
        }

        DArgumentRecorder ICollectionArgumentAdapter.ForNullable<T>(Func<TRecord, IReadOnlyList<T?>?, bool> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullable(recorder);
        }

        DArgumentRecorder ICollectionArgumentAdapter.ForNullable<T>(Action<TRecord, IReadOnlyList<T?>?> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullable<T>(wrapper);

            bool wrapper(TRecord dataRecord, IReadOnlyList<T?>? argument)
            {
                recorder(dataRecord, argument);

                return true;
            }
        }

        DArgumentRecorder ICollectionArgumentAdapter.ForNullableCollection<T>(Func<TRecord, IReadOnlyList<T>?, bool> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullableCollection(recorder);
        }

        DArgumentRecorder ICollectionArgumentAdapter.ForNullableCollection<T>(Action<TRecord, IReadOnlyList<T>?> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullableCollection<T>(wrapper);

            bool wrapper(TRecord dataRecord, IReadOnlyList<T>? argument)
            {
                recorder(dataRecord, argument);

                return true;
            }
        }

        DArgumentRecorder ICollectionArgumentAdapter.ForNullableElements<T>(Func<TRecord, IReadOnlyList<T?>, bool> recorder) where T : class
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullableElements(recorder);
        }

        DArgumentRecorder ICollectionArgumentAdapter.ForNullableElements<T>(Action<TRecord, IReadOnlyList<T?>> recorder) where T : class
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullableElements<T>(wrapper);

            bool wrapper(TRecord dataRecord, IReadOnlyList<T?> argument)
            {
                recorder(dataRecord, argument);

                return true;
            }
        }

        DArgumentRecorder ICollectionArgumentAdapter.ForNullableElements<T>(Func<TRecord, IReadOnlyList<T?>, bool> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullableElements(recorder);
        }

        DArgumentRecorder ICollectionArgumentAdapter.ForNullableElements<T>(Action<TRecord, IReadOnlyList<T?>> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullableElements<T>(wrapper);

            bool wrapper(TRecord dataRecord, IReadOnlyList<T?> argument)
            {
                recorder(dataRecord, argument);

                return true;
            }
        }

        private static DArgumentRecorder For<T>(Func<TRecord, IReadOnlyList<T>, bool> recorder)
        {
            return wrapper;

            bool wrapper(TRecord dataRecord, object? argument)
            {
                if (dataRecord is null)
                {
                    throw new ArgumentNullException(nameof(dataRecord));
                }

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

                    return recorder(dataRecord, converted);
                }

                if (argument.GetType().GetElementType() != typeof(T))
                {
                    return false;
                }

                if (tListArgument.Any(static (argumentElement) => argumentElement is null))
                {
                    return false;
                }

                return recorder(dataRecord, tListArgument);
            }
        }

        private static DArgumentRecorder ForNullable<T>(Func<TRecord, IReadOnlyList<T?>?, bool> recorder) where T : class
        {
            return wrapper;

            bool wrapper(TRecord dataRecord, object? argument)
            {
                if (dataRecord is null)
                {
                    throw new ArgumentNullException(nameof(dataRecord));
                }

                if (argument is null)
                {
                    return recorder(dataRecord, null);
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

                    return recorder(dataRecord, converted);
                }

                return recorder(dataRecord, tListArgument);
            }
        }

        private static DArgumentRecorder ForNullable<T>(Func<TRecord, IReadOnlyList<T?>?, bool> recorder) where T : struct
        {
            return wrapper;

            bool wrapper(TRecord dataRecord, object? argument)
            {
                if (dataRecord is null)
                {
                    throw new ArgumentNullException(nameof(dataRecord));
                }

                if (argument is null)
                {
                    return recorder(dataRecord, null);
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

                    return recorder(dataRecord, converted);
                }

                return recorder(dataRecord, tListArgument);
            }
        }

        private static DArgumentRecorder ForNullableElements<T>(Func<TRecord, IReadOnlyList<T?>, bool> recorder) where T : class
        {
            return ForNullable<T>(wrapper);

            bool wrapper(TRecord dataRecord, IReadOnlyList<T?>? argument)
            {
                if (argument is null)
                {
                    return false;
                }

                return recorder(dataRecord, argument);
            }
        }

        private static DArgumentRecorder ForNullableElements<T>(Func<TRecord, IReadOnlyList<T?>, bool> recorder) where T : struct
        {
            return ForNullable<T>(wrapper);

            bool wrapper(TRecord dataRecord, IReadOnlyList<T?>? argument)
            {
                if (argument is null)
                {
                    return false;
                }

                return recorder(dataRecord, argument);
            }
        }

        private static DArgumentRecorder ForNullableCollection<T>(Func<TRecord, IReadOnlyList<T>?, bool> recorder)
        {
            return wrapper;

            bool wrapper(TRecord dataRecord, object? argument)
            {
                if (argument is null)
                {
                    return recorder(dataRecord, null);
                }

                return For(recorder)(dataRecord, argument);
            }
        }
    }
}
