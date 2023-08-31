namespace SharpAttributeParser.Mappers.Repositories.Semantic;

/// <summary>Records the arguments of some constructor parameter to provided records.</summary>
/// <typeparam name="TRecord">The type to which arguments are recorded.</typeparam>
public interface IDetachedMappedSemanticConstructorArgumentRecorder<in TRecord>
{
    /// <summary>Attempts to record an argument of some constructor parameter.</summary>
    /// <param name="dataRecord">The record to which the argument is recorded.</param>
    /// <param name="argument">The argument of the constructor parameter.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    public abstract bool TryRecordArgument(TRecord dataRecord, object? argument);
}
