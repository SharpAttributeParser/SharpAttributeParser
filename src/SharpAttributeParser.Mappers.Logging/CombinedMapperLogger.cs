namespace SharpAttributeParser.Mappers.Logging;

using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;

using SharpAttributeParser.Logging;
using SharpAttributeParser.Mappers.Logging.CombinedMapperComponents;
using SharpAttributeParser.Mappers.Repositories;

using System;

/// <inheritdoc cref="ICombinedMapperLogger{TCategoryName}"/>
public sealed class CombinedMapperLogger<TCategoryName> : ICombinedMapperLogger<TCategoryName>
{
    private readonly ITypeParameterLogger TypeParameter;
    private readonly IConstructorParameterLogger ConstructorParameter;
    private readonly INamedParameterLogger NamedParameter;

    /// <summary>Instantiates a <see cref="CombinedMapperLogger{TCategoryName}"/>, handling logging for <see cref="ICombinedMapper{TRecord}"/>.</summary>
    /// <param name="logger">The logger used to log messages.</param>
    public CombinedMapperLogger(ILogger<TCategoryName> logger)
    {
        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        TypeParameter = new TypeParameterLogger(logger);
        ConstructorParameter = new ConstructorParameterLogger(logger);
        NamedParameter = new NamedParameterLogger(logger);
    }

    ITypeParameterLogger ICombinedMapperLogger.TypeParameter => TypeParameter;
    IConstructorParameterLogger ICombinedMapperLogger.ConstructorParameter => ConstructorParameter;
    INamedParameterLogger ICombinedMapperLogger.NamedParameter => NamedParameter;

    private sealed class TypeParameterLogger : ITypeParameterLogger
    {
        private readonly ILogger Logger;

        public TypeParameterLogger(ILogger logger)
        {
            Logger = logger;
        }

        IDisposable? ITypeParameterLogger.BeginScopeMappingTypeParameter<TRecorder>(ITypeParameterSymbol parameter, IBuiltTypeMappingRepository<TRecorder> mappingRepository)
        {
            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            if (mappingRepository is null)
            {
                throw new ArgumentNullException(nameof(mappingRepository));
            }

            return ScopeDefinitions.MappingTypeParameter(Logger, parameter.Name, parameter.Ordinal, mappingRepository.Named.Count, mappingRepository.Indexed.Count);
        }

        void ITypeParameterLogger.FailedToMapTypeParameter() => MessageDefinitions.FailedToMapTypeParameter(Logger, null);
    }

    private sealed class ConstructorParameterLogger : IConstructorParameterLogger
    {
        private readonly ILogger Logger;

        public ConstructorParameterLogger(ILogger logger)
        {
            Logger = logger;
        }

        IDisposable? IConstructorParameterLogger.BeginScopeMappingConstructorParameter<TRecorder>(IParameterSymbol parameter, IBuiltConstructorMappingRepository<TRecorder> mappingRepository)
        {
            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            if (mappingRepository is null)
            {
                throw new ArgumentNullException(nameof(mappingRepository));
            }

            return ScopeDefinitions.MappingConstructorParameter(Logger, parameter.Name, mappingRepository.Named.Count);
        }

        void IConstructorParameterLogger.FailedToMapConstructorParameter() => MessageDefinitions.FailedToMapConstructorParameter(Logger, null);
    }

    private sealed class NamedParameterLogger : INamedParameterLogger
    {
        private readonly ILogger Logger;

        public NamedParameterLogger(ILogger logger)
        {
            Logger = logger;
        }

        IDisposable? INamedParameterLogger.BeginScopeMappingNamedParameter<TRecorder>(string parameterName, IBuiltNamedMappingRepository<TRecorder> mappingRepository)
        {
            if (parameterName is null)
            {
                throw new ArgumentNullException(nameof(parameterName));
            }

            if (mappingRepository is null)
            {
                throw new ArgumentNullException(nameof(mappingRepository));
            }

            return ScopeDefinitions.MappingNamedParameter(Logger, parameterName, mappingRepository.Named.Count);
        }

        void INamedParameterLogger.FailedToMapNamedParameter() => MessageDefinitions.FailedToMapNamedParameter(Logger, null);
    }

    private static class ScopeDefinitions
    {
        public static Func<ILogger, string, int, int, int, IDisposable?> MappingTypeParameter { get; }
        public static Func<ILogger, string, int, IDisposable?> MappingConstructorParameter { get; }
        public static Func<ILogger, string, int, IDisposable?> MappingNamedParameter { get; }

        static ScopeDefinitions()
        {
            MappingTypeParameter = LoggerMessage.DefineScope<string, int, int, int>("[TypeParameterName: {TypeParameterName}, TypeParameterIndex: {TypeParameterIndex}, NamedMappingCount: {NamedMappingCount}, IndexedMappingCount: {IndexedMappingCount}]");
            MappingConstructorParameter = LoggerMessage.DefineScope<string, int>("[ConstructorParameterName: {ConstructorParameterName}, NamedMappingCount: {NamedMappingCount}]");
            MappingNamedParameter = LoggerMessage.DefineScope<string, int>("[NamedParameterName: {NamedParameterName}, NamedMappingCount: {NamedMappingCount}]");
        }
    }

    private static class MessageDefinitions
    {
        public static Action<ILogger, Exception?> FailedToMapTypeParameter { get; }
        public static Action<ILogger, Exception?> FailedToMapConstructorParameter { get; }
        public static Action<ILogger, Exception?> FailedToMapNamedParameter { get; }

        static MessageDefinitions()
        {
            FailedToMapTypeParameter = LoggerMessage.Define(LogLevel.Debug, EventIDs.FailedToMapTypeParameter, "Failed to map a type parameter to a recorder, as a mapping did not exist.");
            FailedToMapConstructorParameter = LoggerMessage.Define(LogLevel.Debug, EventIDs.FailedToMapConstructorParameter, "Failed to map a constructor parameter to a recorder, as a mapping did not exist.");
            FailedToMapNamedParameter = LoggerMessage.Define(LogLevel.Debug, EventIDs.FailedToMapNamedParameter, "Failed to map a named parameter to a recorder, as a mapping did not exist.");
        }
    }

    private static class EventIDs
    {
        public static EventId FailedToMapTypeParameter { get; }
        public static EventId FailedToMapConstructorParameter { get; }
        public static EventId FailedToMapNamedParameter { get; }

        static EventIDs()
        {
            SequentialEventID eventIDs = new();

            FailedToMapTypeParameter = new(eventIDs.Next, nameof(FailedToMapTypeParameter));
            FailedToMapConstructorParameter = new(eventIDs.Next, nameof(FailedToMapConstructorParameter));
            FailedToMapNamedParameter = new(eventIDs.Next, nameof(FailedToMapNamedParameter));
        }
    }
}
