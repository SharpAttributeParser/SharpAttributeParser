﻿namespace SharpAttributeParser.Mappers.Repositories;

using System;
using System.Collections.Generic;

/// <inheritdoc cref="INamedMappingRepositoryFactory{TRecorder, TRecorderFactory}"/>
public sealed class NamedMappingRepositoryFactory<TRecorder, TRecorderFactory> : INamedMappingRepositoryFactory<TRecorder, TRecorderFactory>
{
    private TRecorderFactory RecorderFactory { get; }

    /// <summary>Instantiates a <see cref="NamedMappingRepositoryFactory{TRecorder, TRecorderFactory}"/>, handling creation of <see cref="INamedMappingRepository{TRecorder, TRecorderFactory}"/>.</summary>
    /// <param name="recorderFactory">Handles creation of recorders for the created repositories.</param>
    /// <exception cref="ArgumentNullException"/>
    public NamedMappingRepositoryFactory(TRecorderFactory recorderFactory)
    {
        RecorderFactory = recorderFactory ?? throw new ArgumentNullException(nameof(recorderFactory));
    }

    INamedMappingRepository<TRecorder, TRecorderFactory> INamedMappingRepositoryFactory<TRecorder, TRecorderFactory>.Create(IEqualityComparer<string> parameterNameComparer, bool throwOnMultipleBuilds)
    {
        if (parameterNameComparer is null)
        {
            throw new ArgumentNullException(nameof(parameterNameComparer));
        }

        return new NamedMappingRepository(RecorderFactory, parameterNameComparer, throwOnMultipleBuilds);
    }

    private sealed class NamedMappingRepository : INamedMappingRepository<TRecorder, TRecorderFactory>
    {
        private TRecorderFactory RecorderFactory { get; }

        private Dictionary<string, TRecorder> NamedMappings { get; }

        private bool HasBeenBuilt { get; set; }
        private bool ThrowOnMultipleBuilds { get; }

        public NamedMappingRepository(TRecorderFactory recorderFactory, IEqualityComparer<string> parameterNameComparer, bool throwOnMultipleBuilds)
        {
            RecorderFactory = recorderFactory;

            NamedMappings = new(parameterNameComparer);
            ThrowOnMultipleBuilds = throwOnMultipleBuilds;
        }

        void IAppendableNamedMappingRepository<TRecorder, TRecorderFactory>.AddNamedMapping(string parameterName, TRecorder recorder)
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

        void IAppendableNamedMappingRepository<TRecorder, TRecorderFactory>.AddNamedMapping(string parameterName, Func<TRecorderFactory, TRecorder> recorderDelegate)
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

        IBuiltNamedMappingRepository<TRecorder> IBuildableMappingRepository<IBuiltNamedMappingRepository<TRecorder>>.Build()
        {
            if (HasBeenBuilt && ThrowOnMultipleBuilds)
            {
                throw new InvalidOperationException("The repository could not be built, as it already has been - and support for multiple builds was disabled.");
            }

            HasBeenBuilt = true;

            return new BuiltNamedMappingRepository(NamedMappings);
        }

        private void AddNamedMapping(string parameterName, TRecorder recorder)
        {
            if (HasBeenBuilt)
            {
                throw new InvalidOperationException("New mappings cannot be added to hte repository, as it has already been built.");
            }

            if (NamedMappings.ContainsKey(parameterName))
            {
                throw new ArgumentException($"A recorder has already been mapped to the named parameter with the provided name (\"{parameterName}\").", nameof(parameterName));
            }

            NamedMappings.Add(parameterName, recorder);
        }

        private sealed class BuiltNamedMappingRepository : IBuiltNamedMappingRepository<TRecorder>
        {
            private IReadOnlyDictionary<string, TRecorder> Named { get; }

            public BuiltNamedMappingRepository(IReadOnlyDictionary<string, TRecorder> named)
            {
                Named = named;
            }

            IReadOnlyDictionary<string, TRecorder> IBuiltNamedMappingRepository<TRecorder>.Named => Named;
        }
    }
}
