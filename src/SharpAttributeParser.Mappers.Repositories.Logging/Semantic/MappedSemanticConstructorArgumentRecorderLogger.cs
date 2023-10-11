namespace SharpAttributeParser.Mappers.Repositories.Logging.Semantic;

using Microsoft.Extensions.Logging;

using SharpAttributeParser.Logging;
using SharpAttributeParser.Mappers.Repositories.Semantic;

using System;

/// <inheritdoc cref="IMappedSemanticConstructorArgumentRecorderLogger{TCategoryName}"/>
public sealed class MappedSemanticConstructorArgumentRecorderLogger<TCategoryName> : IMappedSemanticConstructorArgumentRecorderLogger<TCategoryName>
{
    private readonly ILogger Logger;

    /// <summary>Instantiates a <see cref="MappedSemanticConstructorArgumentRecorderLogger{TCategoryName}"/>, handling logging for <see cref="IDetachedMappedSemanticConstructorArgumentRecorder{TRecord}"/>.</summary>
    /// <param name="logger">The logger used to log messages.</param>
    public MappedSemanticConstructorArgumentRecorderLogger(ILogger<TCategoryName> logger)
    {
        Logger = logger;
    }

    void IMappedSemanticConstructorArgumentRecorderLogger.ConstructorArgumentNotFollowingPattern() => MessageDefinitions.ConstructorArgumentNotFollowingPattern(Logger, null);

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
