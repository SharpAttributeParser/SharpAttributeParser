namespace SharpAttributeParser.Mappers.Repositories.Split;

/// <summary>Handles creation of <see cref="IDetachedMappedSplitConstructorArgumentRecorderProvider{TSemanticRecord, TSyntacticRecord}"/>.</summary>
/// <typeparam name="TSemanticRecord">The type to which arguments are recorded.</typeparam>
/// <typeparam name="TSyntacticRecord">The type to which syntactic information about arguments is recorded.</typeparam>
public interface IDetachedMappedSplitConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>
{
    /// <summary>Handles creation of recorders related to non-optional, non-<see langword="params"/> constructor parameters.</summary>
    public abstract IDetachedMappedSplitNormalConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> Normal { get; }

    /// <summary>Handles creation of recorders related to <see langword="params"/> constructor parameters.</summary>
    public abstract IDetachedMappedSplitParamsConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> Params { get; }

    /// <summary>Handles creation of recorders related to optional constructor parameters.</summary>
    public abstract IDetachedMappedSplitOptionalConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> Optional { get; }
}
