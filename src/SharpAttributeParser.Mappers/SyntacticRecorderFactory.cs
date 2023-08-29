namespace SharpAttributeParser.Mappers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser.Mappers.Logging;
using SharpAttributeParser.Mappers.MappedRecorders;

using System;
using System.Collections.Generic;

/// <inheritdoc cref="ISyntacticRecorderFactory"/>
public sealed class SyntacticRecorderFactory : ISyntacticRecorderFactory
{
    private ISyntacticArgumentRecorderLoggerFactory LoggerFactory { get; }

    /// <summary>Instantiates a <see cref="SyntacticRecorderFactory"/>, handling creation of <see cref="ISyntacticRecorder"/> using <see cref="ISyntacticMapper{TRecord}"/>.</summary>
    /// <param name="loggerFactory">Handles creation of the loggers used by the created recorders.</param>
    public SyntacticRecorderFactory(ISyntacticArgumentRecorderLoggerFactory? loggerFactory = null)
    {
        LoggerFactory = loggerFactory ?? NullSyntacticArgumentRecorderLoggerFactory.Singleton;
    }

    ISyntacticRecorder<TRecord> ISyntacticRecorderFactory.Create<TRecord>(ISyntacticMapper<TRecord> mapper, TRecord dataRecord)
    {
        if (mapper is null)
        {
            throw new ArgumentNullException(nameof(mapper));
        }

        if (dataRecord is null)
        {
            throw new ArgumentNullException(nameof(dataRecord));
        }

        var recorderLogger = LoggerFactory.Create<ISyntacticRecorder<TRecord>>();

        return new Recorder<TRecord, RecordBuilder<TRecord>>(new RecordBuilderMapper<TRecord>(mapper), new RecordBuilder<TRecord>(dataRecord), recorderLogger);
    }

    ISyntacticRecorder<TRecord> ISyntacticRecorderFactory.Create<TRecord, TRecordBuilder>(ISyntacticMapper<TRecordBuilder> mapper, TRecordBuilder recordBuilder)
    {
        if (mapper is null)
        {
            throw new ArgumentNullException(nameof(mapper));
        }

        if (recordBuilder is null)
        {
            throw new ArgumentNullException(nameof(recordBuilder));
        }

        var recorderLogger = LoggerFactory.Create<ISyntacticRecorder<TRecord>>();

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

    private sealed class RecordBuilderMapper<TRecord> : ISyntacticMapper<RecordBuilder<TRecord>>
    {
        private ISyntacticMapper<TRecord> WrappedMapper { get; }

        public RecordBuilderMapper(ISyntacticMapper<TRecord> wrappedMapper)
        {
            WrappedMapper = wrappedMapper;
        }

        IMappedSyntacticTypeArgumentRecorder? ISyntacticMapper<RecordBuilder<TRecord>>.TryMapTypeParameter(ITypeParameterSymbol parameter, RecordBuilder<TRecord> dataRecord)
        {
            return WrappedMapper.TryMapTypeParameter(parameter, dataRecord.Target);
        }

        IMappedSyntacticConstructorArgumentRecorder? ISyntacticMapper<RecordBuilder<TRecord>>.TryMapConstructorParameter(IParameterSymbol parameter, RecordBuilder<TRecord> dataRecord)
        {
            return WrappedMapper.TryMapConstructorParameter(parameter, dataRecord.Target);
        }

        IMappedSyntacticNamedArgumentRecorder? ISyntacticMapper<RecordBuilder<TRecord>>.TryMapNamedParameter(string parameterName, RecordBuilder<TRecord> dataRecord)
        {
            return WrappedMapper.TryMapNamedParameter(parameterName, dataRecord.Target);
        }
    }

    private sealed class Recorder<TRecord, TRecordBuilder> : ISyntacticRecorder<TRecord> where TRecordBuilder : IRecordBuilder<TRecord>
    {
        private TRecordBuilder RecordBuilder { get; }

        private ISyntacticTypeArgumentRecorder TypeArgument { get; }
        private ISyntacticConstructorArgumentRecorder ConstructorArgument { get; }
        private ISyntacticNamedArgumentRecorder NamedArgument { get; }

        public Recorder(ISyntacticMapper<TRecordBuilder> argumentRecorderMapper, TRecordBuilder recordBuilder, ISyntacticArgumentRecorderLogger logger)
        {
            RecordBuilder = recordBuilder;

            TypeArgument = new TypeArgumentRecorder(argumentRecorderMapper, recordBuilder, logger);
            ConstructorArgument = new ConstructorArgumentRecorder(argumentRecorderMapper, recordBuilder, logger);
            NamedArgument = new NamedArgumentRecorder(argumentRecorderMapper, recordBuilder, logger);
        }

        ISyntacticTypeArgumentRecorder ISyntacticRecorder.TypeArgument => TypeArgument;
        ISyntacticConstructorArgumentRecorder ISyntacticRecorder.ConstructorArgument => ConstructorArgument;
        ISyntacticNamedArgumentRecorder ISyntacticRecorder.NamedArgument => NamedArgument;

        TRecord ISyntacticRecorder<TRecord>.GetRecord() => RecordBuilder.Build();

        private sealed class TypeArgumentRecorder : ISyntacticTypeArgumentRecorder
        {
            private ISyntacticMapper<TRecordBuilder> ArgumentMapper { get; }
            private TRecordBuilder RecordBuilder { get; }

            private ISyntacticArgumentRecorderLogger Logger { get; }

            public TypeArgumentRecorder(ISyntacticMapper<TRecordBuilder> argumentMapper, TRecordBuilder recordBuilder, ISyntacticArgumentRecorderLogger logger)
            {
                ArgumentMapper = argumentMapper;
                RecordBuilder = recordBuilder;

                Logger = logger;
            }

            bool ISyntacticTypeArgumentRecorder.TryRecordArgument(ITypeParameterSymbol parameter, ExpressionSyntax syntax)
            {
                if (parameter is null)
                {
                    throw new ArgumentNullException(nameof(parameter));
                }

                if (syntax is null)
                {
                    throw new ArgumentNullException(nameof(syntax));
                }

                using var _ = Logger.TypeArgument.BeginScopeRecordingTypeArgument(parameter, syntax);

                if (ArgumentMapper.TryMapTypeParameter(parameter, RecordBuilder) is not IMappedSyntacticTypeArgumentRecorder argumentRecorder)
                {
                    Logger.TypeArgument.FailedToMapTypeParameterToRecorder();

                    return false;
                }

                return argumentRecorder.TryRecordArgument(syntax);
            }
        }

        private sealed class ConstructorArgumentRecorder : ISyntacticConstructorArgumentRecorder
        {
            private ISyntacticMapper<TRecordBuilder> ArgumentMapper { get; }
            private TRecordBuilder RecordBuilder { get; }

            private ISyntacticArgumentRecorderLogger Logger { get; }

            public ConstructorArgumentRecorder(ISyntacticMapper<TRecordBuilder> argumentMapper, TRecordBuilder recordBuilder, ISyntacticArgumentRecorderLogger logger)
            {
                ArgumentMapper = argumentMapper;
                RecordBuilder = recordBuilder;

                Logger = logger;
            }

            bool ISyntacticConstructorArgumentRecorder.TryRecordArgument(IParameterSymbol parameter, ExpressionSyntax syntax)
            {
                if (parameter is null)
                {
                    throw new ArgumentNullException(nameof(parameter));
                }

                if (syntax is null)
                {
                    throw new ArgumentNullException(nameof(syntax));
                }

                using var _ = Logger.ConstructorArgument.BeginScopeRecordingNormalConstructorArgument(parameter, syntax);

                if (TryMapParameter(parameter) is not IMappedSyntacticConstructorArgumentRecorder argumentRecorder)
                {
                    return false;
                }

                return argumentRecorder.TryRecordArgument(syntax);
            }

            bool ISyntacticConstructorArgumentRecorder.TryRecordParamsArgument(IParameterSymbol parameter, IReadOnlyList<ExpressionSyntax> elementSyntax)
            {
                if (parameter is null)
                {
                    throw new ArgumentNullException(nameof(parameter));
                }

                if (elementSyntax is null)
                {
                    throw new ArgumentNullException(nameof(elementSyntax));
                }

                using var _ = Logger.ConstructorArgument.BeginScopeRecordingParamsConstructorArgument(parameter, elementSyntax);

                if (TryMapParameter(parameter) is not IMappedSyntacticConstructorArgumentRecorder argumentRecorder)
                {
                    return false;
                }

                return argumentRecorder.TryRecordParamsArgument(elementSyntax);
            }

            bool ISyntacticConstructorArgumentRecorder.TryRecordDefaultArgument(IParameterSymbol parameter)
            {
                if (parameter is null)
                {
                    throw new ArgumentNullException(nameof(parameter));
                }

                using var _ = Logger.ConstructorArgument.BeginScopeRecordingDefaultConstructorArgument(parameter);

                if (TryMapParameter(parameter) is not IMappedSyntacticConstructorArgumentRecorder argumentRecorder)
                {
                    return false;
                }

                return argumentRecorder.TryRecordDefaultArgument();
            }

            private IMappedSyntacticConstructorArgumentRecorder? TryMapParameter(IParameterSymbol parameter)
            {
                if (ArgumentMapper.TryMapConstructorParameter(parameter, RecordBuilder) is not IMappedSyntacticConstructorArgumentRecorder argumentRecorder)
                {
                    Logger.ConstructorArgument.FailedToMapConstructorParameterToRecorder();

                    return null;
                }

                return argumentRecorder;
            }
        }

        private sealed class NamedArgumentRecorder : ISyntacticNamedArgumentRecorder
        {
            private ISyntacticMapper<TRecordBuilder> ArgumentMapper { get; }
            private TRecordBuilder RecordBuilder { get; }

            private ISyntacticArgumentRecorderLogger Logger { get; }

            public NamedArgumentRecorder(ISyntacticMapper<TRecordBuilder> argumentMapper, TRecordBuilder recordBuilder, ISyntacticArgumentRecorderLogger logger)
            {
                ArgumentMapper = argumentMapper;
                RecordBuilder = recordBuilder;

                Logger = logger;
            }

            bool ISyntacticNamedArgumentRecorder.TryRecordArgument(string parameterName, ExpressionSyntax syntax)
            {
                if (parameterName is null)
                {
                    throw new ArgumentNullException(nameof(parameterName));
                }

                if (syntax is null)
                {
                    throw new ArgumentNullException(nameof(syntax));
                }

                if (ArgumentMapper.TryMapNamedParameter(parameterName, RecordBuilder) is not IMappedSyntacticNamedArgumentRecorder argumentRecorder)
                {
                    Logger.NamedArgument.FailedToMapNamedParameterToRecorder();

                    return false;
                }

                return argumentRecorder.TryRecordArgument(syntax);
            }
        }
    }
}
