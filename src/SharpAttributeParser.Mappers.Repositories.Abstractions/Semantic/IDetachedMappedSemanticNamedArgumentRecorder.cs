namespace SharpAttributeParser.Mappers.Repositories.Semantic;

/// <summary>Records the arguments of some named parameter to provided records.</summary>
/// <typeparam name="TRecord">The type to which arguments are recorded.</typeparam>
public interface IDetachedMappedSemanticNamedArgumentRecorder<in TRecord>
{
    /// <summary>Attempts to record an argument of some named parameter.</summary>
    /// <param name="dataRecord">The record to which the argument is recorded.</param>
    /// <param name="argument">The argument of the named parameter.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    public abstract bool TryRecordArgument(TRecord dataRecord, object? argument);
}
