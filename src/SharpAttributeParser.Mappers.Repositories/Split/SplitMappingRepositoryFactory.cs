namespace SharpAttributeParser.Mappers.Repositories.Split;

using System;
using System.Collections.Generic;

/// <inheritdoc cref="ISplitMappingRepositoryFactory{TSyntacticRecord, TSyntacticRecord}"/>
public sealed class SplitMappingRepositoryFactory<TSemanticRecord, TSyntacticRecord> : ISplitMappingRepositoryFactory<TSemanticRecord, TSyntacticRecord>
{
    private ITypeMappingRepositoryFactory<IDetachedMappedSplitTypeArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitTypeArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>> TypeMappingRepositoryFactory { get; }
    private IConstructorMappingRepositoryFactory<IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>> ConstructorMappingRepositoryFactory { get; }
    private INamedMappingRepositoryFactory<IDetachedMappedSplitNamedArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitNamedArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>> NamedMappingRepositoryFactory { get; }

    /// <summary>Instantiates a <see cref="SplitMappingRepositoryFactory{TSyntacticRecord, TSyntacticRecord}"/>, handling creation of <see cref="ISplitMappingRepository{TSyntacticRecord, TSyntacticRecord}"/>.</summary>
    /// <param name="typeMappingRepositoryFactory">Handles creation of repositories for mappings from type parameters to recorders.</param>
    /// <param name="constructorMappingRepositoryFactory">Handles creation of repositories for mappings from constructor parameters to recorders.</param>
    /// <param name="namedMappingRepositoryFactory">Handles creation of repositories for mappings from constructor parameters to recorders.</param>
    /// <exception cref="ArgumentNullException"/>
    public SplitMappingRepositoryFactory(ITypeMappingRepositoryFactory<IDetachedMappedSplitTypeArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitTypeArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>> typeMappingRepositoryFactory,
        IConstructorMappingRepositoryFactory<IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>> constructorMappingRepositoryFactory,
        INamedMappingRepositoryFactory<IDetachedMappedSplitNamedArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitNamedArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>> namedMappingRepositoryFactory)
    {
        TypeMappingRepositoryFactory = typeMappingRepositoryFactory ?? throw new ArgumentNullException(nameof(typeMappingRepositoryFactory));
        ConstructorMappingRepositoryFactory = constructorMappingRepositoryFactory ?? throw new ArgumentNullException(nameof(constructorMappingRepositoryFactory));
        NamedMappingRepositoryFactory = namedMappingRepositoryFactory ?? throw new ArgumentNullException(nameof(namedMappingRepositoryFactory));
    }

    ISplitMappingRepository<TSemanticRecord, TSyntacticRecord> IMappingRepositoryFactory<ISplitMappingRepository<TSemanticRecord, TSyntacticRecord>>.Create(IEqualityComparer<string> parameterNameComparer, bool throwOnMultipleBuilds)
    {
        if (parameterNameComparer is null)
        {
            throw new ArgumentNullException(nameof(parameterNameComparer));
        }

        return Create(parameterNameComparer, parameterNameComparer, parameterNameComparer, throwOnMultipleBuilds);
    }

    ISplitMappingRepository<TSemanticRecord, TSyntacticRecord> IMappingRepositoryFactory<ISplitMappingRepository<TSemanticRecord, TSyntacticRecord>>.Create(IEqualityComparer<string> typeParameterNameComparer, IEqualityComparer<string> constructorParameterNameComparer, IEqualityComparer<string> namedParameterNameComparer, bool throwOnMultipleBuilds)
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

    private ISplitMappingRepository<TSemanticRecord, TSyntacticRecord> Create(IEqualityComparer<string> typeParameterNameComparer, IEqualityComparer<string> constructorParameterNameComparer, IEqualityComparer<string> namedParameterNameComparer, bool throwOnMultipleBuilds)
    {
        var typeParameters = TypeMappingRepositoryFactory.Create(typeParameterNameComparer, throwOnMultipleBuilds) ?? throw new InvalidOperationException($"A {nameof(ITypeMappingRepositoryFactory<object, object>)} produced a null {nameof(ITypeMappingRepository<object, object>)}.");
        var constructorParameters = ConstructorMappingRepositoryFactory.Create(constructorParameterNameComparer, throwOnMultipleBuilds) ?? throw new InvalidOperationException($"A {nameof(IConstructorMappingRepositoryFactory<object, object>)} produced a null {nameof(IConstructorMappingRepository<object, object>)}.");
        var namedParameters = NamedMappingRepositoryFactory.Create(namedParameterNameComparer, throwOnMultipleBuilds) ?? throw new InvalidOperationException($"A {nameof(INamedMappingRepositoryFactory<object, object>)} produced a null {nameof(INamedMappingRepository<object, object>)}.");

        return new Repository(typeParameters, constructorParameters, namedParameters);
    }

    private sealed class Repository : ISplitMappingRepository<TSemanticRecord, TSyntacticRecord>
    {
        private ITypeMappingRepository<IDetachedMappedSplitTypeArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitTypeArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>> TypeParameters { get; }
        private IConstructorMappingRepository<IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>> ConstructorParameters { get; }
        private INamedMappingRepository<IDetachedMappedSplitNamedArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitNamedArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>> NamedParameters { get; }

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
            var typeParameterRepository = TypeParameters.Build() ?? throw new InvalidOperationException($"Building a {nameof(ITypeMappingRepository<object, object>)} resulted in a null {nameof(IBuiltTypeMappingRepository<object>)}.");
            var constructorParameterRepository = ConstructorParameters.Build() ?? throw new InvalidOperationException($"Building a {nameof(IConstructorMappingRepositoryFactory<object, object>)} resulted in a null {nameof(IBuiltConstructorMappingRepository<object>)}.");
            var namedParameterRepository = NamedParameters.Build() ?? throw new InvalidOperationException($"Building a {nameof(INamedMappingRepositoryFactory<object, object>)} resulted in a null {nameof(IBuiltNamedMappingRepository<object>)}.");

            return new BuiltRepository(typeParameterRepository, constructorParameterRepository, namedParameterRepository);
        }

        private sealed class BuiltRepository : IBuiltSplitMappingRepository<TSemanticRecord, TSyntacticRecord>
        {
            private IBuiltTypeMappingRepository<IDetachedMappedSplitTypeArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>> TypeParameters { get; }
            private IBuiltConstructorMappingRepository<IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>> ConstructorParameters { get; }
            private IBuiltNamedMappingRepository<IDetachedMappedSplitNamedArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>> NamedParameters { get; }

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
