namespace SharpAttributeParser.Mappers.Repositories.Adaptive;

using SharpAttributeParser.Mappers.Repositories.Combined;
using SharpAttributeParser.Mappers.Repositories.Semantic;

/// <summary>Provides recorders that record the arguments of some named parameter to provided records.</summary>
/// <typeparam name="TCombinedRecord">The type to which arguments are recorded when parsed with syntactic context.</typeparam>
/// <typeparam name="TSemanticRecord">The type to which arguments are recorded when parsed without syntactic context.</typeparam>
public interface IDetachedMappedAdaptiveNamedArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>
{
    /// <summary>The recorder used when arguments are parsed with syntactic context.</summary>
    public abstract IDetachedMappedCombinedNamedArgumentRecorder<TCombinedRecord> Combined { get; }

    /// <summary>The recorder used when arguments are parsed without syntactic context.</summary>
    public abstract IDetachedMappedSemanticNamedArgumentRecorder<TSemanticRecord> Semantic { get; }
}
