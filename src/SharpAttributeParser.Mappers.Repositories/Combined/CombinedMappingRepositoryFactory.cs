namespace SharpAttributeParser.Mappers.Repositories.Combined;

using System;

/// <inheritdoc cref="ICombinedMappingRepositoryFactory{TRecord}"/>
public sealed class CombinedMappingRepositoryFactory<TRecord> : ICombinedMappingRepositoryFactory<TRecord>
{
    private readonly ITypeMappingRepositoryFactory<IDetachedMappedCombinedTypeArgumentRecorder<TRecord>, IDetachedMappedCombinedTypeArgumentRecorderFactory<TRecord>> TypeMappingRepositoryFactory;
    private readonly IConstructorMappingRepositoryFactory<IDetachedMappedCombinedConstructorArgumentRecorder<TRecord>, IDetachedMappedCombinedConstructorArgumentRecorderFactory<TRecord>> ConstructorMappingRepositoryFactory;
    private readonly INamedMappingRepositoryFactory<IDetachedMappedCombinedNamedArgumentRecorder<TRecord>, IDetachedMappedCombinedNamedArgumentRecorderFactory<TRecord>> NamedMappingRepositoryFactory;

    /// <summary>Instantiates a <see cref="CombinedMappingRepositoryFactory{TRecord}"/>, handling creation of <see cref="ICombinedMappingRepository{TRecord}"/>.</summary>
    /// <param name="typeMappingRepositoryFactory">Handles creation of repositories for mappings from type parameters to recorders.</param>
    /// <param name="constructorMappingRepositoryFactory">Handles creation of repositories for mappings from constructor parameters to recorders.</param>
    /// <param name="namedMappingRepositoryFactory">Handles creation of repositories for mappings from constructor parameters to recorders.</param>
    public CombinedMappingRepositoryFactory(ITypeMappingRepositoryFactory<IDetachedMappedCombinedTypeArgumentRecorder<TRecord>, IDetachedMappedCombinedTypeArgumentRecorderFactory<TRecord>> typeMappingRepositoryFactory,
        IConstructorMappingRepositoryFactory<IDetachedMappedCombinedConstructorArgumentRecorder<TRecord>, IDetachedMappedCombinedConstructorArgumentRecorderFactory<TRecord>> constructorMappingRepositoryFactory,
        INamedMappingRepositoryFactory<IDetachedMappedCombinedNamedArgumentRecorder<TRecord>, IDetachedMappedCombinedNamedArgumentRecorderFactory<TRecord>> namedMappingRepositoryFactory)
    {
        TypeMappingRepositoryFactory = typeMappingRepositoryFactory ?? throw new ArgumentNullException(nameof(typeMappingRepositoryFactory));
        ConstructorMappingRepositoryFactory = constructorMappingRepositoryFactory ?? throw new ArgumentNullException(nameof(constructorMappingRepositoryFactory));
        NamedMappingRepositoryFactory = namedMappingRepositoryFactory ?? throw new ArgumentNullException(nameof(namedMappingRepositoryFactory));
    }

    ICombinedMappingRepository<TRecord> IMappingRepositoryFactory<ICombinedMappingRepository<TRecord>>.Create(IParameterComparer comparer, bool throwOnMultipleBuilds)
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

    private sealed class Repository : ICombinedMappingRepository<TRecord>
    {
        private readonly ITypeMappingRepository<IDetachedMappedCombinedTypeArgumentRecorder<TRecord>, IDetachedMappedCombinedTypeArgumentRecorderFactory<TRecord>> TypeParameters;
        private readonly IConstructorMappingRepository<IDetachedMappedCombinedConstructorArgumentRecorder<TRecord>, IDetachedMappedCombinedConstructorArgumentRecorderFactory<TRecord>> ConstructorParameters;
        private readonly INamedMappingRepository<IDetachedMappedCombinedNamedArgumentRecorder<TRecord>, IDetachedMappedCombinedNamedArgumentRecorderFactory<TRecord>> NamedParameters;

        public Repository(ITypeMappingRepository<IDetachedMappedCombinedTypeArgumentRecorder<TRecord>, IDetachedMappedCombinedTypeArgumentRecorderFactory<TRecord>> typeParameters,
            IConstructorMappingRepository<IDetachedMappedCombinedConstructorArgumentRecorder<TRecord>, IDetachedMappedCombinedConstructorArgumentRecorderFactory<TRecord>> constructorParameters,
            INamedMappingRepository<IDetachedMappedCombinedNamedArgumentRecorder<TRecord>, IDetachedMappedCombinedNamedArgumentRecorderFactory<TRecord>> namedParameters)
        {
            TypeParameters = typeParameters;
            ConstructorParameters = constructorParameters;
            NamedParameters = namedParameters;
        }

        ITypeMappingRepository<IDetachedMappedCombinedTypeArgumentRecorder<TRecord>, IDetachedMappedCombinedTypeArgumentRecorderFactory<TRecord>> ICombinedMappingRepository<TRecord>.TypeParameters => TypeParameters;
        IConstructorMappingRepository<IDetachedMappedCombinedConstructorArgumentRecorder<TRecord>, IDetachedMappedCombinedConstructorArgumentRecorderFactory<TRecord>> ICombinedMappingRepository<TRecord>.ConstructorParameters => ConstructorParameters;
        INamedMappingRepository<IDetachedMappedCombinedNamedArgumentRecorder<TRecord>, IDetachedMappedCombinedNamedArgumentRecorderFactory<TRecord>> ICombinedMappingRepository<TRecord>.NamedParameters => NamedParameters;

        IAppendableTypeMappingRepository<IDetachedMappedCombinedTypeArgumentRecorder<TRecord>, IDetachedMappedCombinedTypeArgumentRecorderFactory<TRecord>> IAppendableCombinedMappingRepository<TRecord>.TypeParameters => TypeParameters;
        IAppendableConstructorMappingRepository<IDetachedMappedCombinedConstructorArgumentRecorder<TRecord>, IDetachedMappedCombinedConstructorArgumentRecorderFactory<TRecord>> IAppendableCombinedMappingRepository<TRecord>.ConstructorParameters => ConstructorParameters;
        IAppendableNamedMappingRepository<IDetachedMappedCombinedNamedArgumentRecorder<TRecord>, IDetachedMappedCombinedNamedArgumentRecorderFactory<TRecord>> IAppendableCombinedMappingRepository<TRecord>.NamedParameters => NamedParameters;

        IBuiltCombinedMappingRepository<TRecord> IBuildableMappingRepository<IBuiltCombinedMappingRepository<TRecord>>.Build()
        {
            var typeParameterRepository = TypeParameters.Build();
            var constructorParameterRepository = ConstructorParameters.Build();
            var namedParameterRepository = NamedParameters.Build();

            return new BuiltRepository(typeParameterRepository, constructorParameterRepository, namedParameterRepository);
        }

        private sealed class BuiltRepository : IBuiltCombinedMappingRepository<TRecord>
        {
            private readonly IBuiltTypeMappingRepository<IDetachedMappedCombinedTypeArgumentRecorder<TRecord>> TypeParameters;
            private readonly IBuiltConstructorMappingRepository<IDetachedMappedCombinedConstructorArgumentRecorder<TRecord>> ConstructorParameters;
            private readonly IBuiltNamedMappingRepository<IDetachedMappedCombinedNamedArgumentRecorder<TRecord>> NamedParameters;

            public BuiltRepository(IBuiltTypeMappingRepository<IDetachedMappedCombinedTypeArgumentRecorder<TRecord>> typeParameters,
                IBuiltConstructorMappingRepository<IDetachedMappedCombinedConstructorArgumentRecorder<TRecord>> constructorParameters,
                IBuiltNamedMappingRepository<IDetachedMappedCombinedNamedArgumentRecorder<TRecord>> namedParameters)
            {
                TypeParameters = typeParameters;
                ConstructorParameters = constructorParameters;
                NamedParameters = namedParameters;
            }

            IBuiltTypeMappingRepository<IDetachedMappedCombinedTypeArgumentRecorder<TRecord>> IBuiltCombinedMappingRepository<TRecord>.TypeParameters => TypeParameters;
            IBuiltConstructorMappingRepository<IDetachedMappedCombinedConstructorArgumentRecorder<TRecord>> IBuiltCombinedMappingRepository<TRecord>.ConstructorParameters => ConstructorParameters;
            IBuiltNamedMappingRepository<IDetachedMappedCombinedNamedArgumentRecorder<TRecord>> IBuiltCombinedMappingRepository<TRecord>.NamedParameters => NamedParameters;
        }
    }
}
