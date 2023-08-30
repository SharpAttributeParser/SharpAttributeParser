namespace SharpAttributeParser.Mappers.AAdaptiveMapperCases;

using Moq;

using SharpAttributeParser.Mappers.Logging;
using SharpAttributeParser.Mappers.Repositories;
using SharpAttributeParser.Mappers.Repositories.Adaptive;
using SharpAttributeParser.Mappers.Repositories.Combined;
using SharpAttributeParser.Mappers.Repositories.Semantic;

using System;

internal sealed class MapperContext<TCombinedRecord, TSemanticRecord>
{
    public static MapperContext<TCombinedRecord, TSemanticRecord> Create()
    {
        var parameterComparer = Mock.Of<IParameterComparer>();

        Mock<IMappedCombinedArgumentRecorderFactory> combinedRecorderFactoryMock = new();
        Mock<IMappedSemanticArgumentRecorderFactory> semanticRecorderFactoryMock = new();
        Mock<IAdaptiveMappingRepositoryFactory<TCombinedRecord, TSemanticRecord>> repositoryFactoryMock = new() { DefaultValue = DefaultValue.Mock };

        Mock<ICombinedMapperLogger<AAdaptiveMapper<TCombinedRecord, TSemanticRecord>>> combinedLoggerMock = new() { DefaultValue = DefaultValue.Mock };
        Mock<ISemanticMapperLogger<AAdaptiveMapper<TCombinedRecord, TSemanticRecord>>> semanticLoggerMock = new() { DefaultValue = DefaultValue.Mock };

        Mock<Action<IAppendableAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord>>> addMappingsDelegateMock = new();

        AdaptiveMapperDependencyProvider<TCombinedRecord, TSemanticRecord> dependencyProvider = new(parameterComparer, combinedRecorderFactoryMock.Object, semanticRecorderFactoryMock.Object, repositoryFactoryMock.Object, combinedLoggerMock.Object, semanticLoggerMock.Object);

        AdaptiveMapper mapper = new(dependencyProvider, addMappingsDelegateMock.Object);

        return new(mapper, parameterComparer, combinedRecorderFactoryMock, semanticRecorderFactoryMock, repositoryFactoryMock, combinedLoggerMock, semanticLoggerMock, addMappingsDelegateMock);
    }

    public AdaptiveMapper Mapper { get; }

    public IParameterComparer ParameterComparer { get; }

    public Mock<IMappedCombinedArgumentRecorderFactory> CombinedRecorderFactoryMock { get; }
    public Mock<IMappedSemanticArgumentRecorderFactory> SemanticRecorderFactoryMock { get; }
    public Mock<IAdaptiveMappingRepositoryFactory<TCombinedRecord, TSemanticRecord>> RepositoryFactoryMock { get; }

    public Mock<ICombinedMapperLogger<AAdaptiveMapper<TCombinedRecord, TSemanticRecord>>> CombinedLoggerMock { get; }
    public Mock<ISemanticMapperLogger<AAdaptiveMapper<TCombinedRecord, TSemanticRecord>>> SemanticLoggerMock { get; }

    public Mock<Action<IAppendableAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord>>> AddMappingsDelegateMock { get; }

    private MapperContext(AdaptiveMapper mapper, IParameterComparer parameterComparer, Mock<IMappedCombinedArgumentRecorderFactory> combinedRecorderFactoryMock, Mock<IMappedSemanticArgumentRecorderFactory> semanticRecorderFactoryMock, Mock<IAdaptiveMappingRepositoryFactory<TCombinedRecord, TSemanticRecord>> repositoryFactoryMock, Mock<ICombinedMapperLogger<AAdaptiveMapper<TCombinedRecord, TSemanticRecord>>> combinedLoggerMock, Mock<ISemanticMapperLogger<AAdaptiveMapper<TCombinedRecord, TSemanticRecord>>> semanticLoggerMock, Mock<Action<IAppendableAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord>>> addMappingsDelegateMock)
    {
        Mapper = mapper;

        ParameterComparer = parameterComparer;

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

        public AdaptiveMapper(IAdaptiveMapperDependencyProvider<TCombinedRecord, TSemanticRecord> dependencyProvider, Action<IAppendableAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord>> addMappingsDelegate) : base(dependencyProvider)
        {
            AddMappingsDelegate = addMappingsDelegate;
        }

        public void Initialize() => InitializeMapper();

        protected override void AddMappings(IAppendableAdaptiveMappingRepository<TCombinedRecord, TSemanticRecord> repository) => AddMappingsDelegate(repository);
    }
}
