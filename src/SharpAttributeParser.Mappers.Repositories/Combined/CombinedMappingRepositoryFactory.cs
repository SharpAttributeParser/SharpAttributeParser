﻿namespace SharpAttributeParser.Mappers.Repositories.Combined;

using System;
using System.Collections.Generic;

/// <inheritdoc cref="ICombinedMappingRepositoryFactory{TRecord}"/>
public sealed class CombinedMappingRepositoryFactory<TRecord> : ICombinedMappingRepositoryFactory<TRecord>
{
    private ITypeMappingRepositoryFactory<IDetachedMappedCombinedTypeArgumentRecorder<TRecord>, IDetachedMappedCombinedTypeArgumentRecorderFactory<TRecord>> TypeMappingRepositoryFactory { get; }
    private IConstructorMappingRepositoryFactory<IDetachedMappedCombinedConstructorArgumentRecorder<TRecord>, IDetachedMappedCombinedConstructorArgumentRecorderFactory<TRecord>> ConstructorMappingRepositoryFactory { get; }
    private INamedMappingRepositoryFactory<IDetachedMappedCombinedNamedArgumentRecorder<TRecord>, IDetachedMappedCombinedNamedArgumentRecorderFactory<TRecord>> NamedMappingRepositoryFactory { get; }

    /// <summary>Instantiates a <see cref="CombinedMappingRepositoryFactory{TRecord}"/>, handling creation of <see cref="ICombinedMappingRepository{TRecord}"/>.</summary>
    /// <param name="typeMappingRepositoryFactory">Handles creation of repositories for mappings from type parameters to recorders.</param>
    /// <param name="constructorMappingRepositoryFactory">Handles creation of repositories for mappings from constructor parameters to recorders.</param>
    /// <param name="namedMappingRepositoryFactory">Handles creation of repositories for mappings from constructor parameters to recorders.</param>
    /// <exception cref="ArgumentNullException"/>
    public CombinedMappingRepositoryFactory(ITypeMappingRepositoryFactory<IDetachedMappedCombinedTypeArgumentRecorder<TRecord>, IDetachedMappedCombinedTypeArgumentRecorderFactory<TRecord>> typeMappingRepositoryFactory,
        IConstructorMappingRepositoryFactory<IDetachedMappedCombinedConstructorArgumentRecorder<TRecord>, IDetachedMappedCombinedConstructorArgumentRecorderFactory<TRecord>> constructorMappingRepositoryFactory,
        INamedMappingRepositoryFactory<IDetachedMappedCombinedNamedArgumentRecorder<TRecord>, IDetachedMappedCombinedNamedArgumentRecorderFactory<TRecord>> namedMappingRepositoryFactory)
    {
        TypeMappingRepositoryFactory = typeMappingRepositoryFactory ?? throw new ArgumentNullException(nameof(typeMappingRepositoryFactory));
        ConstructorMappingRepositoryFactory = constructorMappingRepositoryFactory ?? throw new ArgumentNullException(nameof(constructorMappingRepositoryFactory));
        NamedMappingRepositoryFactory = namedMappingRepositoryFactory ?? throw new ArgumentNullException(nameof(namedMappingRepositoryFactory));
    }

    ICombinedMappingRepository<TRecord> IMappingRepositoryFactory<ICombinedMappingRepository<TRecord>>.Create(IEqualityComparer<string> parameterNameComparer, bool throwOnMultipleBuilds)
    {
        if (parameterNameComparer is null)
        {
            throw new ArgumentNullException(nameof(parameterNameComparer));
        }

        return Create(parameterNameComparer, parameterNameComparer, parameterNameComparer, throwOnMultipleBuilds);
    }

    ICombinedMappingRepository<TRecord> IMappingRepositoryFactory<ICombinedMappingRepository<TRecord>>.Create(IEqualityComparer<string> typeParameterNameComparer, IEqualityComparer<string> constructorParameterNameComparer, IEqualityComparer<string> namedParameterNameComparer, bool throwOnMultipleBuilds)
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

    private ICombinedMappingRepository<TRecord> Create(IEqualityComparer<string> typeParameterNameComparer, IEqualityComparer<string> constructorParameterNameComparer, IEqualityComparer<string> namedParameterNameComparer, bool throwOnMultipleBuilds)
    {
        var typeParameters = TypeMappingRepositoryFactory.Create(typeParameterNameComparer, throwOnMultipleBuilds) ?? throw new InvalidOperationException($"A {nameof(ITypeMappingRepositoryFactory<object, object>)} produced a null {nameof(ITypeMappingRepository<object, object>)}.");
        var constructorParameters = ConstructorMappingRepositoryFactory.Create(constructorParameterNameComparer, throwOnMultipleBuilds) ?? throw new InvalidOperationException($"A {nameof(IConstructorMappingRepositoryFactory<object, object>)} produced a null {nameof(IConstructorMappingRepository<object, object>)}.");
        var namedParameters = NamedMappingRepositoryFactory.Create(namedParameterNameComparer, throwOnMultipleBuilds) ?? throw new InvalidOperationException($"A {nameof(INamedMappingRepositoryFactory<object, object>)} produced a null {nameof(INamedMappingRepository<object, object>)}.");

        return new Repository(typeParameters, constructorParameters, namedParameters);
    }

    private sealed class Repository : ICombinedMappingRepository<TRecord>
    {
        private ITypeMappingRepository<IDetachedMappedCombinedTypeArgumentRecorder<TRecord>, IDetachedMappedCombinedTypeArgumentRecorderFactory<TRecord>> TypeParameters { get; }
        private IConstructorMappingRepository<IDetachedMappedCombinedConstructorArgumentRecorder<TRecord>, IDetachedMappedCombinedConstructorArgumentRecorderFactory<TRecord>> ConstructorParameters { get; }
        private INamedMappingRepository<IDetachedMappedCombinedNamedArgumentRecorder<TRecord>, IDetachedMappedCombinedNamedArgumentRecorderFactory<TRecord>> NamedParameters { get; }

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
            var typeParameterRepository = TypeParameters.Build() ?? throw new InvalidOperationException($"Building a {nameof(ITypeMappingRepository<object, object>)} resulted in a null {nameof(IBuiltTypeMappingRepository<object>)}.");
            var constructorParameterRepository = ConstructorParameters.Build() ?? throw new InvalidOperationException($"Building a {nameof(IConstructorMappingRepositoryFactory<object, object>)} resulted in a null {nameof(IBuiltConstructorMappingRepository<object>)}.");
            var namedParameterRepository = NamedParameters.Build() ?? throw new InvalidOperationException($"Building a {nameof(INamedMappingRepositoryFactory<object, object>)} resulted in a null {nameof(IBuiltNamedMappingRepository<object>)}.");

            return new BuiltRepository(typeParameterRepository, constructorParameterRepository, namedParameterRepository);
        }

        private sealed class BuiltRepository : IBuiltCombinedMappingRepository<TRecord>
        {
            private IBuiltTypeMappingRepository<IDetachedMappedCombinedTypeArgumentRecorder<TRecord>> TypeParameters { get; }
            private IBuiltConstructorMappingRepository<IDetachedMappedCombinedConstructorArgumentRecorder<TRecord>> ConstructorParameters { get; }
            private IBuiltNamedMappingRepository<IDetachedMappedCombinedNamedArgumentRecorder<TRecord>> NamedParameters { get; }

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