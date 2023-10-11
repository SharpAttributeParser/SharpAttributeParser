namespace SharpAttributeParser.Mappers.Logging;

using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;

using SharpAttributeParser.Logging;
using SharpAttributeParser.Mappers.Logging.SemanticArgumentRecorderComponents;

using System;

/// <inheritdoc cref="ISemanticArgumentRecorderLogger{TCategoryName}"/>
public sealed class SemanticArgumentRecorderLogger<TCategoryName> : ISemanticArgumentRecorderLogger<TCategoryName>
{
    private readonly ITypeArgumentLogger TypeArgument;
    private readonly IConstructorArgumentLogger ConstructorArgument;
    private readonly INamedArgumentLogger NamedArgument;

    /// <summary>Instantiates a <see cref="SemanticArgumentRecorderLogger{TCategoryName}"/>, handling logging for <see cref="ISemanticRecorder"/>.</summary>
    /// <param name="logger">The logger used to log messages.</param>
    public SemanticArgumentRecorderLogger(ILogger<TCategoryName> logger)
    {
        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        TypeArgument = new TypeArgumentLogger(logger);
        ConstructorArgument = new ConstructorArgumentLogger(logger);
        NamedArgument = new NamedArgumentLogger(logger);
    }

    ITypeArgumentLogger ISemanticArgumentRecorderLogger.TypeArgument => TypeArgument;
    IConstructorArgumentLogger ISemanticArgumentRecorderLogger.ConstructorArgument => ConstructorArgument;
    INamedArgumentLogger ISemanticArgumentRecorderLogger.NamedArgument => NamedArgument;

    private sealed class TypeArgumentLogger : ITypeArgumentLogger
    {
        private readonly ILogger Logger;

        public TypeArgumentLogger(ILogger logger)
        {
            Logger = logger;
        }

        IDisposable? ITypeArgumentLogger.BeginScopeRecordingTypeArgument(ITypeParameterSymbol parameter, ITypeSymbol argument)
        {
            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            if (argument is null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            return ScopeDefinitions.RecordingTypeArgument(Logger, parameter.Name, parameter.Ordinal, argument.Name);
        }

        void ITypeArgumentLogger.FailedToMapTypeParameterToRecorder() => MessageDefinitions.FailedToMapTypeParameterToRecorder(Logger, null);
    }

    private sealed class ConstructorArgumentLogger : IConstructorArgumentLogger
    {
        private readonly ILogger Logger;

        public ConstructorArgumentLogger(ILogger logger)
        {
            Logger = logger;
        }

        IDisposable? IConstructorArgumentLogger.BeginScopeRecordingConstructorArgument(IParameterSymbol parameter, object? argument)
        {
            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            return ScopeDefinitions.RecordingConstructorArgument(Logger, parameter.Name);
        }

        void IConstructorArgumentLogger.FailedToMapConstructorParameterToRecorder() => MessageDefinitions.FailedToMapConstructorParameterToRecorder(Logger, null);
    }

    private sealed class NamedArgumentLogger : INamedArgumentLogger
    {
        private readonly ILogger Logger;

        public NamedArgumentLogger(ILogger logger)
        {
            Logger = logger;
        }

        IDisposable? INamedArgumentLogger.BeginScopeRecordingNamedArgument(string parameterName, object? argument)
        {
            if (parameterName is null)
            {
                throw new ArgumentNullException(nameof(parameterName));
            }

            return ScopeDefinitions.RecordingNamedArgument(Logger, parameterName);
        }

        void INamedArgumentLogger.FailedToMapNamedParameterToRecorder() => MessageDefinitions.FailedToMapNamedParameterToRecorder(Logger, null);
    }

    private static class ScopeDefinitions
    {
        public static Func<ILogger, string, int, string, IDisposable?> RecordingTypeArgument { get; }
        public static Func<ILogger, string, IDisposable?> RecordingConstructorArgument { get; }
        public static Func<ILogger, string, IDisposable?> RecordingNamedArgument { get; }

        static ScopeDefinitions()
        {
            RecordingTypeArgument = LoggerMessage.DefineScope<string, int, string>("[TypeParameterName: {TypeParameterName}, TypeParameterIndex: {TypeParameterIndex}, TypeArgumentClassName: {TypeArgumentClassName}]");
            RecordingConstructorArgument = LoggerMessage.DefineScope<string>("ConstructorParameterName: {ConstructorParameterName}");
            RecordingNamedArgument = LoggerMessage.DefineScope<string>("NamedParameterName: {NamedParameterName}");
        }
    }

    private static class MessageDefinitions
    {
        public static Action<ILogger, Exception?> FailedToMapTypeParameterToRecorder { get; }
        public static Action<ILogger, Exception?> FailedToMapConstructorParameterToRecorder { get; }
        public static Action<ILogger, Exception?> FailedToMapNamedParameterToRecorder { get; }

        static MessageDefinitions()
        {
            FailedToMapTypeParameterToRecorder = LoggerMessage.Define(LogLevel.Debug, EventIDs.FailedToMapTypeParameterToRecorder, "Failed to record a type argument, as the type parameter could not be mapped to a recorder.");
            FailedToMapConstructorParameterToRecorder = LoggerMessage.Define(LogLevel.Debug, EventIDs.FailedToMapConstructorParameterToRecorder, "Failed to record a constructor argument, as the constructor parameter could not be mapped to a recorder.");
            FailedToMapNamedParameterToRecorder = LoggerMessage.Define(LogLevel.Debug, EventIDs.FailedToMapNamedParameterToRecorder, "Failed to record a named argument, as the named parameter could not be mapped to a recorder.");
        }
    }

    private static class EventIDs
    {
        public static EventId FailedToMapTypeParameterToRecorder { get; }
        public static EventId FailedToMapConstructorParameterToRecorder { get; }
        public static EventId FailedToMapNamedParameterToRecorder { get; }

        static EventIDs()
        {
            SequentialEventID eventIDs = new();

            FailedToMapTypeParameterToRecorder = new(eventIDs.Next, nameof(FailedToMapTypeParameterToRecorder));
            FailedToMapConstructorParameterToRecorder = new(eventIDs.Next, nameof(FailedToMapConstructorParameterToRecorder));
            FailedToMapNamedParameterToRecorder = new(eventIDs.Next, nameof(FailedToMapNamedParameterToRecorder));
        }
    }
}
