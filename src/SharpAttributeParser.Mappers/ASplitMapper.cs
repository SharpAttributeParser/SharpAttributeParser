namespace SharpAttributeParser.Mappers;

using Microsoft.CodeAnalysis;

using SharpAttributeParser.Mappers.Logging;
using SharpAttributeParser.Mappers.MappedRecorders;
using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Mappers.Repositories.Split;
using SharpAttributeParser.Mappers.Repositories.Syntactic;

using System;
using System.Collections.Generic;

/// <summary>An abstract <see cref="ISemanticMapper{TRecorder}"/> and <see cref="ISyntacticMapper{TRecord}"/>, mapping attribute parameters to recorders responsible for recording arguments of that parameter.</summary>
/// <typeparam name="TSemanticRecord">The type to which arguments are recorded.</typeparam>
/// <typeparam name="TSyntacticRecord">The type to which syntactic information about arguments is recorded.</typeparam>
public abstract class ASplitMapper<TSemanticRecord, TSyntacticRecord> : ISemanticMapper<TSemanticRecord>, ISyntacticMapper<TSyntacticRecord>
{
    private static Lazy<ISplitMappingRepositoryFactory<TSemanticRecord, TSyntacticRecord>> DefaultRepositoryFactory { get; } = new(DefaultRepositoryFactories.SplitFactory<TSemanticRecord, TSyntacticRecord>);

    private bool IsInitialized { get; set; }

    private IBuiltSplitMappingRepository<TSemanticRecord, TSyntacticRecord> Mappings { get; set; } = null!;

    private IEqualityComparer<string> ParameterNameComparer { get; }
    private IMappedSemanticArgumentRecorderFactory SemanticRecorderFactory { get; }
    private IMappedSyntacticArgumentRecorderFactory SyntacticRecorderFactory { get; }
    private ISplitMappingRepositoryFactory<TSemanticRecord, TSyntacticRecord> RepositoryFactory { get; }

    private ISemanticMapperLogger SemanticLogger { get; }
    private ISyntacticMapperLogger SyntacticLogger { get; }

    /// <summary>Instantiates a <see cref="ASplitMapper{TSyntacticRecord, TSemanticRecord}"/>, mapping attribute parameters to recorders responsible for recording arguments of that parameter.</summary>
    /// <param name="parameterNameComparer">Determines equality when comparing parameter names. If <see langword="null"/>, <see cref="StringComparer.OrdinalIgnoreCase"/> is used.</param>
    /// <param name="semanticRecorderFactory">Handles creation of mapped recorders for recording arguments.</param>
    /// <param name="syntacticRecorderFactory">Handles creation of mapped recorders for recording syntactic information about arguments.</param>
    /// <param name="repositoryFactory">Handles creation of repositories.</param>
    /// <param name="semanticLogger">The logger used to log messages when acting as a semantic mapper.</param>
    /// <param name="syntacticLogger">The logger used to log messages when acting as a syntactic mapper.</param>
    protected ASplitMapper(IEqualityComparer<string>? parameterNameComparer = null, IMappedSemanticArgumentRecorderFactory? semanticRecorderFactory = null, IMappedSyntacticArgumentRecorderFactory? syntacticRecorderFactory = null, ISplitMappingRepositoryFactory<TSemanticRecord, TSyntacticRecord>? repositoryFactory = null, ISemanticMapperLogger<ASplitMapper<TSemanticRecord, TSyntacticRecord>>? semanticLogger = null, ISyntacticMapperLogger<ASplitMapper<TSemanticRecord, TSyntacticRecord>>? syntacticLogger = null)
    {
        ParameterNameComparer = parameterNameComparer ?? StringComparer.OrdinalIgnoreCase;
        SemanticRecorderFactory = semanticRecorderFactory ?? DefaultRecorderFactories.SemanticFactory();
        SyntacticRecorderFactory = syntacticRecorderFactory ?? DefaultRecorderFactories.SyntacticFactory();
        RepositoryFactory = repositoryFactory ?? DefaultRepositoryFactory.Value;

        SemanticLogger = semanticLogger ?? NullSemanticMapperLogger<ASplitMapper<TSemanticRecord, TSyntacticRecord>>.Singleton;
        SyntacticLogger = syntacticLogger ?? NullSyntacticMapperLogger<ASplitMapper<TSemanticRecord, TSyntacticRecord>>.Singleton;
    }

    /// <summary>Initializes the mapper. If not yet performed, initialization will occur when the mapper is first used.</summary>
    /// <exception cref="InvalidOperationException"/>
    protected void InitializeMapper()
    {
        if (IsInitialized)
        {
            return;
        }

        InvokeAddMappings();

        IsInitialized = true;
    }

    private void InvokeAddMappings()
    {
        var repository = RepositoryFactory.Create(ParameterNameComparer, throwOnMultipleBuilds: true) ?? throw new InvalidOperationException($"A {nameof(ISplitMappingRepositoryFactory<object, object>)} produced a null {nameof(ISplitMappingRepository<object, object>)}.");

        AddMappings(repository);

        Mappings = repository.Build();
    }

    /// <summary>Allows mappings from parameters to recorders to be added.</summary>
    /// <param name="repository">The repository to which mappings are added.</param>
    /// <remarks>The method will be invoked during initialization of the mapper.</remarks>
    /// <exception cref="ArgumentNullException"/>
    protected abstract void AddMappings(IAppendableSplitMappingRepository<TSemanticRecord, TSyntacticRecord> repository);

    /// <inheritdoc/>
    public IMappedSemanticTypeArgumentRecorder? TryMapTypeParameter(ITypeParameterSymbol parameter, TSemanticRecord dataRecord)
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

        using var _ = SemanticLogger.TypeParameter.BeginScopeMappingTypeParameter(parameter, Mappings.TypeParameters);

        if (TryGetTypeParameterRecorder(parameter) is not IDetachedMappedSplitTypeArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord> recorderProvider)
        {
            SemanticLogger.TypeParameter.FailedToMapTypeParameter();

            return null;
        }

        if (recorderProvider.Semantic is not IDetachedMappedSemanticTypeArgumentRecorder<TSemanticRecord> recorder)
        {
            throw new InvalidOperationException($"The {nameof(IDetachedMappedSemanticTypeArgumentRecorder<object>)} of a {nameof(IDetachedMappedSplitTypeArgumentRecorderProvider<object, object>)} was null.");
        }

        return SemanticRecorderFactory.TypeParameter.Create(dataRecord, recorder);
    }

    /// <inheritdoc/>
    public IMappedSyntacticTypeArgumentRecorder? TryMapTypeParameter(ITypeParameterSymbol parameter, TSyntacticRecord dataRecord)
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

        using var _ = SyntacticLogger.TypeParameter.BeginScopeMappingTypeParameter(parameter, Mappings.TypeParameters);

        if (TryGetTypeParameterRecorder(parameter) is not IDetachedMappedSplitTypeArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord> recorderProvider)
        {
            SyntacticLogger.TypeParameter.FailedToMapTypeParameter();

            return null;
        }

        if (recorderProvider.Syntactic is not IDetachedMappedSyntacticTypeArgumentRecorder<TSyntacticRecord> recorder)
        {
            throw new InvalidOperationException($"The {nameof(IDetachedMappedSyntacticTypeArgumentRecorder<object>)} of a {nameof(IDetachedMappedSplitTypeArgumentRecorderProvider<object, object>)} was null.");
        }

        return SyntacticRecorderFactory.TypeParameter.Create(dataRecord, recorder);
    }

    /// <inheritdoc/>
    public IMappedSemanticConstructorArgumentRecorder? TryMapConstructorParameter(IParameterSymbol parameter, TSemanticRecord dataRecord)
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

        using var _ = SemanticLogger.ConstructorParameter.BeginScopeMappingConstructorParameter(parameter, Mappings.ConstructorParameters);

        if (Mappings.ConstructorParameters.Named.TryGetValue(parameter.Name, out var recorderProvider) is false)
        {
            SemanticLogger.ConstructorParameter.FailedToMapConstructorParameter();

            return null;
        }

        if (recorderProvider.Semantic is not IDetachedMappedSemanticConstructorArgumentRecorder<TSemanticRecord> recorder)
        {
            throw new InvalidOperationException($"The {nameof(IDetachedMappedSemanticConstructorArgumentRecorder<object>)} of a {nameof(IDetachedMappedSplitConstructorArgumentRecorderProvider<object, object>)} was null.");
        }

        return SemanticRecorderFactory.ConstructorParameter.Create(dataRecord, recorder);
    }

    /// <inheritdoc/>
    public IMappedSyntacticConstructorArgumentRecorder? TryMapConstructorParameter(IParameterSymbol parameter, TSyntacticRecord dataRecord)
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

        using var _ = SyntacticLogger.ConstructorParameter.BeginScopeMappingConstructorParameter(parameter, Mappings.ConstructorParameters);

        if (Mappings.ConstructorParameters.Named.TryGetValue(parameter.Name, out var recorderProvider) is false)
        {
            SyntacticLogger.ConstructorParameter.FailedToMapConstructorParameter();

            return null;
        }

        if (recorderProvider.Syntactic is not IDetachedMappedSyntacticConstructorArgumentRecorder<TSyntacticRecord> recorder)
        {
            throw new InvalidOperationException($"The {nameof(IDetachedMappedSyntacticConstructorArgumentRecorder<object>)} of a {nameof(IDetachedMappedSplitConstructorArgumentRecorderProvider<object, object>)} was null.");
        }

        return SyntacticRecorderFactory.ConstructorParameter.Create(dataRecord, recorder);
    }

    /// <inheritdoc/>
    public IMappedSemanticNamedArgumentRecorder? TryMapNamedParameter(string parameterName, TSemanticRecord dataRecord)
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

        using var _ = SemanticLogger.NamedParameter.BeginScopeMappingNamedParameter(parameterName, Mappings.NamedParameters);

        if (Mappings.NamedParameters.Named.TryGetValue(parameterName, out var recorderProvider) is false)
        {
            SemanticLogger.NamedParameter.FailedToMapNamedParameter();

            return null;
        }

        if (recorderProvider.Semantic is not IDetachedMappedSemanticNamedArgumentRecorder<TSemanticRecord> recorder)
        {
            throw new InvalidOperationException($"The {nameof(IDetachedMappedSemanticNamedArgumentRecorder<object>)} of a {nameof(IDetachedMappedSplitNamedArgumentRecorderProvider<object, object>)} was null.");
        }

        return SemanticRecorderFactory.NamedParameter.Create(dataRecord, recorder);
    }

    /// <inheritdoc/>
    public IMappedSyntacticNamedArgumentRecorder? TryMapNamedParameter(string parameterName, TSyntacticRecord dataRecord)
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

        using var _ = SyntacticLogger.NamedParameter.BeginScopeMappingNamedParameter(parameterName, Mappings.NamedParameters);

        if (Mappings.NamedParameters.Named.TryGetValue(parameterName, out var recorderProvider) is false)
        {
            SyntacticLogger.NamedParameter.FailedToMapNamedParameter();

            return null;
        }

        if (recorderProvider.Syntactic is not IDetachedMappedSyntacticNamedArgumentRecorder<TSyntacticRecord> recorder)
        {
            throw new InvalidOperationException($"The {nameof(IDetachedMappedSyntacticNamedArgumentRecorder<object>)} of a {nameof(IDetachedMappedSplitNamedArgumentRecorderProvider<object, object>)} was null.");
        }

        return SyntacticRecorderFactory.NamedParameter.Create(dataRecord, recorder);
    }

    private IDetachedMappedSplitTypeArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>? TryGetTypeParameterRecorder(ITypeParameterSymbol parameter)
    {
        var existsIndexedMapping = Mappings.TypeParameters.Indexed.TryGetValue(parameter.Ordinal, out var indexedProvider);
        var existsNamedMapping = Mappings.TypeParameters.Named.TryGetValue(parameter.Name, out var namedProvider);

        return (existsIndexedMapping, existsNamedMapping) switch
        {
            (false, false) => null,
            (true, false) => indexedProvider,
            (false, true) => namedProvider,
            (true, true) => throw new InvalidOperationException("The mapper unexpectedly consists of mappings from both the names and the indices of type parameters.")
        };
    }
}
