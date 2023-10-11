namespace SharpAttributeParser.Mappers.Repositories.Combined;

using System;

/// <inheritdoc cref="IDetachedMappedCombinedConstructorArgumentRecorderFactory{TRecord}"/>
public sealed class DetachedMappedCombinedConstructorArgumentRecorderFactory<TRecord> : IDetachedMappedCombinedConstructorArgumentRecorderFactory<TRecord>
{
    private readonly IDetachedMappedCombinedNormalConstructorArgumentRecorderFactory<TRecord> Normal;
    private readonly IDetachedMappedCombinedParamsConstructorArgumentRecorderFactory<TRecord> Params;
    private readonly IDetachedMappedCombinedOptionalConstructorArgumentRecorderFactory<TRecord> Optional;

    /// <summary>Instantiates a <see cref="DetachedMappedCombinedConstructorArgumentRecorderFactory{TRecord}"/>, handling creation of <see cref="IDetachedMappedCombinedConstructorArgumentRecorder{TRecord}"/>.</summary>
    /// <param name="normalFactory">Handles creation of recorders related to non-optional, non-<see langword="params"/> constructor parameters.</param>
    /// <param name="paramsFactory">Handles creation of recorders related to <see langword="params"/> constructor parameters.</param>
    /// <param name="optionalFactory">Handles creation of recorders related to optional constructor parameters.</param>
    public DetachedMappedCombinedConstructorArgumentRecorderFactory(IDetachedMappedCombinedNormalConstructorArgumentRecorderFactory<TRecord> normalFactory, IDetachedMappedCombinedParamsConstructorArgumentRecorderFactory<TRecord> paramsFactory, IDetachedMappedCombinedOptionalConstructorArgumentRecorderFactory<TRecord> optionalFactory)
    {
        Normal = normalFactory ?? throw new ArgumentNullException(nameof(normalFactory));
        Params = paramsFactory ?? throw new ArgumentNullException(nameof(paramsFactory));
        Optional = optionalFactory ?? throw new ArgumentNullException(nameof(optionalFactory));
    }

    IDetachedMappedCombinedNormalConstructorArgumentRecorderFactory<TRecord> IDetachedMappedCombinedConstructorArgumentRecorderFactory<TRecord>.Normal => Normal;
    IDetachedMappedCombinedParamsConstructorArgumentRecorderFactory<TRecord> IDetachedMappedCombinedConstructorArgumentRecorderFactory<TRecord>.Params => Params;
    IDetachedMappedCombinedOptionalConstructorArgumentRecorderFactory<TRecord> IDetachedMappedCombinedConstructorArgumentRecorderFactory<TRecord>.Optional => Optional;
}
