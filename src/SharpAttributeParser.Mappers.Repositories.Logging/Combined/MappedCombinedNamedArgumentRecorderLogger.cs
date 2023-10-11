namespace SharpAttributeParser.Mappers.Repositories.Logging.Combined;

using Microsoft.Extensions.Logging;

using SharpAttributeParser.Logging;
using SharpAttributeParser.Mappers.Repositories.Combined;

using System;

/// <inheritdoc cref="IMappedCombinedNamedArgumentRecorderLogger{TCategoryName}"/>
public sealed class MappedCombinedNamedArgumentRecorderLogger<TCategoryName> : IMappedCombinedNamedArgumentRecorderLogger<TCategoryName>
{
    private readonly ILogger Logger;

    /// <summary>Instantiates a <see cref="MappedCombinedNamedArgumentRecorderLogger{TCategoryName}"/>, handling logging for <see cref="IDetachedMappedCombinedNamedArgumentRecorder{TRecord}"/>.</summary>
    /// <param name="logger">The logger used to log messages.</param>
    public MappedCombinedNamedArgumentRecorderLogger(ILogger<TCategoryName> logger)
    {
        Logger = logger;
    }

    void IMappedCombinedNamedArgumentRecorderLogger.NamedArgumentNotFollowingPattern() => MessageDefinitions.NamedArgumentNotFollowingPattern(Logger, null);

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
