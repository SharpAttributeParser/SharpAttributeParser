namespace SharpAttributeParser.Mappers.Repositories.Adaptive;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser.Mappers.Repositories.Combined;
using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Patterns;

using System;

/// <inheritdoc cref="IDetachedMappedAdaptiveNamedArgumentRecorderProviderFactory{TCombinedRecord, TSemanticRecord}"/>
public sealed class DetachedMappedAdaptiveNamedArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> : IDetachedMappedAdaptiveNamedArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>
{
    private readonly IDetachedMappedCombinedNamedArgumentRecorderFactory<TCombinedRecord> CombinedFactory;
    private readonly IDetachedMappedSemanticNamedArgumentRecorderFactory<TSemanticRecord> SemanticFactory;

    /// <summary>Instantiates a <see cref="DetachedMappedAdaptiveNamedArgumentRecorderProviderFactory{TCombinedRecord, TSemanticRecord}"/>, handling creation of <see cref="IDetachedMappedAdaptiveNamedArgumentRecorderProvider{TCombinedRecord, TSemanticRecord}"/> related to named parameters.</summary>
    /// <param name="combinedFactory">Handles creation of the recorders used when arguments are parsed with syntactic context.</param>
    /// <param name="semanticFactory">Handles creation of the recorders used when arguments are parsed without syntactic context.</param>
    public DetachedMappedAdaptiveNamedArgumentRecorderProviderFactory(IDetachedMappedCombinedNamedArgumentRecorderFactory<TCombinedRecord> combinedFactory, IDetachedMappedSemanticNamedArgumentRecorderFactory<TSemanticRecord> semanticFactory)
    {
        CombinedFactory = combinedFactory ?? throw new ArgumentNullException(nameof(combinedFactory));
        SemanticFactory = semanticFactory ?? throw new ArgumentNullException(nameof(semanticFactory));
    }

    IDetachedMappedAdaptiveNamedArgumentRecorderProvider<TCombinedRecord, TSemanticRecord> IDetachedMappedAdaptiveNamedArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>.Create(Func<TCombinedRecord, object?, ExpressionSyntax, bool> combinedRecorder, Func<TSemanticRecord, object?, bool> semanticRecorder)
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

    IDetachedMappedAdaptiveNamedArgumentRecorderProvider<TCombinedRecord, TSemanticRecord> IDetachedMappedAdaptiveNamedArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>.Create(Action<TCombinedRecord, object?, ExpressionSyntax> combinedRecorder, Action<TSemanticRecord, object?> semanticRecorder)
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

    IDetachedMappedAdaptiveNamedArgumentRecorderProvider<TCombinedRecord, TSemanticRecord> IDetachedMappedAdaptiveNamedArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>.Create<T>(IArgumentPattern<T> pattern, Func<TCombinedRecord, T, ExpressionSyntax, bool> combinedRecorder, Func<TSemanticRecord, T, bool> semanticRecorder)
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

    IDetachedMappedAdaptiveNamedArgumentRecorderProvider<TCombinedRecord, TSemanticRecord> IDetachedMappedAdaptiveNamedArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>.Create<T>(IArgumentPattern<T> pattern, Action<TCombinedRecord, T, ExpressionSyntax> combinedRecorder, Action<TSemanticRecord, T> semanticRecorder)
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

    IDetachedMappedAdaptiveNamedArgumentRecorderProvider<TCombinedRecord, TSemanticRecord> IDetachedMappedAdaptiveNamedArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>.Create<T>(Func<IArgumentPatternFactory, IArgumentPattern<T>> patternDelegate, Func<TCombinedRecord, T, ExpressionSyntax, bool> combinedRecorder, Func<TSemanticRecord, T, bool> semanticRecorder)
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

    IDetachedMappedAdaptiveNamedArgumentRecorderProvider<TCombinedRecord, TSemanticRecord> IDetachedMappedAdaptiveNamedArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>.Create<T>(Func<IArgumentPatternFactory, IArgumentPattern<T>> patternDelegate, Action<TCombinedRecord, T, ExpressionSyntax> combinedRecorder, Action<TSemanticRecord, T> semanticRecorder)
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

    private static IDetachedMappedAdaptiveNamedArgumentRecorderProvider<TCombinedRecord, TSemanticRecord> Create(IDetachedMappedCombinedNamedArgumentRecorder<TCombinedRecord> combined, IDetachedMappedSemanticNamedArgumentRecorder<TSemanticRecord> semantic) => new Provider(combined, semantic);

    private sealed class Provider : IDetachedMappedAdaptiveNamedArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>
    {
        private readonly IDetachedMappedCombinedNamedArgumentRecorder<TCombinedRecord> Combined;
        private readonly IDetachedMappedSemanticNamedArgumentRecorder<TSemanticRecord> Semantic;

        public Provider(IDetachedMappedCombinedNamedArgumentRecorder<TCombinedRecord> combined, IDetachedMappedSemanticNamedArgumentRecorder<TSemanticRecord> semantic)
        {
            Combined = combined;
            Semantic = semantic;
        }

        IDetachedMappedCombinedNamedArgumentRecorder<TCombinedRecord> IDetachedMappedAdaptiveNamedArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>.Combined => Combined;
        IDetachedMappedSemanticNamedArgumentRecorder<TSemanticRecord> IDetachedMappedAdaptiveNamedArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>.Semantic => Semantic;
    }
}
