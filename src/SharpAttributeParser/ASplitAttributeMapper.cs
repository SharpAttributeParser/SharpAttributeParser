namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

/// <summary>An abstract <see cref="ISemanticAttributeMapper{TRecord}"/> and <see cref="ISyntacticAttributeMapper{TRecord}"/>, allowing attributes to be parsed both semantically and syntactically, but separately. Mappings are provided through the following methods:
/// <list type="bullet">
/// <item><see cref="AddTypeParameterMappings"/></item>
/// <item><see cref="AddParameterMappings"/></item>
/// </list></summary>
/// <typeparam name="TSemanticRecord">The type to which the arguments of the mapped parameters are recorded.</typeparam>
/// <typeparam name="TSyntacticRecord">The type to which the syntactical information about arguments of the mapped parameters are recorded.</typeparam>
public abstract class ASplitAttributeMapper<TSemanticRecord, TSyntacticRecord> : ISemanticAttributeMapper<TSemanticRecord>, ISyntacticAttributeMapper<TSyntacticRecord>
{
    /// <summary>Provides adapters that may be applied to parsed arguments before invoking a recorder.</summary>
    [SuppressMessage("Major Code Smell", "S2743: Static fields should not be used in generic types", Justification = "Type uses TRecord.")]
    protected static IArgumentAdapterProvider Adapters { get; } = new ArgumentAdapterProvider();

    private bool IsInitialized { get; set; }

    private IReadOnlyDictionary<int, DSemanticTypeArgumentRecorder> SemanticIndexedTypeParameterMappings { get; set; } = null!;
    private IReadOnlyDictionary<string, DSemanticTypeArgumentRecorder> SemanticNamedTypeParameterMappings { get; set; } = null!;
    private IReadOnlyDictionary<string, DSemanticArgumentRecorder> SemanticParameterMappings { get; set; } = null!;

    private IReadOnlyDictionary<int, DSyntacticTypeArgumentRecorder> SyntacticIndexedTypeParameterMappings { get; set; } = null!;
    private IReadOnlyDictionary<string, DSyntacticTypeArgumentRecorder> SyntacticNamedTypeParameterMappings { get; set; } = null!;
    private IReadOnlyDictionary<string, DSyntacticArgumentRecorder> SyntacticParameterMappings { get; set; } = null!;

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

        Dictionary<int, DSemanticTypeArgumentRecorder> semanticIndexedParameterMappingsDictionary = new();
        Dictionary<string, DSemanticTypeArgumentRecorder> semanticNamedParameterMappingsDictionary = new(comparer);
        Dictionary<string, DSemanticArgumentRecorder> semanticParameterMappingsDictionary = new(comparer);

        Dictionary<int, DSyntacticTypeArgumentRecorder> syntacticIndexedTypeParameterMappingsDictionary = new();
        Dictionary<string, DSyntacticTypeArgumentRecorder> syntacticNamedTypeParameterMappingsDictionary = new(comparer);
        Dictionary<string, DSyntacticArgumentRecorder> syntacticParameterMappingsDictionary = new(comparer);

        PopulateTypeMappingsDictionaries(semanticIndexedParameterMappingsDictionary, semanticNamedParameterMappingsDictionary, syntacticIndexedTypeParameterMappingsDictionary,
            syntacticNamedTypeParameterMappingsDictionary, typeParameterMappings);

        PopulateMappingsDictionary(semanticParameterMappingsDictionary, syntacticParameterMappingsDictionary, parameterMappings);

        SemanticIndexedTypeParameterMappings = semanticIndexedParameterMappingsDictionary;
        SemanticNamedTypeParameterMappings = semanticNamedParameterMappingsDictionary;
        SemanticParameterMappings = semanticParameterMappingsDictionary;

        SyntacticIndexedTypeParameterMappings = syntacticIndexedTypeParameterMappingsDictionary;
        SyntacticNamedTypeParameterMappings = syntacticNamedTypeParameterMappingsDictionary;
        SyntacticParameterMappings = syntacticParameterMappingsDictionary;

        IsInitialized = true;
    }

    private static void PopulateTypeMappingsDictionaries(IDictionary<int, DSemanticTypeArgumentRecorder> semanticIndexedDictionary, IDictionary<string, DSemanticTypeArgumentRecorder> semanticNamedDictionary,
        IDictionary<int, DSyntacticTypeArgumentRecorder> syntacticIndexedDictionary, IDictionary<string, DSyntacticTypeArgumentRecorder> syntacticNamedDictionary, IEnumerable<(OneOf<int, string>, ITypeArgumentRecorders)> mappings)
    {
        foreach (var (parameter, recorders) in mappings)
        {
            if (recorders is null)
            {
                throw new InvalidOperationException($"A {nameof(ITypeArgumentRecorders)} in the provided collection of mappings was null.");
            }

            if (recorders.Semantic is null)
            {
                throw new InvalidOperationException($"A semantic recorder in the provided collection of mappings was null.");
            }

            if (recorders.Syntactic is null)
            {
                throw new InvalidOperationException($"A syntactic recorder in the provided collection of mappings was null.");
            }

            var dictionaryDelegate = parameter.Match<Action<ITypeArgumentRecorders>>
            (
                (index) =>
                {
                    if (index < 0)
                    {
                        throw new InvalidOperationException("The index of a parameter in the provided collection of mappings was negative.");
                    }

                    return (recorders) =>
                    {
                        semanticIndexedDictionary.Add(index, recorders.Semantic);
                        syntacticIndexedDictionary.Add(index, recorders.Syntactic);
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
                        semanticNamedDictionary.Add(name, recorders.Semantic);
                        syntacticNamedDictionary.Add(name, recorders.Syntactic);
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

    private static void PopulateMappingsDictionary(IDictionary<string, DSemanticArgumentRecorder> semanticDictionary, IDictionary<string, DSyntacticArgumentRecorder> syntacticDictionary, IEnumerable<(string, IArgumentRecorders)> mappings)
    {
        foreach (var (parameterName, recorders) in mappings)
        {
            if (parameterName is null)
            {
                throw new InvalidOperationException("The name of a parameter in the provided collection of parameter mappings was null.");
            }

            if (recorders is null)
            {
                throw new InvalidOperationException($"A {nameof(IArgumentRecorders)} in the provided collection of mappings was null.");
            }

            if (recorders.Semantic is null)
            {
                throw new InvalidOperationException($"A semantic recorder in the provided collection of mappings was null.");
            }

            if (recorders.Syntactic is null)
            {
                throw new InvalidOperationException($"A syntactic recorder in the provided collection of mappings was null.");
            }

            try
            {
                semanticDictionary.Add(parameterName, recorders.Semantic);
                syntacticDictionary.Add(parameterName, recorders.Syntactic);
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
    protected virtual IEnumerable<(OneOf<int, string> IndexOrName, ITypeArgumentRecorders Recorders)> AddTypeParameterMappings() => Enumerable.Empty<(OneOf<int, string>, ITypeArgumentRecorders)>();

    /// <summary>Maps the names of constructor or named parameters to recorders, responsible for recording the argument of the parameter.</summary>
    /// <returns>The mappings from parameter name to recorder.</returns>
    protected virtual IEnumerable<(string Name, IArgumentRecorders Recorders)> AddParameterMappings() => Enumerable.Empty<(string, IArgumentRecorders)>();

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

        return new SemanticAttributeTypeArgumentRecorder(dataRecord, mapping);
    }

    /// <inheritdoc/>
    public ISyntacticAttributeArgumentRecorder? TryMapTypeParameter(ITypeParameterSymbol parameter, TSyntacticRecord dataRecord)
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

        if (TryGetSyntacticTypeParameterMapping(parameter) is not DSyntacticTypeArgumentRecorder mapping)
        {
            return null;
        }

        return new SyntacticAttributeTypeArgumentRecorder(dataRecord, mapping);
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
    public ISyntacticAttributeConstructorArgumentRecorder? TryMapConstructorParameter(IParameterSymbol parameter, TSyntacticRecord dataRecord)
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

        if (SyntacticParameterMappings.TryGetValue(parameter.Name, out var mapping) is false)
        {
            return null;
        }

        return new SyntacticAttributeConstructorArgumentRecorder(dataRecord, mapping);
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

    /// <inheritdoc/>
    public ISyntacticAttributeArgumentRecorder? TryMapNamedParameter(string parameterName, TSyntacticRecord dataRecord)
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

        if (SyntacticParameterMappings.TryGetValue(parameterName, out var mapping) is false)
        {
            return null;
        }

        return new SyntacticAttributeArgumentRecorder(dataRecord, mapping);
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

    private DSyntacticTypeArgumentRecorder? TryGetSyntacticTypeParameterMapping(ITypeParameterSymbol parameter)
    {
        var hasIndexedMapping = SyntacticIndexedTypeParameterMappings.TryGetValue(parameter.Ordinal, out var indexedMapping);
        var hasNamedMapping = SyntacticNamedTypeParameterMappings.TryGetValue(parameter.Name, out var namedMapping);

        return (hasIndexedMapping, hasNamedMapping) switch
        {
            (false, false) => null,
            (true, false) => indexedMapping,
            (false, true) => namedMapping,
            (true, true) => attemptToResolveConflictingMappings(indexedMapping, namedMapping)
        };

        static DSyntacticTypeArgumentRecorder? attemptToResolveConflictingMappings(DSyntacticTypeArgumentRecorder indexedMapping, DSyntacticTypeArgumentRecorder namedMapping)
        {
            if (Equals(indexedMapping, namedMapping))
            {
                return indexedMapping;
            }

            return null;
        }
    }

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

    /// <summary>Responsible for recording the syntactical information about the argument of a type-parameter.</summary>
    /// <param name="dataRecord">The <typeparamref name="TSyntacticRecord"/> to which the syntactical information is recorded.</param>
    /// <param name="syntax">The syntactial information about the argument of the parameter.</param>
    /// <returns>A <see cref="bool"/> indicating whether the syntactical information was successfully recorded.</returns>
    /// <exception cref="ArgumentNullException"/>
    protected delegate bool DSyntacticTypeArgumentRecorder(TSyntacticRecord dataRecord, ExpressionSyntax syntax);

    /// <summary>Responsible for recording the syntactical information about the argument of a parameter.</summary>
    /// <param name="dataRecord">The <typeparamref name="TSyntacticRecord"/> to which the syntactical information is recorded.</param>
    /// <param name="syntax">The <see cref="ExpressionSyntax"/> syntactically describing the argument of the parameter, or each element if expressed as a <see langword="params"/>-array.</param>
    /// <returns>A <see cref="bool"/> indicating whether the syntactical information was successfully recorded.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    protected delegate bool DSyntacticArgumentRecorder(TSyntacticRecord dataRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax);

    /// <summary>Provides the recorders responsible for recording the argument of a type-parameter.</summary>
    protected interface ITypeArgumentRecorders
    {
        /// <summary>The <see cref="DSemanticTypeArgumentRecorder"/>, used when parsing semantically.</summary>
        public abstract DSemanticTypeArgumentRecorder Semantic { get; }

        /// <summary>The <see cref="DSyntacticTypeArgumentRecorder"/>, used when parsing syntactically.</summary>
        public abstract DSyntacticTypeArgumentRecorder Syntactic { get; }
    }

    /// <summary>Provides the recorders responsible for recording the argument of a parameter.</summary>
    protected interface IArgumentRecorders
    {
        /// <summary>The <see cref="DSemanticArgumentRecorder"/>, used when parsing semantically.</summary>
        public abstract DSemanticArgumentRecorder Semantic { get; }

        /// <summary>The <see cref="DSyntacticArgumentRecorder"/>, used when parsing syntactically.</summary>
        public abstract DSyntacticArgumentRecorder Syntactic { get; }
    }

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
        /// <summary>Produces a <see cref="ITypeArgumentRecorders"/> which invokes the provided recorders.</summary>
        /// <param name="semanticRecorder">Responsible for recording the argument.</param>
        /// <param name="syntacticRecorder">Responsible for recording the syntactical information about the argument.</param>
        /// <returns>The produced <see cref="ITypeArgumentRecorders"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract ITypeArgumentRecorders For(Func<TSemanticRecord, ITypeSymbol, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder);

        /// <summary>Produces a <see cref="ITypeArgumentRecorders"/> which invokes the provided recorders and returns <see langword="true"/>.</summary>
        /// <param name="semanticRecorder">Responsible for recording the argument.</param>
        /// <param name="syntacticRecorder">Responsible for recording the syntactical information about the argument.</param>
        /// <returns>The produced <see cref="ITypeArgumentRecorders"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract ITypeArgumentRecorders For(Action<TSemanticRecord, ITypeSymbol> semanticRecorder, Action<TSyntacticRecord, ExpressionSyntax> syntacticRecorder);
    }

    /// <summary>Provides adapters that may be applied to parsed attribute arguments before invoking a recorder.</summary>
    protected interface ISimpleArgumentAdapter
    {
        /// <summary>Produces a <see cref="IArgumentRecorders"/> which ensures that the argument is of type <typeparamref name="T"/> before invoking the provided recorders, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the argument.</typeparam>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/>.</param>
        /// <param name="syntacticRecorder">Responsible for recording the syntactical information about the argument.</param>
        /// <returns>The produced <see cref="IArgumentRecorders"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorders For<T>(Func<TSemanticRecord, T, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder) where T : notnull;

        /// <summary>Produces a <see cref="IArgumentRecorders"/> which ensures that the argument is of type <typeparamref name="T"/> before invoking the provided recorders and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the argument.</typeparam>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/>.</param>
        /// <param name="syntacticRecorder">Responsible for recording the syntactical information about the argument.</param>
        /// <returns>The produced <see cref="IArgumentRecorders"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorders For<T>(Action<TSemanticRecord, T> semanticRecorder, Action<TSyntacticRecord, ExpressionSyntax> syntacticRecorder) where T : notnull;

        /// <summary>Produces a <see cref="IArgumentRecorders"/> which ensures that the argument is of type <typeparamref name="T"/>, or <see langword="null"/>, before invoking the provided recorders, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the argument.</typeparam>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/> or <see langword="null"/>.</param>
        /// <param name="syntacticRecorder">Responsible for recording the syntactical information about the argument.</param>
        /// <returns>The produced <see cref="IArgumentRecorders"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorders ForNullable<T>(Func<TSemanticRecord, T?, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder) where T : class;

        /// <summary>Produces a <see cref="IArgumentRecorders"/> which ensures that the argument is of type <typeparamref name="T"/>, or <see langword="null"/>, before invoking the provided recorders and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the argument.</typeparam>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/> or <see langword="null"/>.</param>
        /// <param name="syntacticRecorder">Responsible for recording the syntactical information about the argument.</param>
        /// <returns>The produced <see cref="IArgumentRecorders"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorders ForNullable<T>(Action<TSemanticRecord, T?> semanticRecorder, Action<TSyntacticRecord, ExpressionSyntax> syntacticRecorder) where T : class;

        /// <summary>Produces a <see cref="IArgumentRecorders"/> which ensures that the argument is of type <typeparamref name="T"/>, or <see langword="null"/>, before invoking the provided recorders, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the argument.</typeparam>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/> or <see langword="null"/>.</param>
        /// <param name="syntacticRecorder">Responsible for recording the syntactical information about the argument.</param>
        /// <returns>The produced <see cref="IArgumentRecorders"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorders ForNullable<T>(Func<TSemanticRecord, T?, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder) where T : struct;

        /// <summary>Produces a <see cref="IArgumentRecorders"/> which ensures that the argument is of type <typeparamref name="T"/>, or <see langword="null"/>, before invoking the provided recorders and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the argument.</typeparam>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/> or <see langword="null"/>, when parsing purely.</param>
        /// <param name="syntacticRecorder">Responsible for recording the syntactical information about the argument.</param>
        /// <returns>The produced <see cref="IArgumentRecorders"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorders ForNullable<T>(Action<TSemanticRecord, T?> semanticRecorder, Action<TSyntacticRecord, ExpressionSyntax> syntacticRecorder) where T : struct;
    }

    /// <summary>Provides adapters that may be applied to parsed array-valued attribute arguments, which may not be expressed as <see langword="params"/>-arrays, before invoking a recorder.</summary>
    protected interface INonParamsArrayArgumentAdapter
    {
        /// <summary>Produces a <see cref="IArgumentRecorders"/> which ensures that the argument is an array with elements of type <typeparamref name="T"/> before invoking the provided recorders, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <param name="syntacticRecorder">Responsible for recording the syntactical information about the argument.</param>
        /// <returns>The produced <see cref="IArgumentRecorders"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorders For<T>(Func<TSemanticRecord, IReadOnlyList<T>, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder) where T : notnull;

        /// <summary>Produces a <see cref="IArgumentRecorders"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before invoking the provided recorders and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <param name="syntacticRecorder">Responsible for recording the syntactical information about the argument.</param>
        /// <returns>The produced <see cref="IArgumentRecorders"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorders For<T>(Action<TSemanticRecord, IReadOnlyList<T>> semanticRecorder, Action<TSyntacticRecord, ExpressionSyntax> syntacticRecorder) where T : notnull;

        /// <summary>Produces a <see cref="IArgumentRecorders"/> which ensures that the argument is either <see langword="null"/> or a collection with elements of type <typeparamref name="T"/> before invoking the provided recorders, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <param name="syntacticRecorder">Responsible for recording the syntactical information about the argument.</param>
        /// <returns>The produced <see cref="IArgumentRecorders"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorders ForNullableCollection<T>(Func<TSemanticRecord, IReadOnlyList<T>?, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder) where T : notnull;

        /// <summary>Produces a <see cref="IArgumentRecorders"/> which ensures that the argument is either <see langword="null"/> or a collection with elements of type <typeparamref name="T"/> before invoking the provided recorders and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <param name="syntacticRecorder">Responsible for recording the syntactical information about the argument.</param>
        /// <returns>The produced <see cref="IArgumentRecorders"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorders ForNullableCollection<T>(Action<TSemanticRecord, IReadOnlyList<T>?> semanticRecorder, Action<TSyntacticRecord, ExpressionSyntax> syntacticRecorder) where T : notnull;

        /// <summary>Produces a <see cref="IArgumentRecorders"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where both elements and the collection itself may be <see langword="null"/>, before invoking the provided recorders, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <param name="syntacticRecorder">Responsible for recording the syntactical information about the argument.</param>
        /// <returns>The produced <see cref="IArgumentRecorders"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorders ForNullable<T>(Func<TSemanticRecord, IReadOnlyList<T?>?, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder) where T : class;

        /// <summary>Produces a <see cref="IArgumentRecorders"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where both elements and the collection itself may be <see langword="null"/>, before invoking the provided recorders and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <param name="syntacticRecorder">Responsible for recording the syntactical information about the argument.</param>
        /// <returns>The produced <see cref="IArgumentRecorders"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorders ForNullable<T>(Action<TSemanticRecord, IReadOnlyList<T?>?> semanticRecorder, Action<TSyntacticRecord, ExpressionSyntax> syntacticRecorder) where T : class;

        /// <summary>Produces a <see cref="IArgumentRecorders"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where both elements and the collection itself may be <see langword="null"/>, before invoking the provided recorders, and which otherwise returns <see langword="false"/>.</summary>s
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <param name="syntacticRecorder">Responsible for recording the syntactical information about the argument.</param>
        /// <returns>The produced <see cref="IArgumentRecorders"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorders ForNullable<T>(Func<TSemanticRecord, IReadOnlyList<T?>?, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder) where T : struct;

        /// <summary>Produces a <see cref="IArgumentRecorders"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where both elements and the collection itself may be <see langword="null"/>, before invoking the provided recorders and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <param name="syntacticRecorder">Responsible for recording the syntactical information about the argument.</param>
        /// <returns>The produced <see cref="IArgumentRecorders"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorders ForNullable<T>(Action<TSemanticRecord, IReadOnlyList<T?>?> semanticRecorder, Action<TSyntacticRecord, ExpressionSyntax> syntacticRecorder) where T : struct;

        /// <summary>Produces a <see cref="IArgumentRecorders"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where the elements may be <see langword="null"/>, before invoking the provided recorders, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <param name="syntacticRecorder">Responsible for recording the syntactical information about the argument.</param>
        /// <returns>The produced <see cref="IArgumentRecorders"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorders ForNullableElements<T>(Func<TSemanticRecord, IReadOnlyList<T?>, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder) where T : class;

        /// <summary>Produces a <see cref="IArgumentRecorders"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where the elements may be <see langword="null"/>, before invoking the provided recorders and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <param name="syntacticRecorder">Responsible for recording the syntactical information about the argument.</param>
        /// <returns>The produced <see cref="IArgumentRecorders"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorders ForNullableElements<T>(Action<TSemanticRecord, IReadOnlyList<T?>> semanticRecorder, Action<TSyntacticRecord, ExpressionSyntax> syntacticRecorder) where T : class;

        /// <summary>Produces a <see cref="IArgumentRecorders"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where the elements may be <see langword="null"/>, before invoking the provided recorders, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <param name="syntacticRecorder">Responsible for recording the syntactical information about the argument.</param>
        /// <returns>The produced <see cref="IArgumentRecorders"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorders ForNullableElements<T>(Func<TSemanticRecord, IReadOnlyList<T?>, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder) where T : struct;

        /// <summary>Produces a <see cref="IArgumentRecorders"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where the elements may be <see langword="null"/>, before invoking the provided recorders and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <param name="syntacticRecorder">Responsible for recording the syntactical information about the argument.</param>
        /// <returns>The produced <see cref="IArgumentRecorders"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorders ForNullableElements<T>(Action<TSemanticRecord, IReadOnlyList<T?>> semanticRecorder, Action<TSyntacticRecord, ExpressionSyntax> syntacticRecorder) where T : struct;
    }

    /// <summary>Provides adapters that may be applied to parsed array-valued attribute arguments, which may be expressed as <see langword="params"/>-arrays, before invoking a recorder.</summary>
    protected interface IParamsArrayArgumentAdapter
    {
        /// <summary>Produces a <see cref="IArgumentRecorders"/> which ensures that the argument is an array with elements of type <typeparamref name="T"/> before invoking the provided recorders, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <param name="syntacticRecorder">Responsible for recording the syntactical information about the argument.</param>
        /// <returns>The produced <see cref="IArgumentRecorders"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorders For<T>(Func<TSemanticRecord, IReadOnlyList<T>, bool> semanticRecorder, Func<TSyntacticRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> syntacticRecorder) where T : notnull;

        /// <summary>Produces a <see cref="IArgumentRecorders"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before invoking the provided recorders and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <param name="syntacticRecorder">Responsible for recording the syntactical information about the argument.</param>
        /// <returns>The produced <see cref="IArgumentRecorders"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorders For<T>(Action<TSemanticRecord, IReadOnlyList<T>> semanticRecorder, Action<TSyntacticRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> syntacticRecorder) where T : notnull;

        /// <summary>Produces a <see cref="IArgumentRecorders"/> which ensures that the argument is either <see langword="null"/> or a collection with elements of type <typeparamref name="T"/> before invoking the provided recorders, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <param name="syntacticRecorder">Responsible for recording the syntactical information about the argument.</param>
        /// <returns>The produced <see cref="IArgumentRecorders"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorders ForNullableCollection<T>(Func<TSemanticRecord, IReadOnlyList<T>?, bool> semanticRecorder, Func<TSyntacticRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> syntacticRecorder) where T : notnull;

        /// <summary>Produces a <see cref="IArgumentRecorders"/> which ensures that the argument is either <see langword="null"/> or a collection with elements of type <typeparamref name="T"/> before invoking the provided recorders and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <param name="syntacticRecorder">Responsible for recording the syntactical information about the argument.</param>
        /// <returns>The produced <see cref="IArgumentRecorders"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorders ForNullableCollection<T>(Action<TSemanticRecord, IReadOnlyList<T>?> semanticRecorder, Action<TSyntacticRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> syntacticRecorder) where T : notnull;

        /// <summary>Produces a <see cref="IArgumentRecorders"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where both elements and the collection itself may be <see langword="null"/>, before invoking the provided recorders, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <param name="syntacticRecorder">Responsible for recording the syntactical information about the argument.</param>
        /// <returns>The produced <see cref="IArgumentRecorders"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorders ForNullable<T>(Func<TSemanticRecord, IReadOnlyList<T?>?, bool> semanticRecorder, Func<TSyntacticRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> syntacticRecorder) where T : class;

        /// <summary>Produces a <see cref="IArgumentRecorders"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where both elements and the collection itself may be <see langword="null"/>, before invoking the provided recorders and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <param name="syntacticRecorder">Responsible for recording the syntactical information about the argument.</param>
        /// <returns>The produced <see cref="IArgumentRecorders"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorders ForNullable<T>(Action<TSemanticRecord, IReadOnlyList<T?>?> semanticRecorder, Action<TSyntacticRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> syntacticRecorder) where T : class;

        /// <summary>Produces a <see cref="IArgumentRecorders"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where both elements and the collection itself may be <see langword="null"/>, before invoking the provided recorders, and which otherwise returns <see langword="false"/>.</summary>s
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <param name="syntacticRecorder">Responsible for recording the syntactical information about the argument.</param>
        /// <returns>The produced <see cref="IArgumentRecorders"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorders ForNullable<T>(Func<TSemanticRecord, IReadOnlyList<T?>?, bool> semanticRecorder, Func<TSyntacticRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> syntacticRecorder) where T : struct;

        /// <summary>Produces a <see cref="IArgumentRecorders"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where both elements and the collection itself may be <see langword="null"/>, before invoking the provided recorders and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <param name="syntacticRecorder">Responsible for recording the syntactical information about the argument.</param>
        /// <returns>The produced <see cref="IArgumentRecorders"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorders ForNullable<T>(Action<TSemanticRecord, IReadOnlyList<T?>?> semanticRecorder, Action<TSyntacticRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> syntacticRecorder) where T : struct;

        /// <summary>Produces a <see cref="IArgumentRecorders"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where the elements may be <see langword="null"/>, before invoking the provided recorders, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <param name="syntacticRecorder">Responsible for recording the syntactical information about the argument.</param>
        /// <returns>The produced <see cref="IArgumentRecorders"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorders ForNullableElements<T>(Func<TSemanticRecord, IReadOnlyList<T?>, bool> semanticRecorder, Func<TSyntacticRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> syntacticRecorder) where T : class;

        /// <summary>Produces a <see cref="IArgumentRecorders"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where the elements may be <see langword="null"/>, before invoking the provided recorders and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <param name="syntacticRecorder">Responsible for recording the syntactical information about the argument.</param>
        /// <returns>The produced <see cref="IArgumentRecorders"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorders ForNullableElements<T>(Action<TSemanticRecord, IReadOnlyList<T?>> semanticRecorder, Action<TSyntacticRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> syntacticRecorder) where T : class;

        /// <summary>Produces a <see cref="IArgumentRecorders"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where the elements may be <see langword="null"/>, before invoking the provided recorders, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <param name="syntacticRecorder">Responsible for recording the syntactical information about the argument.</param>
        /// <returns>The produced <see cref="IArgumentRecorders"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorders ForNullableElements<T>(Func<TSemanticRecord, IReadOnlyList<T?>, bool> semanticRecorder, Func<TSyntacticRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> syntacticRecorder) where T : struct;

        /// <summary>Produces a <see cref="IArgumentRecorders"/> which ensures that the argument is a collection with elements of type <typeparamref name="T"/>, where the elements may be <see langword="null"/>, before invoking the provided recorders and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
        /// <param name="semanticRecorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
        /// <param name="syntacticRecorder">Responsible for recording the syntactical information about the argument.</param>
        /// <returns>The produced <see cref="IArgumentRecorders"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract IArgumentRecorders ForNullableElements<T>(Action<TSemanticRecord, IReadOnlyList<T?>> semanticRecorder, Action<TSyntacticRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> syntacticRecorder) where T : struct;
    }

    private static class SyntacticAdapterUtility
    {
        public static DSyntacticArgumentRecorder ForNonParams(Func<TSyntacticRecord, ExpressionSyntax, bool> recorder)
        {
            return wrapper;

            bool wrapper(TSyntacticRecord dataRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
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

                return recorder(dataRecord, syntax.AsT0);
            }
        }

        public static DSyntacticArgumentRecorder ForParams(Func<TSyntacticRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> recorder)
        {
            return wrapper;

            bool wrapper(TSyntacticRecord dataRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
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

                return recorder(dataRecord, syntax);
            }
        }
    }

    private sealed class SemanticAttributeTypeArgumentRecorder : ISemanticAttributeArgumentRecorder
    {
        private TSemanticRecord DataRecord { get; }
        private DSemanticTypeArgumentRecorder Recorder { get; }

        public SemanticAttributeTypeArgumentRecorder(TSemanticRecord dataRecord, DSemanticTypeArgumentRecorder recorder)
        {
            DataRecord = dataRecord;
            Recorder = recorder;
        }

        bool ISemanticAttributeArgumentRecorder.RecordArgument(object? argument)
        {
            if (argument is not ITypeSymbol typeArgument)
            {
                return false;
            }

            return Recorder(DataRecord, typeArgument);
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

    private sealed class SyntacticAttributeTypeArgumentRecorder : ISyntacticAttributeArgumentRecorder
    {
        private TSyntacticRecord DataRecord { get; }
        private DSyntacticTypeArgumentRecorder Recorder { get; }

        public SyntacticAttributeTypeArgumentRecorder(TSyntacticRecord dataRecord, DSyntacticTypeArgumentRecorder recorder)
        {
            DataRecord = dataRecord;
            Recorder = recorder;
        }

        bool ISyntacticAttributeArgumentRecorder.RecordArgumentSyntax(ExpressionSyntax syntax)
        {
            if (syntax is null)
            {
                throw new ArgumentNullException(nameof(syntax));
            }

            return Recorder(DataRecord, syntax);
        }
    }

    private sealed class SyntacticAttributeArgumentRecorder : ISyntacticAttributeArgumentRecorder
    {
        private TSyntacticRecord DataRecord { get; }
        private DSyntacticArgumentRecorder Recorder { get; }

        public SyntacticAttributeArgumentRecorder(TSyntacticRecord dataRecord, DSyntacticArgumentRecorder recorder)
        {
            DataRecord = dataRecord;
            Recorder = recorder;
        }

        bool ISyntacticAttributeArgumentRecorder.RecordArgumentSyntax(ExpressionSyntax syntax)
        {
            if (syntax is null)
            {
                throw new ArgumentNullException(nameof(syntax));
            }

            return Recorder(DataRecord, syntax);
        }
    }

    private sealed class SyntacticAttributeConstructorArgumentRecorder : ISyntacticAttributeConstructorArgumentRecorder
    {
        private TSyntacticRecord DataRecord { get; }
        private DSyntacticArgumentRecorder Recorder { get; }

        public SyntacticAttributeConstructorArgumentRecorder(TSyntacticRecord dataRecord, DSyntacticArgumentRecorder recorder)
        {
            DataRecord = dataRecord;
            Recorder = recorder;
        }

        bool ISyntacticAttributeArgumentRecorder.RecordArgumentSyntax(ExpressionSyntax syntax)
        {
            if (syntax is null)
            {
                throw new ArgumentNullException(nameof(syntax));
            }

            return Recorder(DataRecord, syntax);
        }

        bool ISyntacticAttributeConstructorArgumentRecorder.RecordParamsArgumentSyntax(IReadOnlyList<ExpressionSyntax> elementSyntax)
        {
            if (elementSyntax is null)
            {
                throw new ArgumentNullException(nameof(elementSyntax));
            }

            return Recorder(DataRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(elementSyntax));
        }
    }

    private sealed class TypeArgumentRecorders : ITypeArgumentRecorders
    {
        private DSemanticTypeArgumentRecorder Semantic { get; }
        private DSyntacticTypeArgumentRecorder Syntactic { get; }

        public TypeArgumentRecorders(DSemanticTypeArgumentRecorder semantic, DSyntacticTypeArgumentRecorder syntactic)
        {
            Semantic = semantic;
            Syntactic = syntactic;
        }

        DSemanticTypeArgumentRecorder ITypeArgumentRecorders.Semantic => Semantic;
        DSyntacticTypeArgumentRecorder ITypeArgumentRecorders.Syntactic => Syntactic;
    }

    private sealed class ArgumentRecorders : IArgumentRecorders
    {
        private DSemanticArgumentRecorder Semantic { get; }
        private DSyntacticArgumentRecorder Syntactic { get; }

        public ArgumentRecorders(DSemanticArgumentRecorder semantic, DSyntacticArgumentRecorder syntactic)
        {
            Semantic = semantic;
            Syntactic = syntactic;
        }

        DSemanticArgumentRecorder IArgumentRecorders.Semantic => Semantic;
        DSyntacticArgumentRecorder IArgumentRecorders.Syntactic => Syntactic;
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
        ITypeArgumentRecorders ITypeArgumentAdapter.For(Func<TSemanticRecord, ITypeSymbol, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder)
        {
            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            if (syntacticRecorder is null)
            {
                throw new ArgumentNullException(nameof(syntacticRecorder));
            }

            return For(semanticRecorder, syntacticRecorder);
        }

        ITypeArgumentRecorders ITypeArgumentAdapter.For(Action<TSemanticRecord, ITypeSymbol> semanticRecorder, Action<TSyntacticRecord, ExpressionSyntax> syntacticRecorder)
        {
            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            if (syntacticRecorder is null)
            {
                throw new ArgumentNullException(nameof(syntacticRecorder));
            }

            return For(semanticWrapper, syntacticWrapper);

            bool semanticWrapper(TSemanticRecord dataRecord, ITypeSymbol argument)
            {
                semanticRecorder(dataRecord, argument);

                return true;
            }

            bool syntacticWrapper(TSyntacticRecord dataRecord, ExpressionSyntax syntax)
            {
                syntacticRecorder(dataRecord, syntax);

                return true;
            }
        }

        private static ITypeArgumentRecorders For(Func<TSemanticRecord, ITypeSymbol, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder)
        {
            return new TypeArgumentRecorders(semanticWrapper, syntacticWrapper);

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

            bool syntacticWrapper(TSyntacticRecord dataRecord, ExpressionSyntax syntax)
            {
                if (dataRecord is null)
                {
                    throw new ArgumentNullException(nameof(dataRecord));
                }

                if (syntax is null)
                {
                    throw new ArgumentNullException(nameof(syntax));
                }

                return syntacticRecorder(dataRecord, syntax);
            }
        }
    }

    private sealed class SimpleArgumentAdapter : ISimpleArgumentAdapter
    {
        IArgumentRecorders ISimpleArgumentAdapter.For<T>(Func<TSemanticRecord, T, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder)
        {
            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            if (syntacticRecorder is null)
            {
                throw new ArgumentNullException(nameof(syntacticRecorder));
            }

            return For(semanticRecorder, syntacticRecorder);
        }

        IArgumentRecorders ISimpleArgumentAdapter.For<T>(Action<TSemanticRecord, T> semanticRecorder, Action<TSyntacticRecord, ExpressionSyntax> syntacticRecorder)
        {
            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            if (syntacticRecorder is null)
            {
                throw new ArgumentNullException(nameof(syntacticRecorder));
            }

            return For<T>(semanticWrapper, syntacticWrapper);

            bool semanticWrapper(TSemanticRecord dataRecord, T argument)
            {
                semanticRecorder(dataRecord, argument);

                return true;
            }

            bool syntacticWrapper(TSyntacticRecord dataRecord, ExpressionSyntax syntax)
            {
                syntacticRecorder(dataRecord, syntax);

                return true;
            }
        }

        IArgumentRecorders ISimpleArgumentAdapter.ForNullable<T>(Func<TSemanticRecord, T?, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder) where T : class
        {
            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            if (syntacticRecorder is null)
            {
                throw new ArgumentNullException(nameof(syntacticRecorder));
            }

            return ForNullable(semanticRecorder, syntacticRecorder);
        }

        IArgumentRecorders ISimpleArgumentAdapter.ForNullable<T>(Action<TSemanticRecord, T?> semanticRecorder, Action<TSyntacticRecord, ExpressionSyntax> syntacticRecorder) where T : class
        {
            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            if (syntacticRecorder is null)
            {
                throw new ArgumentNullException(nameof(syntacticRecorder));
            }

            return ForNullable<T>(semanticWrapper, syntacticWrapper);

            bool semanticWrapper(TSemanticRecord dataRecord, T? argument)
            {
                semanticRecorder(dataRecord, argument);

                return true;
            }

            bool syntacticWrapper(TSyntacticRecord dataRecord, ExpressionSyntax syntax)
            {
                syntacticRecorder(dataRecord, syntax);

                return true;
            }
        }

        IArgumentRecorders ISimpleArgumentAdapter.ForNullable<T>(Func<TSemanticRecord, T?, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder) where T : struct
        {
            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            if (syntacticRecorder is null)
            {
                throw new ArgumentNullException(nameof(syntacticRecorder));
            }

            return ForNullable(semanticRecorder, syntacticRecorder);
        }

        IArgumentRecorders ISimpleArgumentAdapter.ForNullable<T>(Action<TSemanticRecord, T?> semanticRecorder, Action<TSyntacticRecord, ExpressionSyntax> syntacticRecorder) where T : struct
        {
            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            if (syntacticRecorder is null)
            {
                throw new ArgumentNullException(nameof(syntacticRecorder));
            }

            return ForNullable<T>(semanticWrapper, syntacticWrapper);

            bool semanticWrapper(TSemanticRecord dataRecord, T? argument)
            {
                semanticRecorder(dataRecord, argument);

                return true;
            }

            bool syntacticWrapper(TSyntacticRecord dataRecord, ExpressionSyntax syntax)
            {
                syntacticRecorder(dataRecord, syntax);

                return true;
            }
        }

        private static IArgumentRecorders For<T>(Func<TSemanticRecord, T, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder)
        {
            return new ArgumentRecorders(semanticWrapper, SyntacticAdapterUtility.ForNonParams(syntacticRecorder));

            bool semanticWrapper(TSemanticRecord dataRecord, object? argument)
            {
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

        private static IArgumentRecorders ForNullable<T>(Func<TSemanticRecord, T?, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder) where T : class
        {
            return new ArgumentRecorders(semanticWrapper, SyntacticAdapterUtility.ForNonParams(syntacticRecorder));

            bool semanticWrapper(TSemanticRecord dataRecord, object? argument)
            {
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

        private static IArgumentRecorders ForNullable<T>(Func<TSemanticRecord, T?, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder) where T : struct
        {
            return new ArgumentRecorders(semanticWrapper, SyntacticAdapterUtility.ForNonParams(syntacticRecorder));

            bool semanticWrapper(TSemanticRecord dataRecord, object? argument)
            {
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
        IArgumentRecorders INonParamsArrayArgumentAdapter.For<T>(Func<TSemanticRecord, IReadOnlyList<T>, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder)
        {
            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            if (syntacticRecorder is null)
            {
                throw new ArgumentNullException(nameof(syntacticRecorder));
            }

            return For(semanticRecorder, syntacticRecorder);
        }

        IArgumentRecorders INonParamsArrayArgumentAdapter.For<T>(Action<TSemanticRecord, IReadOnlyList<T>> semanticRecorder, Action<TSyntacticRecord, ExpressionSyntax> syntacticRecorder)
        {
            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            if (syntacticRecorder is null)
            {
                throw new ArgumentNullException(nameof(syntacticRecorder));
            }

            return For<T>(semanticWrapper, syntacticWrapper);

            bool semanticWrapper(TSemanticRecord dataRecord, IReadOnlyList<T> argument)
            {
                semanticRecorder(dataRecord, argument);

                return true;
            }

            bool syntacticWrapper(TSyntacticRecord dataRecord, ExpressionSyntax syntax)
            {
                syntacticRecorder(dataRecord, syntax);

                return true;
            }
        }

        IArgumentRecorders INonParamsArrayArgumentAdapter.ForNullable<T>(Func<TSemanticRecord, IReadOnlyList<T?>?, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder) where T : class
        {
            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            if (syntacticRecorder is null)
            {
                throw new ArgumentNullException(nameof(syntacticRecorder));
            }

            return ForNullable(semanticRecorder, syntacticRecorder);
        }

        IArgumentRecorders INonParamsArrayArgumentAdapter.ForNullable<T>(Action<TSemanticRecord, IReadOnlyList<T?>?> semanticRecorder, Action<TSyntacticRecord, ExpressionSyntax> syntacticRecorder) where T : class
        {
            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            if (syntacticRecorder is null)
            {
                throw new ArgumentNullException(nameof(syntacticRecorder));
            }

            return ForNullable<T>(semanticWrapper, syntacticWrapper);

            bool semanticWrapper(TSemanticRecord dataRecord, IReadOnlyList<T?>? argument)
            {
                semanticRecorder(dataRecord, argument);

                return true;
            }

            bool syntacticWrapper(TSyntacticRecord dataRecord, ExpressionSyntax syntax)
            {
                syntacticRecorder(dataRecord, syntax);

                return true;
            }
        }

        IArgumentRecorders INonParamsArrayArgumentAdapter.ForNullable<T>(Func<TSemanticRecord, IReadOnlyList<T?>?, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder)
        {
            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            if (syntacticRecorder is null)
            {
                throw new ArgumentNullException(nameof(syntacticRecorder));
            }

            return ForNullable(semanticRecorder, syntacticRecorder);
        }

        IArgumentRecorders INonParamsArrayArgumentAdapter.ForNullable<T>(Action<TSemanticRecord, IReadOnlyList<T?>?> semanticRecorder, Action<TSyntacticRecord, ExpressionSyntax> syntacticRecorder)
        {
            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            if (syntacticRecorder is null)
            {
                throw new ArgumentNullException(nameof(syntacticRecorder));
            }

            return ForNullable<T>(semanticWrapper, syntacticWrapper);

            bool semanticWrapper(TSemanticRecord dataRecord, IReadOnlyList<T?>? argument)
            {
                semanticRecorder(dataRecord, argument);

                return true;
            }

            bool syntacticWrapper(TSyntacticRecord dataRecord, ExpressionSyntax syntax)
            {
                syntacticRecorder(dataRecord, syntax);

                return true;
            }
        }

        IArgumentRecorders INonParamsArrayArgumentAdapter.ForNullableCollection<T>(Func<TSemanticRecord, IReadOnlyList<T>?, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder)
        {
            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            if (syntacticRecorder is null)
            {
                throw new ArgumentNullException(nameof(syntacticRecorder));
            }

            return ForNullableCollection(semanticRecorder, syntacticRecorder);
        }

        IArgumentRecorders INonParamsArrayArgumentAdapter.ForNullableCollection<T>(Action<TSemanticRecord, IReadOnlyList<T>?> semanticRecorder, Action<TSyntacticRecord, ExpressionSyntax> syntacticRecorder)
        {
            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            if (syntacticRecorder is null)
            {
                throw new ArgumentNullException(nameof(syntacticRecorder));
            }

            return ForNullableCollection<T>(semanticWrapper, syntacticWrapper);

            bool semanticWrapper(TSemanticRecord dataRecord, IReadOnlyList<T>? argument)
            {
                semanticRecorder(dataRecord, argument);

                return true;
            }

            bool syntacticWrapper(TSyntacticRecord dataRecord, ExpressionSyntax syntax)
            {
                syntacticRecorder(dataRecord, syntax);

                return true;
            }
        }

        IArgumentRecorders INonParamsArrayArgumentAdapter.ForNullableElements<T>(Func<TSemanticRecord, IReadOnlyList<T?>, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder) where T : class
        {
            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            if (syntacticRecorder is null)
            {
                throw new ArgumentNullException(nameof(syntacticRecorder));
            }

            return ForNullableElements(semanticRecorder, syntacticRecorder);
        }

        IArgumentRecorders INonParamsArrayArgumentAdapter.ForNullableElements<T>(Action<TSemanticRecord, IReadOnlyList<T?>> semanticRecorder, Action<TSyntacticRecord, ExpressionSyntax> syntacticRecorder) where T : class
        {
            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            if (syntacticRecorder is null)
            {
                throw new ArgumentNullException(nameof(syntacticRecorder));
            }

            return ForNullableElements<T>(semanticWrapper, syntacticWrapper);

            bool semanticWrapper(TSemanticRecord dataRecord, IReadOnlyList<T?> argument)
            {
                semanticRecorder(dataRecord, argument);

                return true;
            }

            bool syntacticWrapper(TSyntacticRecord dataRecord, ExpressionSyntax syntax)
            {
                syntacticRecorder(dataRecord, syntax);

                return true;
            }
        }

        IArgumentRecorders INonParamsArrayArgumentAdapter.ForNullableElements<T>(Func<TSemanticRecord, IReadOnlyList<T?>, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder)
        {
            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            if (syntacticRecorder is null)
            {
                throw new ArgumentNullException(nameof(syntacticRecorder));
            }

            return ForNullableElements(semanticRecorder, syntacticRecorder);
        }

        IArgumentRecorders INonParamsArrayArgumentAdapter.ForNullableElements<T>(Action<TSemanticRecord, IReadOnlyList<T?>> semanticRecorder, Action<TSyntacticRecord, ExpressionSyntax> syntacticRecorder)
        {
            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            if (syntacticRecorder is null)
            {
                throw new ArgumentNullException(nameof(syntacticRecorder));
            }

            return ForNullableElements<T>(semanticWrapper, syntacticWrapper);

            bool semanticWrapper(TSemanticRecord dataRecord, IReadOnlyList<T?> argument)
            {
                semanticRecorder(dataRecord, argument);

                return true;
            }

            bool syntacticWrapper(TSyntacticRecord dataRecord, ExpressionSyntax syntax)
            {
                syntacticRecorder(dataRecord, syntax);

                return true;
            }
        }

        private static IArgumentRecorders For<T>(Func<TSemanticRecord, IReadOnlyList<T>, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder)
        {
            return new ArgumentRecorders(semanticWrapper, SyntacticAdapterUtility.ForNonParams(syntacticRecorder));

            bool semanticWrapper(TSemanticRecord dataRecord, object? argument) => CommonArrayConverters.NonNullable<T>(argument).Match
            (
                static (error) => false,
                (converted) => semanticRecorder(dataRecord, converted)
            );
        }

        private static IArgumentRecorders ForNullable<T>(Func<TSemanticRecord, IReadOnlyList<T?>?, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder) where T : class
        {
            return new ArgumentRecorders(semanticWrapper, SyntacticAdapterUtility.ForNonParams(syntacticRecorder));

            bool semanticWrapper(TSemanticRecord dataRecord, object? argument) => CommonArrayConverters.Nullable<T>(argument).Match
            (
                static (error) => false,
                (converted) => semanticRecorder(dataRecord, converted)
            );
        }

        private static IArgumentRecorders ForNullable<T>(Func<TSemanticRecord, IReadOnlyList<T?>?, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder) where T : struct
        {
            return new ArgumentRecorders(semanticWrapper, SyntacticAdapterUtility.ForNonParams(syntacticRecorder));

            bool semanticWrapper(TSemanticRecord dataRecord, object? argument) => CommonArrayConverters.Nullable<T?>(argument).Match
            (
                static (error) => false,
                (converted) => semanticRecorder(dataRecord, converted)
            );
        }

        private static IArgumentRecorders ForNullableCollection<T>(Func<TSemanticRecord, IReadOnlyList<T>?, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder)
        {
            return new ArgumentRecorders(semanticWrapper, SyntacticAdapterUtility.ForNonParams(syntacticRecorder));

            bool semanticWrapper(TSemanticRecord dataRecord, object? argument) => CommonArrayConverters.NullableCollection<T>(argument).Match
            (
                static (error) => false,
                (converted) => semanticRecorder(dataRecord, converted)
            );
        }

        private static IArgumentRecorders ForNullableElements<T>(Func<TSemanticRecord, IReadOnlyList<T?>, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder) where T : class
        {
            return new ArgumentRecorders(semanticWrapper, SyntacticAdapterUtility.ForNonParams(syntacticRecorder));

            bool semanticWrapper(TSemanticRecord dataRecord, object? argument) => CommonArrayConverters.NullableElements<T?>(argument).Match
            (
                static (error) => false,
                (converted) => semanticRecorder(dataRecord, converted)
            );
        }

        private static IArgumentRecorders ForNullableElements<T>(Func<TSemanticRecord, IReadOnlyList<T?>, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder) where T : struct
        {
            return new ArgumentRecorders(semanticWrapper, SyntacticAdapterUtility.ForNonParams(syntacticRecorder));

            bool semanticWrapper(TSemanticRecord dataRecord, object? argument) => CommonArrayConverters.NullableElements<T?>(argument).Match
            (
                static (error) => false,
                (converted) => semanticRecorder(dataRecord, converted)
            );
        }
    }

    private sealed class ParamsArrayArgumentAdapter : IParamsArrayArgumentAdapter
    {
        IArgumentRecorders IParamsArrayArgumentAdapter.For<T>(Func<TSemanticRecord, IReadOnlyList<T>, bool> semanticRecorder, Func<TSyntacticRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> syntacticRecorder)
        {
            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            if (syntacticRecorder is null)
            {
                throw new ArgumentNullException(nameof(syntacticRecorder));
            }

            return For(semanticRecorder, syntacticRecorder);
        }

        IArgumentRecorders IParamsArrayArgumentAdapter.For<T>(Action<TSemanticRecord, IReadOnlyList<T>> semanticRecorder, Action<TSyntacticRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> syntacticRecorder)
        {
            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            if (syntacticRecorder is null)
            {
                throw new ArgumentNullException(nameof(syntacticRecorder));
            }

            return For<T>(semanticWrapper, syntacticWrapper);

            bool semanticWrapper(TSemanticRecord dataRecord, IReadOnlyList<T> argument)
            {
                semanticRecorder(dataRecord, argument);

                return true;
            }

            bool syntacticWrapper(TSyntacticRecord dataRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
            {
                syntacticRecorder(dataRecord, syntax);

                return true;
            }
        }

        IArgumentRecorders IParamsArrayArgumentAdapter.ForNullable<T>(Func<TSemanticRecord, IReadOnlyList<T?>?, bool> semanticRecorder, Func<TSyntacticRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> syntacticRecorder) where T : class
        {
            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            if (syntacticRecorder is null)
            {
                throw new ArgumentNullException(nameof(syntacticRecorder));
            }

            return ForNullable(semanticRecorder, syntacticRecorder);
        }

        IArgumentRecorders IParamsArrayArgumentAdapter.ForNullable<T>(Action<TSemanticRecord, IReadOnlyList<T?>?> semanticRecorder, Action<TSyntacticRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> syntacticRecorder) where T : class
        {
            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            if (syntacticRecorder is null)
            {
                throw new ArgumentNullException(nameof(syntacticRecorder));
            }

            return ForNullable<T>(semanticWrapper, syntacticWrapper);

            bool semanticWrapper(TSemanticRecord dataRecord, IReadOnlyList<T?>? argument)
            {
                semanticRecorder(dataRecord, argument);

                return true;
            }

            bool syntacticWrapper(TSyntacticRecord dataRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
            {
                syntacticRecorder(dataRecord, syntax);

                return true;
            }
        }

        IArgumentRecorders IParamsArrayArgumentAdapter.ForNullable<T>(Func<TSemanticRecord, IReadOnlyList<T?>?, bool> semanticRecorder, Func<TSyntacticRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> syntacticRecorder)
        {
            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            if (syntacticRecorder is null)
            {
                throw new ArgumentNullException(nameof(syntacticRecorder));
            }

            return ForNullable(semanticRecorder, syntacticRecorder);
        }

        IArgumentRecorders IParamsArrayArgumentAdapter.ForNullable<T>(Action<TSemanticRecord, IReadOnlyList<T?>?> semanticRecorder, Action<TSyntacticRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> syntacticRecorder)
        {
            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            if (syntacticRecorder is null)
            {
                throw new ArgumentNullException(nameof(syntacticRecorder));
            }

            return ForNullable<T>(semanticWrapper, syntacticWrapper);

            bool semanticWrapper(TSemanticRecord dataRecord, IReadOnlyList<T?>? argument)
            {
                semanticRecorder(dataRecord, argument);

                return true;
            }

            bool syntacticWrapper(TSyntacticRecord dataRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
            {
                syntacticRecorder(dataRecord, syntax);

                return true;
            }
        }

        IArgumentRecorders IParamsArrayArgumentAdapter.ForNullableCollection<T>(Func<TSemanticRecord, IReadOnlyList<T>?, bool> semanticRecorder, Func<TSyntacticRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> syntacticRecorder)
        {
            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            if (syntacticRecorder is null)
            {
                throw new ArgumentNullException(nameof(syntacticRecorder));
            }

            return ForNullableCollection(semanticRecorder, syntacticRecorder);
        }

        IArgumentRecorders IParamsArrayArgumentAdapter.ForNullableCollection<T>(Action<TSemanticRecord, IReadOnlyList<T>?> semanticRecorder, Action<TSyntacticRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> syntacticRecorder)
        {
            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            if (syntacticRecorder is null)
            {
                throw new ArgumentNullException(nameof(syntacticRecorder));
            }

            return ForNullableCollection<T>(semanticWrapper, syntacticWrapper);

            bool semanticWrapper(TSemanticRecord dataRecord, IReadOnlyList<T>? argument)
            {
                semanticRecorder(dataRecord, argument);

                return true;
            }

            bool syntacticWrapper(TSyntacticRecord dataRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
            {
                syntacticRecorder(dataRecord, syntax);

                return true;
            }
        }

        IArgumentRecorders IParamsArrayArgumentAdapter.ForNullableElements<T>(Func<TSemanticRecord, IReadOnlyList<T?>, bool> semanticRecorder, Func<TSyntacticRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> syntacticRecorder) where T : class
        {
            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            if (syntacticRecorder is null)
            {
                throw new ArgumentNullException(nameof(syntacticRecorder));
            }

            return ForNullableElements(semanticRecorder, syntacticRecorder);
        }

        IArgumentRecorders IParamsArrayArgumentAdapter.ForNullableElements<T>(Action<TSemanticRecord, IReadOnlyList<T?>> semanticRecorder, Action<TSyntacticRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> syntacticRecorder) where T : class
        {
            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            if (syntacticRecorder is null)
            {
                throw new ArgumentNullException(nameof(syntacticRecorder));
            }

            return ForNullableElements<T>(semanticWrapper, syntacticWrapper);

            bool semanticWrapper(TSemanticRecord dataRecord, IReadOnlyList<T?> argument)
            {
                semanticRecorder(dataRecord, argument);

                return true;
            }

            bool syntacticWrapper(TSyntacticRecord dataRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
            {
                syntacticRecorder(dataRecord, syntax);

                return true;
            }
        }

        IArgumentRecorders IParamsArrayArgumentAdapter.ForNullableElements<T>(Func<TSemanticRecord, IReadOnlyList<T?>, bool> semanticRecorder, Func<TSyntacticRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> syntacticRecorder)
        {
            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            if (syntacticRecorder is null)
            {
                throw new ArgumentNullException(nameof(syntacticRecorder));
            }

            return ForNullableElements(semanticRecorder, syntacticRecorder);
        }

        IArgumentRecorders IParamsArrayArgumentAdapter.ForNullableElements<T>(Action<TSemanticRecord, IReadOnlyList<T?>> semanticRecorder, Action<TSyntacticRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> syntacticRecorder)
        {
            if (semanticRecorder is null)
            {
                throw new ArgumentNullException(nameof(semanticRecorder));
            }

            if (syntacticRecorder is null)
            {
                throw new ArgumentNullException(nameof(syntacticRecorder));
            }

            return ForNullableElements<T>(semanticWrapper, syntacticWrapper);

            bool semanticWrapper(TSemanticRecord dataRecord, IReadOnlyList<T?> argument)
            {
                semanticRecorder(dataRecord, argument);

                return true;
            }

            bool syntacticWrapper(TSyntacticRecord dataRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
            {
                syntacticRecorder(dataRecord, syntax);

                return true;
            }
        }

        private static IArgumentRecorders For<T>(Func<TSemanticRecord, IReadOnlyList<T>, bool> semanticRecorder, Func<TSyntacticRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> syntacticRecorder)
        {
            return new ArgumentRecorders(semanticWrapper, SyntacticAdapterUtility.ForParams(syntacticRecorder));

            bool semanticWrapper(TSemanticRecord dataRecord, object? argument) => CommonArrayConverters.NonNullable<T>(argument).Match
            (
                static (error) => false,
                (converted) => semanticRecorder(dataRecord, converted)
            );
        }

        private static IArgumentRecorders ForNullable<T>(Func<TSemanticRecord, IReadOnlyList<T?>?, bool> semanticRecorder, Func<TSyntacticRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> syntacticRecorder) where T : class
        {
            return new ArgumentRecorders(semanticWrapper, SyntacticAdapterUtility.ForParams(syntacticRecorder));

            bool semanticWrapper(TSemanticRecord dataRecord, object? argument) => CommonArrayConverters.Nullable<T>(argument).Match
            (
                static (error) => false,
                (converted) => semanticRecorder(dataRecord, converted)
            );
        }

        private static IArgumentRecorders ForNullable<T>(Func<TSemanticRecord, IReadOnlyList<T?>?, bool> semanticRecorder, Func<TSyntacticRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> syntacticRecorder) where T : struct
        {
            return new ArgumentRecorders(semanticWrapper, SyntacticAdapterUtility.ForParams(syntacticRecorder));

            bool semanticWrapper(TSemanticRecord dataRecord, object? argument) => CommonArrayConverters.Nullable<T?>(argument).Match
            (
                static (error) => false,
                (converted) => semanticRecorder(dataRecord, converted)
            );
        }

        private static IArgumentRecorders ForNullableCollection<T>(Func<TSemanticRecord, IReadOnlyList<T>?, bool> semanticRecorder, Func<TSyntacticRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> syntacticRecorder)
        {
            return new ArgumentRecorders(semanticWrapper, SyntacticAdapterUtility.ForParams(syntacticRecorder));

            bool semanticWrapper(TSemanticRecord dataRecord, object? argument) => CommonArrayConverters.NullableCollection<T>(argument).Match
            (
                static (error) => false,
                (converted) => semanticRecorder(dataRecord, converted)
            );
        }

        private static IArgumentRecorders ForNullableElements<T>(Func<TSemanticRecord, IReadOnlyList<T?>, bool> semanticRecorder, Func<TSyntacticRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> syntacticRecorder) where T : class
        {
            return new ArgumentRecorders(semanticWrapper, SyntacticAdapterUtility.ForParams(syntacticRecorder));

            bool semanticWrapper(TSemanticRecord dataRecord, object? argument) => CommonArrayConverters.NullableElements<T?>(argument).Match
            (
                static (error) => false,
                (converted) => semanticRecorder(dataRecord, converted)
            );
        }

        private static IArgumentRecorders ForNullableElements<T>(Func<TSemanticRecord, IReadOnlyList<T?>, bool> semanticRecorder, Func<TSyntacticRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> syntacticRecorder) where T : struct
        {
            return new ArgumentRecorders(semanticWrapper, SyntacticAdapterUtility.ForParams(syntacticRecorder));

            bool semanticWrapper(TSemanticRecord dataRecord, object? argument) => CommonArrayConverters.NullableElements<T?>(argument).Match
            (
                static (error) => false,
                (converted) => semanticRecorder(dataRecord, converted)
            );
        }
    }
}
