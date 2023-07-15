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
    /// <summary>Provides adapters that may be applied to syntactical information before invoking a recorder.</summary>
    [SuppressMessage("Major Code Smell", "S2743: Static fields should not be used in generic types", Justification = "Type uses TRecord.")]
    protected static IArgumentSyntaxAdapterProvider Adapters { get; } = new ArgumentSyntaxAdapterProvider();

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
        foreach (var (parameter, recorder) in mappings)
        {
            if (recorder is null)
            {
                throw new InvalidOperationException("A recorder in the provided collection of mappings was null.");
            }

            var dictionaryDelegate = parameter.Match<Action<DTypeArgumentSyntaxRecorder>>
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
                dictionaryDelegate(recorder);
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

    private static void PopulateMappingsDictionary(IDictionary<string, DArgumentSyntaxRecorder> dictionary, IEnumerable<(string, DArgumentSyntaxRecorder)> mappings)
    {
        foreach (var (parameterName, recorder) in mappings)
        {
            if (parameterName is null)
            {
                throw new InvalidOperationException("The name of a parameter in the provided collection of parameter mappings was null.");
            }

            if (recorder is null)
            {
                throw new InvalidOperationException("A recorder in the provided collection of parameter mappings was null.");
            }

            try
            {
                dictionary.Add(parameterName, recorder);
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
    protected virtual IEnumerable<(OneOf<int, string> IndexOrName, DTypeArgumentSyntaxRecorder Recorder)> AddTypeParameterMappings() => Enumerable.Empty<(OneOf<int, string>, DTypeArgumentSyntaxRecorder)>();

    /// <summary>Maps the names of constructor or named parameters to recorders, responsible for recording the argument of the parameter.</summary>
    /// <returns>The mappings from parameter name to recorder.</returns>
    protected virtual IEnumerable<(string Name, DArgumentSyntaxRecorder Recorder)> AddParameterMappings() => Enumerable.Empty<(string, DArgumentSyntaxRecorder)>();

    /// <inheritdoc/>
    public ISyntacticAttributeArgumentRecorder? TryMapTypeParameter(ITypeParameterSymbol parameter, TRecord dataRecord)
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

        if (TryGetTypeParameterMapping(parameter) is not DTypeArgumentSyntaxRecorder mapping)
        {
            return null;
        }

        return new AttributeTypeArgumentRecorder(dataRecord, mapping);
    }

    /// <inheritdoc/>
    public ISyntacticAttributeConstructorArgumentRecorder? TryMapConstructorParameter(IParameterSymbol parameter, TRecord dataRecord)
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
    public ISyntacticAttributeArgumentRecorder? TryMapNamedParameter(string parameterName, TRecord dataRecord)
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
    /// <param name="dataRecord">The <typeparamref name="TRecord"/> to which the syntactical information is recorded.</param>
    /// <param name="syntax">The syntactial information about the argument of the parameter.</param>
    /// <returns>A <see cref="bool"/> indicating whether the syntactical information was successfully recorded.</returns>
    /// <exception cref="ArgumentNullException"/>
    protected delegate bool DTypeArgumentSyntaxRecorder(TRecord dataRecord, ExpressionSyntax syntax);

    /// <summary>Responsible for recording the syntactical information about the argument of a parameter.</summary>
    /// <param name="dataRecord">The <typeparamref name="TRecord"/> to which the syntactical information is recorded.</param>
    /// <param name="syntax">The <see cref="ExpressionSyntax"/> syntactically describing the argument of the parameter, or each element if expressed as a <see langword="params"/>-array.</param>
    /// <returns>A <see cref="bool"/> indicating whether the syntactical information was successfully recorded.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    protected delegate bool DArgumentSyntaxRecorder(TRecord dataRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax);

    /// <summary>Provides adapters that may be applied to syntactial information before invoking a recorder.</summary>
    protected interface IArgumentSyntaxAdapterProvider
    {
        /// <summary>Provides adapters related to type-arguments.</summary>
        public abstract ITypeArgumentSyntaxAdapter TypeArgument { get; }

        /// <summary>Provides adapters related to simple, non-array valued arguments.</summary>
        public abstract ISimpleArgumentSyntaxAdapter SimpleArgument { get; }

        /// <summary>Provides adapters related to array-valued arguments.</summary>
        public abstract IArrayArgumentSyntaxAdapter ArrayArgument { get; }
    }

    /// <summary>Provides adapters that may be applied to syntactial information of type-arguments before invoking a recorder.</summary>
    protected interface ITypeArgumentSyntaxAdapter
    {
        /// <summary>Converts the provided recorder to a <see cref="DTypeArgumentSyntaxRecorder"/>.</summary>
        /// <param name="recorder">Responsible for recording the syntactical information about the argument.</param>
        /// <remarks>Applying this adapter is not required, as the provided recorder already is of the desired type.</remarks>
        /// <returns>The converted <see cref="DTypeArgumentSyntaxRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DTypeArgumentSyntaxRecorder For(Func<TRecord, ExpressionSyntax, bool> recorder);

        /// <summary>Produces a <see cref="DTypeArgumentSyntaxRecorder"/> which invokes the provided recorder and returns <see langword="true"/>.</summary>
        /// <param name="recorder">Responsible for recording the syntactical information about the argument.</param>
        /// <returns>The produced <see cref="DTypeArgumentSyntaxRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DTypeArgumentSyntaxRecorder For(Action<TRecord, ExpressionSyntax> recorder);
    }

    /// <summary>Provides adapters that may be applied to syntactial information of non-array valued arguments before invoking a recorder.</summary>
    protected interface ISimpleArgumentSyntaxAdapter
    {
        /// <summary>Produces a <see cref="DArgumentSyntaxRecorder"/> which ensures that the argument is a single <see cref="ExpressionSyntax"/> before invoking the provided recorder, and which otherwise returns <see langword="false"/>.</summary>
        /// <param name="recorder">Responsible for recording the syntactical information about the argument.</param>
        /// <returns>The produced <see cref="DArgumentSyntaxRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentSyntaxRecorder For(Func<TRecord, ExpressionSyntax, bool> recorder);

        /// <summary>Produces a <see cref="DArgumentSyntaxRecorder"/> which ensures that the argument is a single <see cref="ExpressionSyntax"/> before invoking the provided recorder and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <param name="recorder">Responsible for recording the syntactical information about the argument.</param>
        /// <returns>The produced <see cref="DArgumentSyntaxRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentSyntaxRecorder For(Action<TRecord, ExpressionSyntax> recorder);
    }

    /// <summary>Provides adapters that may be applied to syntactial information before invoking a recorder.</summary>
    protected interface IArrayArgumentSyntaxAdapter
    {
        /// <summary>Converts the provided recorder to a <see cref="DArgumentSyntaxRecorder"/>.</summary>
        /// <param name="recorder">Responsible for recording the syntactical information about the argument, which may have been expressed as a <see langword="params"/>-array.</param>
        /// <remarks>Applying this adapter is not required, as the provided recorder already is of the desired type.</remarks>
        /// <returns>The converted <see cref="DArgumentSyntaxRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentSyntaxRecorder ForParams(Func<TRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> recorder);

        /// <summary>Produces a <see cref="DArgumentSyntaxRecorder"/> which invokes the provided recorder and returns <see langword="true"/>.</summary>
        /// <param name="recorder">Responsible for recording the syntactical information about the argument, which may have been expressed as a <see langword="params"/>-array.</param>
        /// <returns>The produced <see cref="DArgumentSyntaxRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentSyntaxRecorder ForParams(Action<TRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> recorder);

        /// <summary>Produces a <see cref="DArgumentSyntaxRecorder"/> which ensures that the argument is a single <see cref="ExpressionSyntax"/> before invoking the provided recorder, and which otherwise returns <see langword="false"/>.</summary>
        /// <param name="recorder">Responsible for recording the syntactical information about the argument, which may not have been expressed as a <see langword="params"/>-array.</param>
        /// <returns>The produced <see cref="DArgumentSyntaxRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentSyntaxRecorder ForNonParams(Func<TRecord, ExpressionSyntax, bool> recorder);

        /// <summary>Produces a <see cref="DArgumentSyntaxRecorder"/> which ensures that the argument is a single <see cref="ExpressionSyntax"/> before invoking the provided recorder and returning <see langword="true"/>, and which otherwise returns <see langword="false"/>.</summary>
        /// <param name="recorder">Responsible for recording the syntactical information about the argument, which may not have been expressed as a <see langword="params"/>-array.</param>
        /// <returns>The produced <see cref="DArgumentSyntaxRecorder"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public abstract DArgumentSyntaxRecorder ForNonParams(Action<TRecord, ExpressionSyntax> recorder);
    }

    private static class AdapterUtility
    {
        public static DArgumentSyntaxRecorder ForNonParams(Func<TRecord, ExpressionSyntax, bool> recorder)
        {
            return wrapper;

            bool wrapper(TRecord dataRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
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

        public static DArgumentSyntaxRecorder ForParams(Func<TRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> recorder)
        {
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

                return recorder(dataRecord, syntax);
            }
        }
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

        bool ISyntacticAttributeArgumentRecorder.RecordArgumentSyntax(ExpressionSyntax syntax)
        {
            if (syntax is null)
            {
                throw new ArgumentNullException(nameof(syntax));
            }

            return Recorder(DataRecord, syntax);
        }
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

        bool ISyntacticAttributeArgumentRecorder.RecordArgumentSyntax(ExpressionSyntax syntax)
        {
            if (syntax is null)
            {
                throw new ArgumentNullException(nameof(syntax));
            }

            return Recorder(DataRecord, syntax);
        }
    }

    private sealed class AttributeConstructorArgumentRecorder : ISyntacticAttributeConstructorArgumentRecorder
    {
        private TRecord DataRecord { get; }
        private DArgumentSyntaxRecorder Recorder { get; }

        public AttributeConstructorArgumentRecorder(TRecord dataRecord, DArgumentSyntaxRecorder recorder)
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

    private sealed class ArgumentSyntaxAdapterProvider : IArgumentSyntaxAdapterProvider
    {
        ITypeArgumentSyntaxAdapter IArgumentSyntaxAdapterProvider.TypeArgument { get; } = new TypeArgumentSyntaxAdapter();
        ISimpleArgumentSyntaxAdapter IArgumentSyntaxAdapterProvider.SimpleArgument { get; } = new SimpleArgumentSyntaxAdapter();
        IArrayArgumentSyntaxAdapter IArgumentSyntaxAdapterProvider.ArrayArgument { get; } = new ArrayArgumentSyntaxAdapter();
    }

    private sealed class TypeArgumentSyntaxAdapter : ITypeArgumentSyntaxAdapter
    {
        DTypeArgumentSyntaxRecorder ITypeArgumentSyntaxAdapter.For(Func<TRecord, ExpressionSyntax, bool> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return For(recorder);
        }

        DTypeArgumentSyntaxRecorder ITypeArgumentSyntaxAdapter.For(Action<TRecord, ExpressionSyntax> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return For(wrapper);

            bool wrapper(TRecord dataRecord, ExpressionSyntax syntax)
            {
                recorder(dataRecord, syntax);

                return true;
            }
        }

        private static DTypeArgumentSyntaxRecorder For(Func<TRecord, ExpressionSyntax, bool> recorder)
        {
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

                return recorder(dataRecord, syntax);
            }
        }
    }

    private sealed class SimpleArgumentSyntaxAdapter : ISimpleArgumentSyntaxAdapter
    {
        DArgumentSyntaxRecorder ISimpleArgumentSyntaxAdapter.For(Func<TRecord, ExpressionSyntax, bool> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return AdapterUtility.ForNonParams(recorder);
        }

        DArgumentSyntaxRecorder ISimpleArgumentSyntaxAdapter.For(Action<TRecord, ExpressionSyntax> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return AdapterUtility.ForNonParams(wrapper);

            bool wrapper(TRecord dataRecord, ExpressionSyntax syntax)
            {
                recorder(dataRecord, syntax);

                return true;
            }
        }
    }

    private sealed class ArrayArgumentSyntaxAdapter : IArrayArgumentSyntaxAdapter
    {
        DArgumentSyntaxRecorder IArrayArgumentSyntaxAdapter.ForParams(Func<TRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return AdapterUtility.ForParams(recorder);
        }

        DArgumentSyntaxRecorder IArrayArgumentSyntaxAdapter.ForParams(Action<TRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return AdapterUtility.ForParams(wrapper);

            bool wrapper(TRecord dataRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
            {
                recorder(dataRecord, syntax);

                return true;
            }
        }

        DArgumentSyntaxRecorder IArrayArgumentSyntaxAdapter.ForNonParams(Func<TRecord, ExpressionSyntax, bool> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return AdapterUtility.ForNonParams(recorder);
        }

        DArgumentSyntaxRecorder IArrayArgumentSyntaxAdapter.ForNonParams(Action<TRecord, ExpressionSyntax> recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            return AdapterUtility.ForNonParams(wrapper);

            bool wrapper(TRecord dataRecord, ExpressionSyntax syntax)
            {
                recorder(dataRecord, syntax);

                return true;
            }
        }
    }
}
