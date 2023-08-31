namespace SharpAttributeParser.Mappers.Repositories.Adaptive;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser.Mappers.Repositories.Combined;
using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Patterns;

using System;

/// <inheritdoc cref="IDetachedMappedAdaptiveNormalConstructorArgumentRecorderProviderFactory{TCombinedRecord, TSemanticRecord}"/>
public sealed class DetachedMappedAdaptiveNormalConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> : IDetachedMappedAdaptiveNormalConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>
{
    private IDetachedMappedCombinedNormalConstructorArgumentRecorderFactory<TCombinedRecord> CombinedFactory { get; }
    private IDetachedMappedSemanticConstructorArgumentRecorderFactory<TSemanticRecord> SemanticFactory { get; }

    /// <summary>Instantiates a <see cref="DetachedMappedAdaptiveNormalConstructorArgumentRecorderProviderFactory{TCombinedRecord, TSemanticRecord}"/>, handling creation of <see cref="IDetachedMappedAdaptiveConstructorArgumentRecorderProvider{TCombinedRecord, TSemanticRecord}"/> related to non-optional, non-<see langword="params"/> constructor parameters.</summary>
    /// <param name="combinedFactory">Handles creation of the recorders used when arguments are parsed with syntactic context.</param>
    /// <param name="semanticFactory">Handles creation of the recorders used when arguments are parsed without syntactic context.</param>
    public DetachedMappedAdaptiveNormalConstructorArgumentRecorderProviderFactory(IDetachedMappedCombinedNormalConstructorArgumentRecorderFactory<TCombinedRecord> combinedFactory, IDetachedMappedSemanticConstructorArgumentRecorderFactory<TSemanticRecord> semanticFactory)
    {
        CombinedFactory = combinedFactory ?? throw new ArgumentNullException(nameof(combinedFactory));
        SemanticFactory = semanticFactory ?? throw new ArgumentNullException(nameof(semanticFactory));
    }

    IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord> IDetachedMappedAdaptiveNormalConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>.Create(Func<TCombinedRecord, object?, ExpressionSyntax, bool> combinedRecorder, Func<TSemanticRecord, object?, bool> semanticRecorder)
    {
        if (combinedRecorder is null)
        {
            throw new ArgumentNullException(nameof(combinedRecorder));
        }

        if (semanticRecorder is null)
        {
            throw new ArgumentNullException(nameof(semanticRecorder));
        }

        var combined = CombinedFactory.Create(combinedRecorder);
        var semantic = SemanticFactory.Create(semanticRecorder);

        return Create(combined, semantic);
    }

    IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord> IDetachedMappedAdaptiveNormalConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>.Create(Action<TCombinedRecord, object?, ExpressionSyntax> combinedRecorder, Action<TSemanticRecord, object?> semanticRecorder)
    {
        if (combinedRecorder is null)
        {
            throw new ArgumentNullException(nameof(combinedRecorder));
        }

        if (semanticRecorder is null)
        {
            throw new ArgumentNullException(nameof(semanticRecorder));
        }

        var combined = CombinedFactory.Create(combinedRecorder);
        var semantic = SemanticFactory.Create(semanticRecorder);

        return Create(combined, semantic);
    }

    IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord> IDetachedMappedAdaptiveNormalConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>.Create<T>(IArgumentPattern<T> pattern, Func<TCombinedRecord, T, ExpressionSyntax, bool> combinedRecorder, Func<TSemanticRecord, T, bool> semanticRecorder)
    {
        if (pattern is null)
        {
            throw new ArgumentNullException(nameof(pattern));
        }

        if (combinedRecorder is null)
        {
            throw new ArgumentNullException(nameof(combinedRecorder));
        }

        if (semanticRecorder is null)
        {
            throw new ArgumentNullException(nameof(semanticRecorder));
        }

        var combined = CombinedFactory.Create(pattern, combinedRecorder);
        var semantic = SemanticFactory.Create(pattern, semanticRecorder);

        return Create(combined, semantic);
    }

    IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord> IDetachedMappedAdaptiveNormalConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>.Create<T>(IArgumentPattern<T> pattern, Action<TCombinedRecord, T, ExpressionSyntax> combinedRecorder, Action<TSemanticRecord, T> semanticRecorder)
    {
        if (pattern is null)
        {
            throw new ArgumentNullException(nameof(pattern));
        }

        if (combinedRecorder is null)
        {
            throw new ArgumentNullException(nameof(combinedRecorder));
        }

        if (semanticRecorder is null)
        {
            throw new ArgumentNullException(nameof(semanticRecorder));
        }

        var combined = CombinedFactory.Create(pattern, combinedRecorder);
        var semantic = SemanticFactory.Create(pattern, semanticRecorder);

        return Create(combined, semantic);
    }

    IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord> IDetachedMappedAdaptiveNormalConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>.Create<T>(Func<IArgumentPatternFactory, IArgumentPattern<T>> patternDelegate, Func<TCombinedRecord, T, ExpressionSyntax, bool> combinedRecorder, Func<TSemanticRecord, T, bool> semanticRecorder)
    {
        if (patternDelegate is null)
        {
            throw new ArgumentNullException(nameof(patternDelegate));
        }

        if (combinedRecorder is null)
        {
            throw new ArgumentNullException(nameof(combinedRecorder));
        }

        if (semanticRecorder is null)
        {
            throw new ArgumentNullException(nameof(semanticRecorder));
        }

        var combined = CombinedFactory.Create(patternDelegate, combinedRecorder);
        var semantic = SemanticFactory.Create(patternDelegate, semanticRecorder);

        return Create(combined, semantic);
    }

    IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord> IDetachedMappedAdaptiveNormalConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>.Create<T>(Func<IArgumentPatternFactory, IArgumentPattern<T>> patternDelegate, Action<TCombinedRecord, T, ExpressionSyntax> combinedRecorder, Action<TSemanticRecord, T> semanticRecorder)
    {
        if (patternDelegate is null)
        {
            throw new ArgumentNullException(nameof(patternDelegate));
        }

        if (combinedRecorder is null)
        {
            throw new ArgumentNullException(nameof(combinedRecorder));
        }

        if (semanticRecorder is null)
        {
            throw new ArgumentNullException(nameof(semanticRecorder));
        }

        var combined = CombinedFactory.Create(patternDelegate, combinedRecorder);
        var semantic = SemanticFactory.Create(patternDelegate, semanticRecorder);

        return Create(combined, semantic);
    }

    private static IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord> Create(IDetachedMappedCombinedConstructorArgumentRecorder<TCombinedRecord> combined, IDetachedMappedSemanticConstructorArgumentRecorder<TSemanticRecord> semantic) => new DetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>(combined, semantic);
}
