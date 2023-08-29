namespace SharpAttributeParser.Mappers.Logging;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;

using SharpAttributeParser.Logging;
using SharpAttributeParser.Mappers.Logging.CombinedArgumentRecorderComponents;

using System;
using System.Collections.Generic;

/// <inheritdoc cref="ICombinedArgumentRecorderLogger{TCategoryName}"/>
public sealed class CombinedArgumentRecorderLogger<TCategoryName> : ICombinedArgumentRecorderLogger<TCategoryName>
{
    private ITypeArgumentLogger TypeArgument { get; }
    private IConstructorArgumentLogger ConstructorArgument { get; }
    private INamedArgumentLogger NamedArgument { get; }

    /// <summary>Instantiates a <see cref="CombinedArgumentRecorderLogger{TCategoryName}"/>, handling logging for <see cref="ICombinedRecorder"/>.</summary>
    /// <param name="logger">The logger used to log messages.</param>
    /// <exception cref="ArgumentNullException"/>
    public CombinedArgumentRecorderLogger(ILogger<TCategoryName> logger)
    {
        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        TypeArgument = new TypeArgumentLogger(logger);
        ConstructorArgument = new ConstructorArgumentLogger(logger);
        NamedArgument = new NamedArgumentLogger(logger);
    }

    ITypeArgumentLogger ICombinedArgumentRecorderLogger.TypeArgument => TypeArgument;
    IConstructorArgumentLogger ICombinedArgumentRecorderLogger.ConstructorArgument => ConstructorArgument;
    INamedArgumentLogger ICombinedArgumentRecorderLogger.NamedArgument => NamedArgument;

    private sealed class TypeArgumentLogger : ITypeArgumentLogger
    {
        private ILogger Logger { get; }

        public TypeArgumentLogger(ILogger logger)
        {
            Logger = logger;
        }

        IDisposable? ITypeArgumentLogger.BeginScopeRecordingTypeArgument(ITypeParameterSymbol parameter, ITypeSymbol argument, ExpressionSyntax syntax)
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
        private ILogger Logger { get; }

        public ConstructorArgumentLogger(ILogger logger)
        {
            Logger = logger;
        }

        IDisposable? IConstructorArgumentLogger.BeginScopeRecordingNormalConstructorlArgument(IParameterSymbol parameter, object? argument, ExpressionSyntax syntax) => BeginScopeRecordingConstructorArgument(parameter);
        IDisposable? IConstructorArgumentLogger.BeginScopeRecordingParamsConstructorArgument(IParameterSymbol parameter, object? argument, IReadOnlyList<ExpressionSyntax> elementSyntax) => BeginScopeRecordingConstructorArgument(parameter);
        IDisposable? IConstructorArgumentLogger.BeginScopeRecordingDefaultConstructorArgument(IParameterSymbol parameter, object? argument) => BeginScopeRecordingConstructorArgument(parameter);

        void IConstructorArgumentLogger.FailedToMapConstructorParameterToRecorder() => MessageDefinitions.FailedToMapConstructorParameterToRecorder(Logger, null);

        private IDisposable? BeginScopeRecordingConstructorArgument(IParameterSymbol parameter)
        {
            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            return ScopeDefinitions.RecordingConstructorArgument(Logger, parameter.Name);
        }
    }

    private sealed class NamedArgumentLogger : INamedArgumentLogger
    {
        private ILogger Logger { get; }

        public NamedArgumentLogger(ILogger logger)
        {
            Logger = logger;
        }

        IDisposable? INamedArgumentLogger.BeginScopeRecordingNamedArgument(string parameterName, object? argument, ExpressionSyntax syntax)
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
