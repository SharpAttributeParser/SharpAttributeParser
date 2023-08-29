namespace SharpAttributeParser.Mappers.Repositories.Combined;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser.Mappers.Repositories.Logging.Combined;
using SharpAttributeParser.Patterns;

using System;
using System.Collections.Generic;

/// <inheritdoc cref="IDetachedMappedCombinedNormalConstructorArgumentRecorderFactory{TRecord}"/>
public sealed class DetachedMappedCombinedNormalConstructorArgumentRecorderFactory<TRecord> : IDetachedMappedCombinedNormalConstructorArgumentRecorderFactory<TRecord>
{
    private IArgumentPatternFactory ArgumentPatternFactory { get; }

    private IMappedCombinedConstructorArgumentRecorderLoggerFactory LoggerFactory { get; }

    /// <summary>Instantiates a <see cref="DetachedMappedCombinedNormalConstructorArgumentRecorderFactory{TRecord}"/>, handling creation of <see cref="IDetachedMappedCombinedConstructorArgumentRecorder{TRecord}"/> related to non-optional, non-<see langword="params"/> constructor parameters.</summary>
    /// <param name="argumentPatternFactory">Handles creation of <see cref="IArgumentPattern{T}"/>.</param>
    /// <param name="loggerFactory">Handles creation of the loggers used by the created recorders.</param>
    /// <exception cref="ArgumentNullException"/>
    public DetachedMappedCombinedNormalConstructorArgumentRecorderFactory(IArgumentPatternFactory argumentPatternFactory, IMappedCombinedConstructorArgumentRecorderLoggerFactory? loggerFactory = null)
    {
        ArgumentPatternFactory = argumentPatternFactory ?? throw new ArgumentNullException(nameof(argumentPatternFactory));

        LoggerFactory = loggerFactory ?? NullMappedCombinedConstructorArgumentRecorderLoggerFactory.Singleton;
    }

    IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> IDetachedMappedCombinedNormalConstructorArgumentRecorderFactory<TRecord>.Create(Func<TRecord, object?, ExpressionSyntax, bool> recorder)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return Create(recorder);
    }

    IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> IDetachedMappedCombinedNormalConstructorArgumentRecorderFactory<TRecord>.Create(Action<TRecord, object?, ExpressionSyntax> recorder)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return Create(recorder.TrueReturning());
    }

    IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> IDetachedMappedCombinedNormalConstructorArgumentRecorderFactory<TRecord>.Create<T>(IArgumentPattern<T> pattern, Func<TRecord, T, ExpressionSyntax, bool> recorder)
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

    IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> IDetachedMappedCombinedNormalConstructorArgumentRecorderFactory<TRecord>.Create<T>(IArgumentPattern<T> pattern, Action<TRecord, T, ExpressionSyntax> recorder)
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

    IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> IDetachedMappedCombinedNormalConstructorArgumentRecorderFactory<TRecord>.Create<T>(Func<IArgumentPatternFactory, IArgumentPattern<T>> patternDelegate, Func<TRecord, T, ExpressionSyntax, bool> recorder)
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

    IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> IDetachedMappedCombinedNormalConstructorArgumentRecorderFactory<TRecord>.Create<T>(Func<IArgumentPatternFactory, IArgumentPattern<T>> patternDelegate, Action<TRecord, T, ExpressionSyntax> recorder)
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

    private IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> Create(Func<TRecord, object?, ExpressionSyntax, bool> recorder) => new UnpatternedArgumentRecorder(recorder);
    private IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> Create<T>(IArgumentPattern<T> pattern, Func<TRecord, T, ExpressionSyntax, bool> recorder)
    {
        var logger = LoggerFactory.Create<IDetachedMappedCombinedConstructorArgumentRecorder<TRecord>>();

        return new PatternedArgumentRecorder<T>(pattern, recorder, logger);
    }

    private sealed class UnpatternedArgumentRecorder : IDetachedMappedCombinedConstructorArgumentRecorder<TRecord>
    {
        private Func<TRecord, object?, ExpressionSyntax, bool> Recorder { get; }

        public UnpatternedArgumentRecorder(Func<TRecord, object?, ExpressionSyntax, bool> recorder)
        {
            Recorder = recorder;
        }

        bool IDetachedMappedCombinedConstructorArgumentRecorder<TRecord>.TryRecordArgument(TRecord dataRecord, object? argument, ExpressionSyntax syntax)
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

        bool IDetachedMappedCombinedConstructorArgumentRecorder<TRecord>.TryRecordParamsArgument(TRecord dataRecord, object? argument, IReadOnlyList<ExpressionSyntax> elementSyntax) => false;
        bool IDetachedMappedCombinedConstructorArgumentRecorder<TRecord>.TryRecordDefaultArgument(TRecord dataRecord, object? argument) => false;
    }

    private sealed class PatternedArgumentRecorder<TArgument> : IDetachedMappedCombinedConstructorArgumentRecorder<TRecord>
    {
        private IArgumentPattern<TArgument> Pattern { get; }
        private Func<TRecord, TArgument, ExpressionSyntax, bool> Recorder { get; }

        private IMappedCombinedConstructorArgumentRecorderLogger Logger { get; }

        public PatternedArgumentRecorder(IArgumentPattern<TArgument> pattern, Func<TRecord, TArgument, ExpressionSyntax, bool> recorder, IMappedCombinedConstructorArgumentRecorderLogger logger)
        {
            Pattern = pattern;
            Recorder = recorder;

            Logger = logger;
        }

        bool IDetachedMappedCombinedConstructorArgumentRecorder<TRecord>.TryRecordArgument(TRecord dataRecord, object? argument, ExpressionSyntax syntax)
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
                    Logger.ConstructorArgumentNotFollowingPattern();

                    return false;
                },
                (tArgument) => Recorder(dataRecord, tArgument, syntax)
            );
        }

        bool IDetachedMappedCombinedConstructorArgumentRecorder<TRecord>.TryRecordParamsArgument(TRecord dataRecord, object? argument, IReadOnlyList<ExpressionSyntax> elementSyntax) => false;
        bool IDetachedMappedCombinedConstructorArgumentRecorder<TRecord>.TryRecordDefaultArgument(TRecord dataRecord, object? argument) => false;
    }
}
