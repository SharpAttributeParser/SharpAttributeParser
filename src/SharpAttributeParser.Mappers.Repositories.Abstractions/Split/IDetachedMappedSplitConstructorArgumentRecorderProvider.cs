namespace SharpAttributeParser.Mappers.Repositories.Split;

using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Mappers.Repositories.Syntactic;

/// <summary>Provides recorders that record the arguments of some constructor parameter to provided records.</summary>
/// <typeparam name="TSemanticRecord">The type to which arguments are recorded.</typeparam>
/// <typeparam name="TSyntacticRecord">The type to which syntactic information about arguments is recorded.</typeparam>
public interface IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>
{
    /// <summary>The recorder used when arguments are parsed.</summary>
    public abstract IDetachedMappedSemanticConstructorArgumentRecorder<TSemanticRecord> Semantic { get; }

    /// <summary>The recorder used when extracting syntactic information about arguments.</summary>
    public abstract IDetachedMappedSyntacticConstructorArgumentRecorder<TSyntacticRecord> Syntactic { get; }
}
