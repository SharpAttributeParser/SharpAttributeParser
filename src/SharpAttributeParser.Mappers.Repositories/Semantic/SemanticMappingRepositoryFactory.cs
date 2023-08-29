namespace SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Mappers.Repositories;

using System;
using System.Collections.Generic;

/// <inheritdoc cref="ISemanticMappingRepositoryFactory{TRecord}"/>
public sealed class SemanticMappingRepositoryFactory<TRecord> : ISemanticMappingRepositoryFactory<TRecord>
{
    private ITypeMappingRepositoryFactory<IDetachedMappedSemanticTypeArgumentRecorder<TRecord>, IDetachedMappedSemanticTypeArgumentRecorderFactory<TRecord>> TypeMappingRepositoryFactory { get; }
    private IConstructorMappingRepositoryFactory<IDetachedMappedSemanticConstructorArgumentRecorder<TRecord>, IDetachedMappedSemanticConstructorArgumentRecorderFactory<TRecord>> ConstructorMappingRepositoryFactory { get; }
    private INamedMappingRepositoryFactory<IDetachedMappedSemanticNamedArgumentRecorder<TRecord>, IDetachedMappedSemanticNamedArgumentRecorderFactory<TRecord>> NamedMappingRepositoryFactory { get; }

    /// <summary>Instantiates a <see cref="SemanticMappingRepositoryFactory{TRecord}"/>, handling creation of <see cref="ISemanticMappingRepository{TRecord}"/>.</summary>
    /// <param name="typeMappingRepositoryFactory">Handles creation of repositories for mappings from type parameters to recorders.</param>
    /// <param name="constructorMappingRepositoryFactory">Handles creation of repositories for mappings from constructor parameters to recorders.</param>
    /// <param name="namedMappingRepositoryFactory">Handles creation of repositories for mappings from constructor parameters to recorders.</param>
    /// <exception cref="ArgumentNullException"/>
    public SemanticMappingRepositoryFactory(ITypeMappingRepositoryFactory<IDetachedMappedSemanticTypeArgumentRecorder<TRecord>, IDetachedMappedSemanticTypeArgumentRecorderFactory<TRecord>> typeMappingRepositoryFactory,
        IConstructorMappingRepositoryFactory<IDetachedMappedSemanticConstructorArgumentRecorder<TRecord>, IDetachedMappedSemanticConstructorArgumentRecorderFactory<TRecord>> constructorMappingRepositoryFactory,
        INamedMappingRepositoryFactory<IDetachedMappedSemanticNamedArgumentRecorder<TRecord>, IDetachedMappedSemanticNamedArgumentRecorderFactory<TRecord>> namedMappingRepositoryFactory)
    {
        TypeMappingRepositoryFactory = typeMappingRepositoryFactory ?? throw new ArgumentNullException(nameof(typeMappingRepositoryFactory));
        ConstructorMappingRepositoryFactory = constructorMappingRepositoryFactory ?? throw new ArgumentNullException(nameof(constructorMappingRepositoryFactory));
        NamedMappingRepositoryFactory = namedMappingRepositoryFactory ?? throw new ArgumentNullException(nameof(namedMappingRepositoryFactory));
    }

    ISemanticMappingRepository<TRecord> IMappingRepositoryFactory<ISemanticMappingRepository<TRecord>>.Create(IEqualityComparer<string> parameterNameComparer, bool throwOnMultipleBuilds)
    {
        if (parameterNameComparer is null)
        {
            throw new ArgumentNullException(nameof(parameterNameComparer));
        }

        return Create(parameterNameComparer, parameterNameComparer, parameterNameComparer, throwOnMultipleBuilds);
    }

    ISemanticMappingRepository<TRecord> IMappingRepositoryFactory<ISemanticMappingRepository<TRecord>>.Create(IEqualityComparer<string> typeParameterNameComparer, IEqualityComparer<string> constructorParameterNameComparer, IEqualityComparer<string> namedParameterNameComparer, bool throwOnMultipleBuilds)
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

    private ISemanticMappingRepository<TRecord> Create(IEqualityComparer<string> typeParameterNameComparer, IEqualityComparer<string> constructorParameterNameComparer, IEqualityComparer<string> namedParameterNameComparer, bool throwOnMultipleBuilds)
    {
        var typeParameters = TypeMappingRepositoryFactory.Create(typeParameterNameComparer, throwOnMultipleBuilds) ?? throw new InvalidOperationException($"A {nameof(ITypeMappingRepositoryFactory<object, object>)} produced a null {nameof(ITypeMappingRepository<object, object>)}.");
        var constructorParameters = ConstructorMappingRepositoryFactory.Create(constructorParameterNameComparer, throwOnMultipleBuilds) ?? throw new InvalidOperationException($"A {nameof(IConstructorMappingRepositoryFactory<object, object>)} produced a null {nameof(IConstructorMappingRepository<object, object>)}.");
        var namedParameters = NamedMappingRepositoryFactory.Create(namedParameterNameComparer, throwOnMultipleBuilds) ?? throw new InvalidOperationException($"A {nameof(INamedMappingRepositoryFactory<object, object>)} produced a null {nameof(INamedMappingRepository<object, object>)}.");

        return new Repository(typeParameters, constructorParameters, namedParameters);
    }

    private sealed class Repository : ISemanticMappingRepository<TRecord>
    {
        private ITypeMappingRepository<IDetachedMappedSemanticTypeArgumentRecorder<TRecord>, IDetachedMappedSemanticTypeArgumentRecorderFactory<TRecord>> TypeParameters { get; }
        private IConstructorMappingRepository<IDetachedMappedSemanticConstructorArgumentRecorder<TRecord>, IDetachedMappedSemanticConstructorArgumentRecorderFactory<TRecord>> ConstructorParameters { get; }
        private INamedMappingRepository<IDetachedMappedSemanticNamedArgumentRecorder<TRecord>, IDetachedMappedSemanticNamedArgumentRecorderFactory<TRecord>> NamedParameters { get; }

        public Repository(ITypeMappingRepository<IDetachedMappedSemanticTypeArgumentRecorder<TRecord>, IDetachedMappedSemanticTypeArgumentRecorderFactory<TRecord>> typeParameters,
            IConstructorMappingRepository<IDetachedMappedSemanticConstructorArgumentRecorder<TRecord>, IDetachedMappedSemanticConstructorArgumentRecorderFactory<TRecord>> constructorParameters,
            INamedMappingRepository<IDetachedMappedSemanticNamedArgumentRecorder<TRecord>, IDetachedMappedSemanticNamedArgumentRecorderFactory<TRecord>> namedParameters)
        {
            TypeParameters = typeParameters;
            ConstructorParameters = constructorParameters;
            NamedParameters = namedParameters;
        }

        ITypeMappingRepository<IDetachedMappedSemanticTypeArgumentRecorder<TRecord>, IDetachedMappedSemanticTypeArgumentRecorderFactory<TRecord>> ISemanticMappingRepository<TRecord>.TypeParameters => TypeParameters;
        IConstructorMappingRepository<IDetachedMappedSemanticConstructorArgumentRecorder<TRecord>, IDetachedMappedSemanticConstructorArgumentRecorderFactory<TRecord>> ISemanticMappingRepository<TRecord>.ConstructorParameters => ConstructorParameters;
        INamedMappingRepository<IDetachedMappedSemanticNamedArgumentRecorder<TRecord>, IDetachedMappedSemanticNamedArgumentRecorderFactory<TRecord>> ISemanticMappingRepository<TRecord>.NamedParameters => NamedParameters;

        IAppendableTypeMappingRepository<IDetachedMappedSemanticTypeArgumentRecorder<TRecord>, IDetachedMappedSemanticTypeArgumentRecorderFactory<TRecord>> IAppendableSemanticMappingRepository<TRecord>.TypeParameters => TypeParameters;
        IAppendableConstructorMappingRepository<IDetachedMappedSemanticConstructorArgumentRecorder<TRecord>, IDetachedMappedSemanticConstructorArgumentRecorderFactory<TRecord>> IAppendableSemanticMappingRepository<TRecord>.ConstructorParameters => ConstructorParameters;
        IAppendableNamedMappingRepository<IDetachedMappedSemanticNamedArgumentRecorder<TRecord>, IDetachedMappedSemanticNamedArgumentRecorderFactory<TRecord>> IAppendableSemanticMappingRepository<TRecord>.NamedParameters => NamedParameters;

        IBuiltSemanticMappingRepository<TRecord> IBuildableMappingRepository<IBuiltSemanticMappingRepository<TRecord>>.Build()
        {
            var typeParameterRepository = TypeParameters.Build() ?? throw new InvalidOperationException($"Building a {nameof(ITypeMappingRepository<object, object>)} resulted in a null {nameof(IBuiltTypeMappingRepository<object>)}.");
            var constructorParameterRepository = ConstructorParameters.Build() ?? throw new InvalidOperationException($"Building a {nameof(IConstructorMappingRepositoryFactory<object, object>)} resulted in a null {nameof(IBuiltConstructorMappingRepository<object>)}.");
            var namedParameterRepository = NamedParameters.Build() ?? throw new InvalidOperationException($"Building a {nameof(INamedMappingRepositoryFactory<object, object>)} resulted in a null {nameof(IBuiltNamedMappingRepository<object>)}.");

            return new BuiltRepository(typeParameterRepository, constructorParameterRepository, namedParameterRepository);
        }

        private sealed class BuiltRepository : IBuiltSemanticMappingRepository<TRecord>
        {
            private IBuiltTypeMappingRepository<IDetachedMappedSemanticTypeArgumentRecorder<TRecord>> TypeParameters { get; }
            private IBuiltConstructorMappingRepository<IDetachedMappedSemanticConstructorArgumentRecorder<TRecord>> ConstructorParameters { get; }
            private IBuiltNamedMappingRepository<IDetachedMappedSemanticNamedArgumentRecorder<TRecord>> NamedParameters { get; }

            public BuiltRepository(IBuiltTypeMappingRepository<IDetachedMappedSemanticTypeArgumentRecorder<TRecord>> typeParameters,
                IBuiltConstructorMappingRepository<IDetachedMappedSemanticConstructorArgumentRecorder<TRecord>> constructorParameters,
                IBuiltNamedMappingRepository<IDetachedMappedSemanticNamedArgumentRecorder<TRecord>> namedParameters)
            {
                TypeParameters = typeParameters;
                ConstructorParameters = constructorParameters;
                NamedParameters = namedParameters;
            }

            IBuiltTypeMappingRepository<IDetachedMappedSemanticTypeArgumentRecorder<TRecord>> IBuiltSemanticMappingRepository<TRecord>.TypeParameters => TypeParameters;
            IBuiltConstructorMappingRepository<IDetachedMappedSemanticConstructorArgumentRecorder<TRecord>> IBuiltSemanticMappingRepository<TRecord>.ConstructorParameters => ConstructorParameters;
            IBuiltNamedMappingRepository<IDetachedMappedSemanticNamedArgumentRecorder<TRecord>> IBuiltSemanticMappingRepository<TRecord>.NamedParameters => NamedParameters;
        }
    }
}
