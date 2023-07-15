namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

/// <summary>An abstract <see cref="IAttributeMapper{TRecorder}"/> and <see cref="ISemanticAttributeMapper{TRecord}"/>, allowing attributes to be parsed with or without syntactic context. Mappings are provided through the following methods:
/// <list type="bullet">
/// <item><see cref="AddTypeParameterMappings"/></item>
/// <item><see cref="AddParameterMappings"/></item>
/// </list></summary>
/// <typeparam name="TRecord">The type to which the arguments of the mapped parameters are recorded.</typeparam>
/// <typeparam name="TSemanticRecord">The type to which the arguments of the mapped parameter are recorded, when attributes are parsed without syntactic context.</typeparam>
public abstract class AAdaptiveAttributeMapper<TRecord, TSemanticRecord> : IAttributeMapper<TRecord>, ISemanticAttributeMapper<TSemanticRecord>
{
    /// <summary>Provides adapters that may be applied to parsed arguments before invoking a recorder.</summary>
    [SuppressMessage("Major Code Smell", "S2743: Static fields should not be used in generic types", Justification = "Type uses TRecord.")]
    protected static IArgumentAdapterProvider Adapters { get; } = new ArgumentAdapterProvider();

    private bool IsInitialized { get; set; }

    private IReadOnlyDictionary<int, DSharedTypeArgumentRecorder> SharedIndexedTypeParameterMappings { get; set; } = null!;
    private IReadOnlyDictionary<string, DSharedTypeArgumentRecorder> SharedNamedTypeParameterMappings { get; set; } = null!;
    private IReadOnlyDictionary<string, DSharedArgumentRecorder> SharedParameterMappings { get; set; } = null!;

    private IReadOnlyDictionary<int, DSemanticTypeArgumentRecorder> SemanticIndexedTypeParameterMappings { get; set; } = null!;
    private IReadOnlyDictionary<string, DSemanticTypeArgumentRecorder> SemanticNamedTypeParameterMappings { get; set; } = null!;
    private IReadOnlyDictionary<string, DSemanticArgumentRecorder> SemanticParameterMappings { get; set; } = null!;

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

        Dictionary<int, DSharedTypeArgumentRecorder> sharedIndexedTypeParameterMappingsDictionary = new();
        Dictionary<string, DSharedTypeArgumentRecorder> sharedNamedTypeParameterMappingsDictionary = new(comparer);
        Dictionary<string, DSharedArgumentRecorder> sharedParameterMappingsDictionary = new(comparer);

        Dictionary<int, DSemanticTypeArgumentRecorder> semanticIndexedParameterMappingsDictionary = new();
        Dictionary<string, DSemanticTypeArgumentRecorder> semanticNamedParameterMappingsDictionary = new(comparer);
        Dictionary<string, DSemanticArgumentRecorder> semanticParameterMappingsDictionary = new(comparer);

        PopulateTypeMappingsDictionaries(sharedIndexedTypeParameterMappingsDictionary, sharedNamedTypeParameterMappingsDictionary, semanticIndexedParameterMappingsDictionary,
            semanticNamedParameterMappingsDictionary, typeParameterMappings);

        PopulateMappingsDictionary(sharedParameterMappingsDictionary, semanticParameterMappingsDictionary, parameterMappings);

        SharedIndexedTypeParameterMappings = sharedIndexedTypeParameterMappingsDictionary;
        SharedNamedTypeParameterMappings = sharedNamedTypeParameterMappingsDictionary;
        SharedParameterMappings = sharedParameterMappingsDictionary;

        SemanticIndexedTypeParameterMappings = semanticIndexedParameterMappingsDictionary;
        SemanticNamedTypeParameterMappings = semanticNamedParameterMappingsDictionary;
        SemanticParameterMappings = semanticParameterMappingsDictionary;

        IsInitialized = true;
    }

    private static void PopulateTypeMappingsDictionaries(IDictionary<int, DSharedTypeArgumentRecorder> sharedIndexedDictionary, IDictionary<string, DSharedTypeArgumentRecorder> sharedNamedDictionary,
        IDictionary<int, DSemanticTypeArgumentRecorder> semanticIndexedDictionary, IDictionary<string, DSemanticTypeArgumentRecorder> semanticNamedDictionary, IEnumerable<(OneOf<int, string>, ITypeArgumentRecorderProvider)> mappings)
    {
        foreach (var (parameter, recorders) in mappings)
        {
            if (recorders is null)
            {
                throw new InvalidOperationException($"A {nameof(ITypeArgumentRecorderProvider)} in the provided collection of mappings was null.");
            }

            if (recorders.Shared is null)
            {
                throw new InvalidOperationException($"A shared recorder in the provided collection of mappings was null.");
            }

            if (recorders.Semantic is null)
            {
                throw new InvalidOperationException($"A semantic recorder in the provided collection of mappings was null.");
            }

            var dictionaryDelegate = parameter.Match<Action<ITypeArgumentRecorderProvider>>
            (
                (index) =>
                {
                    if (index < 0)
                    {
                        throw new InvalidOperationException("The index of a parameter in the provided collection of mappings was negative.");
                    }

                    return (recorders) =>
                    {
                        sharedIndexedDictionary.Add(index, recorders.Shared);
                        semanticIndexedDictionary.Add(index, recorders.Semantic);
                    };
                },
                (name) =>
                {
                    if (name is null)
                    {
                        throw new InvalidOperationException("The name of a parameter in the provided collection of mappings was null.");
                    }

                    return (recorders) =>
                    {
                        sharedNamedDictionary.Add(name, recorders.Shared);
                        semanticNamedDictionary.Add(name, recorders.Semantic);
                    };
                }
            );

            try
            {
                dictionaryDelegate(recorders);
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

    private static void PopulateMappingsDictionary(IDictionary<string, DSharedArgumentRecorder> sharedDictionary, IDictionary<string, DSemanticArgumentRecorder> semanticDictionary, IEnumerable<(string, IArgumentRecorderProvider)> mappings)
    {
        foreach (var (parameterName, recorders) in mappings)
        {
            if (parameterName is null)
            {
                throw new InvalidOperationException("The name of a parameter in the provided collection of parameter mappings was null.");
            }

            if (recorders is null)
            {
                throw new InvalidOperationException($"A {nameof(IArgumentRecorderProvider)} in the provided collection of mappings was null.");
            }

            if (recorders.Shared is null)
            {
                throw new InvalidOperationException($"A shared recorder in the provided collection of mappings was null.");
            }

            if (recorders.Semantic is null)
            {
                throw new InvalidOperationException($"A semantic recorder in the provided collection of mappings was null.");
            }

            try
            {
                sharedDictionary.Add(parameterName, recorders.Shared);
                semanticDictionary.Add(parameterName, recorders.Semantic);
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
    /// <exception cref="InvalidOperationException"/>
    protected virtual IEnumerable<(OneOf<int, string> IndexOrName, ITypeArgumentRecorderProvider Recorders)> AddTypeParameterMappings() => Enumerable.Empty<(OneOf<int, string>, ITypeArgumentRecorderProvider)>();

    /// <summary>Maps the names of constructor or named parameters to recorders, responsible for recording the argument of the parameter.</summary>
    /// <returns>The mappings from parameter name to recorder.</returns>
    /// <exception cref="InvalidOperationException"/>
    protected virtual IEnumerable<(string Name, IArgumentRecorderProvider Recorders)> AddParameterMappings() => Enumerable.Empty<(string, IArgumentRecorderProvider)>();

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

        if (TryGetSharedTypeParameterMapping(parameter) is not DSharedTypeArgumentRecorder mapping)
        {
            return null;
        }

        return new AttributeTypeArgumentRecorder(dataRecord, mapping);
    }

    /// <inheritdoc/>
    public ISemanticAttributeArgumentRecorder? TryMapTypeParameter(ITypeParameterSymbol parameter, TSemanticRecord dataRecord)
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

        if (TryGetSemanticTypeParameterMapping(parameter) is not DSemanticTypeArgumentRecorder mapping)
        {
            return null;
        }

        return new SemanticAttributeArgumentRecorder(dataRecord, typeCheckedMapping);

        bool typeCheckedMapping(TSemanticRecord dataRecord, object? argument)
        {
            if (argument is not ITypeSymbol typeArgument)
            {
                return false;
            }

            return mapping(dataRecord, typeArgument);
        }
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

        if (SharedParameterMappings.TryGetValue(parameter.Name, out var mapping) is false)
        {
            return null;
        }

        return new AttributeConstructorArgumentRecorder(dataRecord, mapping);
    }

    /// <inheritdoc/>
    public ISemanticAttributeArgumentRecorder? TryMapConstructorParameter(IParameterSymbol parameter, TSemanticRecord dataRecord)
    {
        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        if (dataRecord is null)
        {
            throw new ArgumentNullException(nameof(dataRecord));
        }

        return TryMapNamedParameter(parameter.Name, dataRecord);
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

        if (SharedParameterMappings.TryGetValue(parameterName, out var mapping) is false)
        {
            return null;
        }

        return new AttributeArgumentRecorder(dataRecord, mapping);
    }

    /// <inheritdoc/>
    public ISemanticAttributeArgumentRecorder? TryMapNamedParameter(string parameterName, TSemanticRecord dataRecord)
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

        if (SemanticParameterMappings.TryGetValue(parameterName, out var mapping) is false)
        {
            return null;
        }

        return new SemanticAttributeArgumentRecorder(dataRecord, mapping);
    }

    private DSemanticTypeArgumentRecorder? TryGetSemanticTypeParameterMapping(ITypeParameterSymbol parameter)
    {
        var hasIndexedMapping = SemanticIndexedTypeParameterMappings.TryGetValue(parameter.Ordinal, out var indexedMapping);
        var hasNamedMapping = SemanticNamedTypeParameterMappings.TryGetValue(parameter.Name, out var namedMapping);

        return (hasIndexedMapping, hasNamedMapping) switch
        {
            (false, false) => null,
            (true, false) => indexedMapping,
            (false, true) => namedMapping,
            (true, true) => attemptToResolveConflictingMappings(indexedMapping, namedMapping)
        };

        static DSemanticTypeArgumentRecorder? attemptToResolveConflictingMappings(DSemanticTypeArgumentRecorder indexedMapping, DSemanticTypeArgumentRecorder namedMapping)
        {
            if (Equals(indexedMapping, namedMapping))
            {
                return indexedMapping;
            }

            return null;
        }
    }

    private DSharedTypeArgumentRecorder? TryGetSharedTypeParameterMapping(ITypeParameterSymbol parameter)
    {
        var hasIndexedMapping = SharedIndexedTypeParameterMappings.TryGetValue(parameter.Ordinal, out var indexedMapping);
        var hasNamedMapping = SharedNamedTypeParameterMappings.TryGetValue(parameter.Name, out var namedMapping);

        return (hasIndexedMapping, hasNamedMapping) switch
        {
            (false, false) => null,
            (true, false) => indexedMapping,
            (false, true) => namedMapping,
            (true, true) => attemptToResolveConflictingMappings(indexedMapping, namedMapping)
        };

        static DSharedTypeArgumentRecorder? attemptToResolveConflictingMappings(DSharedTypeArgumentRecorder indexedMapping, DSharedTypeArgumentRecorder namedMapping)
        {
            if (Equals(indexedMapping, namedMapping))
            {
                return indexedMapping;
            }

            return null;
        }
    }

    /// <summary>Responsible for recording the argument of a type-parameter which was parsed with syntactic context.</summary>
    /// <param name="dataRecord">The <typeparamref name="TRecord"/> to which the argument is recorded.</param>
    /// <param name="argument">The argument of the type-parameter.</param>
    /// <param name="syntax">The syntactial information about the argument of the parameter.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    /// <exception cref="ArgumentNullException"/>
    protected delegate bool DSharedTypeArgumentRecorder(TRecord dataRecord, ITypeSymbol argument, ExpressionSyntax syntax);

    /// <summary>Responsible for recording the argument of a parameter which was parsed with syntactic context.</summary>
    /// <param name="dataRecord">The <typeparamref name="TRecord"/> to which the argument is recorded.</param>
    /// <param name="argument">The argument of the parameter.</param>
    /// <param name="syntax">The <see cref="ExpressionSyntax"/> syntactically describing the argument of the parameter, or each element if expressed as a <see langword="params"/>-array.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    protected delegate bool DSharedArgumentRecorder(TRecord dataRecord, object? argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax);

    /// <summary>Responsible for recording the argument of a type-parameter which was parsed without syntactic context.</summary>
    /// <param name="dataRecord">The <typeparamref name="TSemanticRecord"/> to which the argument is recorded.</param>
    /// <param name="argument">The argument of the type-parameter.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    /// <exception cref="ArgumentNullException"/>
    protected delegate bool DSemanticTypeArgumentRecorder(TSemanticRecord dataRecord, ITypeSymbol argument);

    /// <summary>Responsible for recording the argument of a parameter which was parsed without syntactic context.</summary>
    /// <param name="dataRecord">The <typeparamref name="TSemanticRecord"/> to which the argument is recorded.</param>
    /// <param name="argument">The argument of the parameter.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    /// <exception cref="ArgumentNullException"/>
    protected delegate bool DSemanticArgumentRecorder(TSemanticRecord dataRecord, object? argument);

    /// <summary>Provides the recorders responsible for recording the argument of a type-parameter.</summary>
    protected interface ITypeArgumentRecorderProvider
    {
        /// <summary>The <see cref="DSharedTypeArgumentRecorder"/>, used when parsing with syntactic context.</summary>
        public abstract DSharedTypeArgumentRecorder Shared { get; }

        /// <summary>The <see cref="DSemanticTypeArgumentRecorder"/>, used when parsing without syntactic context.</summary>
        public abstract DSemanticTypeArgumentRecorder Semantic { get; }
    }

    /// <summary>Provides the recorders responsible for recording the argument of a parameter.</summary>
    protected interface IArgumentRecorderProvider
    {
        /// <summary>The <see cref="DSharedArgumentRecorder"/>, used when parsing with syntactic context.</summary>
        public abstract DSharedArgumentRecorder Shared { get; }

        /// <summary>The <see cref="DSemanticArgumentRecorder"/>, used when parsing without syntactic context.</summary>
        public abstract DSemanticArgumentRecorder Semantic { get; }
    }

    /// <summary>Provides adapters that may be applied to parsed attribute arguments before invoking a recorder.</summary>
    protected interface IArgumentAdapterProvider
    {
        /// <summary>Provides adapters related to type-arguments.</summary>
        public abstract ITypeArgumentAdapter TypeArgument { get; }

        /// <summary>Provides adapters related to simple, non-array valued arguments.</summary>
        public abstract ISimpleArgumentAdapter SimpleArgument { get; }

        /// <summary>Provides adapters related to array-valued arguments.</summary>
        public abstract IArrayArgumentAdapterProvider ArrayArgument { get; }
    }

    /// <summary>Provides adapters that may be applied to parsed attribute arguments before invoking a recorder.</summary>
    protected interface IArrayArgumentAdapterProvider
    {
        /// <summary>Provides adapters related to array-valued arguments, which may not be expressed as <see langword="params"/>-arrays.</summary>
        public abstract INonParamsArrayArgumentAdapter NonParams { get; }

        /// <summary>Provides adapters related to array-valued arguments, which may be expressed as <see langword="params"/>-arrays.</summary>
        public abstract IParamsArrayArgumentAdapter Params { get; }
    }

    /// <summary>Provides adapters that may be applied to parsed attribute type-arguments before invoking a recorder.</summary>
    protected interface ITypeArgumentAdapter
    {
        /// <summary>Produces a <see cref="ITypeArgumentRecorderProvider"/> which invokes the provided recorders.</summary>
        /// <param name="sharedRecorder">Responsible for recording the argument when parsing with syntactic context.</param>
        /// <param name="semanticRecorder">Responsible for recording the argument when parsing without syntactic context.</param>
        /// <returns>The produced <see cref="ITypeArgumentRecorderProvider"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract ITypeArgumentRecorderProvider For(Func<TRecord, ITypeSymbol, ExpressionSyntax, bool> sharedRecorder, Func<TSemanticRecord, ITypeSymbol, bool> semanticRecorder);

        /// <summary>Produces a <see cref="ITypeArgumentRecorderProvider"/> which invokes the provided recorders and returns <see langword="true"/>.</summary>
        /// <param name="sharedRecorder">Responsible for recording the argument when parsing with syntactic context.</param>
        /// <param name="semanticRecorder">Responsible for recording the argument when parsing without syntactic context.</param>
        /// <returns>The produced <see cref="ITypeArgumentRecorderProvider"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract ITypeArgumentRecorderProvider For(Action<TRecord, ITypeSymbol, ExpressionSyntax> sharedRecorder, Action<TSemanticRecord, ITypeSymbol> semanticRecorder);
    }

    /// <summary>Provides adapters that may be applied to parsed attribute arguments before invoking a recorder.</summary>
    protected interface ISimpleArgumentAdapter
    {
        /// <summary>Produces a <see cref="IArgumentRecorderProvider"/> which ensures that the argument is of type <typeparamref name="T"/> before invoking the provided recorders, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the argument.</typeparam>
        /// <param name="sharedRecorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/>, when parsing with syntactic context.</param>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/>, when parsing without syntactic context.</param>
        /// <returns>The produced <see cref="IArgumentRecorderProvider"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorderProvider For<T>(Func<TRecord, T, ExpressionSyntax, bool> sharedRecorder, Func<TSemanticRecord, T, bool> semanticRecorder) where T : notnull;

        /// <summary>Produces a <see cref="IArgumentRecorderProvider"/> which ensures that the argument is of type <typeparamref name="T"/> before invoking the provided recorders and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the argument.</typeparam>
        /// <param name="sharedRecorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/>, when parsing with syntactic context.</param>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/>, when parsing without syntactic context.</param>
        /// <returns>The produced <see cref="IArgumentRecorderProvider"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorderProvider For<T>(Action<TRecord, T, ExpressionSyntax> sharedRecorder, Action<TSemanticRecord, T> semanticRecorder) where T : notnull;

        /// <summary>Produces a <see cref="IArgumentRecorderProvider"/> which ensures that the argument is of type <typeparamref name="T"/>, or <see langword="null"/>, before invoking the provided recorders, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the argument.</typeparam>
        /// <param name="sharedRecorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/> or <see langword="null"/>, when parsing with syntactic context.</param>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/> or <see langword="null"/>, when parsing without syntactic context.</param>
        /// <returns>The produced <see cref="IArgumentRecorderProvider"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorderProvider ForNullable<T>(Func<TRecord, T?, ExpressionSyntax, bool> sharedRecorder, Func<TSemanticRecord, T?, bool> semanticRecorder) where T : class;

        /// <summary>Produces a <see cref="IArgumentRecorderProvider"/> which ensures that the argument is of type <typeparamref name="T"/>, or <see langword="null"/>, before invoking the provided recorders and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the argument.</typeparam>
        /// <param name="sharedRecorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/> or <see langword="null"/>, when parsing with syntactic context.</param>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/> or <see langword="null"/>, when parsing without syntactic context.</param>
        /// <returns>The produced <see cref="IArgumentRecorderProvider"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorderProvider ForNullable<T>(Action<TRecord, T?, ExpressionSyntax> sharedRecorder, Action<TSemanticRecord, T?> semanticRecorder) where T : class;

        /// <summary>Produces a <see cref="IArgumentRecorderProvider"/> which ensures that the argument is of type <typeparamref name="T"/>, or <see langword="null"/>, before invoking the provided recorders, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the argument.</typeparam>
        /// <param name="sharedRecorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/> or <see langword="null"/>, when parsing with syntactic context.</param>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/> or <see langword="null"/>, when parsing without syntactic context.</param>
        /// <returns>The produced <see cref="IArgumentRecorderProvider"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorderProvider ForNullable<T>(Func<TRecord, T?, ExpressionSyntax, bool> sharedRecorder, Func<TSemanticRecord, T?, bool> semanticRecorder) where T : struct;

        /// <summary>Produces a <see cref="IArgumentRecorderProvider"/> which ensures that the argument is of type <typeparamref name="T"/>, or <see langword="null"/>, before invoking the provided recorders and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the argument.</typeparam>
        /// <param name="sharedRecorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/> or <see langword="null"/>, when parsing with syntactic context.</param>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/> or <see langword="null"/>, when parsing purely.</param>
        /// <returns>The produced <see cref="IArgumentRecorderProvider"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorderProvider ForNullable<T>(Action<TRecord, T?, ExpressionSyntax> sharedRecorder, Action<TSemanticRecord, T?> semanticRecorder) where T : struct;
    }

    /// <summary>Provides adapters that may be applied to parsed array-valued attribute arguments, which may not be expressed as <see langword="params"/>-arrays, before invoking a recorder.</summary>
    protected interface INonParamsArrayArgumentAdapter
    {
        /// <summary>Produces a <see cref="IArgumentRecorderProvider"/> which ensures that the argument is an array with elements of type <typeparamref name="T"/> before invoking the provided recorders, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="sharedRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing with syntactic context.</param>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing without syntactic context.</param>
        /// <returns>The produced <see cref="IArgumentRecorderProvider"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorderProvider For<T>(Func<TRecord, IReadOnlyList<T>, ExpressionSyntax, bool> sharedRecorder, Func<TSemanticRecord, IReadOnlyList<T>, bool> semanticRecorder) where T : notnull;

        /// <summary>Produces a <see cref="IArgumentRecorderProvider"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before invoking the provided recorders and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="sharedRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing with syntactic context.</param>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing without syntactic context.</param>
        /// <returns>The produced <see cref="IArgumentRecorderProvider"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorderProvider For<T>(Action<TRecord, IReadOnlyList<T>, ExpressionSyntax> sharedRecorder, Action<TSemanticRecord, IReadOnlyList<T>> semanticRecorder) where T : notnull;

        /// <summary>Produces a <see cref="IArgumentRecorderProvider"/> which ensures that the argument is either <see langword="null"/> or a collection with elements of type <typeparamref name="T"/> before invoking the provided recorders, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="sharedRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing with syntactic context.</param>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing without syntactic context.</param>
        /// <returns>The produced <see cref="IArgumentRecorderProvider"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorderProvider ForNullableCollection<T>(Func<TRecord, IReadOnlyList<T>?, ExpressionSyntax, bool> sharedRecorder, Func<TSemanticRecord, IReadOnlyList<T>?, bool> semanticRecorder) where T : notnull;

        /// <summary>Produces a <see cref="IArgumentRecorderProvider"/> which ensures that the argument is either <see langword="null"/> or a collection with elements of type <typeparamref name="T"/> before invoking the provided recorders and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="sharedRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing with syntactic context.</param>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing without syntactic context.</param>
        /// <returns>The produced <see cref="IArgumentRecorderProvider"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorderProvider ForNullableCollection<T>(Action<TRecord, IReadOnlyList<T>?, ExpressionSyntax> sharedRecorder, Action<TSemanticRecord, IReadOnlyList<T>?> semanticRecorder) where T : notnull;

        /// <summary>Produces a <see cref="IArgumentRecorderProvider"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where both elements and the collection itself may be <see langword="null"/>, before invoking the provided recorders, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="sharedRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing with syntactic context.</param>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing without syntactic context.</param>
        /// <returns>The produced <see cref="IArgumentRecorderProvider"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorderProvider ForNullable<T>(Func<TRecord, IReadOnlyList<T?>?, ExpressionSyntax, bool> sharedRecorder, Func<TSemanticRecord, IReadOnlyList<T?>?, bool> semanticRecorder) where T : class;

        /// <summary>Produces a <see cref="IArgumentRecorderProvider"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where both elements and the collection itself may be <see langword="null"/>, before invoking the provided recorders and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="sharedRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing with syntactic context.</param>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing without syntactic context.</param>
        /// <returns>The produced <see cref="IArgumentRecorderProvider"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorderProvider ForNullable<T>(Action<TRecord, IReadOnlyList<T?>?, ExpressionSyntax> sharedRecorder, Action<TSemanticRecord, IReadOnlyList<T?>?> semanticRecorder) where T : class;

        /// <summary>Produces a <see cref="IArgumentRecorderProvider"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where both elements and the collection itself may be <see langword="null"/>, before invoking the provided recorders, and which otherwise returns <see langword="false"/>.</summary>s
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="sharedRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing with syntactic context.</param>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing without syntactic context.</param>
        /// <returns>The produced <see cref="IArgumentRecorderProvider"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorderProvider ForNullable<T>(Func<TRecord, IReadOnlyList<T?>?, ExpressionSyntax, bool> sharedRecorder, Func<TSemanticRecord, IReadOnlyList<T?>?, bool> semanticRecorder) where T : struct;

        /// <summary>Produces a <see cref="IArgumentRecorderProvider"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where both elements and the collection itself may be <see langword="null"/>, before invoking the provided recorders and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="sharedRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing with syntactic context.</param>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing without syntactic context.</param>
        /// <returns>The produced <see cref="IArgumentRecorderProvider"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorderProvider ForNullable<T>(Action<TRecord, IReadOnlyList<T?>?, ExpressionSyntax> sharedRecorder, Action<TSemanticRecord, IReadOnlyList<T?>?> semanticRecorder) where T : struct;

        /// <summary>Produces a <see cref="IArgumentRecorderProvider"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where the elements may be <see langword="null"/>, before invoking the provided recorders, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="sharedRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing with syntactic context.</param>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing without syntactic context.</param>
        /// <returns>The produced <see cref="IArgumentRecorderProvider"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorderProvider ForNullableElements<T>(Func<TRecord, IReadOnlyList<T?>, ExpressionSyntax, bool> sharedRecorder, Func<TSemanticRecord, IReadOnlyList<T?>, bool> semanticRecorder) where T : class;

        /// <summary>Produces a <see cref="IArgumentRecorderProvider"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where the elements may be <see langword="null"/>, before invoking the provided recorders and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="sharedRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing with syntactic context.</param>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing without syntactic context.</param>
        /// <returns>The produced <see cref="IArgumentRecorderProvider"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorderProvider ForNullableElements<T>(Action<TRecord, IReadOnlyList<T?>, ExpressionSyntax> sharedRecorder, Action<TSemanticRecord, IReadOnlyList<T?>> semanticRecorder) where T : class;

        /// <summary>Produces a <see cref="IArgumentRecorderProvider"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where the elements may be <see langword="null"/>, before invoking the provided recorders, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="sharedRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing with syntactic context.</param>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing without syntactic context.</param>
        /// <returns>The produced <see cref="IArgumentRecorderProvider"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorderProvider ForNullableElements<T>(Func<TRecord, IReadOnlyList<T?>, ExpressionSyntax, bool> sharedRecorder, Func<TSemanticRecord, IReadOnlyList<T?>, bool> semanticRecorder) where T : struct;

        /// <summary>Produces a <see cref="IArgumentRecorderProvider"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where the elements may be <see langword="null"/>, before invoking the provided recorders and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="sharedRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing with syntactic context.</param>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing without syntactic context.</param>
        /// <returns>The produced <see cref="IArgumentRecorderProvider"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorderProvider ForNullableElements<T>(Action<TRecord, IReadOnlyList<T?>, ExpressionSyntax> sharedRecorder, Action<TSemanticRecord, IReadOnlyList<T?>> semanticRecorder) where T : struct;
    }

    /// <summary>Provides adapters that may be applied to parsed array-valued attribute arguments, which may be expressed as <see langword="params"/>-arrays, before invoking a recorder.</summary>
    protected interface IParamsArrayArgumentAdapter
    {
        /// <summary>Produces a <see cref="IArgumentRecorderProvider"/> which ensures that the argument is an array with elements of type <typeparamref name="T"/> before invoking the provided recorders, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="sharedRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing with syntactic context.</param>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing without syntactic context.</param>
        /// <returns>The produced <see cref="IArgumentRecorderProvider"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorderProvider For<T>(Func<TRecord, IReadOnlyList<T>, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> sharedRecorder, Func<TSemanticRecord, IReadOnlyList<T>, bool> semanticRecorder) where T : notnull;

        /// <summary>Produces a <see cref="IArgumentRecorderProvider"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before invoking the provided recorders and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="sharedRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing with syntactic context.</param>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing without syntactic context.</param>
        /// <returns>The produced <see cref="IArgumentRecorderProvider"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorderProvider For<T>(Action<TRecord, IReadOnlyList<T>, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> sharedRecorder, Action<TSemanticRecord, IReadOnlyList<T>> semanticRecorder) where T : notnull;

        /// <summary>Produces a <see cref="IArgumentRecorderProvider"/> which ensures that the argument is either <see langword="null"/> or a collection with elements of type <typeparamref name="T"/> before invoking the provided recorders, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="sharedRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing with syntactic context.</param>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing without syntactic context.</param>
        /// <returns>The produced <see cref="IArgumentRecorderProvider"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorderProvider ForNullableCollection<T>(Func<TRecord, IReadOnlyList<T>?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> sharedRecorder, Func<TSemanticRecord, IReadOnlyList<T>?, bool> semanticRecorder) where T : notnull;

        /// <summary>Produces a <see cref="IArgumentRecorderProvider"/> which ensures that the argument is either <see langword="null"/> or a collection with elements of type <typeparamref name="T"/> before invoking the provided recorders and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="sharedRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing with syntactic context.</param>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing without syntactic context.</param>
        /// <returns>The produced <see cref="IArgumentRecorderProvider"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorderProvider ForNullableCollection<T>(Action<TRecord, IReadOnlyList<T>?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> sharedRecorder, Action<TSemanticRecord, IReadOnlyList<T>?> semanticRecorder) where T : notnull;

        /// <summary>Produces a <see cref="IArgumentRecorderProvider"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where both elements and the collection itself may be <see langword="null"/>, before invoking the provided recorders, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="sharedRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing with syntactic context.</param>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing without syntactic context.</param>
        /// <returns>The produced <see cref="IArgumentRecorderProvider"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorderProvider ForNullable<T>(Func<TRecord, IReadOnlyList<T?>?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> sharedRecorder, Func<TSemanticRecord, IReadOnlyList<T?>?, bool> semanticRecorder) where T : class;

        /// <summary>Produces a <see cref="IArgumentRecorderProvider"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where both elements and the collection itself may be <see langword="null"/>, before invoking the provided recorders and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="sharedRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing with syntactic context.</param>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing without syntactic context.</param>
        /// <returns>The produced <see cref="IArgumentRecorderProvider"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorderProvider ForNullable<T>(Action<TRecord, IReadOnlyList<T?>?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> sharedRecorder, Action<TSemanticRecord, IReadOnlyList<T?>?> semanticRecorder) where T : class;

        /// <summary>Produces a <see cref="IArgumentRecorderProvider"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where both elements and the collection itself may be <see langword="null"/>, before invoking the provided recorders, and which otherwise returns <see langword="false"/>.</summary>s
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="sharedRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing with syntactic context.</param>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing without syntactic context.</param>
        /// <returns>The produced <see cref="IArgumentRecorderProvider"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorderProvider ForNullable<T>(Func<TRecord, IReadOnlyList<T?>?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> sharedRecorder, Func<TSemanticRecord, IReadOnlyList<T?>?, bool> semanticRecorder) where T : struct;

        /// <summary>Produces a <see cref="IArgumentRecorderProvider"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where both elements and the collection itself may be <see langword="null"/>, before invoking the provided recorders and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="sharedRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing with syntactic context.</param>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing without syntactic context.</param>
        /// <returns>The produced <see cref="IArgumentRecorderProvider"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorderProvider ForNullable<T>(Action<TRecord, IReadOnlyList<T?>?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> sharedRecorder, Action<TSemanticRecord, IReadOnlyList<T?>?> semanticRecorder) where T : struct;

        /// <summary>Produces a <see cref="IArgumentRecorderProvider"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where the elements may be <see langword="null"/>, before invoking the provided recorders, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="sharedRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing with syntactic context.</param>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing without syntactic context.</param>
        /// <returns>The produced <see cref="IArgumentRecorderProvider"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorderProvider ForNullableElements<T>(Func<TRecord, IReadOnlyList<T?>, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> sharedRecorder, Func<TSemanticRecord, IReadOnlyList<T?>, bool> semanticRecorder) where T : class;

        /// <summary>Produces a <see cref="IArgumentRecorderProvider"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where the elements may be <see langword="null"/>, before invoking the provided recorders and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="sharedRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing with syntactic context.</param>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing without syntactic context.</param>
        /// <returns>The produced <see cref="IArgumentRecorderProvider"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorderProvider ForNullableElements<T>(Action<TRecord, IReadOnlyList<T?>, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> sharedRecorder, Action<TSemanticRecord, IReadOnlyList<T?>> semanticRecorder) where T : class;

        /// <summary>Produces a <see cref="IArgumentRecorderProvider"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where the elements may be <see langword="null"/>, before invoking the provided recorders, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="sharedRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing with syntactic context.</param>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing without syntactic context.</param>
        /// <returns>The produced <see cref="IArgumentRecorderProvider"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorderProvider ForNullableElements<T>(Func<TRecord, IReadOnlyList<T?>, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> sharedRecorder, Func<TSemanticRecord, IReadOnlyList<T?>, bool> semanticRecorder) where T : struct;

        /// <summary>Produces a <see cref="IArgumentRecorderProvider"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where the elements may be <see langword="null"/>, before invoking the provided recorders and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="sharedRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing with syntactic context.</param>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>, when parsing without syntactic context.</param>
        /// <returns>The produced <see cref="IArgumentRecorderProvider"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorderProvider ForNullableElements<T>(Action<TRecord, IReadOnlyList<T?>, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> sharedRecorder, Action<TSemanticRecord, IReadOnlyList<T?>> semanticRecorder) where T : struct;
    }

    /// <inheritdoc cref="ITypeArgumentRecorderProvider"/>
    protected sealed class TypeArgumentRecorderProvider : ITypeArgumentRecorderProvider
    {
        private DSharedTypeArgumentRecorder Shared { get; }
        private DSemanticTypeArgumentRecorder Semantic { get; }

        /// <summary>Instantiates a <see cref="TypeArgumentRecorderProvider"/>, providing the recorders responsible for recording the argument of a type-parameter.</summary>
        /// <param name="shared">The <see cref="DSharedTypeArgumentRecorder"/>, responsible for recording the argument of a type-parameter which was parsed with syntactic context.</param>
        /// <param name="semantic">The <see cref="DSemanticTypeArgumentRecorder"/>, responsible for recording the argument of a type-parameter which was parsed without syntactic context.</param>
        /// <exception cref="ArgumentNullException"/>
        public TypeArgumentRecorderProvider(DSharedTypeArgumentRecorder shared, DSemanticTypeArgumentRecorder semantic)
        {
            Shared = shared ?? throw new ArgumentNullException(nameof(shared));
            Semantic = semantic ?? throw new ArgumentNullException(nameof(semantic));
        }

        DSharedTypeArgumentRecorder ITypeArgumentRecorderProvider.Shared => Shared;
        DSemanticTypeArgumentRecorder ITypeArgumentRecorderProvider.Semantic => Semantic;
    }

    /// <inheritdoc cref="IArgumentRecorderProvider"/>
    protected sealed class ArgumentRecorderProvider : IArgumentRecorderProvider
    {
        private DSharedArgumentRecorder Shared { get; }
        private DSemanticArgumentRecorder Semantic { get; }

        /// <summary>Instantiates a <see cref="ArgumentRecorderProvider"/>, providing the recorders responsible for recording the argument of a parameter.</summary>
        /// <param name="shared">The <see cref="DSharedArgumentRecorder"/>, responsible for recording the argument of a parameter which was parsed with syntactic context.</param>
        /// <param name="semantic">The <see cref="DSemanticArgumentRecorder"/>, responsible for recording the argument of a parameter which was parsed without syntactic context.</param>
        /// <exception cref="ArgumentNullException"/>
        public ArgumentRecorderProvider(DSharedArgumentRecorder shared, DSemanticArgumentRecorder semantic)
        {
            Shared = shared ?? throw new ArgumentNullException(nameof(shared));
            Semantic = semantic ?? throw new ArgumentNullException(nameof(semantic));
        }

        DSharedArgumentRecorder IArgumentRecorderProvider.Shared => Shared;
        DSemanticArgumentRecorder IArgumentRecorderProvider.Semantic => Semantic;
    }

    private static class SharedAdapterUtility
    {
        public static DSharedArgumentRecorder ForNonParams(Func<TRecord, object?, ExpressionSyntax, bool> recorder)
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

        public static DSharedArgumentRecorder ForParams(Func<TRecord, object?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> recorder)
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
        private DSharedTypeArgumentRecorder Recorder { get; }

        public AttributeTypeArgumentRecorder(TRecord dataRecord, DSharedTypeArgumentRecorder recorder)
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
        private DSharedArgumentRecorder Recorder { get; }

        public AttributeArgumentRecorder(TRecord dataRecord, DSharedArgumentRecorder recorder)
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
        private DSharedArgumentRecorder Recorder { get; }

        public AttributeConstructorArgumentRecorder(TRecord dataRecord, DSharedArgumentRecorder recorder)
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

    private sealed class SemanticAttributeArgumentRecorder : ISemanticAttributeArgumentRecorder
    {
        private TSemanticRecord DataRecord { get; }
        private DSemanticArgumentRecorder Recorder { get; }

        public SemanticAttributeArgumentRecorder(TSemanticRecord dataRecord, DSemanticArgumentRecorder recorder)
        {
            DataRecord = dataRecord;
            Recorder = recorder;
        }

        bool ISemanticAttributeArgumentRecorder.RecordArgument(object? argument) => Recorder(DataRecord, argument);
    }

    private sealed class ArgumentAdapterProvider : IArgumentAdapterProvider
    {
        ITypeArgumentAdapter IArgumentAdapterProvider.TypeArgument { get; } = new TypeArgumentAdapter();
        ISimpleArgumentAdapter IArgumentAdapterProvider.SimpleArgument { get; } = new SimpleArgumentAdapter();
        IArrayArgumentAdapterProvider IArgumentAdapterProvider.ArrayArgument { get; } = new ArrayArgumentAdapterProvider();
    }

    private sealed class ArrayArgumentAdapterProvider : IArrayArgumentAdapterProvider
    {
        INonParamsArrayArgumentAdapter IArrayArgumentAdapterProvider.NonParams { get; } = new NonParamsArrayArgumentAdapter();
        IParamsArrayArgumentAdapter IArrayArgumentAdapterProvider.Params { get; } = new ParamsArrayArgumentAdapter();
    }

    private sealed class TypeArgumentAdapter : ITypeArgumentAdapter
    {
        ITypeArgumentRecorderProvider ITypeArgumentAdapter.For(Func<TRecord, ITypeSymbol, ExpressionSyntax, bool> sharedRecorder, Func<TSemanticRecord, ITypeSymbol, bool> semanticRecorder)
        {
            if (sharedRecorder is null)
            {
                throw new ArgumentNullException(nameof(sharedRecorder));
            }

            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            return For(sharedRecorder, semanticRecorder);
        }

        ITypeArgumentRecorderProvider ITypeArgumentAdapter.For(Action<TRecord, ITypeSymbol, ExpressionSyntax> sharedRecorder, Action<TSemanticRecord, ITypeSymbol> semanticRecorder)
        {
            if (sharedRecorder is null)
            {
                throw new ArgumentNullException(nameof(sharedRecorder));
            }

            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            return For(sharedWrapper, semanticWrapper);

            bool sharedWrapper(TRecord dataRecord, ITypeSymbol argument, ExpressionSyntax syntax)
            {
                sharedRecorder(dataRecord, argument, syntax);

                return true;
            }

            bool semanticWrapper(TSemanticRecord dataRecord, ITypeSymbol argument)
            {
                semanticRecorder(dataRecord, argument);

                return true;
            }
        }

        private static ITypeArgumentRecorderProvider For(Func<TRecord, ITypeSymbol, ExpressionSyntax, bool> sharedRecorder, Func<TSemanticRecord, ITypeSymbol, bool> semanticRecorder)
        {
            return new TypeArgumentRecorderProvider(sharedWrapper, semanticWrapper);

            bool sharedWrapper(TRecord dataRecord, ITypeSymbol argument, ExpressionSyntax syntax)
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

                return sharedRecorder(dataRecord, argument, syntax);
            }

            bool semanticWrapper(TSemanticRecord dataRecord, ITypeSymbol argument)
            {
                if (dataRecord is null)
                {
                    throw new ArgumentNullException(nameof(dataRecord));
                }

                if (argument is null)
                {
                    throw new ArgumentNullException(nameof(argument));
                }

                return semanticRecorder(dataRecord, argument);
            }
        }
    }

    private sealed class SimpleArgumentAdapter : ISimpleArgumentAdapter
    {
        IArgumentRecorderProvider ISimpleArgumentAdapter.For<T>(Func<TRecord, T, ExpressionSyntax, bool> sharedRecorder, Func<TSemanticRecord, T, bool> semanticRecorder)
        {
            if (sharedRecorder is null)
            {
                throw new ArgumentNullException(nameof(sharedRecorder));
            }

            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            return For(sharedRecorder, semanticRecorder);
        }

        IArgumentRecorderProvider ISimpleArgumentAdapter.For<T>(Action<TRecord, T, ExpressionSyntax> sharedRecorder, Action<TSemanticRecord, T> semanticRecorder)
        {
            if (sharedRecorder is null)
            {
                throw new ArgumentNullException(nameof(sharedRecorder));
            }

            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            return For<T>(sharedWrapper, semanticWrapper);

            bool sharedWrapper(TRecord dataRecord, T argument, ExpressionSyntax syntax)
            {
                sharedRecorder(dataRecord, argument, syntax);

                return true;
            }

            bool semanticWrapper(TSemanticRecord dataRecord, T argument)
            {
                semanticRecorder(dataRecord, argument);

                return true;
            }
        }

        IArgumentRecorderProvider ISimpleArgumentAdapter.ForNullable<T>(Func<TRecord, T?, ExpressionSyntax, bool> sharedRecorder, Func<TSemanticRecord, T?, bool> semanticRecorder) where T : class
        {
            if (sharedRecorder is null)
            {
                throw new ArgumentNullException(nameof(sharedRecorder));
            }

            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            return ForNullable(sharedRecorder, semanticRecorder);
        }

        IArgumentRecorderProvider ISimpleArgumentAdapter.ForNullable<T>(Action<TRecord, T?, ExpressionSyntax> sharedRecorder, Action<TSemanticRecord, T?> semanticRecorder) where T : class
        {
            if (sharedRecorder is null)
            {
                throw new ArgumentNullException(nameof(sharedRecorder));
            }

            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            return ForNullable<T>(sharedWrapper, semanticWrapper);

            bool sharedWrapper(TRecord dataRecord, T? argument, ExpressionSyntax syntax)
            {
                sharedRecorder(dataRecord, argument, syntax);

                return true;
            }

            bool semanticWrapper(TSemanticRecord dataRecord, T? argument)
            {
                semanticRecorder(dataRecord, argument);

                return true;
            }
        }

        IArgumentRecorderProvider ISimpleArgumentAdapter.ForNullable<T>(Func<TRecord, T?, ExpressionSyntax, bool> sharedRecorder, Func<TSemanticRecord, T?, bool> semanticRecorder) where T : struct
        {
            if (sharedRecorder is null)
            {
                throw new ArgumentNullException(nameof(sharedRecorder));
            }

            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            return ForNullable(sharedRecorder, semanticRecorder);
        }

        IArgumentRecorderProvider ISimpleArgumentAdapter.ForNullable<T>(Action<TRecord, T?, ExpressionSyntax> sharedRecorder, Action<TSemanticRecord, T?> semanticRecorder) where T : struct
        {
            if (sharedRecorder is null)
            {
                throw new ArgumentNullException(nameof(sharedRecorder));
            }

            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            return ForNullable<T>(sharedWrapper, semanticWrapper);

            bool sharedWrapper(TRecord dataRecord, T? argument, ExpressionSyntax syntax)
            {
                sharedRecorder(dataRecord, argument, syntax);

                return true;
            }

            bool semanticWrapper(TSemanticRecord dataRecord, T? argument)
            {
                semanticRecorder(dataRecord, argument);

                return true;
            }
        }

        private static IArgumentRecorderProvider For<T>(Func<TRecord, T, ExpressionSyntax, bool> sharedRecorder, Func<TSemanticRecord, T, bool> semanticRecorder)
        {
            return new ArgumentRecorderProvider(SharedAdapterUtility.ForNonParams(sharedWrapper), semanticWrapper);

            bool sharedWrapper(TRecord dataRecord, object? argument, ExpressionSyntax syntax)
            {
                if (argument is null)
                {
                    return false;
                }

                if (argument is not T tArgument)
                {
                    return false;
                }

                return sharedRecorder(dataRecord, tArgument, syntax);
            }

            bool semanticWrapper(TSemanticRecord dataRecord, object? argument)
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

                return semanticRecorder(dataRecord, tArgument);
            }
        }

        private static IArgumentRecorderProvider ForNullable<T>(Func<TRecord, T?, ExpressionSyntax, bool> sharedRecorder, Func<TSemanticRecord, T?, bool> semanticRecorder) where T : class
        {
            return new ArgumentRecorderProvider(SharedAdapterUtility.ForNonParams(sharedWrapper), semanticWrapper);

            bool sharedWrapper(TRecord dataRecord, object? argument, ExpressionSyntax syntax)
            {
                if (argument is null)
                {
                    return sharedRecorder(dataRecord, null, syntax);
                }

                if (argument is not T tArgument)
                {
                    return false;
                }

                return sharedRecorder(dataRecord, tArgument, syntax);
            }

            bool semanticWrapper(TSemanticRecord dataRecord, object? argument)
            {
                if (dataRecord is null)
                {
                    throw new ArgumentNullException(nameof(dataRecord));
                }

                if (argument is null)
                {
                    return semanticRecorder(dataRecord, null);
                }

                if (argument is not T tArgument)
                {
                    return false;
                }

                return semanticRecorder(dataRecord, tArgument);
            }
        }

        private static IArgumentRecorderProvider ForNullable<T>(Func<TRecord, T?, ExpressionSyntax, bool> sharedRecorder, Func<TSemanticRecord, T?, bool> semanticRecorder) where T : struct
        {
            return new ArgumentRecorderProvider(SharedAdapterUtility.ForNonParams(sharedWrapper), semanticWrapper);

            bool sharedWrapper(TRecord dataRecord, object? argument, ExpressionSyntax syntax)
            {
                if (argument is null)
                {
                    return sharedRecorder(dataRecord, null, syntax);
                }

                if (argument is not T tArgument)
                {
                    return false;
                }

                return sharedRecorder(dataRecord, tArgument, syntax);
            }

            bool semanticWrapper(TSemanticRecord dataRecord, object? argument)
            {
                if (dataRecord is null)
                {
                    throw new ArgumentNullException(nameof(dataRecord));
                }

                if (argument is null)
                {
                    return semanticRecorder(dataRecord, null);
                }

                if (argument is not T tArgument)
                {
                    return false;
                }

                return semanticRecorder(dataRecord, tArgument);
            }
        }
    }

    private sealed class NonParamsArrayArgumentAdapter : INonParamsArrayArgumentAdapter
    {
        IArgumentRecorderProvider INonParamsArrayArgumentAdapter.For<T>(Func<TRecord, IReadOnlyList<T>, ExpressionSyntax, bool> sharedRecorder, Func<TSemanticRecord, IReadOnlyList<T>, bool> semanticRecorder)
        {
            if (sharedRecorder is null)
            {
                throw new ArgumentNullException(nameof(sharedRecorder));
            }

            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            return For(sharedRecorder, semanticRecorder);
        }

        IArgumentRecorderProvider INonParamsArrayArgumentAdapter.For<T>(Action<TRecord, IReadOnlyList<T>, ExpressionSyntax> sharedRecorder, Action<TSemanticRecord, IReadOnlyList<T>> semanticRecorder)
        {
            if (sharedRecorder is null)
            {
                throw new ArgumentNullException(nameof(sharedRecorder));
            }

            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            return For<T>(sharedWrapper, semanticWrapper);

            bool sharedWrapper(TRecord dataRecord, IReadOnlyList<T> argument, ExpressionSyntax syntax)
            {
                sharedRecorder(dataRecord, argument, syntax);

                return true;
            }

            bool semanticWrapper(TSemanticRecord dataRecord, IReadOnlyList<T> argument)
            {
                semanticRecorder(dataRecord, argument);

                return true;
            }
        }

        IArgumentRecorderProvider INonParamsArrayArgumentAdapter.ForNullable<T>(Func<TRecord, IReadOnlyList<T?>?, ExpressionSyntax, bool> sharedRecorder, Func<TSemanticRecord, IReadOnlyList<T?>?, bool> semanticRecorder) where T : class
        {
            if (sharedRecorder is null)
            {
                throw new ArgumentNullException(nameof(sharedRecorder));
            }

            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            return ForNullable(sharedRecorder, semanticRecorder);
        }

        IArgumentRecorderProvider INonParamsArrayArgumentAdapter.ForNullable<T>(Action<TRecord, IReadOnlyList<T?>?, ExpressionSyntax> sharedRecorder, Action<TSemanticRecord, IReadOnlyList<T?>?> semanticRecorder) where T : class
        {
            if (sharedRecorder is null)
            {
                throw new ArgumentNullException(nameof(sharedRecorder));
            }

            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            return ForNullable<T>(sharedWrapper, semanticWrapper);

            bool sharedWrapper(TRecord dataRecord, IReadOnlyList<T?>? argument, ExpressionSyntax syntax)
            {
                sharedRecorder(dataRecord, argument, syntax);

                return true;
            }

            bool semanticWrapper(TSemanticRecord dataRecord, IReadOnlyList<T?>? argument)
            {
                semanticRecorder(dataRecord, argument);

                return true;
            }
        }

        IArgumentRecorderProvider INonParamsArrayArgumentAdapter.ForNullable<T>(Func<TRecord, IReadOnlyList<T?>?, ExpressionSyntax, bool> sharedRecorder, Func<TSemanticRecord, IReadOnlyList<T?>?, bool> semanticRecorder)
        {
            if (sharedRecorder is null)
            {
                throw new ArgumentNullException(nameof(sharedRecorder));
            }

            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            return ForNullable(sharedRecorder, semanticRecorder);
        }

        IArgumentRecorderProvider INonParamsArrayArgumentAdapter.ForNullable<T>(Action<TRecord, IReadOnlyList<T?>?, ExpressionSyntax> sharedRecorder, Action<TSemanticRecord, IReadOnlyList<T?>?> semanticRecorder)
        {
            if (sharedRecorder is null)
            {
                throw new ArgumentNullException(nameof(sharedRecorder));
            }

            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            return ForNullable<T>(sharedWrapper, semanticWrapper);

            bool sharedWrapper(TRecord dataRecord, IReadOnlyList<T?>? argument, ExpressionSyntax syntax)
            {
                sharedRecorder(dataRecord, argument, syntax);

                return true;
            }

            bool semanticWrapper(TSemanticRecord dataRecord, IReadOnlyList<T?>? argument)
            {
                semanticRecorder(dataRecord, argument);

                return true;
            }
        }

        IArgumentRecorderProvider INonParamsArrayArgumentAdapter.ForNullableCollection<T>(Func<TRecord, IReadOnlyList<T>?, ExpressionSyntax, bool> sharedRecorder, Func<TSemanticRecord, IReadOnlyList<T>?, bool> semanticRecorder)
        {
            if (sharedRecorder is null)
            {
                throw new ArgumentNullException(nameof(sharedRecorder));
            }

            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            return ForNullableCollection(sharedRecorder, semanticRecorder);
        }

        IArgumentRecorderProvider INonParamsArrayArgumentAdapter.ForNullableCollection<T>(Action<TRecord, IReadOnlyList<T>?, ExpressionSyntax> sharedRecorder, Action<TSemanticRecord, IReadOnlyList<T>?> semanticRecorder)
        {
            if (sharedRecorder is null)
            {
                throw new ArgumentNullException(nameof(sharedRecorder));
            }

            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            return ForNullableCollection<T>(sharedWrapper, semanticWrapper);

            bool sharedWrapper(TRecord dataRecord, IReadOnlyList<T>? argument, ExpressionSyntax syntax)
            {
                sharedRecorder(dataRecord, argument, syntax);

                return true;
            }

            bool semanticWrapper(TSemanticRecord dataRecord, IReadOnlyList<T>? argument)
            {
                semanticRecorder(dataRecord, argument);

                return true;
            }
        }

        IArgumentRecorderProvider INonParamsArrayArgumentAdapter.ForNullableElements<T>(Func<TRecord, IReadOnlyList<T?>, ExpressionSyntax, bool> sharedRecorder, Func<TSemanticRecord, IReadOnlyList<T?>, bool> semanticRecorder) where T : class
        {
            if (sharedRecorder is null)
            {
                throw new ArgumentNullException(nameof(sharedRecorder));
            }

            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            return ForNullableElements(sharedRecorder, semanticRecorder);
        }

        IArgumentRecorderProvider INonParamsArrayArgumentAdapter.ForNullableElements<T>(Action<TRecord, IReadOnlyList<T?>, ExpressionSyntax> sharedRecorder, Action<TSemanticRecord, IReadOnlyList<T?>> semanticRecorder) where T : class
        {
            if (sharedRecorder is null)
            {
                throw new ArgumentNullException(nameof(sharedRecorder));
            }

            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            return ForNullableElements<T>(sharedWrapper, semanticWrapper);

            bool sharedWrapper(TRecord dataRecord, IReadOnlyList<T?> argument, ExpressionSyntax syntax)
            {
                sharedRecorder(dataRecord, argument, syntax);

                return true;
            }

            bool semanticWrapper(TSemanticRecord dataRecord, IReadOnlyList<T?> argument)
            {
                semanticRecorder(dataRecord, argument);

                return true;
            }
        }

        IArgumentRecorderProvider INonParamsArrayArgumentAdapter.ForNullableElements<T>(Func<TRecord, IReadOnlyList<T?>, ExpressionSyntax, bool> sharedRecorder, Func<TSemanticRecord, IReadOnlyList<T?>, bool> semanticRecorder)
        {
            if (sharedRecorder is null)
            {
                throw new ArgumentNullException(nameof(sharedRecorder));
            }

            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            return ForNullableElements(sharedRecorder, semanticRecorder);
        }

        IArgumentRecorderProvider INonParamsArrayArgumentAdapter.ForNullableElements<T>(Action<TRecord, IReadOnlyList<T?>, ExpressionSyntax> sharedRecorder, Action<TSemanticRecord, IReadOnlyList<T?>> semanticRecorder)
        {
            if (sharedRecorder is null)
            {
                throw new ArgumentNullException(nameof(sharedRecorder));
            }

            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            return ForNullableElements<T>(sharedWrapper, semanticWrapper);

            bool sharedWrapper(TRecord dataRecord, IReadOnlyList<T?> argument, ExpressionSyntax syntax)
            {
                sharedRecorder(dataRecord, argument, syntax);

                return true;
            }

            bool semanticWrapper(TSemanticRecord dataRecord, IReadOnlyList<T?> argument)
            {
                semanticRecorder(dataRecord, argument);

                return true;
            }
        }

        private static IArgumentRecorderProvider For<T>(Func<TRecord, IReadOnlyList<T>, ExpressionSyntax, bool> sharedRecorder, Func<TSemanticRecord, IReadOnlyList<T>, bool> semanticRecorder)
        {
            return new ArgumentRecorderProvider(SharedAdapterUtility.ForNonParams(sharedWrapper), semanticWrapper);

            bool sharedWrapper(TRecord dataRecord, object? argument, ExpressionSyntax syntax) => CommonArrayConverters.NonNullable<T>(argument).Match
            (
                static (error) => false,
                (converted) => sharedRecorder(dataRecord, converted, syntax)
            );

            bool semanticWrapper(TSemanticRecord dataRecord, object? argument)
            {
                if (dataRecord is null)
                {
                    throw new ArgumentNullException(nameof(dataRecord));
                }

                return CommonArrayConverters.NonNullable<T>(argument).Match
                (
                    static (error) => false,
                    (converted) => semanticRecorder(dataRecord, converted)
                );
            }
        }

        private static IArgumentRecorderProvider ForNullable<T>(Func<TRecord, IReadOnlyList<T?>?, ExpressionSyntax, bool> sharedRecorder, Func<TSemanticRecord, IReadOnlyList<T?>?, bool> semanticRecorder) where T : class
        {
            return new ArgumentRecorderProvider(SharedAdapterUtility.ForNonParams(sharedWrapper), semanticWrapper);

            bool sharedWrapper(TRecord dataRecord, object? argument, ExpressionSyntax syntax) => CommonArrayConverters.Nullable<T>(argument).Match
            (
                static (error) => false,
                (converted) => sharedRecorder(dataRecord, converted, syntax)
            );

            bool semanticWrapper(TSemanticRecord dataRecord, object? argument)
            {
                if (dataRecord is null)
                {
                    throw new ArgumentNullException(nameof(dataRecord));
                }

                return CommonArrayConverters.Nullable<T>(argument).Match
                (
                    static (error) => false,
                    (converted) => semanticRecorder(dataRecord, converted)
                );
            }
        }

        private static IArgumentRecorderProvider ForNullable<T>(Func<TRecord, IReadOnlyList<T?>?, ExpressionSyntax, bool> sharedRecorder, Func<TSemanticRecord, IReadOnlyList<T?>?, bool> semanticRecorder) where T : struct
        {
            return new ArgumentRecorderProvider(SharedAdapterUtility.ForNonParams(sharedWrapper), semanticWrapper);

            bool sharedWrapper(TRecord dataRecord, object? argument, ExpressionSyntax syntax) => CommonArrayConverters.Nullable<T?>(argument).Match
            (
                static (error) => false,
                (converted) => sharedRecorder(dataRecord, converted, syntax)
            );

            bool semanticWrapper(TSemanticRecord dataRecord, object? argument)
            {
                if (dataRecord is null)
                {
                    throw new ArgumentNullException(nameof(dataRecord));
                }

                return CommonArrayConverters.Nullable<T?>(argument).Match
                (
                    static (error) => false,
                    (converted) => semanticRecorder(dataRecord, converted)
                );
            }
        }

        private static IArgumentRecorderProvider ForNullableCollection<T>(Func<TRecord, IReadOnlyList<T>?, ExpressionSyntax, bool> sharedRecorder, Func<TSemanticRecord, IReadOnlyList<T>?, bool> semanticRecorder)
        {
            return new ArgumentRecorderProvider(SharedAdapterUtility.ForNonParams(sharedWrapper), semanticWrapper);

            bool sharedWrapper(TRecord dataRecord, object? argument, ExpressionSyntax syntax) => CommonArrayConverters.NullableCollection<T>(argument).Match
            (
                static (error) => false,
                (converted) => sharedRecorder(dataRecord, converted, syntax)
            );

            bool semanticWrapper(TSemanticRecord dataRecord, object? argument)
            {
                if (dataRecord is null)
                {
                    throw new ArgumentNullException(nameof(dataRecord));
                }

                return CommonArrayConverters.NullableCollection<T>(argument).Match
                (
                    static (error) => false,
                    (converted) => semanticRecorder(dataRecord, converted)
                );
            }
        }

        private static IArgumentRecorderProvider ForNullableElements<T>(Func<TRecord, IReadOnlyList<T?>, ExpressionSyntax, bool> sharedRecorder, Func<TSemanticRecord, IReadOnlyList<T?>, bool> semanticRecorder) where T : class
        {
            return new ArgumentRecorderProvider(SharedAdapterUtility.ForNonParams(sharedWrapper), semanticWrapper);

            bool sharedWrapper(TRecord dataRecord, object? argument, ExpressionSyntax syntax) => CommonArrayConverters.NullableElements<T>(argument).Match
            (
                static (error) => false,
                (converted) => sharedRecorder(dataRecord, converted, syntax)
            );

            bool semanticWrapper(TSemanticRecord dataRecord, object? argument)
            {
                if (dataRecord is null)
                {
                    throw new ArgumentNullException(nameof(dataRecord));
                }

                return CommonArrayConverters.NullableElements<T?>(argument).Match
                (
                    static (error) => false,
                    (converted) => semanticRecorder(dataRecord, converted)
                );
            }
        }

        private static IArgumentRecorderProvider ForNullableElements<T>(Func<TRecord, IReadOnlyList<T?>, ExpressionSyntax, bool> sharedRecorder, Func<TSemanticRecord, IReadOnlyList<T?>, bool> semanticRecorder) where T : struct
        {
            return new ArgumentRecorderProvider(SharedAdapterUtility.ForNonParams(sharedWrapper), semanticWrapper);

            bool sharedWrapper(TRecord dataRecord, object? argument, ExpressionSyntax syntax) => CommonArrayConverters.NullableElements<T?>(argument).Match
            (
                static (error) => false,
                (converted) => sharedRecorder(dataRecord, converted, syntax)
            );

            bool semanticWrapper(TSemanticRecord dataRecord, object? argument)
            {
                if (dataRecord is null)
                {
                    throw new ArgumentNullException(nameof(dataRecord));
                }

                return CommonArrayConverters.NullableElements<T?>(argument).Match
                (
                    static (error) => false,
                    (converted) => semanticRecorder(dataRecord, converted)
                );
            }
        }
    }

    private sealed class ParamsArrayArgumentAdapter : IParamsArrayArgumentAdapter
    {
        IArgumentRecorderProvider IParamsArrayArgumentAdapter.For<T>(Func<TRecord, IReadOnlyList<T>, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> sharedRecorder, Func<TSemanticRecord, IReadOnlyList<T>, bool> semanticRecorder)
        {
            if (sharedRecorder is null)
            {
                throw new ArgumentNullException(nameof(sharedRecorder));
            }

            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            return For(sharedRecorder, semanticRecorder);
        }

        IArgumentRecorderProvider IParamsArrayArgumentAdapter.For<T>(Action<TRecord, IReadOnlyList<T>, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> sharedRecorder, Action<TSemanticRecord, IReadOnlyList<T>> semanticRecorder)
        {
            if (sharedRecorder is null)
            {
                throw new ArgumentNullException(nameof(sharedRecorder));
            }

            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            return For<T>(sharedWrapper, semanticWrapper);

            bool sharedWrapper(TRecord dataRecord, IReadOnlyList<T> argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
            {
                sharedRecorder(dataRecord, argument, syntax);

                return true;
            }

            bool semanticWrapper(TSemanticRecord dataRecord, IReadOnlyList<T> argument)
            {
                semanticRecorder(dataRecord, argument);

                return true;
            }
        }

        IArgumentRecorderProvider IParamsArrayArgumentAdapter.ForNullable<T>(Func<TRecord, IReadOnlyList<T?>?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> sharedRecorder, Func<TSemanticRecord, IReadOnlyList<T?>?, bool> semanticRecorder) where T : class
        {
            if (sharedRecorder is null)
            {
                throw new ArgumentNullException(nameof(sharedRecorder));
            }

            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            return ForNullable(sharedRecorder, semanticRecorder);
        }

        IArgumentRecorderProvider IParamsArrayArgumentAdapter.ForNullable<T>(Action<TRecord, IReadOnlyList<T?>?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> sharedRecorder, Action<TSemanticRecord, IReadOnlyList<T?>?> semanticRecorder) where T : class
        {
            if (sharedRecorder is null)
            {
                throw new ArgumentNullException(nameof(sharedRecorder));
            }

            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            return ForNullable<T>(sharedWrapper, semanticWrapper);

            bool sharedWrapper(TRecord dataRecord, IReadOnlyList<T?>? argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
            {
                sharedRecorder(dataRecord, argument, syntax);

                return true;
            }

            bool semanticWrapper(TSemanticRecord dataRecord, IReadOnlyList<T?>? argument)
            {
                semanticRecorder(dataRecord, argument);

                return true;
            }
        }

        IArgumentRecorderProvider IParamsArrayArgumentAdapter.ForNullable<T>(Func<TRecord, IReadOnlyList<T?>?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> sharedRecorder, Func<TSemanticRecord, IReadOnlyList<T?>?, bool> semanticRecorder)
        {
            if (sharedRecorder is null)
            {
                throw new ArgumentNullException(nameof(sharedRecorder));
            }

            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            return ForNullable(sharedRecorder, semanticRecorder);
        }

        IArgumentRecorderProvider IParamsArrayArgumentAdapter.ForNullable<T>(Action<TRecord, IReadOnlyList<T?>?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> sharedRecorder, Action<TSemanticRecord, IReadOnlyList<T?>?> semanticRecorder)
        {
            if (sharedRecorder is null)
            {
                throw new ArgumentNullException(nameof(sharedRecorder));
            }

            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            return ForNullable<T>(sharedWrapper, semanticWrapper);

            bool sharedWrapper(TRecord dataRecord, IReadOnlyList<T?>? argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
            {
                sharedRecorder(dataRecord, argument, syntax);

                return true;
            }

            bool semanticWrapper(TSemanticRecord dataRecord, IReadOnlyList<T?>? argument)
            {
                semanticRecorder(dataRecord, argument);

                return true;
            }
        }

        IArgumentRecorderProvider IParamsArrayArgumentAdapter.ForNullableCollection<T>(Func<TRecord, IReadOnlyList<T>?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> sharedRecorder, Func<TSemanticRecord, IReadOnlyList<T>?, bool> semanticRecorder)
        {
            if (sharedRecorder is null)
            {
                throw new ArgumentNullException(nameof(sharedRecorder));
            }

            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            return ForNullableCollection(sharedRecorder, semanticRecorder);
        }

        IArgumentRecorderProvider IParamsArrayArgumentAdapter.ForNullableCollection<T>(Action<TRecord, IReadOnlyList<T>?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> sharedRecorder, Action<TSemanticRecord, IReadOnlyList<T>?> semanticRecorder)
        {
            if (sharedRecorder is null)
            {
                throw new ArgumentNullException(nameof(sharedRecorder));
            }

            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            return ForNullableCollection<T>(sharedWrapper, semanticWrapper);

            bool sharedWrapper(TRecord dataRecord, IReadOnlyList<T>? argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
            {
                sharedRecorder(dataRecord, argument, syntax);

                return true;
            }

            bool semanticWrapper(TSemanticRecord dataRecord, IReadOnlyList<T>? argument)
            {
                semanticRecorder(dataRecord, argument);

                return true;
            }
        }

        IArgumentRecorderProvider IParamsArrayArgumentAdapter.ForNullableElements<T>(Func<TRecord, IReadOnlyList<T?>, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> sharedRecorder, Func<TSemanticRecord, IReadOnlyList<T?>, bool> semanticRecorder) where T : class
        {
            if (sharedRecorder is null)
            {
                throw new ArgumentNullException(nameof(sharedRecorder));
            }

            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            return ForNullableElements(sharedRecorder, semanticRecorder);
        }

        IArgumentRecorderProvider IParamsArrayArgumentAdapter.ForNullableElements<T>(Action<TRecord, IReadOnlyList<T?>, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> sharedRecorder, Action<TSemanticRecord, IReadOnlyList<T?>> semanticRecorder) where T : class
        {
            if (sharedRecorder is null)
            {
                throw new ArgumentNullException(nameof(sharedRecorder));
            }

            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            return ForNullableElements<T>(sharedWrapper, semanticWrapper);

            bool sharedWrapper(TRecord dataRecord, IReadOnlyList<T?> argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
            {
                sharedRecorder(dataRecord, argument, syntax);

                return true;
            }

            bool semanticWrapper(TSemanticRecord dataRecord, IReadOnlyList<T?> argument)
            {
                semanticRecorder(dataRecord, argument);

                return true;
            }
        }

        IArgumentRecorderProvider IParamsArrayArgumentAdapter.ForNullableElements<T>(Func<TRecord, IReadOnlyList<T?>, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> sharedRecorder, Func<TSemanticRecord, IReadOnlyList<T?>, bool> semanticRecorder)
        {
            if (sharedRecorder is null)
            {
                throw new ArgumentNullException(nameof(sharedRecorder));
            }

            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            return ForNullableElements(sharedRecorder, semanticRecorder);
        }

        IArgumentRecorderProvider IParamsArrayArgumentAdapter.ForNullableElements<T>(Action<TRecord, IReadOnlyList<T?>, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> sharedRecorder, Action<TSemanticRecord, IReadOnlyList<T?>> semanticRecorder)
        {
            if (sharedRecorder is null)
            {
                throw new ArgumentNullException(nameof(sharedRecorder));
            }

            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            return ForNullableElements<T>(sharedWrapper, semanticWrapper);

            bool sharedWrapper(TRecord dataRecord, IReadOnlyList<T?> argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
            {
                sharedRecorder(dataRecord, argument, syntax);

                return true;
            }

            bool semanticWrapper(TSemanticRecord dataRecord, IReadOnlyList<T?> argument)
            {
                semanticRecorder(dataRecord, argument);

                return true;
            }
        }

        private static IArgumentRecorderProvider For<T>(Func<TRecord, IReadOnlyList<T>, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> sharedRecorder, Func<TSemanticRecord, IReadOnlyList<T>, bool> semanticRecorder)
        {
            return new ArgumentRecorderProvider(SharedAdapterUtility.ForParams(sharedWrapper), semanticWrapper);

            bool sharedWrapper(TRecord dataRecord, object? argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) => CommonArrayConverters.NonNullable<T>(argument).Match
            (
                static (error) => false,
                (converted) => sharedRecorder(dataRecord, converted, syntax)
            );

            bool semanticWrapper(TSemanticRecord dataRecord, object? argument)
            {
                if (dataRecord is null)
                {
                    throw new ArgumentNullException(nameof(dataRecord));
                }

                return CommonArrayConverters.NonNullable<T>(argument).Match
                (
                    static (error) => false,
                    (converted) => semanticRecorder(dataRecord, converted)
                );
            }
        }

        private static IArgumentRecorderProvider ForNullable<T>(Func<TRecord, IReadOnlyList<T?>?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> sharedRecorder, Func<TSemanticRecord, IReadOnlyList<T?>?, bool> semanticRecorder) where T : class
        {
            return new ArgumentRecorderProvider(SharedAdapterUtility.ForParams(sharedWrapper), semanticWrapper);

            bool sharedWrapper(TRecord dataRecord, object? argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) => CommonArrayConverters.Nullable<T>(argument).Match
            (
                static (error) => false,
                (converted) => sharedRecorder(dataRecord, converted, syntax)
            );

            bool semanticWrapper(TSemanticRecord dataRecord, object? argument)
            {
                if (dataRecord is null)
                {
                    throw new ArgumentNullException(nameof(dataRecord));
                }

                return CommonArrayConverters.Nullable<T>(argument).Match
                (
                    static (error) => false,
                    (converted) => semanticRecorder(dataRecord, converted)
                );
            }
        }

        private static IArgumentRecorderProvider ForNullable<T>(Func<TRecord, IReadOnlyList<T?>?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> sharedRecorder, Func<TSemanticRecord, IReadOnlyList<T?>?, bool> semanticRecorder) where T : struct
        {
            return new ArgumentRecorderProvider(SharedAdapterUtility.ForParams(sharedWrapper), semanticWrapper);

            bool sharedWrapper(TRecord dataRecord, object? argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) => CommonArrayConverters.Nullable<T?>(argument).Match
            (
                static (error) => false,
                (converted) => sharedRecorder(dataRecord, converted, syntax)
            );

            bool semanticWrapper(TSemanticRecord dataRecord, object? argument)
            {
                if (dataRecord is null)
                {
                    throw new ArgumentNullException(nameof(dataRecord));
                }

                return CommonArrayConverters.Nullable<T?>(argument).Match
                (
                    static (error) => false,
                    (converted) => semanticRecorder(dataRecord, converted)
                );
            }
        }

        private static IArgumentRecorderProvider ForNullableCollection<T>(Func<TRecord, IReadOnlyList<T>?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> sharedRecorder, Func<TSemanticRecord, IReadOnlyList<T>?, bool> semanticRecorder)
        {
            return new ArgumentRecorderProvider(SharedAdapterUtility.ForParams(sharedWrapper), semanticWrapper);

            bool sharedWrapper(TRecord dataRecord, object? argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) => CommonArrayConverters.NullableCollection<T>(argument).Match
            (
                static (error) => false,
                (converted) => sharedRecorder(dataRecord, converted, syntax)
            );

            bool semanticWrapper(TSemanticRecord dataRecord, object? argument)
            {
                if (dataRecord is null)
                {
                    throw new ArgumentNullException(nameof(dataRecord));
                }

                return CommonArrayConverters.NullableCollection<T>(argument).Match
                (
                    static (error) => false,
                    (converted) => semanticRecorder(dataRecord, converted)
                );
            }
        }

        private static IArgumentRecorderProvider ForNullableElements<T>(Func<TRecord, IReadOnlyList<T?>, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> sharedRecorder, Func<TSemanticRecord, IReadOnlyList<T?>, bool> semanticRecorder) where T : class
        {
            return new ArgumentRecorderProvider(SharedAdapterUtility.ForParams(sharedWrapper), semanticWrapper);

            bool sharedWrapper(TRecord dataRecord, object? argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) => CommonArrayConverters.NullableElements<T>(argument).Match
            (
                static (error) => false,
                (converted) => sharedRecorder(dataRecord, converted, syntax)
            );

            bool semanticWrapper(TSemanticRecord dataRecord, object? argument)
            {
                if (dataRecord is null)
                {
                    throw new ArgumentNullException(nameof(dataRecord));
                }

                return CommonArrayConverters.NullableElements<T?>(argument).Match
                (
                    static (error) => false,
                    (converted) => semanticRecorder(dataRecord, converted)
                );
            }
        }

        private static IArgumentRecorderProvider ForNullableElements<T>(Func<TRecord, IReadOnlyList<T?>, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> sharedRecorder, Func<TSemanticRecord, IReadOnlyList<T?>, bool> semanticRecorder) where T : struct
        {
            return new ArgumentRecorderProvider(SharedAdapterUtility.ForParams(sharedWrapper), semanticWrapper);

            bool sharedWrapper(TRecord dataRecord, object? argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) => CommonArrayConverters.NullableElements<T?>(argument).Match
            (
                static (error) => false,
                (converted) => sharedRecorder(dataRecord, converted, syntax)
            );

            bool semanticWrapper(TSemanticRecord dataRecord, object? argument)
            {
                if (dataRecord is null)
                {
                    throw new ArgumentNullException(nameof(dataRecord));
                }

                return CommonArrayConverters.NullableElements<T?>(argument).Match
                (
                    static (error) => false,
                    (converted) => semanticRecorder(dataRecord, converted)
                );
            }
        }
    }
}
