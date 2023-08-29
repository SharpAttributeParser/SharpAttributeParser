namespace SharpAttributeParser.Mappers.Repositories.Split;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Mappers.Repositories.Syntactic;

using System;

/// <inheritdoc cref="IDetachedMappedSplitTypeArgumentRecorderProviderFactory{TSemanticRecord, TSyntacticRecord}"/>
public sealed class DetachedMappedSplitTypeArgumentRecorderFactory<TSemanticRecord, TSyntacticRecord> : IDetachedMappedSplitTypeArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>
{
    private IDetachedMappedSemanticTypeArgumentRecorderFactory<TSemanticRecord> SemanticFactory { get; }
    private IDetachedMappedSyntacticTypeArgumentRecorderFactory<TSyntacticRecord> SyntacticFactory { get; }

    /// <summary>Instantiates a <see cref="DetachedMappedSplitTypeArgumentRecorderFactory{TSemanticRecord, TSyntacticRecord}"/>, handling creation of <see cref="IDetachedMappedSplitTypeArgumentRecorderProvider{TSemanticRecord, TSyntacticRecord}"/> related to type parameters.</summary>
    /// <param name="semanticFactory">Handles creation of the recorders used when arguments are parsed.</param>
    /// <param name="syntacticFactory">Handles creation of the recorders used when syntactic information about arguments is extracted.</param>
    /// <exception cref="ArgumentNullException"/>
    public DetachedMappedSplitTypeArgumentRecorderFactory(IDetachedMappedSemanticTypeArgumentRecorderFactory<TSemanticRecord> semanticFactory, IDetachedMappedSyntacticTypeArgumentRecorderFactory<TSyntacticRecord> syntacticFactory)
    {
        SemanticFactory = semanticFactory ?? throw new ArgumentNullException(nameof(semanticFactory));
        SyntacticFactory = syntacticFactory ?? throw new ArgumentNullException(nameof(syntacticFactory));
    }

    IDetachedMappedSplitTypeArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord> IDetachedMappedSplitTypeArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>.Create(Func<TSemanticRecord, ITypeSymbol, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder)
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

    IDetachedMappedSplitTypeArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord> IDetachedMappedSplitTypeArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>.Create(Action<TSemanticRecord, ITypeSymbol> semanticRecorder, Action<TSyntacticRecord, ExpressionSyntax> syntacticRecorder)
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

    private static IDetachedMappedSplitTypeArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord> Create(IDetachedMappedSemanticTypeArgumentRecorder<TSemanticRecord> semantic, IDetachedMappedSyntacticTypeArgumentRecorder<TSyntacticRecord> syntactic)
    {
        if (semantic is null)
        {
            throw new InvalidOperationException($"A {nameof(IDetachedMappedSemanticTypeArgumentRecorderFactory<object>)} produced a null {nameof(IDetachedMappedSemanticTypeArgumentRecorder<object>)}.");
        }

        if (syntactic is null)
        {
            throw new InvalidOperationException($"A {nameof(IDetachedMappedSyntacticTypeArgumentRecorderFactory<object>)} produced a null {nameof(IDetachedMappedSyntacticTypeArgumentRecorder<object>)}.");
        }

        return new Provider(semantic, syntactic);
    }

    private sealed class Provider : IDetachedMappedSplitTypeArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>
    {
        private IDetachedMappedSemanticTypeArgumentRecorder<TSemanticRecord> Semantic { get; }
        private IDetachedMappedSyntacticTypeArgumentRecorder<TSyntacticRecord> Syntactic { get; }

        public Provider(IDetachedMappedSemanticTypeArgumentRecorder<TSemanticRecord> semantic, IDetachedMappedSyntacticTypeArgumentRecorder<TSyntacticRecord> syntactic)
        {
            Semantic = semantic;
            Syntactic = syntactic;
        }

        IDetachedMappedSemanticTypeArgumentRecorder<TSemanticRecord> IDetachedMappedSplitTypeArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>.Semantic => Semantic;
        IDetachedMappedSyntacticTypeArgumentRecorder<TSyntacticRecord> IDetachedMappedSplitTypeArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>.Syntactic => Syntactic;
    }
}
