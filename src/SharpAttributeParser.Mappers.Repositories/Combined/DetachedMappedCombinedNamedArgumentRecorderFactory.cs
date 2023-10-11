namespace SharpAttributeParser.Mappers.Repositories.Combined;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser.Mappers.Repositories.Logging.Combined;
using SharpAttributeParser.Patterns;

using System;

/// <inheritdoc cref="IDetachedMappedCombinedNamedArgumentRecorderFactory{TRecord}"/>
public sealed class DetachedMappedCombinedNamedArgumentRecorderFactory<TRecord> : IDetachedMappedCombinedNamedArgumentRecorderFactory<TRecord>
{
    private readonly IArgumentPatternFactory ArgumentPatternFactory;

    private readonly IMappedCombinedNamedArgumentRecorderLoggerFactory LoggerFactory;

    /// <summary>Instantiates a <see cref="DetachedMappedCombinedNamedArgumentRecorderFactory{TRecord}"/>, handling creation of <see cref="IDetachedMappedCombinedNamedArgumentRecorder{TRecord}"/>.</summary>
    /// <param name="argumentPatternFactory">Handles creation of <see cref="IArgumentPattern{T}"/>.</param>
    /// <param name="loggerFactory">Handles creation of the loggers used by the created recorders.</param>
    public DetachedMappedCombinedNamedArgumentRecorderFactory(IArgumentPatternFactory argumentPatternFactory, IMappedCombinedNamedArgumentRecorderLoggerFactory? loggerFactory = null)
    {
        ArgumentPatternFactory = argumentPatternFactory ?? throw new ArgumentNullException(nameof(argumentPatternFactory));

        LoggerFactory = loggerFactory ?? NullMappedCombinedNamedArgumentRecorderLoggerFactory.Singleton;
    }

    IDetachedMappedCombinedNamedArgumentRecorder<TRecord> IDetachedMappedCombinedNamedArgumentRecorderFactory<TRecord>.Create(Func<TRecord, object?, ExpressionSyntax, bool> recorder)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return Create(recorder);
    }

    IDetachedMappedCombinedNamedArgumentRecorder<TRecord> IDetachedMappedCombinedNamedArgumentRecorderFactory<TRecord>.Create(Action<TRecord, object?, ExpressionSyntax> recorder)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return Create(recorder.TrueReturning());
    }

    IDetachedMappedCombinedNamedArgumentRecorder<TRecord> IDetachedMappedCombinedNamedArgumentRecorderFactory<TRecord>.Create<T>(IArgumentPattern<T> pattern, Func<TRecord, T, ExpressionSyntax, bool> recorder)
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

    IDetachedMappedCombinedNamedArgumentRecorder<TRecord> IDetachedMappedCombinedNamedArgumentRecorderFactory<TRecord>.Create<T>(IArgumentPattern<T> pattern, Action<TRecord, T, ExpressionSyntax> recorder)
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

    IDetachedMappedCombinedNamedArgumentRecorder<TRecord> IDetachedMappedCombinedNamedArgumentRecorderFactory<TRecord>.Create<T>(Func<IArgumentPatternFactory, IArgumentPattern<T>> patternDelegate, Func<TRecord, T, ExpressionSyntax, bool> recorder)
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

    IDetachedMappedCombinedNamedArgumentRecorder<TRecord> IDetachedMappedCombinedNamedArgumentRecorderFactory<TRecord>.Create<T>(Func<IArgumentPatternFactory, IArgumentPattern<T>> patternDelegate, Action<TRecord, T, ExpressionSyntax> recorder)
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

    private IDetachedMappedCombinedNamedArgumentRecorder<TRecord> Create(Func<TRecord, object?, ExpressionSyntax, bool> recorder) => new UnpatternedArgumentRecorder(recorder);
    private IDetachedMappedCombinedNamedArgumentRecorder<TRecord> Create<T>(IArgumentPattern<T> pattern, Func<TRecord, T, ExpressionSyntax, bool> recorder)
    {
        var logger = LoggerFactory.Create<IDetachedMappedCombinedNamedArgumentRecorder<TRecord>>();

        return new PatternedArgumentRecorder<T>(pattern, recorder, logger);
    }

    private sealed class UnpatternedArgumentRecorder : IDetachedMappedCombinedNamedArgumentRecorder<TRecord>
    {
        private readonly Func<TRecord, object?, ExpressionSyntax, bool> Recorder;

        public UnpatternedArgumentRecorder(Func<TRecord, object?, ExpressionSyntax, bool> recorder)
        {
            Recorder = recorder;
        }

        bool IDetachedMappedCombinedNamedArgumentRecorder<TRecord>.TryRecordArgument(TRecord dataRecord, object? argument, ExpressionSyntax syntax)
        {
            if (dataRecord is null)
            {
                throw new ArgumentNullException(nameof(dataRecord));
            }

            if (syntax is null)
            {
                throw new ArgumentNullException(nameof(syntax));
            }

            return Recorder(dataRecord, argument, syntax);
        }
    }

    private sealed class PatternedArgumentRecorder<TArgument> : IDetachedMappedCombinedNamedArgumentRecorder<TRecord>
    {
        private readonly IArgumentPattern<TArgument> Pattern;
        private readonly Func<TRecord, TArgument, ExpressionSyntax, bool> Recorder;

        private readonly IMappedCombinedNamedArgumentRecorderLogger Logger;

        public PatternedArgumentRecorder(IArgumentPattern<TArgument> pattern, Func<TRecord, TArgument, ExpressionSyntax, bool> recorder, IMappedCombinedNamedArgumentRecorderLogger logger)
        {
            Pattern = pattern;
            Recorder = recorder;

            Logger = logger;
        }

        bool IDetachedMappedCombinedNamedArgumentRecorder<TRecord>.TryRecordArgument(TRecord dataRecord, object? argument, ExpressionSyntax syntax)
        {
            if (dataRecord is null)
            {
                throw new ArgumentNullException(nameof(dataRecord));
            }

            if (syntax is null)
            {
                throw new ArgumentNullException(nameof(syntax));
            }

            return Pattern.TryFit(argument).Match
            (
                (error) =>
                {
                    Logger.NamedArgumentNotFollowingPattern();

                    return false;
                },
                (tArgument) => Recorder(dataRecord, tArgument, syntax)
            );
        }
    }
}
