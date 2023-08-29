namespace SharpAttributeParser.Mappers.Repositories.Logging.Semantic;

/// <summary>A <see cref="IMappedSemanticNamedArgumentRecorderLogger{TCategoryName}"/> with no behaviour.</summary>
/// <typeparam name="TCategoryName">The name of the logging category.</typeparam>
public sealed class NullMappedSemanticNamedArgumentRecorderLogger<TCategoryName> : IMappedSemanticNamedArgumentRecorderLogger<TCategoryName>
{
    /// <summary>The singleton <see cref="NullMappedSemanticNamedArgumentRecorderLogger{TCategoryName}"/>.</summary>
    public static NullMappedSemanticNamedArgumentRecorderLogger<TCategoryName> Singleton { get; } = new();

    private NullMappedSemanticNamedArgumentRecorderLogger() { }

    void IMappedSemanticNamedArgumentRecorderLogger.NamedArgumentNotFollowingPattern() { }
}
