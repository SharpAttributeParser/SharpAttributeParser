namespace SharpAttributeParser.Mappers.Repositories.Split;

using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Mappers.Repositories.Syntactic;

internal sealed class DetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord> : IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>
{
    private readonly IDetachedMappedSemanticConstructorArgumentRecorder<TSemanticRecord> Semantic;
    private readonly IDetachedMappedSyntacticConstructorArgumentRecorder<TSyntacticRecord> Syntactic;

    public DetachedMappedSplitConstructorArgumentRecorderProvider(IDetachedMappedSemanticConstructorArgumentRecorder<TSemanticRecord> semantic, IDetachedMappedSyntacticConstructorArgumentRecorder<TSyntacticRecord> syntactic)
    {
        Semantic = semantic;
        Syntactic = syntactic;
    }

    IDetachedMappedSemanticConstructorArgumentRecorder<TSemanticRecord> IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>.Semantic => Semantic;
    IDetachedMappedSyntacticConstructorArgumentRecorder<TSyntacticRecord> IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>.Syntactic => Syntactic;
}
