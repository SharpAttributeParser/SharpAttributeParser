namespace SharpAttributeParser.Mappers.Repositories.Adaptive;

using SharpAttributeParser.Mappers.Repositories.Combined;
using SharpAttributeParser.Mappers.Repositories.Semantic;

internal sealed class DetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord> : IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>
{
    private readonly IDetachedMappedCombinedConstructorArgumentRecorder<TCombinedRecord> Combined;
    private readonly IDetachedMappedSemanticConstructorArgumentRecorder<TSemanticRecord> Semantic;

    public DetachedMappedAdaptiveConstructorArgumentRecorderProvider(IDetachedMappedCombinedConstructorArgumentRecorder<TCombinedRecord> combined, IDetachedMappedSemanticConstructorArgumentRecorder<TSemanticRecord> semantic)
    {
        Combined = combined;
        Semantic = semantic;
    }

    IDetachedMappedCombinedConstructorArgumentRecorder<TCombinedRecord> IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>.Combined => Combined;
    IDetachedMappedSemanticConstructorArgumentRecorder<TSemanticRecord> IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>.Semantic => Semantic;
}
