namespace SharpAttributeParser.Mappers.Repositories.Semantic;

using SharpAttributeParser.Mappers.Repositories.Logging.Semantic;
using SharpAttributeParser.Patterns;

using System;

/// <inheritdoc cref="IDetachedMappedSemanticConstructorArgumentRecorderFactory{TRecord}"/>
public sealed class DetachedMappedSemanticConstructorArgumentRecorderFactory<TRecord> : IDetachedMappedSemanticConstructorArgumentRecorderFactory<TRecord>
{
    private IArgumentPatternFactory ArgumentPatternFactory { get; }

    private IMappedSemanticConstructorArgumentRecorderLoggerFactory LoggerFactory { get; }

    /// <summary>Instantiates a <see cref="DetachedMappedSemanticConstructorArgumentRecorderFactory{TRecord}"/>, handling creation of <see cref="IDetachedMappedSemanticConstructorArgumentRecorder{TRecord}"/>.</summary>
    /// <param name="argumentPatternFactory">Handles creation of <see cref="IArgumentPattern{T}"/>.</param>
    /// <param name="loggerFactory">Handles creation of the loggers used by the created recorders.</param>
    /// <exception cref="ArgumentNullException"/>
    public DetachedMappedSemanticConstructorArgumentRecorderFactory(IArgumentPatternFactory argumentPatternFactory, IMappedSemanticConstructorArgumentRecorderLoggerFactory? loggerFactory = null)
    {
        ArgumentPatternFactory = argumentPatternFactory ?? throw new ArgumentNullException(nameof(argumentPatternFactory));

        LoggerFactory = loggerFactory ?? NullMappedSemanticConstructorArgumentRecorderLoggerFactory.Singleton;
    }

    IDetachedMappedSemanticConstructorArgumentRecorder<TRecord> IDetachedMappedSemanticConstructorArgumentRecorderFactory<TRecord>.Create(Func<TRecord, object?, bool> recorder)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return Create(recorder);
    }

    IDetachedMappedSemanticConstructorArgumentRecorder<TRecord> IDetachedMappedSemanticConstructorArgumentRecorderFactory<TRecord>.Create(Action<TRecord, object?> recorder)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return Create(recorder.TrueReturning());
    }

    IDetachedMappedSemanticConstructorArgumentRecorder<TRecord> IDetachedMappedSemanticConstructorArgumentRecorderFactory<TRecord>.Create<T>(IArgumentPattern<T> pattern, Func<TRecord, T, bool> recorder)
    {
        if (pattern is null)
        {
            throw new ArgumentNullException(nameof(pattern));
        }

        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return Create(pattern, recorder);
    }

    IDetachedMappedSemanticConstructorArgumentRecorder<TRecord> IDetachedMappedSemanticConstructorArgumentRecorderFactory<TRecord>.Create<T>(IArgumentPattern<T> pattern, Action<TRecord, T> recorder)
    {
        if (pattern is null)
        {
            throw new ArgumentNullException(nameof(pattern));
        }

        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return Create(pattern, recorder.TrueReturning());
    }

    IDetachedMappedSemanticConstructorArgumentRecorder<TRecord> IDetachedMappedSemanticConstructorArgumentRecorderFactory<TRecord>.Create<T>(Func<IArgumentPatternFactory, IArgumentPattern<T>> patternDelegate, Func<TRecord, T, bool> recorder)
    {
        if (patternDelegate is null)
        {
            throw new ArgumentNullException(nameof(patternDelegate));
        }

        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        var pattern = patternDelegate(ArgumentPatternFactory) ?? throw new ArgumentException($"The provided delegate produced a null {nameof(IArgumentPattern<object>)}.", nameof(patternDelegate));

        return Create(pattern, recorder);
    }

    IDetachedMappedSemanticConstructorArgumentRecorder<TRecord> IDetachedMappedSemanticConstructorArgumentRecorderFactory<TRecord>.Create<T>(Func<IArgumentPatternFactory, IArgumentPattern<T>> patternDelegate, Action<TRecord, T> recorder)
    {
        if (patternDelegate is null)
        {
            throw new ArgumentNullException(nameof(patternDelegate));
        }

        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        var pattern = patternDelegate(ArgumentPatternFactory) ?? throw new ArgumentException($"The provided delegate produced a null {nameof(IArgumentPattern<object>)}.", nameof(patternDelegate));

        return Create(pattern, recorder.TrueReturning());
    }

    private IDetachedMappedSemanticConstructorArgumentRecorder<TRecord> Create(Func<TRecord, object?, bool> recorder) => new UnpatternedArgumentRecorder(recorder);
    private IDetachedMappedSemanticConstructorArgumentRecorder<TRecord> Create<T>(IArgumentPattern<T> pattern, Func<TRecord, T, bool> recorder)
    {
        var logger = LoggerFactory.Create<IDetachedMappedSemanticConstructorArgumentRecorder<TRecord>>();

        return new PatternedArgumentRecorder<T>(pattern, recorder, logger);
    }

    private sealed class UnpatternedArgumentRecorder : IDetachedMappedSemanticConstructorArgumentRecorder<TRecord>
    {
        private Func<TRecord, object?, bool> Recorder { get; }

        public UnpatternedArgumentRecorder(Func<TRecord, object?, bool> recorder)
        {
            Recorder = recorder;
        }

        bool IDetachedMappedSemanticConstructorArgumentRecorder<TRecord>.TryRecordArgument(TRecord dataRecord, object? argument)
        {
            if (dataRecord is null)
            {
                throw new ArgumentNullException(nameof(dataRecord));
            }

            return Recorder(dataRecord, argument);
        }
    }

    private sealed class PatternedArgumentRecorder<TArgument> : IDetachedMappedSemanticConstructorArgumentRecorder<TRecord>
    {
        private IArgumentPattern<TArgument> Pattern { get; }
        private Func<TRecord, TArgument, bool> Recorder { get; }

        private IMappedSemanticConstructorArgumentRecorderLogger Logger { get; }

        public PatternedArgumentRecorder(IArgumentPattern<TArgument> pattern, Func<TRecord, TArgument, bool> recorder, IMappedSemanticConstructorArgumentRecorderLogger logger)
        {
            Pattern = pattern;
            Recorder = recorder;

            Logger = logger;
        }

        bool IDetachedMappedSemanticConstructorArgumentRecorder<TRecord>.TryRecordArgument(TRecord dataRecord, object? argument)
        {
            if (dataRecord is null)
            {
                throw new ArgumentNullException(nameof(dataRecord));
            }

            return Pattern.TryFit(argument).Match
            (
                (error) =>
                {
                    Logger.ConstructorArgumentNotFollowingPattern();

                    return false;
                },
                (tArgument) => Recorder(dataRecord, tArgument)
            );
        }
    }
}
