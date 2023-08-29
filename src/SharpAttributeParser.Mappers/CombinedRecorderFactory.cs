namespace SharpAttributeParser.Mappers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser.Mappers.Logging;
using SharpAttributeParser.Mappers.MappedRecorders;

using System;
using System.Collections.Generic;

/// <inheritdoc cref="ICombinedRecorderFactory"/>
public sealed class CombinedRecorderFactory : ICombinedRecorderFactory
{
    private ICombinedArgumentRecorderLoggerFactory LoggerFactory { get; }

    /// <summary>Instantiates a <see cref="CombinedRecorderFactory"/>, handling creation of <see cref="ICombinedRecorder"/> using <see cref="ICombinedMapper{TRecord}"/>.</summary>
    /// <param name="loggerFactory">Handles creation of the loggers used by the created recorders.</param>
    public CombinedRecorderFactory(ICombinedArgumentRecorderLoggerFactory? loggerFactory = null)
    {
        LoggerFactory = loggerFactory ?? NullCombinedArgumentRecorderLoggerFactory.Singleton;
    }

    ICombinedRecorder<TRecord> ICombinedRecorderFactory.Create<TRecord>(ICombinedMapper<TRecord> mapper, TRecord dataRecord)
    {
        if (mapper is null)
        {
            throw new ArgumentNullException(nameof(mapper));
        }

        if (dataRecord is null)
        {
            throw new ArgumentNullException(nameof(dataRecord));
        }

        var recorderLogger = LoggerFactory.Create<ICombinedRecorder<TRecord>>();

        return new Recorder<TRecord, RecordBuilder<TRecord>>(new RecordBuilderMapper<TRecord>(mapper), new RecordBuilder<TRecord>(dataRecord), recorderLogger);
    }

    ICombinedRecorder<TRecord> ICombinedRecorderFactory.Create<TRecord, TRecordBuilder>(ICombinedMapper<TRecordBuilder> mapper, TRecordBuilder recordBuilder)
    {
        if (mapper is null)
        {
            throw new ArgumentNullException(nameof(mapper));
        }

        if (recordBuilder is null)
        {
            throw new ArgumentNullException(nameof(recordBuilder));
        }

        var recorderLogger = LoggerFactory.Create<ICombinedRecorder<TRecord>>();

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

    private sealed class RecordBuilderMapper<TRecord> : ICombinedMapper<RecordBuilder<TRecord>>
    {
        private ICombinedMapper<TRecord> WrappedMapper { get; }

        public RecordBuilderMapper(ICombinedMapper<TRecord> wrappedMapper)
        {
            WrappedMapper = wrappedMapper;
        }

        IMappedCombinedTypeArgumentRecorder? ICombinedMapper<RecordBuilder<TRecord>>.TryMapTypeParameter(ITypeParameterSymbol parameter, RecordBuilder<TRecord> dataRecord)
        {
            return WrappedMapper.TryMapTypeParameter(parameter, dataRecord.Target);
        }

        IMappedCombinedConstructorArgumentRecorder? ICombinedMapper<RecordBuilder<TRecord>>.TryMapConstructorParameter(IParameterSymbol parameter, RecordBuilder<TRecord> dataRecord)
        {
            return WrappedMapper.TryMapConstructorParameter(parameter, dataRecord.Target);
        }

        IMappedCombinedNamedArgumentRecorder? ICombinedMapper<RecordBuilder<TRecord>>.TryMapNamedParameter(string parameterName, RecordBuilder<TRecord> dataRecord)
        {
            return WrappedMapper.TryMapNamedParameter(parameterName, dataRecord.Target);
        }
    }

    private sealed class Recorder<TRecord, TRecordBuilder> : ICombinedRecorder<TRecord> where TRecordBuilder : IRecordBuilder<TRecord>
    {
        private TRecordBuilder RecordBuilder { get; }

        private ICombinedTypeArgumentRecorder TypeArgument { get; }
        private ICombinedConstructorArgumentRecorder ConstructorArgument { get; }
        private ICombinedNamedArgumentRecorder NamedArgument { get; }

        public Recorder(ICombinedMapper<TRecordBuilder> argumentRecorderMapper, TRecordBuilder recordBuilder, ICombinedArgumentRecorderLogger logger)
        {
            RecordBuilder = recordBuilder;

            TypeArgument = new TypeArgumentRecorder(argumentRecorderMapper, recordBuilder, logger);
            ConstructorArgument = new ConstructorArgumentRecorder(argumentRecorderMapper, recordBuilder, logger);
            NamedArgument = new NamedArgumentRecorder(argumentRecorderMapper, recordBuilder, logger);
        }

        ICombinedTypeArgumentRecorder ICombinedRecorder.TypeArgument => TypeArgument;
        ICombinedConstructorArgumentRecorder ICombinedRecorder.ConstructorArgument => ConstructorArgument;
        ICombinedNamedArgumentRecorder ICombinedRecorder.NamedArgument => NamedArgument;

        TRecord ICombinedRecorder<TRecord>.GetRecord() => RecordBuilder.Build();

        private sealed class TypeArgumentRecorder : ICombinedTypeArgumentRecorder
        {
            private ICombinedMapper<TRecordBuilder> ArgumentRecorderMapper { get; }
            private TRecordBuilder RecordBuilder { get; }

            private ICombinedArgumentRecorderLogger Logger { get; }

            public TypeArgumentRecorder(ICombinedMapper<TRecordBuilder> argumentRecorderMapper, TRecordBuilder recordBuilder, ICombinedArgumentRecorderLogger logger)
            {
                ArgumentRecorderMapper = argumentRecorderMapper;
                RecordBuilder = recordBuilder;

                Logger = logger;
            }

            bool ICombinedTypeArgumentRecorder.TryRecordArgument(ITypeParameterSymbol parameter, ITypeSymbol argument, ExpressionSyntax syntax)
            {
                if (parameter is null)
                {
                    throw new ArgumentNullException(nameof(parameter));
                }

                if (argument is null)
                {
                    throw new ArgumentNullException(nameof(argument));
                }

                if (syntax is null)
                {
                    throw new ArgumentNullException(nameof(syntax));
                }

                using var _ = Logger.TypeArgument.BeginScopeRecordingTypeArgument(parameter, argument, syntax);

                if (ArgumentRecorderMapper.TryMapTypeParameter(parameter, RecordBuilder) is not IMappedCombinedTypeArgumentRecorder argumentRecorder)
                {
                    Logger.TypeArgument.FailedToMapTypeParameterToRecorder();

                    return false;
                }

                return argumentRecorder.TryRecordArgument(argument, syntax);
            }
        }

        private sealed class ConstructorArgumentRecorder : ICombinedConstructorArgumentRecorder
        {
            private ICombinedMapper<TRecordBuilder> ArgumentRecorderMapper { get; }
            private TRecordBuilder RecordBuilder { get; }

            private ICombinedArgumentRecorderLogger Logger { get; }

            public ConstructorArgumentRecorder(ICombinedMapper<TRecordBuilder> argumentRecorderMapper, TRecordBuilder recordBuilder, ICombinedArgumentRecorderLogger logger)
            {
                ArgumentRecorderMapper = argumentRecorderMapper;
                RecordBuilder = recordBuilder;

                Logger = logger;
            }

            bool ICombinedConstructorArgumentRecorder.TryRecordArgument(IParameterSymbol parameter, object? argument, ExpressionSyntax syntax)
            {
                if (parameter is null)
                {
                    throw new ArgumentNullException(nameof(parameter));
                }

                if (syntax is null)
                {
                    throw new ArgumentNullException(nameof(syntax));
                }

                using var _ = Logger.ConstructorArgument.BeginScopeRecordingNormalConstructorlArgument(parameter, argument, syntax);

                if (TryMapParameter(parameter) is not IMappedCombinedConstructorArgumentRecorder argumentRecorder)
                {
                    return false;
                }

                return argumentRecorder.TryRecordArgument(argument, syntax);
            }

            bool ICombinedConstructorArgumentRecorder.TryRecordParamsArgument(IParameterSymbol parameter, object? argument, IReadOnlyList<ExpressionSyntax> elementSyntax)
            {
                if (parameter is null)
                {
                    throw new ArgumentNullException(nameof(parameter));
                }

                if (elementSyntax is null)
                {
                    throw new ArgumentNullException(nameof(elementSyntax));
                }

                using var _ = Logger.ConstructorArgument.BeginScopeRecordingParamsConstructorArgument(parameter, argument, elementSyntax);

                if (TryMapParameter(parameter) is not IMappedCombinedConstructorArgumentRecorder argumentRecorder)
                {
                    return false;
                }

                return argumentRecorder.TryRecordParamsArgument(argument, elementSyntax);
            }

            bool ICombinedConstructorArgumentRecorder.TryRecordDefaultArgument(IParameterSymbol parameter, object? argument)
            {
                if (parameter is null)
                {
                    throw new ArgumentNullException(nameof(parameter));
                }

                using var _ = Logger.ConstructorArgument.BeginScopeRecordingDefaultConstructorArgument(parameter, argument);

                if (TryMapParameter(parameter) is not IMappedCombinedConstructorArgumentRecorder argumentRecorder)
                {
                    return false;
                }

                return argumentRecorder.TryRecordDefaultArgument(argument);
            }

            private IMappedCombinedConstructorArgumentRecorder? TryMapParameter(IParameterSymbol parameter)
            {
                if (ArgumentRecorderMapper.TryMapConstructorParameter(parameter, RecordBuilder) is not IMappedCombinedConstructorArgumentRecorder argumentRecorder)
                {
                    Logger.ConstructorArgument.FailedToMapConstructorParameterToRecorder();

                    return null;
                }

                return argumentRecorder;
            }
        }

        private sealed class NamedArgumentRecorder : ICombinedNamedArgumentRecorder
        {
            private ICombinedMapper<TRecordBuilder> ArgumentRecorderMapper { get; }
            private TRecordBuilder RecordBuilder { get; }

            private ICombinedArgumentRecorderLogger Logger { get; }

            public NamedArgumentRecorder(ICombinedMapper<TRecordBuilder> argumentRecorderMapper, TRecordBuilder recordBuilder, ICombinedArgumentRecorderLogger logger)
            {
                ArgumentRecorderMapper = argumentRecorderMapper;
                RecordBuilder = recordBuilder;

                Logger = logger;
            }

            bool ICombinedNamedArgumentRecorder.TryRecordArgument(string parameterName, object? argument, ExpressionSyntax syntax)
            {
                if (parameterName is null)
                {
                    throw new ArgumentNullException(nameof(parameterName));
                }

                if (syntax is null)
                {
                    throw new ArgumentNullException(nameof(syntax));
                }

                using var _ = Logger.NamedArgument.BeginScopeRecordingNamedArgument(parameterName, argument, syntax);

                if (ArgumentRecorderMapper.TryMapNamedParameter(parameterName, RecordBuilder) is not IMappedCombinedNamedArgumentRecorder argumentRecorder)
                {
                    Logger.NamedArgument.FailedToMapNamedParameterToRecorder();

                    return false;
                }

                return argumentRecorder.TryRecordArgument(argument, syntax);
            }
        }
    }
}
