namespace SharpAttributeParser.Mappers.Repositories.Split;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Mappers.Repositories.Syntactic;
using SharpAttributeParser.Patterns;

using System;

/// <inheritdoc cref="IDetachedMappedSplitNamedArgumentRecorderProviderFactory{TSemanticRecord, TSyntacticRecord}"/>
public sealed class DetachedMappedSplitNamedArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> : IDetachedMappedSplitNamedArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>
{
    private readonly IDetachedMappedSemanticNamedArgumentRecorderFactory<TSemanticRecord> SemanticFactory;
    private readonly IDetachedMappedSyntacticNamedArgumentRecorderFactory<TSyntacticRecord> SyntacticFactory;

    /// <summary>Instantiates a <see cref="DetachedMappedSplitNamedArgumentRecorderProviderFactory{TSemanticRecord, TSyntacticRecord}"/>, handling creation of <see cref="IDetachedMappedSplitNamedArgumentRecorderProvider{TSemanticRecord, TSyntacticRecord}"/> related to named parameters.</summary>
    /// <param name="semanticFactory">Handles creation of the recorders used when arguments are parsed.</param>
    /// <param name="syntacticFactory">Handles creation of the recorders used when syntactic information about arguments is extracted.</param>
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

    private static IDetachedMappedSplitNamedArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord> Create(IDetachedMappedSemanticNamedArgumentRecorder<TSemanticRecord> semantic, IDetachedMappedSyntacticNamedArgumentRecorder<TSyntacticRecord> syntactic) => new Provider(semantic, syntactic);

    private sealed class Provider : IDetachedMappedSplitNamedArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>
    {
        private readonly IDetachedMappedSemanticNamedArgumentRecorder<TSemanticRecord> Semantic;
        private readonly IDetachedMappedSyntacticNamedArgumentRecorder<TSyntacticRecord> Syntactic;

        public Provider(IDetachedMappedSemanticNamedArgumentRecorder<TSemanticRecord> semantic, IDetachedMappedSyntacticNamedArgumentRecorder<TSyntacticRecord> syntactic)
        {
            Semantic = semantic;
            Syntactic = syntactic;
        }

        IDetachedMappedSemanticNamedArgumentRecorder<TSemanticRecord> IDetachedMappedSplitNamedArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>.Semantic => Semantic;
        IDetachedMappedSyntacticNamedArgumentRecorder<TSyntacticRecord> IDetachedMappedSplitNamedArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>.Syntactic => Syntactic;
    }
}
