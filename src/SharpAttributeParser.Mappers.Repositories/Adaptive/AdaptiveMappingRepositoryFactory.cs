namespace SharpAttributeParser.Mappers.Repositories.Adaptive;

using System;
using System.Collections.Generic;

/// <inheritdoc cref="IAdaptiveMappingRepositoryFactory{TSyntacticRecord, TSemanticRecord}"/>
public sealed class AdaptiveMappingRepositoryFactory<TCombinedRecord, TSemanticRecord> : IAdaptiveMappingRepositoryFactory<TCombinedRecord, TSemanticRecord>
{
    private ITypeMappingRepositoryFactory<IDetachedMappedAdaptiveTypeArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveTypeArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>> TypeMappingRepositoryFactory { get; }
    private IConstructorMappingRepositoryFactory<IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>> ConstructorMappingRepositoryFactory { get; }
    private INamedMappingRepositoryFactory<IDetachedMappedAdaptiveNamedArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveNamedArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>> NamedMappingRepositoryFactory { get; }

    /// <summary>Instantiates a <see cref="AdaptiveMappingRepositoryFactory{TSyntacticRecord, TSemanticRecord}"/>, handling creation of <see cref="IAdaptiveMappingRepository{TSyntacticRecord, TSemanticRecord}"/>.</summary>
    /// <param name="typeMappingRepositoryFactory">Handles creation of repositories for mappings from type parameters to recorders.</param>
    /// <param name="constructorMappingRepositoryFactory">Handles creation of repositories for mappings from constructor parameters to recorders.</param>
    /// <param name="namedMappingRepositoryFactory">Handles creation of repositories for mappings from constructor parameters to recorders.</param>
    /// <exception cref="ArgumentNullException"/>
    public AdaptiveMappingRepositoryFactory(ITypeMappingRepositoryFactory<IDetachedMappedAdaptiveTypeArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveTypeArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>> typeMappingRepositoryFactory,
        IConstructorMappingRepositoryFactory<IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>> constructorMappingRepositoryFactory,
        INamedMappingRepositoryFactory<IDetachedMappedAdaptiveNamedArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveNamedArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>> namedMappingRepositoryFactory)
    {
        TypeMappingRepositoryFactory = typeMappingRepositoryFactory ?? throw new ArgumentNullException(nameof(typeMappingRepositoryFactory));
        ConstructorMappingRepositoryFactory = constructorMappingRepositoryFactory ?? throw new ArgumentNullException(nameof(constructorMappingRepositoryFactory));
        NamedMappingRepositoryFactory = namedMappingRepositoryFactory ?? throw new ArgumentNullException(nameof(namedMappingRepositoryFactory));
    }

    IAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord> IMappingRepositoryFactory<IAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord>>.Create(IEqualityComparer<string> parameterNameComparer, bool throwOnMultipleBuilds)
    {
        if (parameterNameComparer is null)
        {
            throw new ArgumentNullException(nameof(parameterNameComparer));
        }

        return Create(parameterNameComparer, parameterNameComparer, parameterNameComparer, throwOnMultipleBuilds);
    }

    IAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord> IMappingRepositoryFactory<IAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord>>.Create(IEqualityComparer<string> typeParameterNameComparer, IEqualityComparer<string> constructorParameterNameComparer, IEqualityComparer<string> namedParameterNameComparer, bool throwOnMultipleBuilds)
    {
        if (typeParameterNameComparer is null)
        {
            throw new ArgumentNullException(nameof(typeParameterNameComparer));
        }

        if (constructorParameterNameComparer is null)
        {
            throw new ArgumentNullException(nameof(constructorParameterNameComparer));
        }

        if (namedParameterNameComparer is null)
        {
            throw new ArgumentNullException(nameof(namedParameterNameComparer));
        }

        return Create(typeParameterNameComparer, constructorParameterNameComparer, namedParameterNameComparer, throwOnMultipleBuilds);
    }

    private IAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord> Create(IEqualityComparer<string> typeParameterNameComparer, IEqualityComparer<string> constructorParameterNameComparer, IEqualityComparer<string> namedParameterNameComparer, bool throwOnMultipleBuilds)
    {
        var typeParameters = TypeMappingRepositoryFactory.Create(typeParameterNameComparer, throwOnMultipleBuilds) ?? throw new InvalidOperationException($"A {nameof(ITypeMappingRepositoryFactory<object, object>)} produced a null {nameof(ITypeMappingRepository<object, object>)}.");
        var constructorParameters = ConstructorMappingRepositoryFactory.Create(constructorParameterNameComparer, throwOnMultipleBuilds) ?? throw new InvalidOperationException($"A {nameof(IConstructorMappingRepositoryFactory<object, object>)} produced a null {nameof(IConstructorMappingRepository<object, object>)}.");
        var namedParameters = NamedMappingRepositoryFactory.Create(namedParameterNameComparer, throwOnMultipleBuilds) ?? throw new InvalidOperationException($"A {nameof(INamedMappingRepositoryFactory<object, object>)} produced a null {nameof(INamedMappingRepository<object, object>)}.");

        return new Repository(typeParameters, constructorParameters, namedParameters);
    }

    private sealed class Repository : IAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord>
    {
        private ITypeMappingRepository<IDetachedMappedAdaptiveTypeArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveTypeArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>> TypeParameters { get; }
        private IConstructorMappingRepository<IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>> ConstructorParameters { get; }
        private INamedMappingRepository<IDetachedMappedAdaptiveNamedArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveNamedArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>> NamedParameters { get; }

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
            var typeParameterRepository = TypeParameters.Build() ?? throw new InvalidOperationException($"Building a {nameof(ITypeMappingRepository<object, object>)} resulted in a null {nameof(IBuiltTypeMappingRepository<object>)}.");
            var constructorParameterRepository = ConstructorParameters.Build() ?? throw new InvalidOperationException($"Building a {nameof(IConstructorMappingRepositoryFactory<object, object>)} resulted in a null {nameof(IBuiltConstructorMappingRepository<object>)}.");
            var namedParameterRepository = NamedParameters.Build() ?? throw new InvalidOperationException($"Building a {nameof(INamedMappingRepositoryFactory<object, object>)} resulted in a null {nameof(IBuiltNamedMappingRepository<object>)}.");

            return new BuiltRepository(typeParameterRepository, constructorParameterRepository, namedParameterRepository);
        }

        private sealed class BuiltRepository : IBuiltAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord>
        {
            private IBuiltTypeMappingRepository<IDetachedMappedAdaptiveTypeArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>> TypeParameters { get; }
            private IBuiltConstructorMappingRepository<IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>> ConstructorParameters { get; }
            private IBuiltNamedMappingRepository<IDetachedMappedAdaptiveNamedArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>> NamedParameters { get; }

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
