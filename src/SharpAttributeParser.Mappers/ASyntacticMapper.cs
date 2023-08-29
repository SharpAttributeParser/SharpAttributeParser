namespace SharpAttributeParser.Mappers;

using Microsoft.CodeAnalysis;

using SharpAttributeParser.Mappers.Logging;
using SharpAttributeParser.Mappers.MappedRecorders;
using SharpAttributeParser.Mappers.Repositories.Syntactic;

using System;
using System.Collections.Generic;

/// <summary>An abstract <see cref="ISyntacticMapper{TRecorder}"/>, mapping attribute parameters to recorders responsible for recording syntactic information about the arguments of that parameter.</summary>
/// <typeparam name="TRecord">The type to which syntactic information about arguments is recorded.</typeparam>
public abstract class ASyntacticMapper<TRecord> : ISyntacticMapper<TRecord>
{
    private static Lazy<ISyntacticMappingRepositoryFactory<TRecord>> DefaultRepositoryFactory { get; } = new(DefaultRepositoryFactories.SyntacticFactory<TRecord>);

    private bool IsInitialized { get; set; }

    private IBuiltSyntacticMappingRepository<TRecord> Mappings { get; set; } = null!;

    private IEqualityComparer<string> ParameterNameComparer { get; }
    private IMappedSyntacticArgumentRecorderFactory RecorderFactory { get; }
    private ISyntacticMappingRepositoryFactory<TRecord> RepositoryFactory { get; }

    private ISyntacticMapperLogger Logger { get; }

    /// <summary>Instantiates a <see cref="ASyntacticMapper{TRecord}"/>, mapping attribute parameters to recorders responsible for recording syntactic information about the arguments of that parameter.</summary>
    /// <param name="parameterNameComparer">Determines equality when comparing parameter names. If <see langword="null"/>, <see cref="StringComparer.OrdinalIgnoreCase"/> is used.</param>
    /// <param name="recorderFactory">Handles creation of mapped recorders.</param>
    /// <param name="repositoryFactory">Handles creation of repositories.</param>
    /// <param name="logger">The logger used to log messages.</param>
    protected ASyntacticMapper(IEqualityComparer<string>? parameterNameComparer = null, IMappedSyntacticArgumentRecorderFactory? recorderFactory = null, ISyntacticMappingRepositoryFactory<TRecord>? repositoryFactory = null, ISyntacticMapperLogger<ASyntacticMapper<TRecord>>? logger = null)
    {
        ParameterNameComparer = parameterNameComparer ?? StringComparer.OrdinalIgnoreCase;
        RecorderFactory = recorderFactory ?? DefaultRecorderFactories.SyntacticFactory();
        RepositoryFactory = repositoryFactory ?? DefaultRepositoryFactory.Value;

        Logger = logger ?? NullSyntacticMapperLogger<ASyntacticMapper<TRecord>>.Singleton;
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
        var repository = RepositoryFactory.Create(ParameterNameComparer, throwOnMultipleBuilds: true) ?? throw new InvalidOperationException($"A {nameof(ISyntacticMappingRepositoryFactory<object>)} produced a null {nameof(ISyntacticMappingRepository<object>)}.");

        AddMappings(repository);

        Mappings = repository.Build();
    }

    /// <summary>Allows mappings from parameters to recorders to be added.</summary>
    /// <param name="repository">The repository to which mappings are added.</param>
    /// <remarks>The method will be invoked during initialization of the mapper.</remarks>
    /// <exception cref="ArgumentNullException"/>
    protected abstract void AddMappings(IAppendableSyntacticMappingRepository<TRecord> repository);

    /// <inheritdoc/>
    public IMappedSyntacticTypeArgumentRecorder? TryMapTypeParameter(ITypeParameterSymbol parameter, TRecord dataRecord)
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

        if (TryGetTypeParameterRecorder(parameter) is not IDetachedMappedSyntacticTypeArgumentRecorder<TRecord> recorder)
        {
            Logger.TypeParameter.FailedToMapTypeParameter();

            return null;
        }

        return RecorderFactory.TypeParameter.Create(dataRecord, recorder);
    }

    /// <inheritdoc/>
    public IMappedSyntacticConstructorArgumentRecorder? TryMapConstructorParameter(IParameterSymbol parameter, TRecord dataRecord)
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
    public IMappedSyntacticNamedArgumentRecorder? TryMapNamedParameter(string parameterName, TRecord dataRecord)
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

    private IDetachedMappedSyntacticTypeArgumentRecorder<TRecord>? TryGetTypeParameterRecorder(ITypeParameterSymbol parameter)
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
