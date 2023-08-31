namespace SharpAttributeParser.Mappers.Repositories.Syntactic;

using System;

/// <inheritdoc cref="IDetachedMappedSyntacticConstructorArgumentRecorderFactory{TRecord}"/>
public sealed class DetachedMappedSyntacticConstructorArgumentRecorderFactory<TRecord> : IDetachedMappedSyntacticConstructorArgumentRecorderFactory<TRecord>
{
    private IDetachedMappedSyntacticNormalConstructorArgumentRecorderFactory<TRecord> Normal { get; }
    private IDetachedMappedSyntacticParamsConstructorArgumentRecorderFactory<TRecord> Params { get; }
    private IDetachedMappedSyntacticOptionalConstructorArgumentRecorderFactory<TRecord> Optional { get; }

    /// <summary>Instantiates a <see cref="DetachedMappedSyntacticConstructorArgumentRecorderFactory{TRecord}"/>, handling creation of <see cref="IDetachedMappedSyntacticConstructorArgumentRecorder{TRecord}"/>.</summary>
    /// <param name="normalFactory">Handles creation of recorders related to non-optional, non-<see langword="params"/> constructor parameters.</param>
    /// <param name="paramsFactory">Handles creation of recorders related to <see langword="params"/> constructor parameters.</param>
    /// <param name="optionalFactory">Handles creation of recorders related to optional constructor parameters.</param>
    public DetachedMappedSyntacticConstructorArgumentRecorderFactory(IDetachedMappedSyntacticNormalConstructorArgumentRecorderFactory<TRecord> normalFactory, IDetachedMappedSyntacticParamsConstructorArgumentRecorderFactory<TRecord> paramsFactory, IDetachedMappedSyntacticOptionalConstructorArgumentRecorderFactory<TRecord> optionalFactory)
    {
        Normal = normalFactory ?? throw new ArgumentNullException(nameof(normalFactory));
        Params = paramsFactory ?? throw new ArgumentNullException(nameof(paramsFactory));
        Optional = optionalFactory ?? throw new ArgumentNullException(nameof(optionalFactory));
    }

    IDetachedMappedSyntacticNormalConstructorArgumentRecorderFactory<TRecord> IDetachedMappedSyntacticConstructorArgumentRecorderFactory<TRecord>.Normal => Normal;
    IDetachedMappedSyntacticParamsConstructorArgumentRecorderFactory<TRecord> IDetachedMappedSyntacticConstructorArgumentRecorderFactory<TRecord>.Params => Params;
    IDetachedMappedSyntacticOptionalConstructorArgumentRecorderFactory<TRecord> IDetachedMappedSyntacticConstructorArgumentRecorderFactory<TRecord>.Optional => Optional;
}
