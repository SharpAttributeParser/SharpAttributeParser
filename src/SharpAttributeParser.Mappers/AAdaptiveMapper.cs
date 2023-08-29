﻿namespace SharpAttributeParser.Mappers;

using Microsoft.CodeAnalysis;

using SharpAttributeParser.Mappers.Logging;
using SharpAttributeParser.Mappers.MappedRecorders;
using SharpAttributeParser.Mappers.Repositories.Adaptive;
using SharpAttributeParser.Mappers.Repositories.Combined;
using SharpAttributeParser.Mappers.Repositories.Semantic;

using System;
using System.Collections.Generic;

/// <summary>An abstract <see cref="ICombinedMapper{TRecorder}"/> and <see cref="ISemanticMapper{TRecord}"/>, mapping attribute parameters to recorders responsible for recording arguments of that parameter.</summary>
/// <typeparam name="TCombinedRecord">The type to which arguments are recorded when parsed with syntactic context.</typeparam>
/// <typeparam name="TSemanticRecord">The type to which arguments are recorded when parsed without syntactic context.</typeparam>
public abstract class AAdaptiveMapper<TCombinedRecord, TSemanticRecord> : ICombinedMapper<TCombinedRecord>, ISemanticMapper<TSemanticRecord>
{
    private static Lazy<IAdaptiveMappingRepositoryFactory<TCombinedRecord, TSemanticRecord>> DefaultRepositoryFactory { get; } = new(DefaultRepositoryFactories.AdaptiveFactory<TCombinedRecord, TSemanticRecord>);

    private bool IsInitialized { get; set; }

    private IBuiltAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord> Mappings { get; set; } = null!;

    private IEqualityComparer<string> ParameterNameComparer { get; }
    private IMappedCombinedArgumentRecorderFactory CombinedRecorderFactory { get; }
    private IMappedSemanticArgumentRecorderFactory SemanticRecorderFactory { get; }
    private IAdaptiveMappingRepositoryFactory<TCombinedRecord, TSemanticRecord> RepositoryFactory { get; }

    private ICombinedMapperLogger CombinedLogger { get; }
    private ISemanticMapperLogger SemanticLogger { get; }

    /// <summary>Instantiates a <see cref="AAdaptiveMapper{TCombinedRecord, TSemanticRecord}"/>, mapping attribute parameters to recorders responsible for recording arguments of that parameter.</summary>
    /// <param name="parameterNameComparer">Determines equality when comparing parameter names. If <see langword="null"/>, <see cref="StringComparer.OrdinalIgnoreCase"/> is used.</param>
    /// <param name="combinedRecorderFactory">Handles creation of mapped recorders for recording arguments, together with syntactic information about arguments.</param>
    /// <param name="semanticRecorderFactory">Handles creation of mapped recorders for recording arguments.</param>
    /// <param name="repositoryFactory">Handles creation of repositories.</param>
    /// <param name="combinedLogger">The logger used to log messages when acting as a combined mapper.</param>
    /// <param name="semanticLogger">The logger used to log messages when acting as a semantic mapper.</param>
    protected AAdaptiveMapper(IEqualityComparer<string>? parameterNameComparer = null, IMappedCombinedArgumentRecorderFactory? combinedRecorderFactory = null, IMappedSemanticArgumentRecorderFactory? semanticRecorderFactory = null, IAdaptiveMappingRepositoryFactory<TCombinedRecord, TSemanticRecord>? repositoryFactory = null, ICombinedMapperLogger<AAdaptiveMapper<TCombinedRecord, TSemanticRecord>>? combinedLogger = null, ISemanticMapperLogger<AAdaptiveMapper<TCombinedRecord, TSemanticRecord>>? semanticLogger = null)
    {
        ParameterNameComparer = parameterNameComparer ?? StringComparer.OrdinalIgnoreCase;
        CombinedRecorderFactory = combinedRecorderFactory ?? DefaultRecorderFactories.CombinedFactory();
        SemanticRecorderFactory = semanticRecorderFactory ?? DefaultRecorderFactories.SemanticFactory();
        RepositoryFactory = repositoryFactory ?? DefaultRepositoryFactory.Value;

        CombinedLogger = combinedLogger ?? NullCombinedMapperLogger<AAdaptiveMapper<TCombinedRecord, TSemanticRecord>>.Singleton;
        SemanticLogger = semanticLogger ?? NullSemanticMapperLogger<AAdaptiveMapper<TCombinedRecord, TSemanticRecord>>.Singleton;
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
        var repository = RepositoryFactory.Create(ParameterNameComparer, throwOnMultipleBuilds: true) ?? throw new InvalidOperationException($"A {nameof(IAdaptiveMappingRepositoryFactory<object, object>)} produced a null {nameof(IAdaptiveMappingRepository<object, object>)}.");

        AddMappings(repository);

        Mappings = repository.Build();
    }

    /// <summary>Allows mappings from parameters to recorders to be added.</summary>
    /// <param name="repository">The repository to which mappings are added.</param>
    /// <remarks>The method will be invoked during initialization of the mapper.</remarks>
    /// <exception cref="ArgumentNullException"/>
    protected abstract void AddMappings(IAppendableAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord> repository);

    /// <inheritdoc/>
    public IMappedCombinedTypeArgumentRecorder? TryMapTypeParameter(ITypeParameterSymbol parameter, TCombinedRecord dataRecord)
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

        using var _ = CombinedLogger.TypeParameter.BeginScopeMappingTypeParameter(parameter, Mappings.TypeParameters);

        if (TryGetTypeParameterRecorder(parameter) is not IDetachedMappedAdaptiveTypeArgumentRecorderProvider<TCombinedRecord, TSemanticRecord> recorderProvider)
        {
            CombinedLogger.TypeParameter.FailedToMapTypeParameter();

            return null;
        }

        if (recorderProvider.Combined is not IDetachedMappedCombinedTypeArgumentRecorder<TCombinedRecord> recorder)
        {
            throw new InvalidOperationException($"The {nameof(IDetachedMappedCombinedTypeArgumentRecorder<object>)} of a {nameof(IDetachedMappedAdaptiveTypeArgumentRecorderProvider<object, object>)} was null.");
        }

        return CombinedRecorderFactory.TypeParameter.Create(dataRecord, recorder);
    }

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

        if (TryGetTypeParameterRecorder(parameter) is not IDetachedMappedAdaptiveTypeArgumentRecorderProvider<TCombinedRecord, TSemanticRecord> recorderProvider)
        {
            SemanticLogger.TypeParameter.FailedToMapTypeParameter();

            return null;
        }

        if (recorderProvider.Semantic is not IDetachedMappedSemanticTypeArgumentRecorder<TSemanticRecord> recorder)
        {
            throw new InvalidOperationException($"The {nameof(IDetachedMappedSemanticTypeArgumentRecorder<object>)} of a {nameof(IDetachedMappedAdaptiveTypeArgumentRecorderProvider<object, object>)} was null.");
        }

        return SemanticRecorderFactory.TypeParameter.Create(dataRecord, recorder);
    }

    /// <inheritdoc/>
    public IMappedCombinedConstructorArgumentRecorder? TryMapConstructorParameter(IParameterSymbol parameter, TCombinedRecord dataRecord)
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

        using var _ = CombinedLogger.ConstructorParameter.BeginScopeMappingConstructorParameter(parameter, Mappings.ConstructorParameters);

        if (Mappings.ConstructorParameters.Named.TryGetValue(parameter.Name, out var recorderProvider) is false)
        {
            CombinedLogger.ConstructorParameter.FailedToMapConstructorParameter();

            return null;
        }

        if (recorderProvider.Combined is not IDetachedMappedCombinedConstructorArgumentRecorder<TCombinedRecord> recorder)
        {
            throw new InvalidOperationException($"The {nameof(IDetachedMappedCombinedConstructorArgumentRecorder<object>)} of a {nameof(IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<object, object>)} was null.");
        }

        return CombinedRecorderFactory.ConstructorParameter.Create(dataRecord, recorder);
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
            throw new InvalidOperationException($"The {nameof(IDetachedMappedSemanticConstructorArgumentRecorder<object>)} of a {nameof(IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<object, object>)} was null.");
        }

        return SemanticRecorderFactory.ConstructorParameter.Create(dataRecord, recorder);
    }

    /// <inheritdoc/>
    public IMappedCombinedNamedArgumentRecorder? TryMapNamedParameter(string parameterName, TCombinedRecord dataRecord)
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

        using var _ = CombinedLogger.NamedParameter.BeginScopeMappingNamedParameter(parameterName, Mappings.NamedParameters);

        if (Mappings.NamedParameters.Named.TryGetValue(parameterName, out var recorderProvider) is false)
        {
            CombinedLogger.NamedParameter.FailedToMapNamedParameter();

            return null;
        }

        if (recorderProvider.Combined is not IDetachedMappedCombinedNamedArgumentRecorder<TCombinedRecord> recorder)
        {
            throw new InvalidOperationException($"The {nameof(IDetachedMappedCombinedNamedArgumentRecorder<object>)} of a {nameof(IDetachedMappedAdaptiveNamedArgumentRecorderProvider<object, object>)} was null.");
        }

        return CombinedRecorderFactory.NamedParameter.Create(dataRecord, recorder);
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
            throw new InvalidOperationException($"The {nameof(IDetachedMappedSemanticNamedArgumentRecorder<object>)} of a {nameof(IDetachedMappedAdaptiveNamedArgumentRecorderProvider<object, object>)} was null.");
        }

        return SemanticRecorderFactory.NamedParameter.Create(dataRecord, recorder);
    }

    private IDetachedMappedAdaptiveTypeArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>? TryGetTypeParameterRecorder(ITypeParameterSymbol parameter)
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