namespace SharpAttributeParser.Mappers.Logging;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;

using SharpAttributeParser.Logging;
using SharpAttributeParser.Mappers.Logging.SyntacticArgumentRecorderComponents;

using System;
using System.Collections.Generic;

/// <inheritdoc cref="ISyntacticArgumentRecorderLogger{TCategoryName}"/>
public sealed class SyntacticArgumentRecorderLogger<TCategoryName> : ISyntacticArgumentRecorderLogger<TCategoryName>
{
    private ITypeArgumentsLogger TypeArgument { get; }
    private IConstructorArgumentsLogger ConstructorArgument { get; }
    private INamedArgumentsLogger NamedArgument { get; }

    /// <summary>Instantiates a <see cref="SyntacticArgumentRecorderLogger{TCategoryName}"/>, handling logging for <see cref="ISyntacticRecorder"/>.</summary>
    /// <param name="logger">The logger used to log messages.</param>
    public SyntacticArgumentRecorderLogger(ILogger<TCategoryName> logger)
    {
        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        TypeArgument = new TypeArgumentLogger(logger);
        ConstructorArgument = new ConstructorArgumentLogger(logger);
        NamedArgument = new NamedArgumentLogger(logger);
    }

    ITypeArgumentsLogger ISyntacticArgumentRecorderLogger.TypeArgument => TypeArgument;
    IConstructorArgumentsLogger ISyntacticArgumentRecorderLogger.ConstructorArgument => ConstructorArgument;
    INamedArgumentsLogger ISyntacticArgumentRecorderLogger.NamedArgument => NamedArgument;

    private sealed class TypeArgumentLogger : ITypeArgumentsLogger
    {
        private ILogger Logger { get; }

        public TypeArgumentLogger(ILogger logger)
        {
            Logger = logger;
        }

        IDisposable? ITypeArgumentsLogger.BeginScopeRecordingTypeArgument(ITypeParameterSymbol parameter, ExpressionSyntax syntax)
        {
            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            return ScopeDefinitions.RecordingTypeArgument(Logger, parameter.Name, parameter.Ordinal);
        }

        void ITypeArgumentsLogger.FailedToMapTypeParameterToRecorder() => MessageDefinitions.FailedToMapTypeParameterToRecorder(Logger, null);
    }

    private sealed class ConstructorArgumentLogger : IConstructorArgumentsLogger
    {
        private ILogger Logger { get; }

        public ConstructorArgumentLogger(ILogger logger)
        {
            Logger = logger;
        }

        IDisposable? IConstructorArgumentsLogger.BeginScopeRecordingNormalConstructorArgument(IParameterSymbol parameter, ExpressionSyntax syntax) => BeginScopeRecordingConstructorArgument(parameter);
        IDisposable? IConstructorArgumentsLogger.BeginScopeRecordingParamsConstructorArgument(IParameterSymbol parameter, IReadOnlyList<ExpressionSyntax> elementSyntax) => BeginScopeRecordingConstructorArgument(parameter);
        IDisposable? IConstructorArgumentsLogger.BeginScopeRecordingDefaultConstructorArgument(IParameterSymbol parameter) => BeginScopeRecordingConstructorArgument(parameter);

        void IConstructorArgumentsLogger.FailedToMapConstructorParameterToRecorder() => MessageDefinitions.FailedToMapConstructorParameterToRecorder(Logger, null);

        private IDisposable? BeginScopeRecordingConstructorArgument(IParameterSymbol parameter)
        {
            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            return ScopeDefinitions.RecordingConstructorArgument(Logger, parameter.Name);
        }
    }

    private sealed class NamedArgumentLogger : INamedArgumentsLogger
    {
        private ILogger Logger { get; }

        public NamedArgumentLogger(ILogger logger)
        {
            Logger = logger;
        }

        IDisposable? INamedArgumentsLogger.BeginScopeRecordingNamedArgument(string parameterName, ExpressionSyntax syntax)
        {
            if (parameterName is null)
            {
                throw new ArgumentNullException(nameof(parameterName));
            }

            return ScopeDefinitions.RecordingNamedArgument(Logger, parameterName);
        }

        void INamedArgumentsLogger.FailedToMapNamedParameterToRecorder() => MessageDefinitions.FailedToMapNamedParameterToRecorder(Logger, null);
    }

    private static class ScopeDefinitions
    {
        public static Func<ILogger, string, int, IDisposable?> RecordingTypeArgument { get; }
        public static Func<ILogger, string, IDisposable?> RecordingConstructorArgument { get; }
        public static Func<ILogger, string, IDisposable?> RecordingNamedArgument { get; }

        static ScopeDefinitions()
        {
            RecordingTypeArgument = LoggerMessage.DefineScope<string, int>("[TypeParameterName: {TypeParameterName}, TypeParameterIndex: {TypeParameterIndex}]");
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
            FailedToMapTypeParameterToRecorder = LoggerMessage.Define(LogLevel.Debug, EventIDs.FailedToMapTypeParameterToRecorder, "Failed to syntactically record a type argument, as the type parameter could not be mapped to a recorder.");
            FailedToMapConstructorParameterToRecorder = LoggerMessage.Define(LogLevel.Debug, EventIDs.FailedToMapConstructorParameterToRecorder, "Failed to syntactically record a constructor argument, as the constructor parameter could not be mapped to a recorder.");
            FailedToMapNamedParameterToRecorder = LoggerMessage.Define(LogLevel.Debug, EventIDs.FailedToMapNamedParameterToRecorder, "Failed to syntactically record a named argument, as the named parameter could not be mapped to a recorder.");
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
