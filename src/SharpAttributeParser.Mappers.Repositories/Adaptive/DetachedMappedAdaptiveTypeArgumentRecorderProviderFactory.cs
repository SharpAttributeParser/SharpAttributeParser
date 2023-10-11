namespace SharpAttributeParser.Mappers.Repositories.Adaptive;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser.Mappers.Repositories.Combined;
using SharpAttributeParser.Mappers.Repositories.Semantic;

using System;

/// <inheritdoc cref="IDetachedMappedAdaptiveTypeArgumentRecorderProviderFactory{TCombinedRecord, TSemanticRecord}"/>
public sealed class DetachedMappedAdaptiveTypeArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> : IDetachedMappedAdaptiveTypeArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>
{
    private readonly IDetachedMappedCombinedTypeArgumentRecorderFactory<TCombinedRecord> CombinedFactory;
    private readonly IDetachedMappedSemanticTypeArgumentRecorderFactory<TSemanticRecord> SemanticFactory;

    /// <summary>Instantiates a <see cref="DetachedMappedAdaptiveTypeArgumentRecorderProviderFactory{TCombinedRecord, TSemanticRecord}"/>, handling creation of <see cref="IDetachedMappedAdaptiveTypeArgumentRecorderProvider{TCombinedRecord, TSemanticRecord}"/> related to type parameters.</summary>
    /// <param name="combinedFactory">Handles creation of the recorders used when arguments are parsed with syntactic context.</param>
    /// <param name="semanticFactory">Handles creation of the recorders used when arguments are parsed without syntactic context.</param>
    public DetachedMappedAdaptiveTypeArgumentRecorderProviderFactory(IDetachedMappedCombinedTypeArgumentRecorderFactory<TCombinedRecord> combinedFactory, IDetachedMappedSemanticTypeArgumentRecorderFactory<TSemanticRecord> semanticFactory)
    {
        CombinedFactory = combinedFactory ?? throw new ArgumentNullException(nameof(combinedFactory));
        SemanticFactory = semanticFactory ?? throw new ArgumentNullException(nameof(semanticFactory));
    }

    IDetachedMappedAdaptiveTypeArgumentRecorderProvider<TCombinedRecord, TSemanticRecord> IDetachedMappedAdaptiveTypeArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>.Create(Func<TCombinedRecord, ITypeSymbol, ExpressionSyntax, bool> combinedRecorder, Func<TSemanticRecord, ITypeSymbol, bool> semanticRecorder)
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

    IDetachedMappedAdaptiveTypeArgumentRecorderProvider<TCombinedRecord, TSemanticRecord> IDetachedMappedAdaptiveTypeArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>.Create(Action<TCombinedRecord, ITypeSymbol, ExpressionSyntax> combinedRecorder, Action<TSemanticRecord, ITypeSymbol> semanticRecorder)
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

    private static IDetachedMappedAdaptiveTypeArgumentRecorderProvider<TCombinedRecord, TSemanticRecord> Create(IDetachedMappedCombinedTypeArgumentRecorder<TCombinedRecord> combined, IDetachedMappedSemanticTypeArgumentRecorder<TSemanticRecord> semantic) => new Provider(combined, semantic);

    private sealed class Provider : IDetachedMappedAdaptiveTypeArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>
    {
        private readonly IDetachedMappedCombinedTypeArgumentRecorder<TCombinedRecord> Combined;
        private readonly IDetachedMappedSemanticTypeArgumentRecorder<TSemanticRecord> Semantic;

        public Provider(IDetachedMappedCombinedTypeArgumentRecorder<TCombinedRecord> combined, IDetachedMappedSemanticTypeArgumentRecorder<TSemanticRecord> semantic)
        {
            Combined = combined;
            Semantic = semantic;
        }

        IDetachedMappedCombinedTypeArgumentRecorder<TCombinedRecord> IDetachedMappedAdaptiveTypeArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>.Combined => Combined;
        IDetachedMappedSemanticTypeArgumentRecorder<TSemanticRecord> IDetachedMappedAdaptiveTypeArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>.Semantic => Semantic;
    }
}
