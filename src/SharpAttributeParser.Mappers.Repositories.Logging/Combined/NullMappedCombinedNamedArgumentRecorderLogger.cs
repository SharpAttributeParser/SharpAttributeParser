namespace SharpAttributeParser.Mappers.Repositories.Logging.Combined;

/// <summary>A <see cref="IMappedCombinedNamedArgumentRecorderLogger{TCategoryName}"/> with no behaviour.</summary>
/// <typeparam name="TCategoryName">The name of the logging category.</typeparam>
public sealed class NullMappedCombinedNamedArgumentRecorderLogger<TCategoryName> : IMappedCombinedNamedArgumentRecorderLogger<TCategoryName>
{
    /// <summary>The singleton <see cref="NullMappedCombinedNamedArgumentRecorderLogger{TCategoryName}"/>.</summary>
    public static NullMappedCombinedNamedArgumentRecorderLogger<TCategoryName> Singleton { get; } = new();

    private NullMappedCombinedNamedArgumentRecorderLogger() { }

    void IMappedCombinedNamedArgumentRecorderLogger.NamedArgumentNotFollowingPattern() { }
}
