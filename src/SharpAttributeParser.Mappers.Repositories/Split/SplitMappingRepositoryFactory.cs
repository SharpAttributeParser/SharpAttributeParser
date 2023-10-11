namespace SharpAttributeParser.Mappers.Repositories.Split;

using System;

/// <inheritdoc cref="ISplitMappingRepositoryFactory{TSyntacticRecord, TSyntacticRecord}"/>
public sealed class SplitMappingRepositoryFactory<TSemanticRecord, TSyntacticRecord> : ISplitMappingRepositoryFactory<TSemanticRecord, TSyntacticRecord>
{
    private readonly ITypeMappingRepositoryFactory<IDetachedMappedSplitTypeArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitTypeArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>> TypeMappingRepositoryFactory;
    private readonly IConstructorMappingRepositoryFactory<IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>> ConstructorMappingRepositoryFactory;
    private readonly INamedMappingRepositoryFactory<IDetachedMappedSplitNamedArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitNamedArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>> NamedMappingRepositoryFactory;

    /// <summary>Instantiates a <see cref="SplitMappingRepositoryFactory{TSyntacticRecord, TSyntacticRecord}"/>, handling creation of <see cref="ISplitMappingRepository{TSyntacticRecord, TSyntacticRecord}"/>.</summary>
    /// <param name="typeMappingRepositoryFactory">Handles creation of repositories for mappings from type parameters to recorders.</param>
    /// <param name="constructorMappingRepositoryFactory">Handles creation of repositories for mappings from constructor parameters to recorders.</param>
    /// <param name="namedMappingRepositoryFactory">Handles creation of repositories for mappings from constructor parameters to recorders.</param>
    public SplitMappingRepositoryFactory(ITypeMappingRepositoryFactory<IDetachedMappedSplitTypeArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitTypeArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>> typeMappingRepositoryFactory,
        IConstructorMappingRepositoryFactory<IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>> constructorMappingRepositoryFactory,
        INamedMappingRepositoryFactory<IDetachedMappedSplitNamedArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitNamedArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>> namedMappingRepositoryFactory)
    {
        TypeMappingRepositoryFactory = typeMappingRepositoryFactory ?? throw new ArgumentNullException(nameof(typeMappingRepositoryFactory));
        ConstructorMappingRepositoryFactory = constructorMappingRepositoryFactory ?? throw new ArgumentNullException(nameof(constructorMappingRepositoryFactory));
        NamedMappingRepositoryFactory = namedMappingRepositoryFactory ?? throw new ArgumentNullException(nameof(namedMappingRepositoryFactory));
    }

    ISplitMappingRepository<TSemanticRecord, TSyntacticRecord> IMappingRepositoryFactory<ISplitMappingRepository<TSemanticRecord, TSyntacticRecord>>.Create(IParameterComparer comparer, bool throwOnMultipleBuilds)
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

    private sealed class Repository : ISplitMappingRepository<TSemanticRecord, TSyntacticRecord>
    {
        private readonly ITypeMappingRepository<IDetachedMappedSplitTypeArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitTypeArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>> TypeParameters;
        private readonly IConstructorMappingRepository<IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>> ConstructorParameters;
        private readonly INamedMappingRepository<IDetachedMappedSplitNamedArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitNamedArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>> NamedParameters;

        public Repository(ITypeMappingRepository<IDetachedMappedSplitTypeArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitTypeArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>> typeParameters,
            IConstructorMappingRepository<IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>> constructorParameters,
            INamedMappingRepository<IDetachedMappedSplitNamedArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitNamedArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>> namedParameters)
        {
            TypeParameters = typeParameters;
            ConstructorParameters = constructorParameters;
            NamedParameters = namedParameters;
        }

        ITypeMappingRepository<IDetachedMappedSplitTypeArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitTypeArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>> ISplitMappingRepository<TSemanticRecord, TSyntacticRecord>.TypeParameters => TypeParameters;
        IConstructorMappingRepository<IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>> ISplitMappingRepository<TSemanticRecord, TSyntacticRecord>.ConstructorParameters => ConstructorParameters;
        INamedMappingRepository<IDetachedMappedSplitNamedArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitNamedArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>> ISplitMappingRepository<TSemanticRecord, TSyntacticRecord>.NamedParameters => NamedParameters;

        IAppendableTypeMappingRepository<IDetachedMappedSplitTypeArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitTypeArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>> IAppendableSplitMappingRepository<TSemanticRecord, TSyntacticRecord>.TypeParameters => TypeParameters;
        IAppendableConstructorMappingRepository<IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>> IAppendableSplitMappingRepository<TSemanticRecord, TSyntacticRecord>.ConstructorParameters => ConstructorParameters;
        IAppendableNamedMappingRepository<IDetachedMappedSplitNamedArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitNamedArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>> IAppendableSplitMappingRepository<TSemanticRecord, TSyntacticRecord>.NamedParameters => NamedParameters;

        IBuiltSplitMappingRepository<TSemanticRecord, TSyntacticRecord> IBuildableMappingRepository<IBuiltSplitMappingRepository<TSemanticRecord, TSyntacticRecord>>.Build()
        {
            var typeParameterRepository = TypeParameters.Build();
            var constructorParameterRepository = ConstructorParameters.Build();
            var namedParameterRepository = NamedParameters.Build();

            return new BuiltRepository(typeParameterRepository, constructorParameterRepository, namedParameterRepository);
        }

        private sealed class BuiltRepository : IBuiltSplitMappingRepository<TSemanticRecord, TSyntacticRecord>
        {
            private readonly IBuiltTypeMappingRepository<IDetachedMappedSplitTypeArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>> TypeParameters;
            private readonly IBuiltConstructorMappingRepository<IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>> ConstructorParameters;
            private readonly IBuiltNamedMappingRepository<IDetachedMappedSplitNamedArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>> NamedParameters;

            public BuiltRepository(IBuiltTypeMappingRepository<IDetachedMappedSplitTypeArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>> typeParameters,
                IBuiltConstructorMappingRepository<IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>> constructorParameters,
                IBuiltNamedMappingRepository<IDetachedMappedSplitNamedArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>> namedParameters)
            {
                TypeParameters = typeParameters;
                ConstructorParameters = constructorParameters;
                NamedParameters = namedParameters;
            }

            IBuiltTypeMappingRepository<IDetachedMappedSplitTypeArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>> IBuiltSplitMappingRepository<TSemanticRecord, TSyntacticRecord>.TypeParameters => TypeParameters;
            IBuiltConstructorMappingRepository<IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>> IBuiltSplitMappingRepository<TSemanticRecord, TSyntacticRecord>.ConstructorParameters => ConstructorParameters;
            IBuiltNamedMappingRepository<IDetachedMappedSplitNamedArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>> IBuiltSplitMappingRepository<TSemanticRecord, TSyntacticRecord>.NamedParameters => NamedParameters;
        }
    }
}
