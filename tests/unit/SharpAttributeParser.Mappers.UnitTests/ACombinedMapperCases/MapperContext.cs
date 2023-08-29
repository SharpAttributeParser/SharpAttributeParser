namespace SharpAttributeParser.Mappers.ACombinedMapperCases;

using Moq;

using SharpAttributeParser.Mappers.Logging;
using SharpAttributeParser.Mappers.Repositories.Combined;

using System;
using System.Collections.Generic;

internal sealed class MapperContext<TRecord>
{
    public static MapperContext<TRecord> Create()
    {
        var parameterNameComparer = Mock.Of<IEqualityComparer<string>>();
        Mock<IMappedCombinedArgumentRecorderFactory> recorderFactoryMock = new();
        Mock<ICombinedMappingRepositoryFactory<TRecord>> repositoryFactoryMock = new() { DefaultValue = DefaultValue.Mock };
        Mock<ICombinedMapperLogger<ACombinedMapper<TRecord>>> loggerMock = new() { DefaultValue = DefaultValue.Mock };

        Mock<Action<IAppendableCombinedMappingRepository<TRecord>>> addMappingsDelegateMock = new();

        CombinedMapper mapper = new(parameterNameComparer, recorderFactoryMock.Object, repositoryFactoryMock.Object, loggerMock.Object, addMappingsDelegateMock.Object);

        return new(mapper, parameterNameComparer, recorderFactoryMock, repositoryFactoryMock, loggerMock, addMappingsDelegateMock);
    }

    public CombinedMapper Mapper { get; }

    public IEqualityComparer<string> ParameterNameComparer { get; }
    public Mock<IMappedCombinedArgumentRecorderFactory> RecorderFactoryMock { get; }
    public Mock<ICombinedMappingRepositoryFactory<TRecord>> RepositoryFactoryMock { get; }
    public Mock<ICombinedMapperLogger<ACombinedMapper<TRecord>>> LoggerMock { get; }

    public Mock<Action<IAppendableCombinedMappingRepository<TRecord>>> AddMappingsDelegateMock { get; }

    private MapperContext(CombinedMapper mapper, IEqualityComparer<string> parameterNameComparer, Mock<IMappedCombinedArgumentRecorderFactory> recorderFactoryMock, Mock<ICombinedMappingRepositoryFactory<TRecord>> repositoryFactoryMock, Mock<ICombinedMapperLogger<ACombinedMapper<TRecord>>> loggerMock, Mock<Action<IAppendableCombinedMappingRepository<TRecord>>> addMappingsDelegateMock)
    {
        Mapper = mapper;

        ParameterNameComparer = parameterNameComparer;
        RecorderFactoryMock = recorderFactoryMock;
        RepositoryFactoryMock = repositoryFactoryMock;
        LoggerMock = loggerMock;

        AddMappingsDelegateMock = addMappingsDelegateMock;
    }

    public sealed class CombinedMapper : ACombinedMapper<TRecord>
    {
        private Action<IAppendableCombinedMappingRepository<TRecord>> AddMappingsDelegate { get; }

        public CombinedMapper(IEqualityComparer<string> parameterNameComparer, IMappedCombinedArgumentRecorderFactory recorderFactory, ICombinedMappingRepositoryFactory<TRecord> repositoryFactory, ICombinedMapperLogger<ACombinedMapper<TRecord>> logger, Action<IAppendableCombinedMappingRepository<TRecord>> addMappingsDelegate) : base(parameterNameComparer, recorderFactory, repositoryFactory, logger)
        {
            AddMappingsDelegate = addMappingsDelegate;
        }

        public void Initialize() => InitializeMapper();

        protected override void AddMappings(IAppendableCombinedMappingRepository<TRecord> repository) => AddMappingsDelegate(repository);
    }
}
