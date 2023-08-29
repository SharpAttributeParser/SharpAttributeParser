namespace SharpAttributeParser.Mappers.Repositories.Logging.Combined;

/// <summary>A <see cref="IMappedCombinedConstructorArgumentRecorderLogger{TCategoryName}"/> with no behaviour.</summary>
/// <typeparam name="TCategoryName">The name of the logging category.</typeparam>
public sealed class NullMappedCombinedConstructorArgumentRecorderLogger<TCategoryName> : IMappedCombinedConstructorArgumentRecorderLogger<TCategoryName>
{
    /// <summary>The singleton <see cref="NullMappedCombinedConstructorArgumentRecorderLogger{TCategoryName}"/>.</summary>
    public static NullMappedCombinedConstructorArgumentRecorderLogger<TCategoryName> Singleton { get; } = new();

    private NullMappedCombinedConstructorArgumentRecorderLogger() { }

    void IMappedCombinedConstructorArgumentRecorderLogger.ConstructorArgumentNotFollowingPattern() { }
}
