namespace SharpAttributeParser.Mappers;

using Microsoft.CodeAnalysis;

using SharpAttributeParser.Mappers.Logging;
using SharpAttributeParser.Mappers.MappedRecorders;
using SharpAttributeParser.Mappers.Repositories.Combined;

using System;
using System.Collections.Generic;

/// <summary>An abstract <see cref="ICombinedMapper{TRecorder}"/>, mapping attribute parameters to recorders responsible for recording arguments of that parameter.</summary>
/// <typeparam name="TRecord">The type to which arguments are recorded.</typeparam>
public abstract class ACombinedMapper<TRecord> : ICombinedMapper<TRecord>
{
    private static Lazy<ICombinedMappingRepositoryFactory<TRecord>> DefaultRepositoryFactory { get; } = new(DefaultRepositoryFactories.CombinedFactory<TRecord>);

    private bool IsInitialized { get; set; }

    private IBuiltCombinedMappingRepository<TRecord> Mappings { get; set; } = null!;

    private IEqualityComparer<string> ParameterNameComparer { get; }
    private IMappedCombinedArgumentRecorderFactory RecorderFactory { get; }
    private ICombinedMappingRepositoryFactory<TRecord> RepositoryFactory { get; }

    private ICombinedMapperLogger Logger { get; }

    /// <summary>Instantiates a <see cref="ACombinedMapper{TRecord}"/>, mapping attribute parameters to recorders responsible for recording arguments of that parameter.</summary>
    /// <param name="parameterNameComparer">Determines equality when comparing parameter names. If <see langword="null"/>, <see cref="StringComparer.OrdinalIgnoreCase"/> is used.</param>
    /// <param name="recorderFactory">Handles creation of mapped recorders.</param>
    /// <param name="repositoryFactory">Handles creation of repositories.</param>
    /// <param name="logger">The logger used to log messages.</param>
    protected ACombinedMapper(IEqualityComparer<string>? parameterNameComparer = null, IMappedCombinedArgumentRecorderFactory? recorderFactory = null, ICombinedMappingRepositoryFactory<TRecord>? repositoryFactory = null, ICombinedMapperLogger<ACombinedMapper<TRecord>>? logger = null)
    {
        ParameterNameComparer = parameterNameComparer ?? StringComparer.OrdinalIgnoreCase;
        RecorderFactory = recorderFactory ?? DefaultRecorderFactories.CombinedFactory();
        RepositoryFactory = repositoryFactory ?? DefaultRepositoryFactory.Value;

        Logger = logger ?? NullCombinedMapperLogger<ACombinedMapper<TRecord>>.Singleton;
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
        var repository = RepositoryFactory.Create(ParameterNameComparer, throwOnMultipleBuilds: true) ?? throw new InvalidOperationException($"A {nameof(ICombinedMappingRepositoryFactory<object>)} produced a null {nameof(ICombinedMappingRepository<object>)}.");

        AddMappings(repository);

        Mappings = repository.Build();
    }

    /// <summary>Allows mappings from parameters to recorders to be added.</summary>
    /// <param name="repository">The repository to which mappings are added.</param>
    /// <remarks>The method will be invoked during initialization of the mapper.</remarks>
    /// <exception cref="ArgumentNullException"/>
    protected abstract void AddMappings(IAppendableCombinedMappingRepository<TRecord> repository);

    /// <inheritdoc/>
    public IMappedCombinedTypeArgumentRecorder? TryMapTypeParameter(ITypeParameterSymbol parameter, TRecord dataRecord)
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

        using var _ = Logger.TypeParameter.BeginScopeMappingTypeParameter(parameter, Mappings.TypeParameters);

        if (TryGetTypeParameterRecorder(parameter) is not IDetachedMappedCombinedTypeArgumentRecorder<TRecord> recorder)
        {
            Logger.TypeParameter.FailedToMapTypeParameter();

            return null;
        }

        return RecorderFactory.TypeParameter.Create(dataRecord, recorder);
    }

    /// <inheritdoc/>
    public IMappedCombinedConstructorArgumentRecorder? TryMapConstructorParameter(IParameterSymbol parameter, TRecord dataRecord)
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

        using var _ = Logger.ConstructorParameter.BeginScopeMappingConstructorParameter(parameter, Mappings.ConstructorParameters);

        if (Mappings.ConstructorParameters.Named.TryGetValue(parameter.Name, out var recorder) is false)
        {
            Logger.ConstructorParameter.FailedToMapConstructorParameter();

            return null;
        }

        return RecorderFactory.ConstructorParameter.Create(dataRecord, recorder);
    }

    /// <inheritdoc/>
    public IMappedCombinedNamedArgumentRecorder? TryMapNamedParameter(string parameterName, TRecord dataRecord)
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

        using var _ = Logger.NamedParameter.BeginScopeMappingNamedParameter(parameterName, Mappings.NamedParameters);

        if (Mappings.NamedParameters.Named.TryGetValue(parameterName, out var recorder) is false)
        {
            Logger.NamedParameter.FailedToMapNamedParameter();

            return null;
        }

        return RecorderFactory.NamedParameter.Create(dataRecord, recorder);
    }

    private IDetachedMappedCombinedTypeArgumentRecorder<TRecord>? TryGetTypeParameterRecorder(ITypeParameterSymbol parameter)
    {
        var existsIndexedMapping = Mappings.TypeParameters.Indexed.TryGetValue(parameter.Ordinal, out var indexedRecorder);
        var existsNamedMapping = Mappings.TypeParameters.Named.TryGetValue(parameter.Name, out var namedRecorder);

        return (existsIndexedMapping, existsNamedMapping) switch
        {
            (false, false) => null,
            (true, false) => indexedRecorder,
            (false, true) => namedRecorder,
            (true, true) => throw new InvalidOperationException("The mapper unexpectedly consists of mappings from both the names and the indices of type parameters.")
        };
    }
}
