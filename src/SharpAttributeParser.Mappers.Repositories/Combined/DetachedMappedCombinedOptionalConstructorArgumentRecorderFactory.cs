namespace SharpAttributeParser.Mappers.Repositories.Combined;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;
using OneOf.Types;

using SharpAttributeParser.Mappers.Repositories.Logging.Combined;
using SharpAttributeParser.Patterns;

using System;
using System.Collections.Generic;

/// <inheritdoc cref="IDetachedMappedCombinedOptionalConstructorArgumentRecorderFactory{TRecord}"/>
public sealed class DetachedMappedCombinedOptionalConstructorArgumentRecorderFactory<TRecord> : IDetachedMappedCombinedOptionalConstructorArgumentRecorderFactory<TRecord>
{
    private readonly IArgumentPatternFactory ArgumentPatternFactory;

    private readonly IMappedCombinedConstructorArgumentRecorderLoggerFactory LoggerFactory;

    /// <summary>Instantiates a <see cref="DetachedMappedCombinedOptionalConstructorArgumentRecorderFactory{TRecord}"/>, handling creation of <see cref="IDetachedMappedCombinedConstructorArgumentRecorder{TRecord}"/> related to optional constructor parameters.</summary>
    /// <param name="argumentPatternFactory">Handles creation of <see cref="IArgumentPattern{T}"/>.</param>
    /// <param name="loggerFactory">Handles creation of the loggers used by the created recorders.</param>
    public DetachedMappedCombinedOptionalConstructorArgumentRecorderFactory(IArgumentPatternFactory argumentPatternFactory, IMappedCombinedConstructorArgumentRecorderLoggerFactory? loggerFactory = null)
    {
        ArgumentPatternFactory = argumentPatternFactory ?? throw new ArgumentNullException(nameof(argumentPatternFactory));

        LoggerFactory = loggerFactory ?? NullMappedCombinedConstructorArgumentRecorderLoggerFactory.Singleton;
    }

    IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> IDetachedMappedCombinedOptionalConstructorArgumentRecorderFactory<TRecord>.Create(Func<TRecord, object?, OneOf<None, ExpressionSyntax>, bool> recorder)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return Create(recorder);
    }

    IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> IDetachedMappedCombinedOptionalConstructorArgumentRecorderFactory<TRecord>.Create(Action<TRecord, object?, OneOf<None, ExpressionSyntax>> recorder)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return Create(recorder.TrueReturning());
    }

    IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> IDetachedMappedCombinedOptionalConstructorArgumentRecorderFactory<TRecord>.Create<T>(IArgumentPattern<T> pattern, Func<TRecord, T, OneOf<None, ExpressionSyntax>, bool> recorder)
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

    IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> IDetachedMappedCombinedOptionalConstructorArgumentRecorderFactory<TRecord>.Create<T>(IArgumentPattern<T> pattern, Action<TRecord, T, OneOf<None, ExpressionSyntax>> recorder)
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

    IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> IDetachedMappedCombinedOptionalConstructorArgumentRecorderFactory<TRecord>.Create<T>(Func<IArgumentPatternFactory, IArgumentPattern<T>> patternDelegate, Func<TRecord, T, OneOf<None, ExpressionSyntax>, bool> recorder)
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

    IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> IDetachedMappedCombinedOptionalConstructorArgumentRecorderFactory<TRecord>.Create<T>(Func<IArgumentPatternFactory, IArgumentPattern<T>> patternDelegate, Action<TRecord, T, OneOf<None, ExpressionSyntax>> recorder)
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

    private IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> Create(Func<TRecord, object?, OneOf<None, ExpressionSyntax>, bool> recorder) => new UnpatternedArgumentRecorder(recorder);
    private IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> Create<T>(IArgumentPattern<T> pattern, Func<TRecord, T, OneOf<None, ExpressionSyntax>, bool> recorder)
    {
        var logger = LoggerFactory.Create<IDetachedMappedCombinedConstructorArgumentRecorder<TRecord>>();

        return new PatternedArgumentRecorder<T>(pattern, recorder, logger);
    }

    private sealed class UnpatternedArgumentRecorder : IDetachedMappedCombinedConstructorArgumentRecorder<TRecord>
    {
        private readonly Func<TRecord, object?, OneOf<None, ExpressionSyntax>, bool> Recorder;

        public UnpatternedArgumentRecorder(Func<TRecord, object?, OneOf<None, ExpressionSyntax>, bool> recorder)
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

        bool IDetachedMappedCombinedConstructorArgumentRecorder<TRecord>.TryRecordDefaultArgument(TRecord dataRecord, object? argument)
        {
            if (dataRecord is null)
            {
                throw new ArgumentNullException(nameof(dataRecord));
            }

            return Recorder(dataRecord, argument, new None());
        }
    }

    private sealed class PatternedArgumentRecorder<TArgument> : IDetachedMappedCombinedConstructorArgumentRecorder<TRecord>
    {
        private readonly IArgumentPattern<TArgument> Pattern;
        private readonly Func<TRecord, TArgument, OneOf<None, ExpressionSyntax>, bool> Recorder;

        private readonly IMappedCombinedConstructorArgumentRecorderLogger Logger;

        public PatternedArgumentRecorder(IArgumentPattern<TArgument> pattern, Func<TRecord, TArgument, OneOf<None, ExpressionSyntax>, bool> recorder, IMappedCombinedConstructorArgumentRecorderLogger logger)
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

        bool IDetachedMappedCombinedConstructorArgumentRecorder<TRecord>.TryRecordDefaultArgument(TRecord dataRecord, object? argument)
        {
            if (dataRecord is null)
            {
                throw new ArgumentNullException(nameof(dataRecord));
            }

            return Pattern.TryFit(argument).Match
            (
                (error) => false,
                (tArgument) => Recorder(dataRecord, tArgument, new None())
            );
        }
    }
}
