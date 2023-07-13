namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

/// <summary>An abstract <see cref="IAttributeMapper{TRecorder}"/>, using mappings provided through the following methods:
/// <list type="bullet">
/// <item><see cref="AddTypeParameterMappings"/></item>
/// <item><see cref="AddParameterMappings"/></item>
/// </list></summary>
/// <typeparam name="TRecord">The type to which the arguments of the mapped parameters are recorded.</typeparam>
public abstract class AAttributeMapper<TRecord> : IAttributeMapper<TRecord>
{
    /// <summary>Provides adapters that may be applied to parsed arguments before invoking a recorder.</summary>
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
                throw new InvalidOperationException("A recorder in the provided collection of mappings was null.");
            }

            var dictionaryDelegate = parameter.Match<Action<DTypeArgumentRecorder>>
            (
                (index) =>
                {
                    if (index < 0)
                    {
                        throw new InvalidOperationException("The index of a parameter in the provided collection of mappings was negative.");
                    }

                    return (recorder) => indexedDictionary.Add(index, recorder);
                },
                (name) =>
                {
                    if (name is null)
                    {
                        throw new InvalidOperationException("The name of a parameter in the provided collection of mappings was null.");
                    }

                    return (recorder) => namedDictionary.Add(name, recorder);
                }
            );

            try
            {
                dictionaryDelegate(mapping);
            }
            catch (ArgumentException e)
            {
                parameter.Switch
                (
                    (index) => throw new InvalidOperationException($"A recorder has already been mapped to a parameter with the provided index, \"{index}\".", e),
                    (name) => throw new InvalidOperationException($"A recorder has already been mapped to a parameter with the provided name, \"{name}\".", e)
                );
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
                throw new InvalidOperationException("A recorder in the provided collection of parameter mappings was null.");
            }

            try
            {
                dictionary.Add(parameterName, mapping);
            }
            catch (ArgumentException e)
            {
                throw new InvalidOperationException($"A recorder has already been mapped to a parameter with the provided name, \"{parameterName}\".", e);
            }
        }
    }

    /// <summary>Determines equality when comparing parameter names. The default comparer is <see cref="StringComparer.OrdinalIgnoreCase"/>.</summary>
    protected virtual IEqualityComparer<string> GetComparer() => StringComparer.OrdinalIgnoreCase;

    /// <summary>Maps the indices of type-parameters to recorders, responsible for recording the argument of the parameter.</summary>
    /// <returns>The mappings from type-parameter index to recorder.</returns>
    protected virtual IEnumerable<(OneOf<int, string> IndexOrName, DTypeArgumentRecorder Recorder)> AddTypeParameterMappings() => Enumerable.Empty<(OneOf<int, string>, DTypeArgumentRecorder)>();

    /// <summary>Maps the names of constructor or named parameters to recorders, responsible for recording the argument of the parameter.</summary>
    /// <returns>The mappings from parameter name to recorder.</returns>
    protected virtual IEnumerable<(string Name, DArgumentRecorder Recorder)> AddParameterMappings() => Enumerable.Empty<(string, DArgumentRecorder)>();

    /// <inheritdoc/>
    public IAttributeArgumentRecorder? TryMapTypeParameter(ITypeParameterSymbol parameter, TRecord dataRecord)
    {
        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        if (dataRecord is null)
        {
            throw new ArgumentNullException(nameof(dataRecord));
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
        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        if (dataRecord is null)
        {
            throw new ArgumentNullException(nameof(dataRecord));
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
        if (parameterName is null)
        {
            throw new ArgumentNullException(nameof(parameterName));
        }

        if (dataRecord is null)
        {
            throw new ArgumentNullException(nameof(dataRecord));
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
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    protected delegate bool DArgumentRecorder(TRecord dataRecord, object? argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax);

    /// <summary>Provides adapters that may be applied to parsed attribute arguments before invoking a recorder.</summary>
    protected interface IArgumentAdapterProvider
    {
        /// <summary>Provides adapters related to type-arguments.</summary>
        public abstract ITypeArgumentAdapter TypeArgument { get; }

        /// <summary>Provides adapters related to simple, non-array valued arguments.</summary>
        public abstract ISimpleArgumentAdapter SimpleArgument { get; }

        /// <summary>Provides adapters related to array-valued arguments, which may not be expressed as <see langword="params"/>-arrays.</summary>
        public abstract INonParamsArrayArgumentAdapter NonParamsArrayArgument { get; }

        /// <summary>Provides adapters related to array-valued arguments, which may be expressed as <see langword="params"/>-arrays.</summary>
        public abstract IParamsArrayArgumentAdapter ParamsArrayArgument { get; }
    }

    /// <summary>Provides adapters that may be applied to parsed attribute type-arguments before invoking a recorder.</summary>
    protected interface ITypeArgumentAdapter
    {
        /// <summary>Converts the provided recorder to a <see cref="DTypeArgumentRecorder"/>.</summary>
        /// <param name="recorder">Responsible for recording the argument.</param>
        /// <remarks>Applying this adapter is not required, as the provided recorder already is of the desired type.</remarks>
        /// <returns>The converted <see cref="DTypeArgumentRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DTypeArgumentRecorder For(Func<TRecord, ITypeSymbol, ExpressionSyntax, bool> recorder);

        /// <summary>Produces a <see cref="DTypeArgumentRecorder"/> which invokes the provided recorder and returns <see langword="true"/>.</summary>
        /// <param name="recorder">Responsible for recording the argument.</param>
        /// <returns>The produced <see cref="DTypeArgumentRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DTypeArgumentRecorder For(Action<TRecord, ITypeSymbol, ExpressionSyntax> recorder);
    }

    /// <summary>Provides adapters that may be applied to parsed attribute arguments before invoking a recorder.</summary>
    protected interface ISimpleArgumentAdapter
    {
        /// <summary>Produces a <see cref="DArgumentRecorder"/> which ensures that the argument is of type <typeparamref name="T"/> before invoking the provided recorder, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/>.</param>
        /// <returns>The produced <see cref="DArgumentRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder For<T>(Func<TRecord, T, ExpressionSyntax, bool> recorder) where T : notnull;

        /// <summary>Produces a <see cref="DArgumentRecorder"/> which ensures that the argument is of type <typeparamref name="T"/> before invoking the provided recorder and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/>.</param>
        /// <returns>The produced <see cref="DArgumentRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder For<T>(Action<TRecord, T, ExpressionSyntax> recorder) where T : notnull;

        /// <summary>Produces a <see cref="DArgumentRecorder"/> which ensures that the argument is of type <typeparamref name="T"/>, or <see langword="null"/>, before invoking the provided recorder, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/> or <see langword="null"/>.</param>
        /// <returns>The produced <see cref="DArgumentRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder ForNullable<T>(Func<TRecord, T?, ExpressionSyntax, bool> recorder) where T : class;

        /// <summary>Produces a <see cref="DArgumentRecorder"/> which ensures that the argument is of type <typeparamref name="T"/>, or <see langword="null"/>, before invoking the provided recorder and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/> or <see langword="null"/>.</param>
        /// <returns>The produced <see cref="DArgumentRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder ForNullable<T>(Action<TRecord, T?, ExpressionSyntax> recorder) where T : class;

        /// <summary>Produces a <see cref="DArgumentRecorder"/> which ensures that the argument is of type <typeparamref name="T"/>, or <see langword="null"/>, before invoking the provided recorder, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/> or <see langword="null"/>.</param>
        /// <returns>The produced <see cref="DArgumentRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder ForNullable<T>(Func<TRecord, T?, ExpressionSyntax, bool> recorder) where T : struct;

        /// <summary>Produces a <see cref="DArgumentRecorder"/> which ensures that the argument is of type <typeparamref name="T"/>, or <see langword="null"/>, before invoking the provided recorder and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/> or <see langword="null"/>.</param>
        /// <returns>The produced <see cref="DArgumentRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder ForNullable<T>(Action<TRecord, T?, ExpressionSyntax> recorder) where T : struct;
    }

    /// <summary>Provides adapters that may be applied to parsed array-valued attribute arguments, which may not be expressed as <see langword="params"/>-arrays, before invoking a recorder.</summary>
    protected interface INonParamsArrayArgumentAdapter
    {
        /// <summary>Produces a <see cref="DArgumentRecorder"/> which ensures that the argument is an array with elements of type <typeparamref name="T"/> before invoking the provided recorder, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>The produced <see cref="DArgumentRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder For<T>(Func<TRecord, IReadOnlyList<T>, ExpressionSyntax, bool> recorder) where T : notnull;

        /// <summary>Produces a <see cref="DArgumentRecorder"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before invoking the provided recorder and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>The produced <see cref="DArgumentRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder For<T>(Action<TRecord, IReadOnlyList<T>, ExpressionSyntax> recorder) where T : notnull;

        /// <summary>Produces a <see cref="DArgumentRecorder"/> which ensures that the argument is either <see langword="null"/> or a collection with elements of type <typeparamref name="T"/> before invoking the provided recorder, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>The produced <see cref="DArgumentRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder ForNullableCollection<T>(Func<TRecord, IReadOnlyList<T>?, ExpressionSyntax, bool> recorder) where T : notnull;

        /// <summary>Produces a <see cref="DArgumentRecorder"/> which ensures that the argument is either <see langword="null"/> or a collection with elements of type <typeparamref name="T"/> before invoking the provided recorder and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>The produced <see cref="DArgumentRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder ForNullableCollection<T>(Action<TRecord, IReadOnlyList<T>?, ExpressionSyntax> recorder) where T : notnull;

        /// <summary>Produces a <see cref="DArgumentRecorder"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where both elements and the collection itself may be <see langword="null"/>, before invoking the provided recorder, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>The produced <see cref="DArgumentRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder ForNullable<T>(Func<TRecord, IReadOnlyList<T?>?, ExpressionSyntax, bool> recorder) where T : class;

        /// <summary>Produces a <see cref="DArgumentRecorder"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where both elements and the collection itself may be <see langword="null"/>, before invoking the provided recorder and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>The produced <see cref="DArgumentRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder ForNullable<T>(Action<TRecord, IReadOnlyList<T?>?, ExpressionSyntax> recorder) where T : class;

        /// <summary>Produces a <see cref="DArgumentRecorder"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where both elements and the collection itself may be <see langword="null"/>, before invoking the provided recorder, and which otherwise returns <see langword="false"/>.</summary>s
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>The produced <see cref="DArgumentRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder ForNullable<T>(Func<TRecord, IReadOnlyList<T?>?, ExpressionSyntax, bool> recorder) where T : struct;

        /// <summary>Produces a <see cref="DArgumentRecorder"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where both elements and the collection itself may be <see langword="null"/>, before invoking the provided recorder and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>The produced <see cref="DArgumentRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder ForNullable<T>(Action<TRecord, IReadOnlyList<T?>?, ExpressionSyntax> recorder) where T : struct;

        /// <summary>Produces a <see cref="DArgumentRecorder"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where the elements may be <see langword="null"/>, before invoking the provided recorder, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>The produced <see cref="DArgumentRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder ForNullableElements<T>(Func<TRecord, IReadOnlyList<T?>, ExpressionSyntax, bool> recorder) where T : class;

        /// <summary>Produces a <see cref="DArgumentRecorder"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where the elements may be <see langword="null"/>, before invoking the provided recorder and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>The produced <see cref="DArgumentRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder ForNullableElements<T>(Action<TRecord, IReadOnlyList<T?>, ExpressionSyntax> recorder) where T : class;

        /// <summary>Produces a <see cref="DArgumentRecorder"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where the elements may be <see langword="null"/>, before invoking the provided recorder, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>The produced <see cref="DArgumentRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder ForNullableElements<T>(Func<TRecord, IReadOnlyList<T?>, ExpressionSyntax, bool> recorder) where T : struct;

        /// <summary>Produces a <see cref="DArgumentRecorder"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where the elements may be <see langword="null"/>, before invoking the provided recorder and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>The produced <see cref="DArgumentRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder ForNullableElements<T>(Action<TRecord, IReadOnlyList<T?>, ExpressionSyntax> recorder) where T : struct;
    }

    /// <summary>Provides adapters that may be applied to parsed array-valued attribute arguments, which may be expressed as <see langword="params"/>-arrays, before invoking a recorder.</summary>
    protected interface IParamsArrayArgumentAdapter
    {
        /// <summary>Produces a <see cref="DArgumentRecorder"/> which ensures that the argument is an array with elements of type <typeparamref name="T"/> before invoking the provided recorder, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>The produced <see cref="DArgumentRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder For<T>(Func<TRecord, IReadOnlyList<T>, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> recorder) where T : notnull;

        /// <summary>Produces a <see cref="DArgumentRecorder"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before invoking the provided recorder and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>The produced <see cref="DArgumentRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder For<T>(Action<TRecord, IReadOnlyList<T>, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> recorder) where T : notnull;

        /// <summary>Produces a <see cref="DArgumentRecorder"/> which ensures that the argument is either <see langword="null"/> or a collection with elements of type <typeparamref name="T"/> before invoking the provided recorder, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>The produced <see cref="DArgumentRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder ForNullableCollection<T>(Func<TRecord, IReadOnlyList<T>?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> recorder) where T : notnull;

        /// <summary>Produces a <see cref="DArgumentRecorder"/> which ensures that the argument is either <see langword="null"/> or a collection with elements of type <typeparamref name="T"/> before invoking the provided recorder and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>The produced <see cref="DArgumentRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder ForNullableCollection<T>(Action<TRecord, IReadOnlyList<T>?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> recorder) where T : notnull;

        /// <summary>Produces a <see cref="DArgumentRecorder"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where both elements and the collection itself may be <see langword="null"/>, before invoking the provided recorder, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>The produced <see cref="DArgumentRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder ForNullable<T>(Func<TRecord, IReadOnlyList<T?>?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> recorder) where T : class;

        /// <summary>Produces a <see cref="DArgumentRecorder"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where both elements and the collection itself may be <see langword="null"/>, before invoking the provided recorder and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>The produced <see cref="DArgumentRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder ForNullable<T>(Action<TRecord, IReadOnlyList<T?>?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> recorder) where T : class;

        /// <summary>Produces a <see cref="DArgumentRecorder"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where both elements and the collection itself may be <see langword="null"/>, before invoking the provided recorder, and which otherwise returns <see langword="false"/>.</summary>s
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>The produced <see cref="DArgumentRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder ForNullable<T>(Func<TRecord, IReadOnlyList<T?>?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> recorder) where T : struct;

        /// <summary>Produces a <see cref="DArgumentRecorder"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where both elements and the collection itself may be <see langword="null"/>, before invoking the provided recorder and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>The produced <see cref="DArgumentRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder ForNullable<T>(Action<TRecord, IReadOnlyList<T?>?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> recorder) where T : struct;

        /// <summary>Produces a <see cref="DArgumentRecorder"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where the elements may be <see langword="null"/>, before invoking the provided recorder, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>The produced <see cref="DArgumentRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder ForNullableElements<T>(Func<TRecord, IReadOnlyList<T?>, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> recorder) where T : class;

        /// <summary>Produces a <see cref="DArgumentRecorder"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where the elements may be <see langword="null"/>, before invoking the provided recorder and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>The produced <see cref="DArgumentRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder ForNullableElements<T>(Action<TRecord, IReadOnlyList<T?>, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> recorder) where T : class;

        /// <summary>Produces a <see cref="DArgumentRecorder"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where the elements may be <see langword="null"/>, before invoking the provided recorder, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>The produced <see cref="DArgumentRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder ForNullableElements<T>(Func<TRecord, IReadOnlyList<T?>, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> recorder) where T : struct;

        /// <summary>Produces a <see cref="DArgumentRecorder"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where the elements may be <see langword="null"/>, before invoking the provided recorder and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <returns>The produced <see cref="DArgumentRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentRecorder ForNullableElements<T>(Action<TRecord, IReadOnlyList<T?>, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> recorder) where T : struct;
    }

    private static class AdapterUtility
    {
        public static DArgumentRecorder ForNonParams(Func<TRecord, object?, ExpressionSyntax, bool> recorder)
        {
            return wrapper;

            bool wrapper(TRecord dataRecord, object? argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
            {
                if (dataRecord is null)
                {
                    throw new ArgumentNullException(nameof(dataRecord));
                }

                if (syntax.IsT1)
                {
                    return false;
                }

                if (syntax.AsT0 is null)
                {
                    throw new ArgumentException($"The {nameof(ExpressionSyntax)} of the provided {nameof(OneOf<object, object>)} was null.", nameof(syntax));
                }

                return recorder(dataRecord, argument, syntax.AsT0);
            }
        }

        public static DArgumentRecorder ForParams(Func<TRecord, object?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> recorder)
        {
            return wrapper;

            bool wrapper(TRecord dataRecord, object? argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
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

                return recorder(dataRecord, argument, syntax);
            }
        }
    }

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
            if (syntax is null)
            {
                throw new ArgumentNullException(nameof(syntax));
            }

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

        bool IAttributeArgumentRecorder.RecordArgument(object? argument, ExpressionSyntax syntax)
        {
            if (syntax is null)
            {
                throw new ArgumentNullException(nameof(syntax));
            }

            return Recorder(DataRecord, argument, syntax);
        }
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

        bool IAttributeArgumentRecorder.RecordArgument(object? argument, ExpressionSyntax syntax)
        {
            if (syntax is null)
            {
                throw new ArgumentNullException(nameof(syntax));
            }

            return Recorder(DataRecord, argument, syntax);
        }

        bool IAttributeConstructorArgumentRecorder.RecordParamsArgument(object? argument, IReadOnlyList<ExpressionSyntax> elementSyntax)
        {
            if (elementSyntax is null)
            {
                throw new ArgumentNullException(nameof(elementSyntax));
            }

            return Recorder(DataRecord, argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(elementSyntax));
        }
    }

    private sealed class ArgumentAdapterProvider : IArgumentAdapterProvider
    {
        ITypeArgumentAdapter IArgumentAdapterProvider.TypeArgument { get; } = new TypeArgumentAdapter();
        ISimpleArgumentAdapter IArgumentAdapterProvider.SimpleArgument { get; } = new SimpleArgumentAdapter();
        INonParamsArrayArgumentAdapter IArgumentAdapterProvider.NonParamsArrayArgument { get; } = new NonParamsArrayArgumentAdapter();
        IParamsArrayArgumentAdapter IArgumentAdapterProvider.ParamsArrayArgument { get; } = new ParamsArrayArgumentAdapter();
    }

    private sealed class TypeArgumentAdapter : ITypeArgumentAdapter
    {
        DTypeArgumentRecorder ITypeArgumentAdapter.For(Func<TRecord, ITypeSymbol, ExpressionSyntax, bool> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return For(recorder);
        }

        DTypeArgumentRecorder ITypeArgumentAdapter.For(Action<TRecord, ITypeSymbol, ExpressionSyntax> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return For(wrapper);

            bool wrapper(TRecord dataRecord, ITypeSymbol argument, ExpressionSyntax syntax)
            {
                recorder(dataRecord, argument, syntax);

                return true;
            }
        }

        private static DTypeArgumentRecorder For(Func<TRecord, ITypeSymbol, ExpressionSyntax, bool> recorder)
        {
            return wrapper;

            bool wrapper(TRecord dataRecord, ITypeSymbol argument, ExpressionSyntax syntax)
            {
                if (dataRecord is null)
                {
                    throw new ArgumentNullException(nameof(dataRecord));
                }

                if (argument is null)
                {
                    throw new ArgumentNullException(nameof(argument));
                }

                if (syntax is null)
                {
                    throw new ArgumentNullException(nameof(syntax));
                }

                return recorder(dataRecord, argument, syntax);
            }
        }
    }

    private sealed class SimpleArgumentAdapter : ISimpleArgumentAdapter
    {
        DArgumentRecorder ISimpleArgumentAdapter.For<T>(Func<TRecord, T, ExpressionSyntax, bool> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return For(recorder);
        }

        DArgumentRecorder ISimpleArgumentAdapter.For<T>(Action<TRecord, T, ExpressionSyntax> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return For<T>(wrapper);

            bool wrapper(TRecord dataRecord, T argument, ExpressionSyntax syntax)
            {
                recorder(dataRecord, argument, syntax);

                return true;
            }
        }

        DArgumentRecorder ISimpleArgumentAdapter.ForNullable<T>(Func<TRecord, T?, ExpressionSyntax, bool> recorder) where T : class
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullable(recorder);
        }

        DArgumentRecorder ISimpleArgumentAdapter.ForNullable<T>(Action<TRecord, T?, ExpressionSyntax> recorder) where T : class
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullable<T>(wrapper);

            bool wrapper(TRecord dataRecord, T? argument, ExpressionSyntax syntax)
            {
                recorder(dataRecord, argument, syntax);

                return true;
            }
        }

        DArgumentRecorder ISimpleArgumentAdapter.ForNullable<T>(Func<TRecord, T?, ExpressionSyntax, bool> recorder) where T : struct
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullable(recorder);
        }

        DArgumentRecorder ISimpleArgumentAdapter.ForNullable<T>(Action<TRecord, T?, ExpressionSyntax> recorder) where T : struct
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullable<T>(wrapper);

            bool wrapper(TRecord dataRecord, T? argument, ExpressionSyntax syntax)
            {
                recorder(dataRecord, argument, syntax);

                return true;
            }
        }

        private static DArgumentRecorder For<T>(Func<TRecord, T, ExpressionSyntax, bool> recorder)
        {
            return AdapterUtility.ForNonParams(wrapper);

            bool wrapper(TRecord dataRecord, object? argument, ExpressionSyntax syntax)
            {
                if (argument is null)
                {
                    return false;
                }

                if (argument is not T tArgument)
                {
                    return false;
                }

                return recorder(dataRecord, tArgument, syntax);
            }
        }

        private static DArgumentRecorder ForNullable<T>(Func<TRecord, T?, ExpressionSyntax, bool> recorder) where T : class
        {
            return AdapterUtility.ForNonParams(wrapper);

            bool wrapper(TRecord dataRecord, object? argument, ExpressionSyntax syntax)
            {
                if (argument is null)
                {
                    return recorder(dataRecord, null, syntax);
                }

                if (argument is not T tArgument)
                {
                    return false;
                }

                return recorder(dataRecord, tArgument, syntax);
            }
        }

        private static DArgumentRecorder ForNullable<T>(Func<TRecord, T?, ExpressionSyntax, bool> recorder) where T : struct
        {
            return AdapterUtility.ForNonParams(wrapper);

            bool wrapper(TRecord dataRecord, object? argument, ExpressionSyntax syntax)
            {
                if (argument is null)
                {
                    return recorder(dataRecord, null, syntax);
                }

                if (argument is not T tArgument)
                {
                    return false;
                }

                return recorder(dataRecord, tArgument, syntax);
            }
        }
    }

    private sealed class NonParamsArrayArgumentAdapter : INonParamsArrayArgumentAdapter
    {
        DArgumentRecorder INonParamsArrayArgumentAdapter.For<T>(Func<TRecord, IReadOnlyList<T>, ExpressionSyntax, bool> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return For(recorder);
        }

        DArgumentRecorder INonParamsArrayArgumentAdapter.For<T>(Action<TRecord, IReadOnlyList<T>, ExpressionSyntax> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return For<T>(wrapper);

            bool wrapper(TRecord dataRecord, IReadOnlyList<T> argument, ExpressionSyntax syntax)
            {
                recorder(dataRecord, argument, syntax);

                return true;
            }
        }

        DArgumentRecorder INonParamsArrayArgumentAdapter.ForNullable<T>(Func<TRecord, IReadOnlyList<T?>?, ExpressionSyntax, bool> recorder) where T : class
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullable(recorder);
        }

        DArgumentRecorder INonParamsArrayArgumentAdapter.ForNullable<T>(Action<TRecord, IReadOnlyList<T?>?, ExpressionSyntax> recorder) where T : class
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullable<T>(wrapper);

            bool wrapper(TRecord dataRecord, IReadOnlyList<T?>? argument, ExpressionSyntax syntax)
            {
                recorder(dataRecord, argument, syntax);

                return true;
            }
        }

        DArgumentRecorder INonParamsArrayArgumentAdapter.ForNullable<T>(Func<TRecord, IReadOnlyList<T?>?, ExpressionSyntax, bool> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullable(recorder);
        }

        DArgumentRecorder INonParamsArrayArgumentAdapter.ForNullable<T>(Action<TRecord, IReadOnlyList<T?>?, ExpressionSyntax> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullable<T>(wrapper);

            bool wrapper(TRecord dataRecord, IReadOnlyList<T?>? argument, ExpressionSyntax syntax)
            {
                recorder(dataRecord, argument, syntax);

                return true;
            }
        }

        DArgumentRecorder INonParamsArrayArgumentAdapter.ForNullableCollection<T>(Func<TRecord, IReadOnlyList<T>?, ExpressionSyntax, bool> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullableCollection(recorder);
        }

        DArgumentRecorder INonParamsArrayArgumentAdapter.ForNullableCollection<T>(Action<TRecord, IReadOnlyList<T>?, ExpressionSyntax> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullableCollection<T>(wrapper);

            bool wrapper(TRecord dataRecord, IReadOnlyList<T>? argument, ExpressionSyntax syntax)
            {
                recorder(dataRecord, argument, syntax);

                return true;
            }
        }

        DArgumentRecorder INonParamsArrayArgumentAdapter.ForNullableElements<T>(Func<TRecord, IReadOnlyList<T?>, ExpressionSyntax, bool> recorder) where T : class
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullableElements(recorder);
        }

        DArgumentRecorder INonParamsArrayArgumentAdapter.ForNullableElements<T>(Action<TRecord, IReadOnlyList<T?>, ExpressionSyntax> recorder) where T : class
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullableElements<T>(wrapper);

            bool wrapper(TRecord dataRecord, IReadOnlyList<T?> argument, ExpressionSyntax syntax)
            {
                recorder(dataRecord, argument, syntax);

                return true;
            }
        }

        DArgumentRecorder INonParamsArrayArgumentAdapter.ForNullableElements<T>(Func<TRecord, IReadOnlyList<T?>, ExpressionSyntax, bool> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullableElements(recorder);
        }

        DArgumentRecorder INonParamsArrayArgumentAdapter.ForNullableElements<T>(Action<TRecord, IReadOnlyList<T?>, ExpressionSyntax> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullableElements<T>(wrapper);

            bool wrapper(TRecord dataRecord, IReadOnlyList<T?> argument, ExpressionSyntax syntax)
            {
                recorder(dataRecord, argument, syntax);

                return true;
            }
        }

        private static DArgumentRecorder For<T>(Func<TRecord, IReadOnlyList<T>, ExpressionSyntax, bool> recorder)
        {
            return AdapterUtility.ForNonParams(wrapper);

            bool wrapper(TRecord dataRecord, object? argument, ExpressionSyntax syntax) => CommonArrayConverters.NonNullable<T>(argument).Match
            (
                static (error) => false,
                (converted) => recorder(dataRecord, converted, syntax)
            );
        }

        private static DArgumentRecorder ForNullable<T>(Func<TRecord, IReadOnlyList<T?>?, ExpressionSyntax, bool> recorder) where T : class
        {
            return AdapterUtility.ForNonParams(wrapper);

            bool wrapper(TRecord dataRecord, object? argument, ExpressionSyntax syntax) => CommonArrayConverters.Nullable<T>(argument).Match
            (
                static (error) => false,
                (converted) => recorder(dataRecord, converted, syntax)
            );
        }

        private static DArgumentRecorder ForNullable<T>(Func<TRecord, IReadOnlyList<T?>?, ExpressionSyntax, bool> recorder) where T : struct
        {
            return AdapterUtility.ForNonParams(wrapper);

            bool wrapper(TRecord dataRecord, object? argument, ExpressionSyntax syntax) => CommonArrayConverters.Nullable<T?>(argument).Match
            (
                static (error) => false,
                (converted) => recorder(dataRecord, converted, syntax)
            );
        }

        private static DArgumentRecorder ForNullableCollection<T>(Func<TRecord, IReadOnlyList<T>?, ExpressionSyntax, bool> recorder)
        {
            return AdapterUtility.ForNonParams(wrapper);

            bool wrapper(TRecord dataRecord, object? argument, ExpressionSyntax syntax) => CommonArrayConverters.NullableCollection<T>(argument).Match
            (
                static (error) => false,
                (converted) => recorder(dataRecord, converted, syntax)
            );
        }

        private static DArgumentRecorder ForNullableElements<T>(Func<TRecord, IReadOnlyList<T?>, ExpressionSyntax, bool> recorder) where T : class
        {
            return AdapterUtility.ForNonParams(wrapper);

            bool wrapper(TRecord dataRecord, object? argument, ExpressionSyntax syntax) => CommonArrayConverters.NullableElements<T>(argument).Match
            (
                static (error) => false,
                (converted) => recorder(dataRecord, converted, syntax)
            );
        }

        private static DArgumentRecorder ForNullableElements<T>(Func<TRecord, IReadOnlyList<T?>, ExpressionSyntax, bool> recorder) where T : struct
        {
            return AdapterUtility.ForNonParams(wrapper);

            bool wrapper(TRecord dataRecord, object? argument, ExpressionSyntax syntax) => CommonArrayConverters.NullableElements<T?>(argument).Match
            (
                static (error) => false,
                (converted) => recorder(dataRecord, converted, syntax)
            );
        }
    }

    private sealed class ParamsArrayArgumentAdapter : IParamsArrayArgumentAdapter
    {
        DArgumentRecorder IParamsArrayArgumentAdapter.For<T>(Func<TRecord, IReadOnlyList<T>, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return For(recorder);
        }

        DArgumentRecorder IParamsArrayArgumentAdapter.For<T>(Action<TRecord, IReadOnlyList<T>, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return For<T>(wrapper);

            bool wrapper(TRecord dataRecord, IReadOnlyList<T> argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
            {
                recorder(dataRecord, argument, syntax);

                return true;
            }
        }

        DArgumentRecorder IParamsArrayArgumentAdapter.ForNullable<T>(Func<TRecord, IReadOnlyList<T?>?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> recorder) where T : class
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullable(recorder);
        }

        DArgumentRecorder IParamsArrayArgumentAdapter.ForNullable<T>(Action<TRecord, IReadOnlyList<T?>?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> recorder) where T : class
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullable<T>(wrapper);

            bool wrapper(TRecord dataRecord, IReadOnlyList<T?>? argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
            {
                recorder(dataRecord, argument, syntax);

                return true;
            }
        }

        DArgumentRecorder IParamsArrayArgumentAdapter.ForNullable<T>(Func<TRecord, IReadOnlyList<T?>?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullable(recorder);
        }

        DArgumentRecorder IParamsArrayArgumentAdapter.ForNullable<T>(Action<TRecord, IReadOnlyList<T?>?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullable<T>(wrapper);

            bool wrapper(TRecord dataRecord, IReadOnlyList<T?>? argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
            {
                recorder(dataRecord, argument, syntax);

                return true;
            }
        }

        DArgumentRecorder IParamsArrayArgumentAdapter.ForNullableCollection<T>(Func<TRecord, IReadOnlyList<T>?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullableCollection(recorder);
        }

        DArgumentRecorder IParamsArrayArgumentAdapter.ForNullableCollection<T>(Action<TRecord, IReadOnlyList<T>?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullableCollection<T>(wrapper);

            bool wrapper(TRecord dataRecord, IReadOnlyList<T>? argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
            {
                recorder(dataRecord, argument, syntax);

                return true;
            }
        }

        DArgumentRecorder IParamsArrayArgumentAdapter.ForNullableElements<T>(Func<TRecord, IReadOnlyList<T?>, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> recorder) where T : class
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullableElements(recorder);
        }

        DArgumentRecorder IParamsArrayArgumentAdapter.ForNullableElements<T>(Action<TRecord, IReadOnlyList<T?>, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> recorder) where T : class
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullableElements<T>(wrapper);

            bool wrapper(TRecord dataRecord, IReadOnlyList<T?> argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
            {
                recorder(dataRecord, argument, syntax);

                return true;
            }
        }

        DArgumentRecorder IParamsArrayArgumentAdapter.ForNullableElements<T>(Func<TRecord, IReadOnlyList<T?>, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullableElements(recorder);
        }

        DArgumentRecorder IParamsArrayArgumentAdapter.ForNullableElements<T>(Action<TRecord, IReadOnlyList<T?>, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return ForNullableElements<T>(wrapper);

            bool wrapper(TRecord dataRecord, IReadOnlyList<T?> argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
            {
                recorder(dataRecord, argument, syntax);

                return true;
            }
        }

        private static DArgumentRecorder For<T>(Func<TRecord, IReadOnlyList<T>, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> recorder)
        {
            return AdapterUtility.ForParams(wrapper);

            bool wrapper(TRecord dataRecord, object? argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) => CommonArrayConverters.NonNullable<T>(argument).Match
            (
                static (error) => false,
                (converted) => recorder(dataRecord, converted, syntax)
            );
        }

        private static DArgumentRecorder ForNullable<T>(Func<TRecord, IReadOnlyList<T?>?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> recorder) where T : class
        {
            return AdapterUtility.ForParams(wrapper);

            bool wrapper(TRecord dataRecord, object? argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) => CommonArrayConverters.Nullable<T>(argument).Match
            (
                static (error) => false,
                (converted) => recorder(dataRecord, converted, syntax)
            );
        }

        private static DArgumentRecorder ForNullable<T>(Func<TRecord, IReadOnlyList<T?>?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> recorder) where T : struct
        {
            return AdapterUtility.ForParams(wrapper);

            bool wrapper(TRecord dataRecord, object? argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) => CommonArrayConverters.Nullable<T?>(argument).Match
            (
                static (error) => false,
                (converted) => recorder(dataRecord, converted, syntax)
            );
        }

        private static DArgumentRecorder ForNullableCollection<T>(Func<TRecord, IReadOnlyList<T>?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> recorder)
        {
            return AdapterUtility.ForParams(wrapper);

            bool wrapper(TRecord dataRecord, object? argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) => CommonArrayConverters.NullableCollection<T>(argument).Match
            (
                static (error) => false,
                (converted) => recorder(dataRecord, converted, syntax)
            );
        }

        private static DArgumentRecorder ForNullableElements<T>(Func<TRecord, IReadOnlyList<T?>, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> recorder) where T : class
        {
            return AdapterUtility.ForParams(wrapper);

            bool wrapper(TRecord dataRecord, object? argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) => CommonArrayConverters.NullableElements<T>(argument).Match
            (
                static (error) => false,
                (converted) => recorder(dataRecord, converted, syntax)
            );
        }

        private static DArgumentRecorder ForNullableElements<T>(Func<TRecord, IReadOnlyList<T?>, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> recorder) where T : struct
        {
            return AdapterUtility.ForParams(wrapper);

            bool wrapper(TRecord dataRecord, object? argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) => CommonArrayConverters.NullableElements<T?>(argument).Match
            (
                static (error) => false,
                (converted) => recorder(dataRecord, converted, syntax)
            );
        }
    }
}
