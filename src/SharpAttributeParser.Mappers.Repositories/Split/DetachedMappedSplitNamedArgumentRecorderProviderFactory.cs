namespace SharpAttributeParser.Mappers.Repositories.Split;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Mappers.Repositories.Syntactic;
using SharpAttributeParser.Patterns;

using System;

/// <inheritdoc cref="IDetachedMappedSplitNamedArgumentRecorderProviderFactory{TSemanticRecord, TSyntacticRecord}"/>
public sealed class DetachedMappedSplitNamedArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> : IDetachedMappedSplitNamedArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>
{
    private IDetachedMappedSemanticNamedArgumentRecorderFactory<TSemanticRecord> SemanticFactory { get; }
    private IDetachedMappedSyntacticNamedArgumentRecorderFactory<TSyntacticRecord> SyntacticFactory { get; }

    /// <summary>Instantiates a <see cref="DetachedMappedSplitNamedArgumentRecorderProviderFactory{TSemanticRecord, TSyntacticRecord}"/>, handling creation of <see cref="IDetachedMappedSplitNamedArgumentRecorderProvider{TSemanticRecord, TSyntacticRecord}"/> related to named parameters.</summary>
    /// <param name="semanticFactory">Handles creation of the recorders used when arguments are parsed.</param>
    /// <param name="syntacticFactory">Handles creation of the recorders used when syntactic information about arguments is extracted.</param>
    /// <exception cref="ArgumentNullException"/>
    public DetachedMappedSplitNamedArgumentRecorderProviderFactory(IDetachedMappedSemanticNamedArgumentRecorderFactory<TSemanticRecord> semanticFactory, IDetachedMappedSyntacticNamedArgumentRecorderFactory<TSyntacticRecord> syntacticFactory)
    {
        SemanticFactory = semanticFactory ?? throw new ArgumentNullException(nameof(semanticFactory));
        SyntacticFactory = syntacticFactory ?? throw new ArgumentNullException(nameof(syntacticFactory));
    }

    IDetachedMappedSplitNamedArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord> IDetachedMappedSplitNamedArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>.Create(Func<TSemanticRecord, object?, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder)
    {
        if (semanticRecorder is null)
        {
            throw new ArgumentNullException(nameof(semanticRecorder));
        }

        if (syntacticRecorder is null)
        {
            throw new ArgumentNullException(nameof(syntacticRecorder));
        }

        var semantic = SemanticFactory.Create(semanticRecorder);
        var syntactic = SyntacticFactory.Create(syntacticRecorder);

        return Create(semantic, syntactic);
    }

    IDetachedMappedSplitNamedArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord> IDetachedMappedSplitNamedArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>.Create(Action<TSemanticRecord, object?> semanticRecorder, Action<TSyntacticRecord, ExpressionSyntax> syntacticRecorder)
    {
        if (semanticRecorder is null)
        {
            throw new ArgumentNullException(nameof(semanticRecorder));
        }

        if (syntacticRecorder is null)
        {
            throw new ArgumentNullException(nameof(syntacticRecorder));
        }

        var semantic = SemanticFactory.Create(semanticRecorder);
        var syntactic = SyntacticFactory.Create(syntacticRecorder);

        return Create(semantic, syntactic);
    }

    IDetachedMappedSplitNamedArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord> IDetachedMappedSplitNamedArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>.Create<T>(IArgumentPattern<T> pattern, Func<TSemanticRecord, T, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder)
    {
        if (pattern is null)
        {
            throw new ArgumentNullException(nameof(pattern));
        }

        if (semanticRecorder is null)
        {
            throw new ArgumentNullException(nameof(semanticRecorder));
        }

        if (syntacticRecorder is null)
        {
            throw new ArgumentNullException(nameof(syntacticRecorder));
        }

        var semantic = SemanticFactory.Create(pattern, semanticRecorder);
        var syntactic = SyntacticFactory.Create(syntacticRecorder);

        return Create(semantic, syntactic);
    }

    IDetachedMappedSplitNamedArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord> IDetachedMappedSplitNamedArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>.Create<T>(IArgumentPattern<T> pattern, Action<TSemanticRecord, T> semanticRecorder, Action<TSyntacticRecord, ExpressionSyntax> syntacticRecorder)
    {
        if (pattern is null)
        {
            throw new ArgumentNullException(nameof(pattern));
        }

        if (semanticRecorder is null)
        {
            throw new ArgumentNullException(nameof(semanticRecorder));
        }

        if (syntacticRecorder is null)
        {
            throw new ArgumentNullException(nameof(syntacticRecorder));
        }

        var semantic = SemanticFactory.Create(pattern, semanticRecorder);
        var syntactic = SyntacticFactory.Create(syntacticRecorder);

        return Create(semantic, syntactic);
    }

    IDetachedMappedSplitNamedArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord> IDetachedMappedSplitNamedArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>.Create<T>(Func<IArgumentPatternFactory, IArgumentPattern<T>> patternDelegate, Func<TSemanticRecord, T, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder)
    {
        if (patternDelegate is null)
        {
            throw new ArgumentNullException(nameof(patternDelegate));
        }

        if (semanticRecorder is null)
        {
            throw new ArgumentNullException(nameof(semanticRecorder));
        }

        if (syntacticRecorder is null)
        {
            throw new ArgumentNullException(nameof(syntacticRecorder));
        }

        var semantic = SemanticFactory.Create(patternDelegate, semanticRecorder);
        var syntactic = SyntacticFactory.Create(syntacticRecorder);

        return Create(semantic, syntactic);
    }

    IDetachedMappedSplitNamedArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord> IDetachedMappedSplitNamedArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>.Create<T>(Func<IArgumentPatternFactory, IArgumentPattern<T>> patternDelegate, Action<TSemanticRecord, T> semanticRecorder, Action<TSyntacticRecord, ExpressionSyntax> syntacticRecorder)
    {
        if (patternDelegate is null)
        {
            throw new ArgumentNullException(nameof(patternDelegate));
        }

        if (semanticRecorder is null)
        {
            throw new ArgumentNullException(nameof(semanticRecorder));
        }

        if (syntacticRecorder is null)
        {
            throw new ArgumentNullException(nameof(syntacticRecorder));
        }

        var semantic = SemanticFactory.Create(patternDelegate, semanticRecorder);
        var syntactic = SyntacticFactory.Create(syntacticRecorder);

        return Create(semantic, syntactic);
    }

    private static IDetachedMappedSplitNamedArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord> Create(IDetachedMappedSemanticNamedArgumentRecorder<TSemanticRecord> semantic, IDetachedMappedSyntacticNamedArgumentRecorder<TSyntacticRecord> syntactic)
    {
        if (semantic is null)
        {
            throw new InvalidOperationException($"A {nameof(IDetachedMappedSemanticNamedArgumentRecorderFactory<object>)} produced a null {nameof(IDetachedMappedSemanticNamedArgumentRecorder<object>)}.");
        }

        if (syntactic is null)
        {
            throw new InvalidOperationException($"A {nameof(IDetachedMappedSyntacticNamedArgumentRecorderFactory<object>)} produced a null {nameof(IDetachedMappedSyntacticNamedArgumentRecorder<object>)}.");
        }

        return new Provider(semantic, syntactic);
    }

    private sealed class Provider : IDetachedMappedSplitNamedArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>
    {
        private IDetachedMappedSemanticNamedArgumentRecorder<TSemanticRecord> Semantic { get; }
        private IDetachedMappedSyntacticNamedArgumentRecorder<TSyntacticRecord> Syntactic { get; }

        public Provider(IDetachedMappedSemanticNamedArgumentRecorder<TSemanticRecord> semantic, IDetachedMappedSyntacticNamedArgumentRecorder<TSyntacticRecord> syntactic)
        {
            Semantic = semantic;
            Syntactic = syntactic;
        }

        IDetachedMappedSemanticNamedArgumentRecorder<TSemanticRecord> IDetachedMappedSplitNamedArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>.Semantic => Semantic;
        IDetachedMappedSyntacticNamedArgumentRecorder<TSyntacticRecord> IDetachedMappedSplitNamedArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>.Syntactic => Syntactic;
    }
}
