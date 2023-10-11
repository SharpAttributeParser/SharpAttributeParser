namespace SharpAttributeParser.Mappers.Repositories.Adaptive;

using System;

/// <inheritdoc cref="IAdaptiveMappingRepositoryFactory{TSyntacticRecord, TSemanticRecord}"/>
public sealed class AdaptiveMappingRepositoryFactory<TCombinedRecord, TSemanticRecord> : IAdaptiveMappingRepositoryFactory<TCombinedRecord, TSemanticRecord>
{
    private readonly ITypeMappingRepositoryFactory<IDetachedMappedAdaptiveTypeArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveTypeArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>> TypeMappingRepositoryFactory;
    private readonly IConstructorMappingRepositoryFactory<IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>> ConstructorMappingRepositoryFactory;
    private readonly INamedMappingRepositoryFactory<IDetachedMappedAdaptiveNamedArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveNamedArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>> NamedMappingRepositoryFactory;

    /// <summary>Instantiates a <see cref="AdaptiveMappingRepositoryFactory{TSyntacticRecord, TSemanticRecord}"/>, handling creation of <see cref="IAdaptiveMappingRepository{TSyntacticRecord, TSemanticRecord}"/>.</summary>
    /// <param name="typeMappingRepositoryFactory">Handles creation of repositories for mappings from type parameters to recorders.</param>
    /// <param name="constructorMappingRepositoryFactory">Handles creation of repositories for mappings from constructor parameters to recorders.</param>
    /// <param name="namedMappingRepositoryFactory">Handles creation of repositories for mappings from constructor parameters to recorders.</param>
    public AdaptiveMappingRepositoryFactory(ITypeMappingRepositoryFactory<IDetachedMappedAdaptiveTypeArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveTypeArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>> typeMappingRepositoryFactory,
        IConstructorMappingRepositoryFactory<IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>> constructorMappingRepositoryFactory,
        INamedMappingRepositoryFactory<IDetachedMappedAdaptiveNamedArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveNamedArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>> namedMappingRepositoryFactory)
    {
        TypeMappingRepositoryFactory = typeMappingRepositoryFactory ?? throw new ArgumentNullException(nameof(typeMappingRepositoryFactory));
        ConstructorMappingRepositoryFactory = constructorMappingRepositoryFactory ?? throw new ArgumentNullException(nameof(constructorMappingRepositoryFactory));
        NamedMappingRepositoryFactory = namedMappingRepositoryFactory ?? throw new ArgumentNullException(nameof(namedMappingRepositoryFactory));
    }

    IAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord> IMappingRepositoryFactory<IAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord>>.Create(IParameterComparer comparer, bool throwOnMultipleBuilds)
    {
        if (comparer is null)
        {
            throw new ArgumentNullException(nameof(comparer));
        }

        var typeParameters = TypeMappingRepositoryFactory.Create(comparer.TypeParameter, throwOnMultipleBuilds);
        var constructorParameters = ConstructorMappingRepositoryFactory.Create(comparer.ConstructorParameter, throwOnMultipleBuilds);
        var namedParameters = NamedMappingRepositoryFactory.Create(comparer.NamedParameter, throwOnMultipleBuilds);

        return new Repository(typeParameters, constructorParameters, namedParameters);
    }

    private sealed class Repository : IAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord>
    {
        private readonly ITypeMappingRepository<IDetachedMappedAdaptiveTypeArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveTypeArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>> TypeParameters;
        private readonly IConstructorMappingRepository<IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>> ConstructorParameters;
        private readonly INamedMappingRepository<IDetachedMappedAdaptiveNamedArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveNamedArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>> NamedParameters;

        public Repository(ITypeMappingRepository<IDetachedMappedAdaptiveTypeArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveTypeArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>> typeParameters,
            IConstructorMappingRepository<IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>> constructorParameters,
            INamedMappingRepository<IDetachedMappedAdaptiveNamedArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveNamedArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>> namedParameters)
        {
            TypeParameters = typeParameters;
            ConstructorParameters = constructorParameters;
            NamedParameters = namedParameters;
        }

        ITypeMappingRepository<IDetachedMappedAdaptiveTypeArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveTypeArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>> IAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord>.TypeParameters => TypeParameters;
        IConstructorMappingRepository<IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>> IAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord>.ConstructorParameters => ConstructorParameters;
        INamedMappingRepository<IDetachedMappedAdaptiveNamedArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveNamedArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>> IAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord>.NamedParameters => NamedParameters;

        IAppendableTypeMappingRepository<IDetachedMappedAdaptiveTypeArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveTypeArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>> IAppendableAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord>.TypeParameters => TypeParameters;
        IAppendableConstructorMappingRepository<IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>> IAppendableAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord>.ConstructorParameters => ConstructorParameters;
        IAppendableNamedMappingRepository<IDetachedMappedAdaptiveNamedArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveNamedArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>> IAppendableAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord>.NamedParameters => NamedParameters;

        IBuiltAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord> IBuildableMappingRepository<IBuiltAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord>>.Build()
        {
            var typeParameterRepository = TypeParameters.Build();
            var constructorParameterRepository = ConstructorParameters.Build();
            var namedParameterRepository = NamedParameters.Build();

            return new BuiltRepository(typeParameterRepository, constructorParameterRepository, namedParameterRepository);
        }

        private sealed class BuiltRepository : IBuiltAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord>
        {
            private readonly IBuiltTypeMappingRepository<IDetachedMappedAdaptiveTypeArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>> TypeParameters;
            private readonly IBuiltConstructorMappingRepository<IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>> ConstructorParameters;
            private readonly IBuiltNamedMappingRepository<IDetachedMappedAdaptiveNamedArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>> NamedParameters;

            public BuiltRepository(IBuiltTypeMappingRepository<IDetachedMappedAdaptiveTypeArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>> typeParameters,
                IBuiltConstructorMappingRepository<IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>> constructorParameters,
                IBuiltNamedMappingRepository<IDetachedMappedAdaptiveNamedArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>> namedParameters)
            {
                TypeParameters = typeParameters;
                ConstructorParameters = constructorParameters;
                NamedParameters = namedParameters;
            }

            IBuiltTypeMappingRepository<IDetachedMappedAdaptiveTypeArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>> IBuiltAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord>.TypeParameters => TypeParameters;
            IBuiltConstructorMappingRepository<IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>> IBuiltAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord>.ConstructorParameters => ConstructorParameters;
            IBuiltNamedMappingRepository<IDetachedMappedAdaptiveNamedArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>> IBuiltAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord>.NamedParameters => NamedParameters;
        }
    }
}
