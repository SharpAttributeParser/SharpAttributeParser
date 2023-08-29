namespace SharpAttributeParser.Mappers.Repositories.Syntactic;

/// <summary>Handles creation of <see cref="IDetachedMappedSyntacticConstructorArgumentRecorder{TRecord}"/>.</summary>
/// <typeparam name="TRecord">The type to which syntactic information about arguments is recorded.</typeparam>
public interface IDetachedMappedSyntacticConstructorArgumentRecorderFactory<TRecord>
{
    /// <summary>Handles creation of recorders related to non-optional, non-<see langword="params"/> constructor parameters.</summary>
    public abstract IDetachedMappedSyntacticNormalConstructorArgumentRecorderFactory<TRecord> Normal { get; }

    /// <summary>Handles creation of recorders related to <see langword="params"/> constructor parameters.</summary>
    public abstract IDetachedMappedSyntacticParamsConstructorArgumentRecorderFactory<TRecord> Params { get; }

    /// <summary>Handles creation of recorders related to optional constructor parameters.</summary>
    public abstract IDetachedMappedSyntacticOptionalConstructorArgumentRecorderFactory<TRecord> Optional { get; }
}
