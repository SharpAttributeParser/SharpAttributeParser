namespace SharpAttributeParser.Mappers.Repositories;

using System;
using System.Collections.Generic;

/// <inheritdoc cref="ITypeMappingRepositoryFactory{TRecorder, TRecorderFactory}"/>
public sealed class TypeMappingRepositoryFactory<TRecorder, TRecorderFactory> : ITypeMappingRepositoryFactory<TRecorder, TRecorderFactory>
{
    private readonly TRecorderFactory RecorderFactory;

    /// <summary>Instantiates a <see cref="TypeMappingRepositoryFactory{TRecorder, TRecorderFactory}"/>, handling creation of <see cref="ITypeMappingRepository{TRecorder, TRecorderFactory}"/>.</summary>
    /// <param name="recorderFactory">Handles creation of recorders for the created repositories.</param>
    public TypeMappingRepositoryFactory(TRecorderFactory recorderFactory)
    {
        RecorderFactory = recorderFactory ?? throw new ArgumentNullException(nameof(recorderFactory));
    }

    ITypeMappingRepository<TRecorder, TRecorderFactory> ITypeMappingRepositoryFactory<TRecorder, TRecorderFactory>.Create(ITypeParameterComparer comparer, bool throwOnMultipleBuilds)
    {
        if (comparer is null)
        {
            throw new ArgumentNullException(nameof(comparer));
        }

        return new TypeMappingRepository(RecorderFactory, comparer, throwOnMultipleBuilds);
    }

    private sealed class TypeMappingRepository : ITypeMappingRepository<TRecorder, TRecorderFactory>
    {
        private readonly TRecorderFactory RecorderFactory;

        private readonly Dictionary<int, TRecorder> IndexedMappings = new();
        private readonly Dictionary<string, TRecorder> NamedMappings;

        private bool HasBeenBuilt;
        private readonly bool ThrowOnMultipleBuilds;

        public TypeMappingRepository(TRecorderFactory recorderFactory, ITypeParameterComparer comparer, bool throwOnMultipleBuilds)
        {
            RecorderFactory = recorderFactory;

            NamedMappings = new(comparer.Name);
            ThrowOnMultipleBuilds = throwOnMultipleBuilds;
        }

        void IAppendableTypeMappingRepository<TRecorder, TRecorderFactory>.AddIndexedMapping(int parameterIndex, TRecorder recorder)
        {
            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            AddIndexedMapping(parameterIndex, recorder);
        }

        void IAppendableTypeMappingRepository<TRecorder, TRecorderFactory>.AddIndexedMapping(int parameterIndex, Func<TRecorderFactory, TRecorder> recorderDelegate)
        {
            if (recorderDelegate is null)
            {
                throw new ArgumentNullException(nameof(recorderDelegate));
            }

            var recorder = recorderDelegate(RecorderFactory) ?? throw new ArgumentException($"The provided delegate produced a null {typeof(TRecorder).Name}.", nameof(recorderDelegate));

            AddIndexedMapping(parameterIndex, recorder);
        }

        void IAppendableTypeMappingRepository<TRecorder, TRecorderFactory>.AddNamedMapping(string parameterName, TRecorder recorder)
        {
            if (parameterName is null)
            {
                throw new ArgumentNullException(nameof(parameterName));
            }

            if (recorder is null)
            {
                throw new ArgumentNullException(nameof(recorder));
            }

            AddNamedMapping(parameterName, recorder);
        }

        void IAppendableTypeMappingRepository<TRecorder, TRecorderFactory>.AddNamedMapping(string parameterName, Func<TRecorderFactory, TRecorder> recorderDelegate)
        {
            if (parameterName is null)
            {
                throw new ArgumentNullException(nameof(parameterName));
            }

            if (recorderDelegate is null)
            {
                throw new ArgumentNullException(nameof(recorderDelegate));
            }

            var recorder = recorderDelegate(RecorderFactory) ?? throw new ArgumentException($"The provided delegate produced a null {typeof(TRecorder).Name}.", nameof(recorderDelegate));

            AddNamedMapping(parameterName, recorder);
        }

        IBuiltTypeMappingRepository<TRecorder> IBuildableMappingRepository<IBuiltTypeMappingRepository<TRecorder>>.Build()
        {
            if (HasBeenBuilt && ThrowOnMultipleBuilds)
            {
                throw new InvalidOperationException("The repository could not be built, as it already has been - and support for multiple builds was disabled.");
            }

            HasBeenBuilt = true;

            return new BuiltTypeMappingRepository(IndexedMappings, NamedMappings);
        }

        private void AddIndexedMapping(int parameterIndex, TRecorder recorder)
        {
            VerifyCanAddMapping();

            if (NamedMappings.Count is not 0)
            {
                throw new InvalidOperationException("Cannot add an indexed type-parameter mapping to the repository, as indexed and named type-parameter mappings may not be mixed - and a named mapping has already been added.");
            }

            if (IndexedMappings.ContainsKey(parameterIndex))
            {
                throw new ArgumentException($"A recorder has already been mapped to the type parameter with the provided index ({parameterIndex}).", nameof(parameterIndex));
            }

            IndexedMappings.Add(parameterIndex, recorder);
        }

        private void AddNamedMapping(string parameterName, TRecorder recorder)
        {
            VerifyCanAddMapping();

            if (IndexedMappings.Count is not 0)
            {
                throw new InvalidOperationException("Cannot add a named type-parameter mapping to the repository, as named and indexed type-parameter mappings may not be mixed - and an indexed mapping has already been added.");
            }

            if (NamedMappings.ContainsKey(parameterName))
            {
                throw new ArgumentException($"A recorder has already been mapped to the type parameter with the provided name (\"{parameterName}\").", nameof(parameterName));
            }

            NamedMappings.Add(parameterName, recorder);
        }

        private void VerifyCanAddMapping()
        {
            if (HasBeenBuilt)
            {
                throw new InvalidOperationException("New mappings cannot be added to hte repository, as it has already been built.");
            }
        }

        private sealed class BuiltTypeMappingRepository : IBuiltTypeMappingRepository<TRecorder>
        {
            private readonly IReadOnlyDictionary<int, TRecorder> Indexed;
            private readonly IReadOnlyDictionary<string, TRecorder> Named;

            public BuiltTypeMappingRepository(IReadOnlyDictionary<int, TRecorder> indexed, IReadOnlyDictionary<string, TRecorder> named)
            {
                Indexed = indexed;
                Named = named;
            }

            IReadOnlyDictionary<int, TRecorder> IBuiltTypeMappingRepository<TRecorder>.Indexed => Indexed;
            IReadOnlyDictionary<string, TRecorder> IBuiltTypeMappingRepository<TRecorder>.Named => Named;
        }
    }
}
