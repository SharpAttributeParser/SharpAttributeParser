namespace SharpAttributeParser.Mappers.AAdaptiveMapperCases;

using Moq;

using SharpAttributeParser.Mappers.Logging;
using SharpAttributeParser.Mappers.Repositories.Adaptive;
using SharpAttributeParser.Mappers.Repositories.Combined;
using SharpAttributeParser.Mappers.Repositories.Semantic;

using System;
using System.Collections.Generic;

internal sealed class MapperContext<TCombinedRecord, TSemanticRecord>
{
    public static MapperContext<TCombinedRecord, TSemanticRecord> Create()
    {
        var parameterNameComparer = Mock.Of<IEqualityComparer<string>>();
        Mock<IMappedCombinedArgumentRecorderFactory> combinedRecorderFactoryMock = new();
        Mock<IMappedSemanticArgumentRecorderFactory> semanticRecorderFactoryMock = new();
        Mock<IAdaptiveMappingRepositoryFactory<TCombinedRecord, TSemanticRecord>> repositoryFactoryMock = new() { DefaultValue = DefaultValue.Mock };
        Mock<ICombinedMapperLogger<AAdaptiveMapper<TCombinedRecord, TSemanticRecord>>> combinedLoggerMock = new() { DefaultValue = DefaultValue.Mock };
        Mock<ISemanticMapperLogger<AAdaptiveMapper<TCombinedRecord, TSemanticRecord>>> semanticLoggerMock = new() { DefaultValue = DefaultValue.Mock };

        Mock<Action<IAppendableAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord>>> addMappingsDelegateMock = new();

        AdaptiveMapper mapper = new(parameterNameComparer, combinedRecorderFactoryMock.Object, semanticRecorderFactoryMock.Object, repositoryFactoryMock.Object, combinedLoggerMock.Object, semanticLoggerMock.Object, addMappingsDelegateMock.Object);

        return new(mapper, parameterNameComparer, combinedRecorderFactoryMock, semanticRecorderFactoryMock, repositoryFactoryMock, combinedLoggerMock, semanticLoggerMock, addMappingsDelegateMock);
    }

    public AdaptiveMapper Mapper { get; }

    public IEqualityComparer<string> ParameterNameComparer { get; }
    public Mock<IMappedCombinedArgumentRecorderFactory> CombinedRecorderFactoryMock { get; }
    public Mock<IMappedSemanticArgumentRecorderFactory> SemanticRecorderFactoryMock { get; }
    public Mock<IAdaptiveMappingRepositoryFactory<TCombinedRecord, TSemanticRecord>> RepositoryFactoryMock { get; }
    public Mock<ICombinedMapperLogger<AAdaptiveMapper<TCombinedRecord, TSemanticRecord>>> CombinedLoggerMock { get; }
    public Mock<ISemanticMapperLogger<AAdaptiveMapper<TCombinedRecord, TSemanticRecord>>> SemanticLoggerMock { get; }

    public Mock<Action<IAppendableAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord>>> AddMappingsDelegateMock { get; }

    private MapperContext(AdaptiveMapper mapper, IEqualityComparer<string> parameterNameComparer, Mock<IMappedCombinedArgumentRecorderFactory> combinedRecorderFactoryMock, Mock<IMappedSemanticArgumentRecorderFactory> semanticRecorderFactoryMock, Mock<IAdaptiveMappingRepositoryFactory<TCombinedRecord, TSemanticRecord>> repositoryFactoryMock, Mock<ICombinedMapperLogger<AAdaptiveMapper<TCombinedRecord, TSemanticRecord>>> combinedLoggerMock, Mock<ISemanticMapperLogger<AAdaptiveMapper<TCombinedRecord, TSemanticRecord>>> semanticLoggerMock, Mock<Action<IAppendableAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord>>> addMappingsDelegateMock)
    {
        Mapper = mapper;

        ParameterNameComparer = parameterNameComparer;
        CombinedRecorderFactoryMock = combinedRecorderFactoryMock;
        SemanticRecorderFactoryMock = semanticRecorderFactoryMock;
        RepositoryFactoryMock = repositoryFactoryMock;
        CombinedLoggerMock = combinedLoggerMock;
        SemanticLoggerMock = semanticLoggerMock;

        AddMappingsDelegateMock = addMappingsDelegateMock;
    }

    public sealed class AdaptiveMapper : AAdaptiveMapper<TCombinedRecord, TSemanticRecord>
    {
        private Action<IAppendableAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord>> AddMappingsDelegate { get; }

        public AdaptiveMapper(IEqualityComparer<string> parameterNameComparer, IMappedCombinedArgumentRecorderFactory combinedRecorderFactory, IMappedSemanticArgumentRecorderFactory semanticRecorderFactory, IAdaptiveMappingRepositoryFactory<TCombinedRecord, TSemanticRecord> repositoryFactory, ICombinedMapperLogger<AAdaptiveMapper<TCombinedRecord, TSemanticRecord>> combinedLogger, ISemanticMapperLogger<AAdaptiveMapper<TCombinedRecord, TSemanticRecord>> semanticLogger, Action<IAppendableAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord>> addMappingsDelegate) : base(parameterNameComparer, combinedRecorderFactory, semanticRecorderFactory, repositoryFactory, combinedLogger, semanticLogger)
        {
            AddMappingsDelegate = addMappingsDelegate;
        }

        public void Initialize() => InitializeMapper();

        protected override void AddMappings(IAppendableAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord> repository) => AddMappingsDelegate(repository);
    }
}
