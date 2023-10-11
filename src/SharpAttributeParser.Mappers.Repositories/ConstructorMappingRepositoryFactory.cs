namespace SharpAttributeParser.Mappers.Repositories;

using System;
using System.Collections.Generic;

/// <inheritdoc cref="IConstructorMappingRepositoryFactory{TRecorder, TRecorderFactory}"/>
public sealed class ConstructorMappingRepositoryFactory<TRecorder, TRecorderFactory> : IConstructorMappingRepositoryFactory<TRecorder, TRecorderFactory>
{
    private readonly TRecorderFactory RecorderFactory;

    /// <summary>Instantiates a <see cref="ConstructorMappingRepositoryFactory{TRecorder, TRecorderFactory}"/>, handling creation of <see cref="IConstructorMappingRepository{TRecorder, TRecorderFactory}"/>.</summary>
    /// <param name="recorderFactory">Handles creation of recorders for the created repositories.</param>
    public ConstructorMappingRepositoryFactory(TRecorderFactory recorderFactory)
    {
        RecorderFactory = recorderFactory ?? throw new ArgumentNullException(nameof(recorderFactory));
    }

    IConstructorMappingRepository<TRecorder, TRecorderFactory> IConstructorMappingRepositoryFactory<TRecorder, TRecorderFactory>.Create(IConstructorParameterComparer comparer, bool throwOnMultipleBuilds)
    {
        if (comparer is null)
        {
            throw new ArgumentNullException(nameof(comparer));
        }

        return new ConstructorMappingRepository(RecorderFactory, comparer, throwOnMultipleBuilds);
    }

    private sealed class ConstructorMappingRepository : IConstructorMappingRepository<TRecorder, TRecorderFactory>
    {
        private readonly TRecorderFactory RecorderFactory;

        private readonly Dictionary<string, TRecorder> NamedMappings;

        private bool HasBeenBuilt;
        private readonly bool ThrowOnMultipleBuilds;

        public ConstructorMappingRepository(TRecorderFactory recorderFactory, IConstructorParameterComparer comparer, bool throwOnMultipleBuilds)
        {
            RecorderFactory = recorderFactory;

            NamedMappings = new(comparer.Name);
            ThrowOnMultipleBuilds = throwOnMultipleBuilds;
        }

        void IAppendableConstructorMappingRepository<TRecorder, TRecorderFactory>.AddNamedMapping(string parameterName, TRecorder recorder)
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

        void IAppendableConstructorMappingRepository<TRecorder, TRecorderFactory>.AddNamedMapping(string parameterName, Func<TRecorderFactory, TRecorder> recorderDelegate)
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

        IBuiltConstructorMappingRepository<TRecorder> IBuildableMappingRepository<IBuiltConstructorMappingRepository<TRecorder>>.Build()
        {
            if (HasBeenBuilt && ThrowOnMultipleBuilds)
            {
                throw new InvalidOperationException("The repository could not be built, as it already has been - and support for multiple builds was disabled.");
            }

            HasBeenBuilt = true;

            return new BuiltConstructorMappingRepository(NamedMappings);
        }

        private void AddNamedMapping(string parameterName, TRecorder recorder)
        {
            if (HasBeenBuilt)
            {
                throw new InvalidOperationException("New mappings cannot be added to the repository, as it has already been built.");
            }

            if (NamedMappings.ContainsKey(parameterName))
            {
                throw new ArgumentException($"A recorder has already been mapped to the constructor parameter with the provided name (\"{parameterName}\").", nameof(parameterName));
            }

            NamedMappings.Add(parameterName, recorder);
        }

        private sealed class BuiltConstructorMappingRepository : IBuiltConstructorMappingRepository<TRecorder>
        {
            private readonly IReadOnlyDictionary<string, TRecorder> Named;

            public BuiltConstructorMappingRepository(IReadOnlyDictionary<string, TRecorder> named)
            {
                Named = named;
            }

            IReadOnlyDictionary<string, TRecorder> IBuiltConstructorMappingRepository<TRecorder>.Named => Named;
        }
    }
}
