namespace SharpAttributeParser.Mappers.Repositories.Semantic;

using SharpAttributeParser.Mappers.Repositories.Logging.Semantic;
using SharpAttributeParser.Patterns;

using System;

/// <inheritdoc cref="IDetachedMappedSemanticNamedArgumentRecorderFactory{TRecord}"/>
public sealed class DetachedMappedSemanticNamedArgumentRecorderFactory<TRecord> : IDetachedMappedSemanticNamedArgumentRecorderFactory<TRecord>
{
    private IArgumentPatternFactory ArgumentPatternFactory { get; }

    private IMappedSemanticNamedArgumentRecorderLoggerFactory LoggerFactory { get; }

    /// <summary>Instantiates a <see cref="DetachedMappedSemanticNamedArgumentRecorderFactory{TRecord}"/>, handling creation of <see cref="IDetachedMappedSemanticNamedArgumentRecorder{TRecord}"/>.</summary>
    /// <param name="argumentPatternFactory">Handles creation of <see cref="IArgumentPattern{T}"/>.</param>
    /// <param name="loggerFactory">Handles creation of the loggers used by the created recorders.</param>
    /// <exception cref="ArgumentNullException"/>
    public DetachedMappedSemanticNamedArgumentRecorderFactory(IArgumentPatternFactory argumentPatternFactory, IMappedSemanticNamedArgumentRecorderLoggerFactory? loggerFactory = null)
    {
        ArgumentPatternFactory = argumentPatternFactory ?? throw new ArgumentNullException(nameof(argumentPatternFactory));

        LoggerFactory = loggerFactory ?? NullMappedSemanticNamedArgumentRecorderLoggerFactory.Singleton;
    }

    IDetachedMappedSemanticNamedArgumentRecorder<TRecord> IDetachedMappedSemanticNamedArgumentRecorderFactory<TRecord>.Create(Func<TRecord, object?, bool> recorder)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return Create(recorder);
    }

    IDetachedMappedSemanticNamedArgumentRecorder<TRecord> IDetachedMappedSemanticNamedArgumentRecorderFactory<TRecord>.Create(Action<TRecord, object?> recorder)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return Create(recorder.TrueReturning());
    }

    IDetachedMappedSemanticNamedArgumentRecorder<TRecord> IDetachedMappedSemanticNamedArgumentRecorderFactory<TRecord>.Create<T>(IArgumentPattern<T> pattern, Func<TRecord, T, bool> recorder)
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

    IDetachedMappedSemanticNamedArgumentRecorder<TRecord> IDetachedMappedSemanticNamedArgumentRecorderFactory<TRecord>.Create<T>(IArgumentPattern<T> pattern, Action<TRecord, T> recorder)
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

    IDetachedMappedSemanticNamedArgumentRecorder<TRecord> IDetachedMappedSemanticNamedArgumentRecorderFactory<TRecord>.Create<T>(Func<IArgumentPatternFactory, IArgumentPattern<T>> patternDelegate, Func<TRecord, T, bool> recorder)
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

    IDetachedMappedSemanticNamedArgumentRecorder<TRecord> IDetachedMappedSemanticNamedArgumentRecorderFactory<TRecord>.Create<T>(Func<IArgumentPatternFactory, IArgumentPattern<T>> patternDelegate, Action<TRecord, T> recorder)
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

    private IDetachedMappedSemanticNamedArgumentRecorder<TRecord> Create(Func<TRecord, object?, bool> recorder) => new UnpatternedArgumentRecorder(recorder);
    private IDetachedMappedSemanticNamedArgumentRecorder<TRecord> Create<T>(IArgumentPattern<T> pattern, Func<TRecord, T, bool> recorder)
    {
        var logger = LoggerFactory.Create<IDetachedMappedSemanticNamedArgumentRecorder<TRecord>>();

        return new PatternedArgumentRecorder<T>(pattern, recorder, logger);
    }

    private sealed class UnpatternedArgumentRecorder : IDetachedMappedSemanticNamedArgumentRecorder<TRecord>
    {
        private Func<TRecord, object?, bool> Recorder { get; }

        public UnpatternedArgumentRecorder(Func<TRecord, object?, bool> recorder)
        {
            Recorder = recorder;
        }

        bool IDetachedMappedSemanticNamedArgumentRecorder<TRecord>.TryRecordArgument(TRecord dataRecord, object? argument)
        {
            if (dataRecord is null)
            {
                throw new ArgumentNullException(nameof(dataRecord));
            }

            return Recorder(dataRecord, argument);
        }
    }

    private sealed class PatternedArgumentRecorder<TArgument> : IDetachedMappedSemanticNamedArgumentRecorder<TRecord>
    {
        private IArgumentPattern<TArgument> Pattern { get; }
        private Func<TRecord, TArgument, bool> Recorder { get; }

        private IMappedSemanticNamedArgumentRecorderLogger Logger { get; }

        public PatternedArgumentRecorder(IArgumentPattern<TArgument> pattern, Func<TRecord, TArgument, bool> recorder, IMappedSemanticNamedArgumentRecorderLogger logger)
        {
            Pattern = pattern;
            Recorder = recorder;

            Logger = logger;
        }

        bool IDetachedMappedSemanticNamedArgumentRecorder<TRecord>.TryRecordArgument(TRecord dataRecord, object? argument)
        {
            if (dataRecord is null)
            {
                throw new ArgumentNullException(nameof(dataRecord));
            }

            return Pattern.TryFit(argument).Match
            (
                (error) =>
                {
                    Logger.NamedArgumentNotFollowingPattern();

                    return false;
                },
                (tArgument) => Recorder(dataRecord, tArgument)
            );
        }
    }
}
