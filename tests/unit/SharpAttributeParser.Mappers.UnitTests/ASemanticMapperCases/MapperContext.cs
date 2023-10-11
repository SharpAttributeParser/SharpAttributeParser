namespace SharpAttributeParser.Mappers.ASemanticMapperCases;

using Moq;

using SharpAttributeParser.Mappers.Logging;
using SharpAttributeParser.Mappers.Repositories;
using SharpAttributeParser.Mappers.Repositories.Semantic;

using System;

internal sealed class MapperContext<TRecord>
{
    public static MapperContext<TRecord> Create()
    {
        var parameterComparer = Mock.Of<IParameterComparer>();

        Mock<IMappedSemanticArgumentRecorderFactory> recorderFactoryMock = new();
        Mock<ISemanticMappingRepositoryFactory<TRecord>> repositoryFactoryMock = new() { DefaultValue = DefaultValue.Mock };

        Mock<ISemanticMapperLogger<ASemanticMapper<TRecord>>> loggerMock = new() { DefaultValue = DefaultValue.Mock };

        Mock<Action<IAppendableSemanticMappingRepository<TRecord>>> addMappingsDelegateMock = new();

        SemanticMapperDependencyProvider<TRecord> dependencyProvider = new(parameterComparer, recorderFactoryMock.Object, repositoryFactoryMock.Object, loggerMock.Object);

        SemanticMapper mapper = new(dependencyProvider, addMappingsDelegateMock.Object);

        return new(mapper, parameterComparer, recorderFactoryMock, repositoryFactoryMock, loggerMock, addMappingsDelegateMock);
    }

    public SemanticMapper Mapper { get; }

    public IParameterComparer ParameterComparer { get; }

    public Mock<IMappedSemanticArgumentRecorderFactory> RecorderFactoryMock { get; }
    public Mock<ISemanticMappingRepositoryFactory<TRecord>> RepositoryFactoryMock { get; }

    public Mock<ISemanticMapperLogger<ASemanticMapper<TRecord>>> LoggerMock { get; }

    public Mock<Action<IAppendableSemanticMappingRepository<TRecord>>> AddMappingsDelegateMock { get; }

    private MapperContext(SemanticMapper mapper, IParameterComparer parameterComparer, Mock<IMappedSemanticArgumentRecorderFactory> recorderFactoryMock, Mock<ISemanticMappingRepositoryFactory<TRecord>> repositoryFactoryMock, Mock<ISemanticMapperLogger<ASemanticMapper<TRecord>>> loggerMock, Mock<Action<IAppendableSemanticMappingRepository<TRecord>>> addMappingsDelegateMock)
    {
        Mapper = mapper;

        ParameterComparer = parameterComparer;

        RecorderFactoryMock = recorderFactoryMock;
        RepositoryFactoryMock = repositoryFactoryMock;

        LoggerMock = loggerMock;

        AddMappingsDelegateMock = addMappingsDelegateMock;
    }

    public sealed class SemanticMapper : ASemanticMapper<TRecord>
    {
        private readonly Action<IAppendableSemanticMappingRepository<TRecord>> AddMappingsDelegate;

        public SemanticMapper(ISemanticMapperDependencyProvider<TRecord> dependencyProvider, Action<IAppendableSemanticMappingRepository<TRecord>> addMappingsDelegate) : base(dependencyProvider)
        {
            AddMappingsDelegate = addMappingsDelegate;
        }

        public void Initialize() => InitializeMapper();

        protected override void AddMappings(IAppendableSemanticMappingRepository<TRecord> repository) => AddMappingsDelegate(repository);
    }
}
