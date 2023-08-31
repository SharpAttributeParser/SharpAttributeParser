namespace SharpAttributeParser.Mappers.Repositories.Split;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;
using OneOf.Types;

using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Mappers.Repositories.Syntactic;
using SharpAttributeParser.Patterns;

using System;

/// <inheritdoc cref="IDetachedMappedSplitOptionalConstructorArgumentRecorderProviderFactory{TSemanticRecord, TSyntacticRecord}"/>
public sealed class DetachedMappedSplitOptionalConstructorArgumentRecorderFactory<TSemanticRecord, TSyntacticRecord> : IDetachedMappedSplitOptionalConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>
{
    private IDetachedMappedSemanticConstructorArgumentRecorderFactory<TSemanticRecord> SemanticFactory { get; }
    private IDetachedMappedSyntacticOptionalConstructorArgumentRecorderFactory<TSyntacticRecord> SyntacticFactory { get; }

    /// <summary>Instantiates a <see cref="DetachedMappedSplitOptionalConstructorArgumentRecorderFactory{TSemanticRecord, TSyntacticRecord}"/>, handling creation of <see cref="IDetachedMappedSplitConstructorArgumentRecorderProvider{TSemanticRecord, TSyntacticRecord}"/> related to optional constructor parameters.</summary>
    /// <param name="semanticFactory">Handles creation of the recorders used when arguments are parsed.</param>
    /// <param name="syntacticFactory">Handles creation of the recorders used when syntactic information about arguments is extracted.</param>
    public DetachedMappedSplitOptionalConstructorArgumentRecorderFactory(IDetachedMappedSemanticConstructorArgumentRecorderFactory<TSemanticRecord> semanticFactory, IDetachedMappedSyntacticOptionalConstructorArgumentRecorderFactory<TSyntacticRecord> syntacticFactory)
    {
        SemanticFactory = semanticFactory ?? throw new ArgumentNullException(nameof(semanticFactory));
        SyntacticFactory = syntacticFactory ?? throw new ArgumentNullException(nameof(syntacticFactory));
    }

    IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord> IDetachedMappedSplitOptionalConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>.Create(Func<TSemanticRecord, object?, bool> semanticRecorder, Func<TSyntacticRecord, OneOf<None, ExpressionSyntax>, bool> syntacticRecorder)
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

    IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord> IDetachedMappedSplitOptionalConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>.Create(Action<TSemanticRecord, object?> semanticRecorder, Action<TSyntacticRecord, OneOf<None, ExpressionSyntax>> syntacticRecorder)
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

    IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord> IDetachedMappedSplitOptionalConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>.Create<T>(IArgumentPattern<T> pattern, Func<TSemanticRecord, T, bool> semanticRecorder, Func<TSyntacticRecord, OneOf<None, ExpressionSyntax>, bool> syntacticRecorder)
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

    IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord> IDetachedMappedSplitOptionalConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>.Create<T>(IArgumentPattern<T> pattern, Action<TSemanticRecord, T> semanticRecorder, Action<TSyntacticRecord, OneOf<None, ExpressionSyntax>> syntacticRecorder)
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

    IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord> IDetachedMappedSplitOptionalConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>.Create<T>(Func<IArgumentPatternFactory, IArgumentPattern<T>> patternDelegate, Func<TSemanticRecord, T, bool> semanticRecorder, Func<TSyntacticRecord, OneOf<None, ExpressionSyntax>, bool> syntacticRecorder)
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

    IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord> IDetachedMappedSplitOptionalConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>.Create<T>(Func<IArgumentPatternFactory, IArgumentPattern<T>> patternDelegate, Action<TSemanticRecord, T> semanticRecorder, Action<TSyntacticRecord, OneOf<None, ExpressionSyntax>> syntacticRecorder)
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

    private static IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord> Create(IDetachedMappedSemanticConstructorArgumentRecorder<TSemanticRecord> semantic, IDetachedMappedSyntacticConstructorArgumentRecorder<TSyntacticRecord> syntactic) => new DetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>(semantic, syntactic);
}
