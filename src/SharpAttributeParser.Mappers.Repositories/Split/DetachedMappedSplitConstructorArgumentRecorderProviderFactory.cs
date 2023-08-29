namespace SharpAttributeParser.Mappers.Repositories.Split;

using System;

/// <inheritdoc cref="IDetachedMappedSplitConstructorArgumentRecorderProviderFactory{TSyntacticRecord, TSyntacticRecord}"/>
public sealed class DetachedMappedSplitConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> : IDetachedMappedSplitConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>
{
    private IDetachedMappedSplitNormalConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> NormalFactory { get; }
    private IDetachedMappedSplitParamsConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> ParamsFactory { get; }
    private IDetachedMappedSplitOptionalConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> OptionalFactory { get; }

    /// <summary>Instantiates a <see cref="DetachedMappedSplitConstructorArgumentRecorderProviderFactory{TSemanticRecord, TSyntacticRecord}"/>, handling creation of <see cref="IDetachedMappedSplitConstructorArgumentRecorderProvider{TSemanticRecord, TSyntacticRecord}"/>.</summary>
    /// <param name="normalFactory">Handles creation of recorders related to non-optional, non-<see langword="params"/> constructor parameters.</param>
    /// <param name="paramsFactory">Handles creation of recorders related to <see langword="params"/> constructor parameters.</param>
    /// <param name="optionalFactory">Handles creation of recorders related to optional constructor parameters.</param>
    /// <exception cref="ArgumentNullException"/>
    public DetachedMappedSplitConstructorArgumentRecorderProviderFactory(IDetachedMappedSplitNormalConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> normalFactory, IDetachedMappedSplitParamsConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> paramsFactory, IDetachedMappedSplitOptionalConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> optionalFactory)
    {
        NormalFactory = normalFactory ?? throw new ArgumentNullException(nameof(normalFactory));
        ParamsFactory = paramsFactory ?? throw new ArgumentNullException(nameof(paramsFactory));
        OptionalFactory = optionalFactory ?? throw new ArgumentNullException(nameof(optionalFactory));
    }

    IDetachedMappedSplitNormalConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> IDetachedMappedSplitConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>.Normal => NormalFactory;
    IDetachedMappedSplitParamsConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> IDetachedMappedSplitConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>.Params => ParamsFactory;
    IDetachedMappedSplitOptionalConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> IDetachedMappedSplitConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>.Optional => OptionalFactory;
}
