namespace SharpAttributeParser.Mappers.Repositories.Logging.Combined;

using Microsoft.Extensions.Logging;

using SharpAttributeParser.Logging;
using SharpAttributeParser.Mappers.Repositories.Combined;

using System;

/// <inheritdoc cref="IMappedCombinedConstructorArgumentRecorderLogger{TCategoryName}"/>
public sealed class MappedCombinedConstructorArgumentRecorderLogger<TCategoryName> : IMappedCombinedConstructorArgumentRecorderLogger<TCategoryName>
{
    private ILogger Logger { get; }

    /// <summary>Instantiates a <see cref="MappedCombinedConstructorArgumentRecorderLogger{TCategoryName}"/>, handling logging for <see cref="IDetachedMappedCombinedConstructorArgumentRecorder{TRecord}"/>.</summary>
    /// <param name="logger">The logger used to log messages.</param>
    public MappedCombinedConstructorArgumentRecorderLogger(ILogger<TCategoryName> logger)
    {
        Logger = logger;
    }

    void IMappedCombinedConstructorArgumentRecorderLogger.ConstructorArgumentNotFollowingPattern() => MessageDefinitions.ConstructorArgumentNotFollowingPattern(Logger, null);

    private static class MessageDefinitions
    {
        public static Action<ILogger, Exception?> ConstructorArgumentNotFollowingPattern { get; }

        static MessageDefinitions()
        {
            ConstructorArgumentNotFollowingPattern = LoggerMessage.Define(LogLevel.Debug, EventIDs.ConstructorArgumentNotFollowingPattern, "Failed to record a constructor argument, as the argument did not follow the specified pattern.");
        }
    }

    private static class EventIDs
    {
        public static EventId ConstructorArgumentNotFollowingPattern { get; }

        static EventIDs()
        {
            SequentialEventID eventID = new();

            ConstructorArgumentNotFollowingPattern = new(eventID.Next, nameof(ConstructorArgumentNotFollowingPattern));
        }
    }
}
