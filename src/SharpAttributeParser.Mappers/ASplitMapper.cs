namespace SharpAttributeParser.Mappers;

using Microsoft.CodeAnalysis;

using SharpAttributeParser.Mappers.MappedRecorders;
using SharpAttributeParser.Mappers.Repositories.Split;

using System;

/// <summary>An abstract <see cref="ISemanticMapper{TRecorder}"/> and <see cref="ISyntacticMapper{TRecord}"/>, mapping attribute parameters to recorders responsible for recording arguments of that parameter.</summary>
/// <typeparam name="TSemanticRecord">The type to which arguments are recorded.</typeparam>
/// <typeparam name="TSyntacticRecord">The type to which syntactic information about arguments is recorded.</typeparam>
public abstract class ASplitMapper<TSemanticRecord, TSyntacticRecord> : ISemanticMapper<TSemanticRecord>, ISyntacticMapper<TSyntacticRecord>
{
    private bool IsInitialized;

    private IBuiltSplitMappingRepository<TSemanticRecord, TSyntacticRecord> Mappings = null!;

    private readonly ISplitMapperDependencyProvider<TSemanticRecord, TSyntacticRecord> DependencyProvider;

    /// <summary>Instantiates a <see cref="ASplitMapper{TSyntacticRecord, TSemanticRecord}"/>, mapping attribute parameters to recorders responsible for recording arguments of that parameter.</summary>
    /// <param name="dependencyProvider">Provides the dependencies of the mapper. If <see langword="null"/>, a default provider will be used.</param>
    protected ASplitMapper(ISplitMapperDependencyProvider<TSemanticRecord, TSyntacticRecord>? dependencyProvider = null)
    {
        DependencyProvider = dependencyProvider ?? new SplitMapperDependencyProvider<TSemanticRecord, TSyntacticRecord>();
    }

    /// <summary>Initializes the mapper. If not yet performed, initialization will occur when the mapper is first used.</summary>
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
        var repository = DependencyProvider.RepositoryFactory.Create(DependencyProvider.ParameterComparer, throwOnMultipleBuilds: true);

        AddMappings(repository);

        Mappings = repository.Build();
    }

    /// <summary>Allows mappings from parameters to recorders to be added.</summary>
    /// <param name="repository">The repository to which mappings are added.</param>
    /// <remarks>The method will be invoked during initialization of the mapper.</remarks>
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

        using var _ = DependencyProvider.SemanticLogger.TypeParameter.BeginScopeMappingTypeParameter(parameter, Mappings.TypeParameters);

        if (TryGetTypeParameterRecorder(parameter) is not IDetachedMappedSplitTypeArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord> recorderProvider)
        {
            DependencyProvider.SemanticLogger.TypeParameter.FailedToMapTypeParameter();

            return null;
        }

        return DependencyProvider.SemanticRecorderFactory.TypeParameter.Create(dataRecord, recorderProvider.Semantic);
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

        using var _ = DependencyProvider.SyntacticLogger.TypeParameter.BeginScopeMappingTypeParameter(parameter, Mappings.TypeParameters);

        if (TryGetTypeParameterRecorder(parameter) is not IDetachedMappedSplitTypeArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord> recorderProvider)
        {
            DependencyProvider.SyntacticLogger.TypeParameter.FailedToMapTypeParameter();

            return null;
        }

        return DependencyProvider.SyntacticRecorderFactory.TypeParameter.Create(dataRecord, recorderProvider.Syntactic);
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

        using var _ = DependencyProvider.SemanticLogger.ConstructorParameter.BeginScopeMappingConstructorParameter(parameter, Mappings.ConstructorParameters);

        if (Mappings.ConstructorParameters.Named.TryGetValue(parameter.Name, out var recorderProvider) is false)
        {
            DependencyProvider.SemanticLogger.ConstructorParameter.FailedToMapConstructorParameter();

            return null;
        }

        return DependencyProvider.SemanticRecorderFactory.ConstructorParameter.Create(dataRecord, recorderProvider.Semantic);
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

        using var _ = DependencyProvider.SyntacticLogger.ConstructorParameter.BeginScopeMappingConstructorParameter(parameter, Mappings.ConstructorParameters);

        if (Mappings.ConstructorParameters.Named.TryGetValue(parameter.Name, out var recorderProvider) is false)
        {
            DependencyProvider.SyntacticLogger.ConstructorParameter.FailedToMapConstructorParameter();

            return null;
        }

        return DependencyProvider.SyntacticRecorderFactory.ConstructorParameter.Create(dataRecord, recorderProvider.Syntactic);
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

        using var _ = DependencyProvider.SemanticLogger.NamedParameter.BeginScopeMappingNamedParameter(parameterName, Mappings.NamedParameters);

        if (Mappings.NamedParameters.Named.TryGetValue(parameterName, out var recorderProvider) is false)
        {
            DependencyProvider.SemanticLogger.NamedParameter.FailedToMapNamedParameter();

            return null;
        }

        return DependencyProvider.SemanticRecorderFactory.NamedParameter.Create(dataRecord, recorderProvider.Semantic);
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

        using var _ = DependencyProvider.SyntacticLogger.NamedParameter.BeginScopeMappingNamedParameter(parameterName, Mappings.NamedParameters);

        if (Mappings.NamedParameters.Named.TryGetValue(parameterName, out var recorderProvider) is false)
        {
            DependencyProvider.SyntacticLogger.NamedParameter.FailedToMapNamedParameter();

            return null;
        }

        return DependencyProvider.SyntacticRecorderFactory.NamedParameter.Create(dataRecord, recorderProvider.Syntactic);
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
