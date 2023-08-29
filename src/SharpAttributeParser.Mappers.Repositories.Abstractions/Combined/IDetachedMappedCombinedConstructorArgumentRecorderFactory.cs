namespace SharpAttributeParser.Mappers.Repositories.Combined;

/// <summary>Handles creation of <see cref="IDetachedMappedCombinedConstructorArgumentRecorder{TRecord}"/>.</summary>
/// <typeparam name="TRecord">The type to which arguments are recorded.</typeparam>
public interface IDetachedMappedCombinedConstructorArgumentRecorderFactory<TRecord>
{
    /// <summary>Handles creation of recorders related to non-optional, non-<see langword="params"/> constructor parameters.</summary>
    public abstract IDetachedMappedCombinedNormalConstructorArgumentRecorderFactory<TRecord> Normal { get; }

    /// <summary>Handles creation of recorders related to <see langword="params"/> constructor parameters.</summary>
    public abstract IDetachedMappedCombinedParamsConstructorArgumentRecorderFactory<TRecord> Params { get; }

    /// <summary>Handles creation of recorders related to optional constructor parameters.</summary>
    public abstract IDetachedMappedCombinedOptionalConstructorArgumentRecorderFactory<TRecord> Optional { get; }
}
