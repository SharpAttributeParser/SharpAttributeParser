namespace SharpAttributeParser.Mappers.ASyntacticMapperCases;

using Moq;

using SharpAttributeParser.Mappers.Logging;
using SharpAttributeParser.Mappers.Repositories;
using SharpAttributeParser.Mappers.Repositories.Syntactic;

using System;

internal sealed class MapperContext<TRecord>
{
    public static MapperContext<TRecord> Create()
    {
        var parameterComparer = Mock.Of<IParameterComparer>();

        Mock<IMappedSyntacticArgumentRecorderFactory> recorderFactoryMock = new();
        Mock<ISyntacticMappingRepositoryFactory<TRecord>> repositoryFactoryMock = new() { DefaultValue = DefaultValue.Mock };

        Mock<ISyntacticMapperLogger<ASyntacticMapper<TRecord>>> loggerMock = new() { DefaultValue = DefaultValue.Mock };

        Mock<Action<IAppendableSyntacticMappingRepository<TRecord>>> addMappingsDelegateMock = new();

        SyntacticMapperDependencyProvider<TRecord> dependencyProvider = new(parameterComparer, recorderFactoryMock.Object, repositoryFactoryMock.Object, loggerMock.Object);

        SyntacticMapper mapper = new(dependencyProvider, addMappingsDelegateMock.Object);

        return new(mapper, parameterComparer, recorderFactoryMock, repositoryFactoryMock, loggerMock, addMappingsDelegateMock);
    }

    public SyntacticMapper Mapper { get; }

    public IParameterComparer ParameterComparer { get; }
    public Mock<IMappedSyntacticArgumentRecorderFactory> RecorderFactoryMock { get; }
    public Mock<ISyntacticMappingRepositoryFactory<TRecord>> RepositoryFactoryMock { get; }
    public Mock<ISyntacticMapperLogger<ASyntacticMapper<TRecord>>> LoggerMock { get; }

    public Mock<Action<IAppendableSyntacticMappingRepository<TRecord>>> AddMappingsDelegateMock { get; }

    private MapperContext(SyntacticMapper mapper, IParameterComparer parameterComparer, Mock<IMappedSyntacticArgumentRecorderFactory> recorderFactoryMock, Mock<ISyntacticMappingRepositoryFactory<TRecord>> repositoryFactoryMock, Mock<ISyntacticMapperLogger<ASyntacticMapper<TRecord>>> loggerMock, Mock<Action<IAppendableSyntacticMappingRepository<TRecord>>> addMappingsDelegateMock)
    {
        Mapper = mapper;

        ParameterComparer = parameterComparer;
        RecorderFactoryMock = recorderFactoryMock;
        RepositoryFactoryMock = repositoryFactoryMock;
        LoggerMock = loggerMock;

        AddMappingsDelegateMock = addMappingsDelegateMock;
    }

    public sealed class SyntacticMapper : ASyntacticMapper<TRecord>
    {
        private Action<IAppendableSyntacticMappingRepository<TRecord>> AddMappingsDelegate { get; }

        public SyntacticMapper(ISyntacticMapperDependencyProvider<TRecord> dependencyProvider, Action<IAppendableSyntacticMappingRepository<TRecord>> addMappingsDelegate) : base(dependencyProvider)
        {
            AddMappingsDelegate = addMappingsDelegate;
        }

        public void Initialize() => InitializeMapper();

        protected override void AddMappings(IAppendableSyntacticMappingRepository<TRecord> repository) => AddMappingsDelegate(repository);
    }
}
