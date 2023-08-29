namespace SharpAttributeParser.Mappers.ASplitMapperCases;

using Moq;

using SharpAttributeParser.Mappers.Logging;
using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Mappers.Repositories.Split;
using SharpAttributeParser.Mappers.Repositories.Syntactic;

using System;
using System.Collections.Generic;

internal sealed class MapperContext<TSemanticRecord, TSyntacticRecord>
{
    public static MapperContext<TSemanticRecord, TSyntacticRecord> Create()
    {
        var parameterNameComparer = Mock.Of<IEqualityComparer<string>>();
        Mock<IMappedSemanticArgumentRecorderFactory> semanticRecorderFactoryMock = new();
        Mock<IMappedSyntacticArgumentRecorderFactory> syntacticRecorderFactoryMock = new();
        Mock<ISplitMappingRepositoryFactory<TSemanticRecord, TSyntacticRecord>> repositoryFactoryMock = new() { DefaultValue = DefaultValue.Mock };
        Mock<ISemanticMapperLogger<ASplitMapper<TSemanticRecord, TSyntacticRecord>>> semanticLoggerMock = new() { DefaultValue = DefaultValue.Mock };
        Mock<ISyntacticMapperLogger<ASplitMapper<TSemanticRecord, TSyntacticRecord>>> syntacticLoggerMock = new() { DefaultValue = DefaultValue.Mock };

        Mock<Action<IAppendableSplitMappingRepository<TSemanticRecord, TSyntacticRecord>>> addMappingsDelegateMock = new();

        SplitMapper mapper = new(parameterNameComparer, semanticRecorderFactoryMock.Object, syntacticRecorderFactoryMock.Object, repositoryFactoryMock.Object, semanticLoggerMock.Object, syntacticLoggerMock.Object, addMappingsDelegateMock.Object);

        return new(mapper, parameterNameComparer, semanticRecorderFactoryMock, syntacticRecorderFactoryMock, repositoryFactoryMock, semanticLoggerMock, syntacticLoggerMock, addMappingsDelegateMock);
    }

    public SplitMapper Mapper { get; }

    public IEqualityComparer<string> ParameterNameComparer { get; }
    public Mock<IMappedSemanticArgumentRecorderFactory> SemanticRecorderFactoryMock { get; }
    public Mock<IMappedSyntacticArgumentRecorderFactory> SyntacticRecorderFactoryMock { get; }
    public Mock<ISplitMappingRepositoryFactory<TSemanticRecord, TSyntacticRecord>> RepositoryFactoryMock { get; }
    public Mock<ISemanticMapperLogger<ASplitMapper<TSemanticRecord, TSyntacticRecord>>> SemanticLoggerMock { get; }
    public Mock<ISyntacticMapperLogger<ASplitMapper<TSemanticRecord, TSyntacticRecord>>> SyntacticLoggerMock { get; }

    public Mock<Action<IAppendableSplitMappingRepository<TSemanticRecord, TSyntacticRecord>>> AddMappingsDelegateMock { get; }

    private MapperContext(SplitMapper mapper, IEqualityComparer<string> parameterNameComparer, Mock<IMappedSemanticArgumentRecorderFactory> semanticRecorderFactoryMock, Mock<IMappedSyntacticArgumentRecorderFactory> syntacticRecorderFactoryMock, Mock<ISplitMappingRepositoryFactory<TSemanticRecord, TSyntacticRecord>> repositoryFactoryMock, Mock<ISemanticMapperLogger<ASplitMapper<TSemanticRecord, TSyntacticRecord>>> semanticLoggerMock, Mock<ISyntacticMapperLogger<ASplitMapper<TSemanticRecord, TSyntacticRecord>>> syntacticLoggerMock, Mock<Action<IAppendableSplitMappingRepository<TSemanticRecord, TSyntacticRecord>>> addMappingsDelegateMock)
    {
        Mapper = mapper;

        ParameterNameComparer = parameterNameComparer;
        SemanticRecorderFactoryMock = semanticRecorderFactoryMock;
        SyntacticRecorderFactoryMock = syntacticRecorderFactoryMock;
        RepositoryFactoryMock = repositoryFactoryMock;
        SemanticLoggerMock = semanticLoggerMock;
        SyntacticLoggerMock = syntacticLoggerMock;

        AddMappingsDelegateMock = addMappingsDelegateMock;
    }

    public sealed class SplitMapper : ASplitMapper<TSemanticRecord, TSyntacticRecord>
    {
        private Action<IAppendableSplitMappingRepository<TSemanticRecord, TSyntacticRecord>> AddMappingsDelegate { get; }

        public SplitMapper(IEqualityComparer<string> parameterNameComparer, IMappedSemanticArgumentRecorderFactory semanticRecorderFactory, IMappedSyntacticArgumentRecorderFactory syntacticRecorderFactory, ISplitMappingRepositoryFactory<TSemanticRecord, TSyntacticRecord> repositoryFactory, ISemanticMapperLogger<ASplitMapper<TSemanticRecord, TSyntacticRecord>> semanticLogger, ISyntacticMapperLogger<ASplitMapper<TSemanticRecord, TSyntacticRecord>> syntacticLogger, Action<IAppendableSplitMappingRepository<TSemanticRecord, TSyntacticRecord>> addMappingsDelegate) : base(parameterNameComparer, semanticRecorderFactory, syntacticRecorderFactory, repositoryFactory, semanticLogger, syntacticLogger)
        {
            AddMappingsDelegate = addMappingsDelegate;
        }

        public void Initialize() => InitializeMapper();

        protected override void AddMappings(IAppendableSplitMappingRepository<TSemanticRecord, TSyntacticRecord> repository) => AddMappingsDelegate(repository);
    }
}
