namespace SharpAttributeParser.Mappers.Repositories.Syntactic;

using SharpAttributeParser.Mappers.Repositories;

using System;

/// <inheritdoc cref="ISyntacticMappingRepositoryFactory{TRecord}"/>
public sealed class SyntacticMappingRepositoryFactory<TRecord> : ISyntacticMappingRepositoryFactory<TRecord>
{
    private ITypeMappingRepositoryFactory<IDetachedMappedSyntacticTypeArgumentRecorder<TRecord>, IDetachedMappedSyntacticTypeArgumentRecorderFactory<TRecord>> TypeMappingRepositoryFactory { get; }
    private IConstructorMappingRepositoryFactory<IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord>, IDetachedMappedSyntacticConstructorArgumentRecorderFactory<TRecord>> ConstructorMappingRepositoryFactory { get; }
    private INamedMappingRepositoryFactory<IDetachedMappedSyntacticNamedArgumentRecorder<TRecord>, IDetachedMappedSyntacticNamedArgumentRecorderFactory<TRecord>> NamedMappingRepositoryFactory { get; }

    /// <summary>Instantiates a <see cref="SyntacticMappingRepositoryFactory{TRecord}"/>, handling construction of <see cref="ISyntacticMappingRepository{TRecord}"/>.</summary>
    /// <param name="typeMappingRepositoryFactory">Handles creation of repositories for mappings from type parameters to recorders.</param>
    /// <param name="constructorMappingRepositoryFactory">Handles creation of repositories for mappings from constructor parameters to recorders.</param>
    /// <param name="namedMappingRepositoryFactory">Handles creation of repositories for mappings from constructor parameters to recorders.</param>
    public SyntacticMappingRepositoryFactory(ITypeMappingRepositoryFactory<IDetachedMappedSyntacticTypeArgumentRecorder<TRecord>, IDetachedMappedSyntacticTypeArgumentRecorderFactory<TRecord>> typeMappingRepositoryFactory,
        IConstructorMappingRepositoryFactory<IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord>, IDetachedMappedSyntacticConstructorArgumentRecorderFactory<TRecord>> constructorMappingRepositoryFactory,
        INamedMappingRepositoryFactory<IDetachedMappedSyntacticNamedArgumentRecorder<TRecord>, IDetachedMappedSyntacticNamedArgumentRecorderFactory<TRecord>> namedMappingRepositoryFactory)
    {
        TypeMappingRepositoryFactory = typeMappingRepositoryFactory ?? throw new ArgumentNullException(nameof(typeMappingRepositoryFactory));
        ConstructorMappingRepositoryFactory = constructorMappingRepositoryFactory ?? throw new ArgumentNullException(nameof(constructorMappingRepositoryFactory));
        NamedMappingRepositoryFactory = namedMappingRepositoryFactory ?? throw new ArgumentNullException(nameof(namedMappingRepositoryFactory));
    }

    ISyntacticMappingRepository<TRecord> IMappingRepositoryFactory<ISyntacticMappingRepository<TRecord>>.Create(IParameterComparer comparer, bool throwOnMultipleBuilds)
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

    private sealed class Repository : ISyntacticMappingRepository<TRecord>
    {
        private ITypeMappingRepository<IDetachedMappedSyntacticTypeArgumentRecorder<TRecord>, IDetachedMappedSyntacticTypeArgumentRecorderFactory<TRecord>> TypeParameters { get; }
        private IConstructorMappingRepository<IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord>, IDetachedMappedSyntacticConstructorArgumentRecorderFactory<TRecord>> ConstructorParameters { get; }
        private INamedMappingRepository<IDetachedMappedSyntacticNamedArgumentRecorder<TRecord>, IDetachedMappedSyntacticNamedArgumentRecorderFactory<TRecord>> NamedParameters { get; }

        public Repository(ITypeMappingRepository<IDetachedMappedSyntacticTypeArgumentRecorder<TRecord>, IDetachedMappedSyntacticTypeArgumentRecorderFactory<TRecord>> typeParameters,
            IConstructorMappingRepository<IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord>, IDetachedMappedSyntacticConstructorArgumentRecorderFactory<TRecord>> constructorParameters,
            INamedMappingRepository<IDetachedMappedSyntacticNamedArgumentRecorder<TRecord>, IDetachedMappedSyntacticNamedArgumentRecorderFactory<TRecord>> namedParameters)
        {
            TypeParameters = typeParameters;
            ConstructorParameters = constructorParameters;
            NamedParameters = namedParameters;
        }

        ITypeMappingRepository<IDetachedMappedSyntacticTypeArgumentRecorder<TRecord>, IDetachedMappedSyntacticTypeArgumentRecorderFactory<TRecord>> ISyntacticMappingRepository<TRecord>.TypeParameters => TypeParameters;
        IConstructorMappingRepository<IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord>, IDetachedMappedSyntacticConstructorArgumentRecorderFactory<TRecord>> ISyntacticMappingRepository<TRecord>.ConstructorParameters => ConstructorParameters;
        INamedMappingRepository<IDetachedMappedSyntacticNamedArgumentRecorder<TRecord>, IDetachedMappedSyntacticNamedArgumentRecorderFactory<TRecord>> ISyntacticMappingRepository<TRecord>.NamedParameters => NamedParameters;

        IAppendableTypeMappingRepository<IDetachedMappedSyntacticTypeArgumentRecorder<TRecord>, IDetachedMappedSyntacticTypeArgumentRecorderFactory<TRecord>> IAppendableSyntacticMappingRepository<TRecord>.TypeParameters => TypeParameters;
        IAppendableConstructorMappingRepository<IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord>, IDetachedMappedSyntacticConstructorArgumentRecorderFactory<TRecord>> IAppendableSyntacticMappingRepository<TRecord>.ConstructorParameters => ConstructorParameters;
        IAppendableNamedMappingRepository<IDetachedMappedSyntacticNamedArgumentRecorder<TRecord>, IDetachedMappedSyntacticNamedArgumentRecorderFactory<TRecord>> IAppendableSyntacticMappingRepository<TRecord>.NamedParameters => NamedParameters;

        IBuiltSyntacticMappingRepository<TRecord> IBuildableMappingRepository<IBuiltSyntacticMappingRepository<TRecord>>.Build()
        {
            var typeParameterRepository = TypeParameters.Build();
            var constructorParameterRepository = ConstructorParameters.Build();
            var namedParameterRepository = NamedParameters.Build();

            return new BuiltRepository(typeParameterRepository, constructorParameterRepository, namedParameterRepository);
        }

        private sealed class BuiltRepository : IBuiltSyntacticMappingRepository<TRecord>
        {
            private IBuiltTypeMappingRepository<IDetachedMappedSyntacticTypeArgumentRecorder<TRecord>> TypeParameters { get; }
            private IBuiltConstructorMappingRepository<IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord>> ConstructorParameters { get; }
            private IBuiltNamedMappingRepository<IDetachedMappedSyntacticNamedArgumentRecorder<TRecord>> NamedParameters { get; }

            public BuiltRepository(IBuiltTypeMappingRepository<IDetachedMappedSyntacticTypeArgumentRecorder<TRecord>> typeParameters,
                IBuiltConstructorMappingRepository<IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord>> constructorParameters,
                IBuiltNamedMappingRepository<IDetachedMappedSyntacticNamedArgumentRecorder<TRecord>> namedParameters)
            {
                TypeParameters = typeParameters;
                ConstructorParameters = constructorParameters;
                NamedParameters = namedParameters;
            }

            IBuiltTypeMappingRepository<IDetachedMappedSyntacticTypeArgumentRecorder<TRecord>> IBuiltSyntacticMappingRepository<TRecord>.TypeParameters => TypeParameters;
            IBuiltConstructorMappingRepository<IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord>> IBuiltSyntacticMappingRepository<TRecord>.ConstructorParameters => ConstructorParameters;
            IBuiltNamedMappingRepository<IDetachedMappedSyntacticNamedArgumentRecorder<TRecord>> IBuiltSyntacticMappingRepository<TRecord>.NamedParameters => NamedParameters;
        }
    }
}
