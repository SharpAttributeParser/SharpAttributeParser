namespace SharpAttributeParser.Mappers;

using Microsoft.CodeAnalysis;

using SharpAttributeParser.Mappers.Logging;
using SharpAttributeParser.Mappers.MappedRecorders;

using System;

/// <inheritdoc cref="ISemanticRecorderFactory"/>
public sealed class SemanticRecorderFactory : ISemanticRecorderFactory
{
    private readonly ISemanticArgumentRecorderLoggerFactory LoggerFactory;

    /// <summary>Instantiates a <see cref="SemanticRecorderFactory"/>, handling creation of <see cref="ISemanticRecorder"/> using <see cref="ISemanticMapper{TRecord}"/>.</summary>
    /// <param name="loggerFactory">Handles creation of the loggers used by the created recorders.</param>
    public SemanticRecorderFactory(ISemanticArgumentRecorderLoggerFactory? loggerFactory = null)
    {
        LoggerFactory = loggerFactory ?? NullSemanticArgumentRecorderLoggerFactory.Singleton;
    }

    ISemanticRecorder<TRecord> ISemanticRecorderFactory.Create<TRecord>(ISemanticMapper<TRecord> mapper, TRecord dataRecord)
    {
        if (mapper is null)
        {
            throw new ArgumentNullException(nameof(mapper));
        }

        if (dataRecord is null)
        {
            throw new ArgumentNullException(nameof(dataRecord));
        }

        var recorderLogger = LoggerFactory.Create<ISemanticRecorder<TRecord>>();

        return new Recorder<TRecord, RecordBuilder<TRecord>>(new RecordBuilderMapper<TRecord>(mapper), new RecordBuilder<TRecord>(dataRecord), recorderLogger);
    }

    ISemanticRecorder<TRecord> ISemanticRecorderFactory.Create<TRecord, TRecordBuilder>(ISemanticMapper<TRecordBuilder> mapper, TRecordBuilder recordBuilder)
    {
        if (mapper is null)
        {
            throw new ArgumentNullException(nameof(mapper));
        }

        if (recordBuilder is null)
        {
            throw new ArgumentNullException(nameof(recordBuilder));
        }

        var recorderLogger = LoggerFactory.Create<ISemanticRecorder<TRecord>>();

        return new Recorder<TRecord, TRecordBuilder>(mapper, recordBuilder, recorderLogger);
    }

    private sealed class RecordBuilder<TRecord> : IRecordBuilder<TRecord>
    {
        public TRecord Target { get; }

        public RecordBuilder(TRecord target)
        {
            Target = target;
        }

        TRecord IRecordBuilder<TRecord>.Build() => Target;
    }

    private sealed class RecordBuilderMapper<TRecord> : ISemanticMapper<RecordBuilder<TRecord>>
    {
        private readonly ISemanticMapper<TRecord> WrappedMapper;

        public RecordBuilderMapper(ISemanticMapper<TRecord> wrappedMapper)
        {
            WrappedMapper = wrappedMapper;
        }

        IMappedSemanticTypeArgumentRecorder? ISemanticMapper<RecordBuilder<TRecord>>.TryMapTypeParameter(ITypeParameterSymbol parameter, RecordBuilder<TRecord> dataRecord) => WrappedMapper.TryMapTypeParameter(parameter, dataRecord.Target);
        IMappedSemanticConstructorArgumentRecorder? ISemanticMapper<RecordBuilder<TRecord>>.TryMapConstructorParameter(IParameterSymbol parameter, RecordBuilder<TRecord> dataRecord) => WrappedMapper.TryMapConstructorParameter(parameter, dataRecord.Target);
        IMappedSemanticNamedArgumentRecorder? ISemanticMapper<RecordBuilder<TRecord>>.TryMapNamedParameter(string parameterName, RecordBuilder<TRecord> dataRecord) => WrappedMapper.TryMapNamedParameter(parameterName, dataRecord.Target);
    }

    private sealed class Recorder<TRecord, TRecordBuilder> : ISemanticRecorder<TRecord> where TRecordBuilder : IRecordBuilder<TRecord>
    {
        private readonly TRecordBuilder RecordBuilder;

        private readonly ISemanticTypeArgumentRecorder TypeArgument;
        private readonly ISemanticConstructorArgumentRecorder ConstructorArgument;
        private readonly ISemanticNamedArgumentRecorder NamedArgument;

        public Recorder(ISemanticMapper<TRecordBuilder> argumentRecorderMapper, TRecordBuilder recordBuilder, ISemanticArgumentRecorderLogger logger)
        {
            RecordBuilder = recordBuilder;

            TypeArgument = new TypeArgumentRecorder(argumentRecorderMapper, recordBuilder, logger);
            ConstructorArgument = new ConstructorArgumentRecorder(argumentRecorderMapper, recordBuilder, logger);
            NamedArgument = new NamedArgumentRecorder(argumentRecorderMapper, recordBuilder, logger);
        }

        ISemanticTypeArgumentRecorder ISemanticRecorder.TypeArgument => TypeArgument;
        ISemanticConstructorArgumentRecorder ISemanticRecorder.ConstructorArgument => ConstructorArgument;
        ISemanticNamedArgumentRecorder ISemanticRecorder.NamedArgument => NamedArgument;

        TRecord ISemanticRecorder<TRecord>.GetRecord() => RecordBuilder.Build();

        private sealed class TypeArgumentRecorder : ISemanticTypeArgumentRecorder
        {
            private readonly ISemanticMapper<TRecordBuilder> ArgumentRecorderMapper;
            private readonly TRecordBuilder RecordBuilder;

            private readonly ISemanticArgumentRecorderLogger Logger;

            public TypeArgumentRecorder(ISemanticMapper<TRecordBuilder> argumentRecorderMapper, TRecordBuilder recordBuilder, ISemanticArgumentRecorderLogger logger)
            {
                ArgumentRecorderMapper = argumentRecorderMapper;
                RecordBuilder = recordBuilder;

                Logger = logger;
            }

            bool ISemanticTypeArgumentRecorder.TryRecordArgument(ITypeParameterSymbol parameter, ITypeSymbol argument)
            {
                if (parameter is null)
                {
                    throw new ArgumentNullException(nameof(parameter));
                }

                if (argument is null)
                {
                    throw new ArgumentNullException(nameof(argument));
                }

                using var _ = Logger.TypeArgument.BeginScopeRecordingTypeArgument(parameter, argument);

                if (ArgumentRecorderMapper.TryMapTypeParameter(parameter, RecordBuilder) is not IMappedSemanticTypeArgumentRecorder argumentRecorder)
                {
                    Logger.TypeArgument.FailedToMapTypeParameterToRecorder();

                    return false;
                }

                return argumentRecorder.TryRecordArgument(argument);
            }
        }

        private sealed class ConstructorArgumentRecorder : ISemanticConstructorArgumentRecorder
        {
            private readonly ISemanticMapper<TRecordBuilder> ArgumentRecorderMapper;
            private readonly TRecordBuilder RecordBuilder;

            private readonly ISemanticArgumentRecorderLogger Logger;

            public ConstructorArgumentRecorder(ISemanticMapper<TRecordBuilder> argumentRecorderMapper, TRecordBuilder recordBuilder, ISemanticArgumentRecorderLogger logger)
            {
                ArgumentRecorderMapper = argumentRecorderMapper;
                RecordBuilder = recordBuilder;

                Logger = logger;
            }

            bool ISemanticConstructorArgumentRecorder.TryRecordArgument(IParameterSymbol parameter, object? argument)
            {
                if (parameter is null)
                {
                    throw new ArgumentNullException(nameof(parameter));
                }

                using var _ = Logger.ConstructorArgument.BeginScopeRecordingConstructorArgument(parameter, argument);

                if (ArgumentRecorderMapper.TryMapConstructorParameter(parameter, RecordBuilder) is not IMappedSemanticConstructorArgumentRecorder argumentRecorder)
                {
                    Logger.ConstructorArgument.FailedToMapConstructorParameterToRecorder();

                    return false;
                }

                return argumentRecorder.TryRecordArgument(argument);
            }
        }

        private sealed class NamedArgumentRecorder : ISemanticNamedArgumentRecorder
        {
            private readonly ISemanticMapper<TRecordBuilder> ArgumentRecorderMapper;
            private readonly TRecordBuilder RecordBuilder;

            private readonly ISemanticArgumentRecorderLogger Logger;

            public NamedArgumentRecorder(ISemanticMapper<TRecordBuilder> argumentRecorderMapper, TRecordBuilder recordBuilder, ISemanticArgumentRecorderLogger logger)
            {
                ArgumentRecorderMapper = argumentRecorderMapper;
                RecordBuilder = recordBuilder;

                Logger = logger;
            }

            bool ISemanticNamedArgumentRecorder.TryRecordArgument(string parameterName, object? argument)
            {
                if (parameterName is null)
                {
                    throw new ArgumentNullException(nameof(parameterName));
                }

                using var _ = Logger.NamedArgument.BeginScopeRecordingNamedArgument(parameterName, argument);

                if (ArgumentRecorderMapper.TryMapNamedParameter(parameterName, RecordBuilder) is not IMappedSemanticNamedArgumentRecorder argumentRecorder)
                {
                    Logger.NamedArgument.FailedToMapNamedParameterToRecorder();

                    return false;
                }

                return argumentRecorder.TryRecordArgument(argument);
            }
        }
    }
}
