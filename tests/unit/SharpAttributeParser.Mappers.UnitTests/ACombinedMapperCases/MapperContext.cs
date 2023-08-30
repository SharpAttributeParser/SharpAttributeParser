namespace SharpAttributeParser.Mappers.ACombinedMapperCases;

using Moq;

using SharpAttributeParser.Mappers.Logging;
using SharpAttributeParser.Mappers.Repositories;
using SharpAttributeParser.Mappers.Repositories.Combined;

using System;

internal sealed class MapperContext<TRecord>
{
    public static MapperContext<TRecord> Create()
    {
        var parameterComparer = Mock.Of<IParameterComparer>();

        Mock<IMappedCombinedArgumentRecorderFactory> recorderFactoryMock = new();
        Mock<ICombinedMappingRepositoryFactory<TRecord>> repositoryFactoryMock = new() { DefaultValue = DefaultValue.Mock };

        Mock<ICombinedMapperLogger<ACombinedMapper<TRecord>>> loggerMock = new() { DefaultValue = DefaultValue.Mock };

        Mock<Action<IAppendableCombinedMappingRepository<TRecord>>> addMappingsDelegateMock = new();

        CombinedMapperDependencyProvider<TRecord> dependencyProvider = new(parameterComparer, recorderFactoryMock.Object, repositoryFactoryMock.Object, loggerMock.Object);

        CombinedMapper mapper = new(dependencyProvider, addMappingsDelegateMock.Object);

        return new(mapper, parameterComparer, recorderFactoryMock, repositoryFactoryMock, loggerMock, addMappingsDelegateMock);
    }

    public CombinedMapper Mapper { get; }

    public IParameterComparer ParameterComparer { get; }

    public Mock<IMappedCombinedArgumentRecorderFactory> RecorderFactoryMock { get; }
    public Mock<ICombinedMappingRepositoryFactory<TRecord>> RepositoryFactoryMock { get; }

    public Mock<ICombinedMapperLogger<ACombinedMapper<TRecord>>> LoggerMock { get; }

    public Mock<Action<IAppendableCombinedMappingRepository<TRecord>>> AddMappingsDelegateMock { get; }

    private MapperContext(CombinedMapper mapper, IParameterComparer parameterComparer, Mock<IMappedCombinedArgumentRecorderFactory> recorderFactoryMock, Mock<ICombinedMappingRepositoryFactory<TRecord>> repositoryFactoryMock, Mock<ICombinedMapperLogger<ACombinedMapper<TRecord>>> loggerMock, Mock<Action<IAppendableCombinedMappingRepository<TRecord>>> addMappingsDelegateMock)
    {
        Mapper = mapper;

        ParameterComparer = parameterComparer;

        RecorderFactoryMock = recorderFactoryMock;
        RepositoryFactoryMock = repositoryFactoryMock;

        LoggerMock = loggerMock;

        AddMappingsDelegateMock = addMappingsDelegateMock;
    }

    public sealed class CombinedMapper : ACombinedMapper<TRecord>
    {
        private Action<IAppendableCombinedMappingRepository<TRecord>> AddMappingsDelegate { get; }

        public CombinedMapper(ICombinedMapperDependencyProvider<TRecord> dependencyProvider, Action<IAppendableCombinedMappingRepository<TRecord>> addMappingsDelegate) : base(dependencyProvider)
        {
            AddMappingsDelegate = addMappingsDelegate;
        }

        public void Initialize() => InitializeMapper();

        protected override void AddMappings(IAppendableCombinedMappingRepository<TRecord> repository) => AddMappingsDelegate(repository);
    }
}
