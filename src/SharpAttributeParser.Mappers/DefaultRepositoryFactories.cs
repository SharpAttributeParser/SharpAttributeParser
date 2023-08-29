namespace SharpAttributeParser.Mappers;

using SharpAttributeParser.Mappers.Repositories;
using SharpAttributeParser.Mappers.Repositories.Adaptive;
using SharpAttributeParser.Mappers.Repositories.Combined;
using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Mappers.Repositories.Split;
using SharpAttributeParser.Mappers.Repositories.Syntactic;
using SharpAttributeParser.Patterns;

/// <summary>Provides common functionality related to creating default repositories.</summary>
internal static class DefaultRepositoryFactories
{
    /// <summary>Creates the default <see cref="ICombinedMappingRepositoryFactory{TRecord}"/>.</summary>
    /// <typeparam name="TRecord">The type to which arguments are recorded by the recorders of the repositories.</typeparam>
    /// <returns>The default <see cref="ICombinedMappingRepositoryFactory{TRecord}"/>.</returns>
    public static ICombinedMappingRepositoryFactory<TRecord> CombinedFactory<TRecord>()
    {
        DetachedMappedCombinedNormalConstructorArgumentRecorderFactory<TRecord> normalConstructorArgumentRecorderFactory = new(ArgumentPatternFactory.Singleton);
        DetachedMappedCombinedParamsConstructorArgumentRecorderFactory<TRecord> paramsConstructorArgumentRecorderFactory = new(ArgumentPatternFactory.Singleton);
        DetachedMappedCombinedOptionalConstructorArgumentRecorderFactory<TRecord> optionalConstructorArgumentRecorderFactory = new(ArgumentPatternFactory.Singleton);

        TypeMappingRepositoryFactory<IDetachedMappedCombinedTypeArgumentRecorder<TRecord>, IDetachedMappedCombinedTypeArgumentRecorderFactory<TRecord>> typeMappingRepositoryFactory = new(new DetachedMappedCombinedTypeArgumentRecorderFactory<TRecord>());
        ConstructorMappingRepositoryFactory<IDetachedMappedCombinedConstructorArgumentRecorder<TRecord>, IDetachedMappedCombinedConstructorArgumentRecorderFactory<TRecord>> constructorMappingRepositoryFactory = new(new DetachedMappedCombinedConstructorArgumentRecorderFactory<TRecord>(normalConstructorArgumentRecorderFactory, paramsConstructorArgumentRecorderFactory, optionalConstructorArgumentRecorderFactory));
        NamedMappingRepositoryFactory<IDetachedMappedCombinedNamedArgumentRecorder<TRecord>, IDetachedMappedCombinedNamedArgumentRecorderFactory<TRecord>> namedMappingRepositoryFactory = new(new DetachedMappedCombinedNamedArgumentRecorderFactory<TRecord>(ArgumentPatternFactory.Singleton));

        return new CombinedMappingRepositoryFactory<TRecord>(typeMappingRepositoryFactory, constructorMappingRepositoryFactory, namedMappingRepositoryFactory);
    }

    /// <summary>Creates the default <see cref="ISemanticMappingRepositoryFactory{TRecord}"/>.</summary>
    /// <typeparam name="TRecord">The type to which arguments are recorded by the recorders of the repositories.</typeparam>
    /// <returns>The default <see cref="ISemanticMappingRepositoryFactory{TRecord}"/>.</returns>
    public static ISemanticMappingRepositoryFactory<TRecord> SemanticFactory<TRecord>()
    {
        TypeMappingRepositoryFactory<IDetachedMappedSemanticTypeArgumentRecorder<TRecord>, IDetachedMappedSemanticTypeArgumentRecorderFactory<TRecord>> typeMappingRepositoryFactory = new(new DetachedMappedSemanticTypeArgumentRecorderFactory<TRecord>());
        ConstructorMappingRepositoryFactory<IDetachedMappedSemanticConstructorArgumentRecorder<TRecord>, IDetachedMappedSemanticConstructorArgumentRecorderFactory<TRecord>> constructorMappingRepositoryFactory = new(new DetachedMappedSemanticConstructorArgumentRecorderFactory<TRecord>(ArgumentPatternFactory.Singleton));
        NamedMappingRepositoryFactory<IDetachedMappedSemanticNamedArgumentRecorder<TRecord>, IDetachedMappedSemanticNamedArgumentRecorderFactory<TRecord>> namedMappingRepositoryFactory = new(new DetachedMappedSemanticNamedArgumentRecorderFactory<TRecord>(ArgumentPatternFactory.Singleton));

        return new SemanticMappingRepositoryFactory<TRecord>(typeMappingRepositoryFactory, constructorMappingRepositoryFactory, namedMappingRepositoryFactory);
    }

    /// <summary>Creates the default <see cref="ISyntacticMappingRepositoryFactory{TRecord}"/>.</summary>
    /// <typeparam name="TRecord">The type to which syntactic information is recorded by the recorders of the repositories.</typeparam>
    /// <returns>The default <see cref="ISyntacticMappingRepositoryFactory{TRecord}"/>.</returns>
    public static ISyntacticMappingRepositoryFactory<TRecord> SyntacticFactory<TRecord>()
    {
        DetachedMappedSyntacticNormalConstructorArgumentRecorderFactory<TRecord> normalConstructorArgumentRecorderFactory = new();
        DetachedMappedSyntacticParamsConstructorArgumentRecorderFactory<TRecord> paramsConstructorArgumentRecorderFactory = new();
        DetachedMappedSyntacticOptionalConstructorArgumentRecorderFactory<TRecord> optionalConstructorArgumentRecorderFactory = new();

        TypeMappingRepositoryFactory<IDetachedMappedSyntacticTypeArgumentRecorder<TRecord>, IDetachedMappedSyntacticTypeArgumentRecorderFactory<TRecord>> typeMappingRepositoryFactory = new(new DetachedMappedSyntacticTypeArgumentRecorderFactory<TRecord>());
        ConstructorMappingRepositoryFactory<IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord>, IDetachedMappedSyntacticConstructorArgumentRecorderFactory<TRecord>> constructorMappingRepositoryFactory = new(new DetachedMappedSyntacticConstructorArgumentRecorderFactory<TRecord>(normalConstructorArgumentRecorderFactory, paramsConstructorArgumentRecorderFactory, optionalConstructorArgumentRecorderFactory));
        NamedMappingRepositoryFactory<IDetachedMappedSyntacticNamedArgumentRecorder<TRecord>, IDetachedMappedSyntacticNamedArgumentRecorderFactory<TRecord>> namedMappingRepositoryFactory = new(new DetachedMappedSyntacticNamedArgumentRecorderFactory<TRecord>());

        return new SyntacticMappingRepositoryFactory<TRecord>(typeMappingRepositoryFactory, constructorMappingRepositoryFactory, namedMappingRepositoryFactory);
    }

    /// <summary>Creates the default <see cref="IAdaptiveMappingRepositoryFactory{TCombinedRecord, TSemanticRecord}"/>.</summary>
    /// <typeparam name="TCombinedRecord">The type to which arguments are recorded by the recorders of the repositories, when attributes are parsed with syntactic context.</typeparam>
    /// <typeparam name="TSemanticRecord">The type to which arguments are recorded by the recorders of the repositories, when attributes are parsed without syntactic context.</typeparam>
    /// <returns>The default <see cref="IAdaptiveMappingRepositoryFactory{TCombinedRecord, TSemanticRecord}"/>.</returns>
    public static IAdaptiveMappingRepositoryFactory<TCombinedRecord, TSemanticRecord> AdaptiveFactory<TCombinedRecord, TSemanticRecord>()
    {
        DetachedMappedCombinedTypeArgumentRecorderFactory<TCombinedRecord> combinedTypeArgumentRecorderFactory = new();
        DetachedMappedSemanticTypeArgumentRecorderFactory<TSemanticRecord> semanticTypeArgumentRecorderFactory = new();

        DetachedMappedCombinedNormalConstructorArgumentRecorderFactory<TCombinedRecord> combinedNormalConstructorArgumentRecorderFactory = new(ArgumentPatternFactory.Singleton);
        DetachedMappedCombinedParamsConstructorArgumentRecorderFactory<TCombinedRecord> combinedParamsConstructorArgumentRecorderFactory = new(ArgumentPatternFactory.Singleton);
        DetachedMappedCombinedOptionalConstructorArgumentRecorderFactory<TCombinedRecord> combinedOptionalConstructorArgumentRecorderFactory = new(ArgumentPatternFactory.Singleton);

        DetachedMappedSemanticConstructorArgumentRecorderFactory<TSemanticRecord> semanticConstructorArgumentRecorderFactory = new(ArgumentPatternFactory.Singleton);

        DetachedMappedCombinedNamedArgumentRecorderFactory<TCombinedRecord> combinedNamedArgumentRecorderFactory = new(ArgumentPatternFactory.Singleton);
        DetachedMappedSemanticNamedArgumentRecorderFactory<TSemanticRecord> semanticNamedArgumentRecorderFactory = new(ArgumentPatternFactory.Singleton);

        DetachedMappedAdaptiveNormalConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> normalConstructorArgumentRecorderFactory = new(combinedNormalConstructorArgumentRecorderFactory, semanticConstructorArgumentRecorderFactory);
        DetachedMappedAdaptiveParamsConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> paramsConstructorArgumentRecorderFactory = new(combinedParamsConstructorArgumentRecorderFactory, semanticConstructorArgumentRecorderFactory);
        DetachedMappedAdaptiveOptionalConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord> optionalConstructorArgumentRecorderFactory = new(combinedOptionalConstructorArgumentRecorderFactory, semanticConstructorArgumentRecorderFactory);

        TypeMappingRepositoryFactory<IDetachedMappedAdaptiveTypeArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveTypeArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>> typeMappingRepositoryFactory = new(new DetachedMappedAdaptiveTypeArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>(combinedTypeArgumentRecorderFactory, semanticTypeArgumentRecorderFactory));
        ConstructorMappingRepositoryFactory<IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>> constructorMappingRepositoryFactory = new(new DetachedMappedAdaptiveConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>(normalConstructorArgumentRecorderFactory, paramsConstructorArgumentRecorderFactory, optionalConstructorArgumentRecorderFactory));
        NamedMappingRepositoryFactory<IDetachedMappedAdaptiveNamedArgumentRecorderProvider<TCombinedRecord, TSemanticRecord>, IDetachedMappedAdaptiveNamedArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>> namedMappingRepositoryFactory = new(new DetachedMappedAdaptiveNamedArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>(combinedNamedArgumentRecorderFactory, semanticNamedArgumentRecorderFactory));

        return new AdaptiveMappingRepositoryFactory<TCombinedRecord, TSemanticRecord>(typeMappingRepositoryFactory, constructorMappingRepositoryFactory, namedMappingRepositoryFactory);
    }

    /// <summary>Creates the default <see cref="ISplitMappingRepositoryFactory{TSemanticRecord, TSyntacticRecord}"/>.</summary>
    /// <typeparam name="TSemanticRecord">The type to which arguments are recorded by the recorders of the repositories.</typeparam>
    /// <typeparam name="TSyntacticRecord">The type to which syntactic information about arguments is recorded by the recorders of the repositories.</typeparam>
    /// <returns>The default <see cref="ISplitMappingRepositoryFactory{TSemanticRecord, TSyntacticRecord}"/>.</returns>
    public static ISplitMappingRepositoryFactory<TSemanticRecord, TSyntacticRecord> SplitFactory<TSemanticRecord, TSyntacticRecord>()
    {
        DetachedMappedSemanticTypeArgumentRecorderFactory<TSemanticRecord> semanticTypeArgumentRecorderFactory = new();
        DetachedMappedSyntacticTypeArgumentRecorderFactory<TSyntacticRecord> syntacticTypeArgumentRecorderFactory = new();

        DetachedMappedSemanticConstructorArgumentRecorderFactory<TSemanticRecord> semanticConstructorArgumentRecorderFactory = new(ArgumentPatternFactory.Singleton);

        DetachedMappedSyntacticNormalConstructorArgumentRecorderFactory<TSyntacticRecord> syntacticNormalConstructorArgumentRecorderFactory = new();
        DetachedMappedSyntacticParamsConstructorArgumentRecorderFactory<TSyntacticRecord> syntacticParamsConstructorArgumentRecorderFactory = new();
        DetachedMappedSyntacticOptionalConstructorArgumentRecorderFactory<TSyntacticRecord> syntacticOptionalConstructorArgumentRecorderFactory = new();

        DetachedMappedSemanticNamedArgumentRecorderFactory<TSemanticRecord> semanticNamedArgumentRecorderFactory = new(ArgumentPatternFactory.Singleton);
        DetachedMappedSyntacticNamedArgumentRecorderFactory<TSyntacticRecord> syntacticNamedArgumentRecorderFactory = new();

        DetachedMappedSplitNormalConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord> normalConstructorArgumentRecorderFactory = new(semanticConstructorArgumentRecorderFactory, syntacticNormalConstructorArgumentRecorderFactory);
        DetachedMappedSplitParamsConstructorArgumentRecorderFactory<TSemanticRecord, TSyntacticRecord> paramsConstructorArgumentRecorderFactory = new(semanticConstructorArgumentRecorderFactory, syntacticParamsConstructorArgumentRecorderFactory);
        DetachedMappedSplitOptionalConstructorArgumentRecorderFactory<TSemanticRecord, TSyntacticRecord> optionalConstructorArgumentRecorderFactory = new(semanticConstructorArgumentRecorderFactory, syntacticOptionalConstructorArgumentRecorderFactory);

        TypeMappingRepositoryFactory<IDetachedMappedSplitTypeArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitTypeArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>> typeMappingRepositoryFactory = new(new DetachedMappedSplitTypeArgumentRecorderFactory<TSemanticRecord, TSyntacticRecord>(semanticTypeArgumentRecorderFactory, syntacticTypeArgumentRecorderFactory));
        ConstructorMappingRepositoryFactory<IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>> constructorMappingRepositoryFactory = new(new DetachedMappedSplitConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>(normalConstructorArgumentRecorderFactory, paramsConstructorArgumentRecorderFactory, optionalConstructorArgumentRecorderFactory));
        NamedMappingRepositoryFactory<IDetachedMappedSplitNamedArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord>, IDetachedMappedSplitNamedArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>> namedMappingRepositoryFactory = new(new DetachedMappedSplitNamedArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>(semanticNamedArgumentRecorderFactory, syntacticNamedArgumentRecorderFactory));

        return new SplitMappingRepositoryFactory<TSemanticRecord, TSyntacticRecord>(typeMappingRepositoryFactory, constructorMappingRepositoryFactory, namedMappingRepositoryFactory);
    }
}
