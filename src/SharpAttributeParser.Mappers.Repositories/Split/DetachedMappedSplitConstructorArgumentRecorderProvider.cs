namespace SharpAttributeParser.Mappers.Repositories.Split;

using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Mappers.Repositories.Syntactic;

/// <inheritdoc cref="IDetachedMappedSplitConstructorArgumentRecorderProvider{TSemanticRecord, TSyntacticRecord}"/>
internal sealed class DetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord> : IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>
{
    private IDetachedMappedSemanticConstructorArgumentRecorder<TSemanticRecord> Semantic { get; }
    private IDetachedMappedSyntacticConstructorArgumentRecorder<TSyntacticRecord> Syntactic { get; }

    /// <summary>Instantiates a <see cref="DetachedMappedSplitConstructorArgumentRecorderProvider{TSemanticRecord, TSyntacticRecord}"/>, providing recorders that record the arguments of some constructor parameter.</summary>
    /// <param name="semantic">The recorder used when arguments are parsed.</param>
    /// <param name="syntactic">The recorder used when syntactic information about arguments is extracted.</param>
    public DetachedMappedSplitConstructorArgumentRecorderProvider(IDetachedMappedSemanticConstructorArgumentRecorder<TSemanticRecord> semantic, IDetachedMappedSyntacticConstructorArgumentRecorder<TSyntacticRecord> syntactic)
    {
        Semantic = semantic;
        Syntactic = syntactic;
    }

    IDetachedMappedSemanticConstructorArgumentRecorder<TSemanticRecord> IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>.Semantic => Semantic;
    IDetachedMappedSyntacticConstructorArgumentRecorder<TSyntacticRecord> IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>.Syntactic => Syntactic;
}
