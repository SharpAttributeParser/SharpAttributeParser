namespace SharpAttributeParser.Mappers.Repositories.Logging.Semantic;

/// <summary>A <see cref="IMappedSemanticConstructorArgumentRecorderLogger{TCategoryName}"/> with no behaviour.</summary>
/// <typeparam name="TCategoryName">The name of the logging category.</typeparam>
public sealed class NullMappedSemanticConstructorArgumentRecorderLogger<TCategoryName> : IMappedSemanticConstructorArgumentRecorderLogger<TCategoryName>
{
    /// <summary>The singleton <see cref="NullMappedSemanticConstructorArgumentRecorderLogger{TCategoryName}"/>.</summary>
    public static NullMappedSemanticConstructorArgumentRecorderLogger<TCategoryName> Singleton { get; } = new();

    private NullMappedSemanticConstructorArgumentRecorderLogger() { }

    void IMappedSemanticConstructorArgumentRecorderLogger.ConstructorArgumentNotFollowingPattern() { }
}
