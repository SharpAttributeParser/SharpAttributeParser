namespace SharpAttributeParser.Mappers.Repositories.Adaptive;

using SharpAttributeParser.Mappers.Repositories.Combined;
using SharpAttributeParser.Mappers.Repositories.Semantic;

/// <inheritdoc cref="IDetachedMappedAdaptiveConstructorArgumentRecorderProvider{TCombinedRecord, TSemanticRecord}"/>
internal sealed class DetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord> : IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>
{
    private IDetachedMappedCombinedConstructorArgumentRecorder<TCombinedRecord> Combined { get; }
    private IDetachedMappedSemanticConstructorArgumentRecorder<TSemanticRecord> Semantic { get; }

    /// <summary>Instantiates a <see cref="DetachedMappedAdaptiveConstructorArgumentRecorderProvider{TCombinedRecord, TSemanticRecord}"/>, providing recorders that record the arguments of some constructor parameter.</summary>
    /// <param name="combined">The recorder used when arguments are parsed with syntactic context.</param>
    /// <param name="semantic">The recorder used when arguments are parsed without syntactic context.</param>
    public DetachedMappedAdaptiveConstructorArgumentRecorderProvider(IDetachedMappedCombinedConstructorArgumentRecorder<TCombinedRecord> combined, IDetachedMappedSemanticConstructorArgumentRecorder<TSemanticRecord> semantic)
    {
        Combined = combined;
        Semantic = semantic;
    }

    IDetachedMappedCombinedConstructorArgumentRecorder<TCombinedRecord> IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>.Combined => Combined;
    IDetachedMappedSemanticConstructorArgumentRecorder<TSemanticRecord> IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>.Semantic => Semantic;
}
