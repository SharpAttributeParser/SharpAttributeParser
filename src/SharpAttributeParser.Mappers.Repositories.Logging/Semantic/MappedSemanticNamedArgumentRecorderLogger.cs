namespace SharpAttributeParser.Mappers.Repositories.Logging.Semantic;

using Microsoft.Extensions.Logging;

using SharpAttributeParser.Logging;
using SharpAttributeParser.Mappers.Repositories.Semantic;

using System;

/// <inheritdoc cref="IMappedSemanticNamedArgumentRecorderLogger{TCategoryName}"/>
public sealed class MappedSemanticNamedArgumentRecorderLogger<TCategoryName> : IMappedSemanticNamedArgumentRecorderLogger<TCategoryName>
{
    private ILogger Logger { get; }

    /// <summary>Instantiates a <see cref="MappedSemanticNamedArgumentRecorderLogger{TCategoryName}"/>, handling logging for <see cref="IDetachedMappedSemanticNamedArgumentRecorder{TRecord}"/>.</summary>
    /// <param name="logger">The logger used to log messages.</param>
    /// <exception cref="ArgumentNullException"/>
    public MappedSemanticNamedArgumentRecorderLogger(ILogger<TCategoryName> logger)
    {
        Logger = logger;
    }

    void IMappedSemanticNamedArgumentRecorderLogger.NamedArgumentNotFollowingPattern() => MessageDefinitions.NamedArgumentNotFollowingPattern(Logger, null);

    private static class MessageDefinitions
    {
        public static Action<ILogger, Exception?> NamedArgumentNotFollowingPattern { get; }

        static MessageDefinitions()
        {
            NamedArgumentNotFollowingPattern = LoggerMessage.Define(LogLevel.Debug, EventIDs.NamedArgumentNotFollowingPattern, "Failed to record a named argument, as the argument did not follow the specified pattern.");
        }
    }

    private static class EventIDs
    {
        public static EventId NamedArgumentNotFollowingPattern { get; }

        static EventIDs()
        {
            SequentialEventID eventID = new();

            NamedArgumentNotFollowingPattern = new(eventID.Next, nameof(NamedArgumentNotFollowingPattern));
        }
    }
}
