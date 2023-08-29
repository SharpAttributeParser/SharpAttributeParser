﻿namespace SharpAttributeParser.Mappers.Repositories.Syntactic;
using SharpAttributeParser.Mappers.Repositories;

using System;
using System.Collections.Generic;

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
    /// <exception cref="ArgumentNullException"/>
    public SyntacticMappingRepositoryFactory(ITypeMappingRepositoryFactory<IDetachedMappedSyntacticTypeArgumentRecorder<TRecord>, IDetachedMappedSyntacticTypeArgumentRecorderFactory<TRecord>> typeMappingRepositoryFactory,
        IConstructorMappingRepositoryFactory<IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord>, IDetachedMappedSyntacticConstructorArgumentRecorderFactory<TRecord>> constructorMappingRepositoryFactory,
        INamedMappingRepositoryFactory<IDetachedMappedSyntacticNamedArgumentRecorder<TRecord>, IDetachedMappedSyntacticNamedArgumentRecorderFactory<TRecord>> namedMappingRepositoryFactory)
    {
        TypeMappingRepositoryFactory = typeMappingRepositoryFactory ?? throw new ArgumentNullException(nameof(typeMappingRepositoryFactory));
        ConstructorMappingRepositoryFactory = constructorMappingRepositoryFactory ?? throw new ArgumentNullException(nameof(constructorMappingRepositoryFactory));
        NamedMappingRepositoryFactory = namedMappingRepositoryFactory ?? throw new ArgumentNullException(nameof(namedMappingRepositoryFactory));
    }

    ISyntacticMappingRepository<TRecord> IMappingRepositoryFactory<ISyntacticMappingRepository<TRecord>>.Create(IEqualityComparer<string> parameterNameComparer, bool throwOnMultipleBuilds)
    {
        if (parameterNameComparer is null)
        {
            throw new ArgumentNullException(nameof(parameterNameComparer));
        }

        return Create(parameterNameComparer, parameterNameComparer, parameterNameComparer, throwOnMultipleBuilds);
    }

    ISyntacticMappingRepository<TRecord> IMappingRepositoryFactory<ISyntacticMappingRepository<TRecord>>.Create(IEqualityComparer<string> typeParameterNameComparer, IEqualityComparer<string> constructorParameterNameComparer, IEqualityComparer<string> namedParameterNameComparer, bool throwOnMultipleBuilds)
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

    private ISyntacticMappingRepository<TRecord> Create(IEqualityComparer<string> typeParameterNameComparer, IEqualityComparer<string> constructorParameterNameComparer, IEqualityComparer<string> namedParameterNameComparer, bool throwOnMultipleBuilds)
    {
        var typeParameters = TypeMappingRepositoryFactory.Create(typeParameterNameComparer, throwOnMultipleBuilds) ?? throw new InvalidOperationException($"A {nameof(ITypeMappingRepositoryFactory<object, object>)} produced a null {nameof(ITypeMappingRepository<object, object>)}.");
        var constructorParameters = ConstructorMappingRepositoryFactory.Create(constructorParameterNameComparer, throwOnMultipleBuilds) ?? throw new InvalidOperationException($"A {nameof(IConstructorMappingRepositoryFactory<object, object>)} produced a null {nameof(IConstructorMappingRepository<object, object>)}.");
        var namedParameters = NamedMappingRepositoryFactory.Create(namedParameterNameComparer, throwOnMultipleBuilds) ?? throw new InvalidOperationException($"A {nameof(INamedMappingRepositoryFactory<object, object>)} produced a null {nameof(INamedMappingRepository<object, object>)}.");

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
            var typeParameterRepository = TypeParameters.Build() ?? throw new InvalidOperationException($"Building a {nameof(ITypeMappingRepository<object, object>)} resulted in a null {nameof(IBuiltTypeMappingRepository<object>)}.");
            var constructorParameterRepository = ConstructorParameters.Build() ?? throw new InvalidOperationException($"Building a {nameof(IConstructorMappingRepositoryFactory<object, object>)} resulted in a null {nameof(IBuiltConstructorMappingRepository<object>)}.");
            var namedParameterRepository = NamedParameters.Build() ?? throw new InvalidOperationException($"Building a {nameof(INamedMappingRepositoryFactory<object, object>)} resulted in a null {nameof(IBuiltNamedMappingRepository<object>)}.");

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
