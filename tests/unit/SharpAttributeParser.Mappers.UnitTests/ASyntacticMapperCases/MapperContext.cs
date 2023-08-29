namespace SharpAttributeParser.Mappers.ASyntacticMapperCases;

using Moq;

using SharpAttributeParser.Mappers.Logging;
using SharpAttributeParser.Mappers.Repositories.Syntactic;

using System;
using System.Collections.Generic;

internal sealed class MapperContext<TRecord>
{
    public static MapperContext<TRecord> Create()
    {
        var parameterNameComparer = Mock.Of<IEqualityComparer<string>>();
        Mock<IMappedSyntacticArgumentRecorderFactory> recorderFactoryMock = new();
        Mock<ISyntacticMappingRepositoryFactory<TRecord>> repositoryFactoryMock = new() { DefaultValue = DefaultValue.Mock };
        Mock<ISyntacticMapperLogger<ASyntacticMapper<TRecord>>> loggerMock = new() { DefaultValue = DefaultValue.Mock };

        Mock<Action<IAppendableSyntacticMappingRepository<TRecord>>> addMappingsDelegateMock = new();

        SyntacticMapper mapper = new(parameterNameComparer, recorderFactoryMock.Object, repositoryFactoryMock.Object, loggerMock.Object, addMappingsDelegateMock.Object);

        return new(mapper, parameterNameComparer, recorderFactoryMock, repositoryFactoryMock, loggerMock, addMappingsDelegateMock);
    }

    public SyntacticMapper Mapper { get; }

    public IEqualityComparer<string> ParameterNameComparer { get; }
    public Mock<IMappedSyntacticArgumentRecorderFactory> RecorderFactoryMock { get; }
    public Mock<ISyntacticMappingRepositoryFactory<TRecord>> RepositoryFactoryMock { get; }
    public Mock<ISyntacticMapperLogger<ASyntacticMapper<TRecord>>> LoggerMock { get; }

    public Mock<Action<IAppendableSyntacticMappingRepository<TRecord>>> AddMappingsDelegateMock { get; }

    private MapperContext(SyntacticMapper mapper, IEqualityComparer<string> parameterNameComparer, Mock<IMappedSyntacticArgumentRecorderFactory> recorderFactoryMock, Mock<ISyntacticMappingRepositoryFactory<TRecord>> repositoryFactoryMock, Mock<ISyntacticMapperLogger<ASyntacticMapper<TRecord>>> loggerMock, Mock<Action<IAppendableSyntacticMappingRepository<TRecord>>> addMappingsDelegateMock)
    {
        Mapper = mapper;

        ParameterNameComparer = parameterNameComparer;
        RecorderFactoryMock = recorderFactoryMock;
        RepositoryFactoryMock = repositoryFactoryMock;
        LoggerMock = loggerMock;

        AddMappingsDelegateMock = addMappingsDelegateMock;
    }

    public sealed class SyntacticMapper : ASyntacticMapper<TRecord>
    {
        private Action<IAppendableSyntacticMappingRepository<TRecord>> AddMappingsDelegate { get; }

        public SyntacticMapper(IEqualityComparer<string> parameterNameComparer, IMappedSyntacticArgumentRecorderFactory recorderFactory, ISyntacticMappingRepositoryFactory<TRecord> repositoryFactory, ISyntacticMapperLogger<ASyntacticMapper<TRecord>> logger, Action<IAppendableSyntacticMappingRepository<TRecord>> addMappingsDelegate) : base(parameterNameComparer, recorderFactory, repositoryFactory, logger)
        {
            AddMappingsDelegate = addMappingsDelegate;
        }

        public void Initialize() => InitializeMapper();

        protected override void AddMappings(IAppendableSyntacticMappingRepository<TRecord> repository) => AddMappingsDelegate(repository);
    }
}
