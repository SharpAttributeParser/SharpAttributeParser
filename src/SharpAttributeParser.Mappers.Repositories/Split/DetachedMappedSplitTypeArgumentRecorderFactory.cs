namespace SharpAttributeParser.Mappers.Repositories.Split;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Mappers.Repositories.Syntactic;

using System;

/// <inheritdoc cref="IDetachedMappedSplitTypeArgumentRecorderProviderFactory{TSemanticRecord, TSyntacticRecord}"/>
public sealed class DetachedMappedSplitTypeArgumentRecorderFactory<TSemanticRecord, TSyntacticRecord> : IDetachedMappedSplitTypeArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>
{
    private readonly IDetachedMappedSemanticTypeArgumentRecorderFactory<TSemanticRecord> SemanticFactory;
    private readonly IDetachedMappedSyntacticTypeArgumentRecorderFactory<TSyntacticRecord> SyntacticFactory;

    /// <summary>Instantiates a <see cref="DetachedMappedSplitTypeArgumentRecorderFactory{TSemanticRecord, TSyntacticRecord}"/>, handling creation of <see cref="IDetachedMappedSplitTypeArgumentRecorderProvider{TSemanticRecord, TSyntacticRecord}"/> related to type parameters.</summary>
    /// <param name="semanticFactory">Handles creation of the recorders used when arguments are parsed.</param>
    /// <param name="syntacticFactory">Handles creation of the recorders used when syntactic information about arguments is extracted.</param>
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

    private static IDetachedMappedSplitTypeArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord> Create(IDetachedMappedSemanticTypeArgumentRecorder<TSemanticRecord> semantic, IDetachedMappedSyntacticTypeArgumentRecorder<TSyntacticRecord> syntactic) => new Provider(semantic, syntactic);

    private sealed class Provider : IDetachedMappedSplitTypeArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>
    {
        private readonly IDetachedMappedSemanticTypeArgumentRecorder<TSemanticRecord> Semantic;
        private readonly IDetachedMappedSyntacticTypeArgumentRecorder<TSyntacticRecord> Syntactic;

        public Provider(IDetachedMappedSemanticTypeArgumentRecorder<TSemanticRecord> semantic, IDetachedMappedSyntacticTypeArgumentRecorder<TSyntacticRecord> syntactic)
        {
            Semantic = semantic;
            Syntactic = syntactic;
        }

        IDetachedMappedSemanticTypeArgumentRecorder<TSemanticRecord> IDetachedMappedSplitTypeArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>.Semantic => Semantic;
        IDetachedMappedSyntacticTypeArgumentRecorder<TSyntacticRecord> IDetachedMappedSplitTypeArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>.Syntactic => Syntactic;
    }
}
