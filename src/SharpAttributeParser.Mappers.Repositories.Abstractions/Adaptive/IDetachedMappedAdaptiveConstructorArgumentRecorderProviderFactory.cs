namespace SharpAttributeParser.Mappers.Repositories.Adaptive;

/// <summary>Handles creation of <see cref="IDetachedMappedAdaptiveConstructorArgumentRecorderProvider{TCombinedRecord, TSemanticRecord}"/>.</summary>
/// <typeparam name="TCombinedRecord">The type to which arguments are recorded when parsed with syntactic conctext.</typeparam>
/// <typeparam name="TSemanticRecord">The type to which arguments are recorded when parsed without syntactic conctext.</typeparam>
public interface IDetachedMappedAdaptiveConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>
{
    /// <summary>Handles creation of recorders related to non-optional, non-<see langword="params"/> constructor parameters.</summary>
    public abstract IDetachedMappedAdaptiveNormalConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> Normal { get; }

    /// <summary>Handles creation of recorders related to <see langword="params"/> constructor parameters.</summary>
    public abstract IDetachedMappedAdaptiveParamsConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> Params { get; }

    /// <summary>Handles creation of recorders related to optional constructor parameters.</summary>
    public abstract IDetachedMappedAdaptiveOptionalConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> Optional { get; }
}
