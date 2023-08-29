namespace SharpAttributeParser.Mappers.ASemanticMapperCases;

using Moq;

using SharpAttributeParser.Mappers.Logging;
using SharpAttributeParser.Mappers.Repositories.Semantic;

using System;
using System.Collections.Generic;

internal sealed class MapperContext<TRecord>
{
    public static MapperContext<TRecord> Create()
    {
        var parameterNameComparer = Mock.Of<IEqualityComparer<string>>();
        Mock<IMappedSemanticArgumentRecorderFactory> recorderFactoryMock = new();
        Mock<ISemanticMappingRepositoryFactory<TRecord>> repositoryFactoryMock = new() { DefaultValue = DefaultValue.Mock };
        Mock<ISemanticMapperLogger<ASemanticMapper<TRecord>>> loggerMock = new() { DefaultValue = DefaultValue.Mock };

        Mock<Action<IAppendableSemanticMappingRepository<TRecord>>> addMappingsDelegateMock = new();

        SemanticMapper mapper = new(parameterNameComparer, recorderFactoryMock.Object, repositoryFactoryMock.Object, loggerMock.Object, addMappingsDelegateMock.Object);

        return new(mapper, parameterNameComparer, recorderFactoryMock, repositoryFactoryMock, loggerMock, addMappingsDelegateMock);
    }

    public SemanticMapper Mapper { get; }

    public IEqualityComparer<string> ParameterNameComparer { get; }
    public Mock<IMappedSemanticArgumentRecorderFactory> RecorderFactoryMock { get; }
    public Mock<ISemanticMappingRepositoryFactory<TRecord>> RepositoryFactoryMock { get; }
    public Mock<ISemanticMapperLogger<ASemanticMapper<TRecord>>> LoggerMock { get; }

    public Mock<Action<IAppendableSemanticMappingRepository<TRecord>>> AddMappingsDelegateMock { get; }

    private MapperContext(SemanticMapper mapper, IEqualityComparer<string> parameterNameComparer, Mock<IMappedSemanticArgumentRecorderFactory> recorderFactoryMock, Mock<ISemanticMappingRepositoryFactory<TRecord>> repositoryFactoryMock, Mock<ISemanticMapperLogger<ASemanticMapper<TRecord>>> loggerMock, Mock<Action<IAppendableSemanticMappingRepository<TRecord>>> addMappingsDelegateMock)
    {
        Mapper = mapper;

        ParameterNameComparer = parameterNameComparer;
        RecorderFactoryMock = recorderFactoryMock;
        RepositoryFactoryMock = repositoryFactoryMock;
        LoggerMock = loggerMock;

        AddMappingsDelegateMock = addMappingsDelegateMock;
    }

    public sealed class SemanticMapper : ASemanticMapper<TRecord>
    {
        private Action<IAppendableSemanticMappingRepository<TRecord>> AddMappingsDelegate { get; }

        public SemanticMapper(IEqualityComparer<string> parameterNameComparer, IMappedSemanticArgumentRecorderFactory recorderFactory, ISemanticMappingRepositoryFactory<TRecord> repositoryFactory, ISemanticMapperLogger<ASemanticMapper<TRecord>> logger, Action<IAppendableSemanticMappingRepository<TRecord>> addMappingsDelegate) : base(parameterNameComparer, recorderFactory, repositoryFactory, logger)
        {
            AddMappingsDelegate = addMappingsDelegate;
        }

        public void Initialize() => InitializeMapper();

        protected override void AddMappings(IAppendableSemanticMappingRepository<TRecord> repository) => AddMappingsDelegate(repository);
    }
}
