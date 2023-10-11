namespace SharpAttributeParser.Mappers.Repositories.Adaptive;

using System;

/// <inheritdoc cref="IDetachedMappedAdaptiveConstructorArgumentRecorderProviderFactory{TSyntacticRecord, TSemanticRecord}"/>
public sealed class DetachedMappedAdaptiveConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> : IDetachedMappedAdaptiveConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>
{
    private readonly IDetachedMappedAdaptiveNormalConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> NormalFactory;
    private readonly IDetachedMappedAdaptiveParamsConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> ParamsFactory;
    private readonly IDetachedMappedAdaptiveOptionalConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> OptionalFactory;

    /// <summary>Instantiates a <see cref="DetachedMappedAdaptiveConstructorArgumentRecorderProviderFactory{TCombinedRecord, TSemanticRecord}"/>, handling creation of <see cref="IDetachedMappedAdaptiveConstructorArgumentRecorderProvider{TCombinedRecord, TSemanticRecord}"/>.</summary>
    /// <param name="normalFactory">Handles creation of recorders related to non-optional, non-<see langword="params"/> constructor parameters.</param>
    /// <param name="paramsFactory">Handles creation of recorders related to <see langword="params"/> constructor parameters.</param>
    /// <param name="optionalFactory">Handles creation of recorders related to optional constructor parameters.</param>
    public DetachedMappedAdaptiveConstructorArgumentRecorderProviderFactory(IDetachedMappedAdaptiveNormalConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> normalFactory, IDetachedMappedAdaptiveParamsConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> paramsFactory, IDetachedMappedAdaptiveOptionalConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> optionalFactory)
    {
        NormalFactory = normalFactory ?? throw new ArgumentNullException(nameof(normalFactory));
        ParamsFactory = paramsFactory ?? throw new ArgumentNullException(nameof(paramsFactory));
        OptionalFactory = optionalFactory ?? throw new ArgumentNullException(nameof(optionalFactory));
    }

    IDetachedMappedAdaptiveNormalConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> IDetachedMappedAdaptiveConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>.Normal => NormalFactory;
    IDetachedMappedAdaptiveParamsConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> IDetachedMappedAdaptiveConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>.Params => ParamsFactory;
    IDetachedMappedAdaptiveOptionalConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> IDetachedMappedAdaptiveConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>.Optional => OptionalFactory;
}
