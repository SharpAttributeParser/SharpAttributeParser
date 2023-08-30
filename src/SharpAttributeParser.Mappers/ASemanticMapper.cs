namespace SharpAttributeParser.Mappers;

using Microsoft.CodeAnalysis;

using SharpAttributeParser.Mappers.MappedRecorders;
using SharpAttributeParser.Mappers.Repositories.Semantic;

using System;

/// <summary>An abstract <see cref="ISemanticMapper{TRecorder}"/>, mapping attribute parameters to recorders responsible for recording arguments of that parameter.</summary>
/// <typeparam name="TRecord">The type to which arguments are recorded.</typeparam>
public abstract class ASemanticMapper<TRecord> : ISemanticMapper<TRecord>
{
    private static Lazy<ISemanticMappingRepositoryFactory<TRecord>> DefaultRepositoryFactory { get; } = new(DefaultRepositoryFactories.SemanticFactory<TRecord>);

    private bool IsInitialized { get; set; }

    private IBuiltSemanticMappingRepository<TRecord> Mappings { get; set; } = null!;

    private ISemanticMapperDependencyProvider<TRecord> DependencyProvider { get; }

    /// <summary>Instantiates a <see cref="ASemanticMapper{TRecord}"/>, mapping attribute parameters to recorders responsible for recording arguments of that parameter.</summary>
    /// <param name="dependencyProvider">Provides the dependencies of the mapper. If <see langword="null"/>, a default provider will be used.</param>
    protected ASemanticMapper(ISemanticMapperDependencyProvider<TRecord>? dependencyProvider = null)
    {
        DependencyProvider = dependencyProvider ?? new SemanticMapperDependencyProvider<TRecord>();
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
        var repository = DependencyProvider.RepositoryFactory.Create(DependencyProvider.ParameterComparer, throwOnMultipleBuilds: true) ?? throw new InvalidOperationException($"A {nameof(ISemanticMappingRepositoryFactory<object>)} produced a null {nameof(ISemanticMappingRepository<object>)}.");

        AddMappings(repository);

        Mappings = repository.Build();
    }

    /// <summary>Allows mappings from parameters to recorders to be added.</summary>
    /// <param name="repository">The repository to which mappings are added.</param>
    /// <remarks>The method will be invoked during initialization of the mapper.</remarks>
    /// <exception cref="ArgumentNullException"/>
    protected abstract void AddMappings(IAppendableSemanticMappingRepository<TRecord> repository);

    /// <inheritdoc/>
    public IMappedSemanticTypeArgumentRecorder? TryMapTypeParameter(ITypeParameterSymbol parameter, TRecord dataRecord)
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

        using var _ = DependencyProvider.Logger.TypeParameter.BeginScopeMappingTypeParameter(parameter, Mappings.TypeParameters);

        if (TryGetTypeParameterRecorder(parameter) is not IDetachedMappedSemanticTypeArgumentRecorder<TRecord> recorder)
        {
            DependencyProvider.Logger.TypeParameter.FailedToMapTypeParameter();

            return null;
        }

        return DependencyProvider.RecorderFactory.TypeParameter.Create(dataRecord, recorder);
    }

    /// <inheritdoc/>
    public IMappedSemanticConstructorArgumentRecorder? TryMapConstructorParameter(IParameterSymbol parameter, TRecord dataRecord)
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

        using var _ = DependencyProvider.Logger.ConstructorParameter.BeginScopeMappingConstructorParameter(parameter, Mappings.ConstructorParameters);

        if (Mappings.ConstructorParameters.Named.TryGetValue(parameter.Name, out var recorder) is false)
        {
            DependencyProvider.Logger.ConstructorParameter.FailedToMapConstructorParameter();

            return null;
        }

        return DependencyProvider.RecorderFactory.ConstructorParameter.Create(dataRecord, recorder);
    }

    /// <inheritdoc/>
    public IMappedSemanticNamedArgumentRecorder? TryMapNamedParameter(string parameterName, TRecord dataRecord)
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

        using var _ = DependencyProvider.Logger.NamedParameter.BeginScopeMappingNamedParameter(parameterName, Mappings.NamedParameters);

        if (Mappings.NamedParameters.Named.TryGetValue(parameterName, out var recorder) is false)
        {
            DependencyProvider.Logger.NamedParameter.FailedToMapNamedParameter();

            return null;
        }

        return DependencyProvider.RecorderFactory.NamedParameter.Create(dataRecord, recorder);
    }

    private IDetachedMappedSemanticTypeArgumentRecorder<TRecord>? TryGetTypeParameterRecorder(ITypeParameterSymbol parameter)
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
