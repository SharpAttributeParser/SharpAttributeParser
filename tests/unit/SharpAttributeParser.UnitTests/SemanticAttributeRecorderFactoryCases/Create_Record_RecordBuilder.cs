namespace SharpAttributeParser.SemanticAttributeRecorderFactoryCases;

using Microsoft.CodeAnalysis;

using Moq;

using System;
using System.Diagnostics.CodeAnalysis;

using Xunit;

public sealed class Create_Record_RecordBuilder
{
    private static ISemanticAttributeRecorder<TRecord> Target<TRecord, TRecordBuilder>(ISemanticAttributeRecorderFactory factory, ISemanticAttributeMapper<TRecordBuilder> mapper, TRecordBuilder recordBuilder) where TRecordBuilder : IRecordBuilder<TRecord> => factory.Create<TRecord, TRecordBuilder>(mapper, recordBuilder);

    [Theory]
    [ClassData(typeof(FactorySources))]
    public void NullMapper_ArgumentNullException(ISemanticAttributeRecorderFactory factory)
    {
        var exception = Record.Exception(() => Target<string, IRecordBuilder<string>>(factory, null!, Mock.Of<IRecordBuilder<string>>()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(FactorySources))]
    public void NullDataRecord_ArgumentNullException(ISemanticAttributeRecorderFactory factory)
    {
        var exception = Record.Exception(() => Target<string, IRecordBuilder<string>>(factory, Mock.Of<ISemanticAttributeMapper<IRecordBuilder<string>>>(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(FactorySources))]
    public void Valid_ProducedRecorderUsesProvidedMapperAndDataRecord(ISemanticAttributeRecorderFactory factory)
    {
        DataRecord dataRecord = new();
        DataRecordBuilder dataRecordBuilder = new(dataRecord);

        var typeParameter = Mock.Of<ITypeParameterSymbol>();
        var constructorParameter = Mock.Of<IParameterSymbol>();
        var namedParameter = string.Empty;

        var typeArgument = Mock.Of<ITypeSymbol>();
        var constructorArgument = Mock.Of<object>();
        var namedArgument = Mock.Of<object>();

        Mock<ISemanticAttributeMapper<DataRecordBuilder>> mapperMock = new();

        mapperMock.Setup(static (mapper) => mapper.TryMapTypeParameter(It.IsAny<ITypeParameterSymbol>(), It.IsAny<DataRecordBuilder>())).Returns<ITypeParameterSymbol, DataRecordBuilder>(tryMapTypeParameter);
        mapperMock.Setup(static (mapper) => mapper.TryMapConstructorParameter(It.IsAny<IParameterSymbol>(), It.IsAny<DataRecordBuilder>())).Returns<IParameterSymbol, DataRecordBuilder>(tryMapConstructorParameter);
        mapperMock.Setup(static (mapper) => mapper.TryMapNamedParameter(It.IsAny<string>(), It.IsAny<DataRecordBuilder>())).Returns<string, DataRecordBuilder>(tryMapNamedParameter);

        var recorder = Target<DataRecord, DataRecordBuilder>(factory, mapperMock.Object, dataRecordBuilder);

        recorder.TryRecordTypeArgument(typeParameter, typeArgument);
        recorder.TryRecordConstructorArgument(constructorParameter, constructorArgument);
        recorder.TryRecordNamedArgument(namedParameter, namedArgument);

        var result = recorder.GetRecord();

        Assert.Equal(dataRecord, result);

        mapperMock.Verify((mapper) => mapper.TryMapTypeParameter(typeParameter, dataRecordBuilder), Times.AtLeastOnce);
        mapperMock.Verify((mapper) => mapper.TryMapConstructorParameter(constructorParameter, dataRecordBuilder), Times.AtLeastOnce);
        mapperMock.Verify((mapper) => mapper.TryMapNamedParameter(namedParameter, dataRecordBuilder), Times.AtLeastOnce);

        Assert.Equal(typeArgument, result.TypeArgument);
        Assert.Equal(constructorArgument, result.ConstructorArgument);
        Assert.Equal(namedArgument, result.NamedArgument);

        ISemanticAttributeArgumentRecorder? tryMapTypeParameter(ITypeParameterSymbol parameter, DataRecordBuilder recordBuilder) => new SemanticAttributeArgumentRecorder((argument) =>
        {
            recordBuilder.WithTypeArgument(argument);

            return true;
        });

        ISemanticAttributeArgumentRecorder? tryMapConstructorParameter(IParameterSymbol parameter, DataRecordBuilder recordBuilder) => new SemanticAttributeArgumentRecorder((argument) =>
        {
            recordBuilder.WithConstructorArgument(argument);

            return true;
        });

        ISemanticAttributeArgumentRecorder? tryMapNamedParameter(string parameterName, DataRecordBuilder recordBuilder) => new SemanticAttributeArgumentRecorder((argument) =>
        {
            recordBuilder.WithNamedArgument(argument);

            return true;
        });
    }

    [SuppressMessage("Design", "CA1034: Nested types should not be visible", Justification = "Type should not be used elsewhere, but DataRecordBuilder requires it to be public.")]
    public sealed class DataRecord
    {
        public object? TypeArgument { get; set; }
        public object? ConstructorArgument { get; set; }
        public object? NamedArgument { get; set; }
    }

    [SuppressMessage("Design", "CA1034: Nested types should not be visible", Justification = "Type should not be used elsewhere, but Moq requires it to be public.")]
    public sealed class DataRecordBuilder : IRecordBuilder<DataRecord>
    {
        private DataRecord BuildTarget { get; }

        public DataRecordBuilder(DataRecord buildTarget)
        {
            BuildTarget = buildTarget;
        }

        public void WithTypeArgument(object? argument) => BuildTarget.TypeArgument = argument;
        public void WithConstructorArgument(object? argument) => BuildTarget.ConstructorArgument = argument;
        public void WithNamedArgument(object? argument) => BuildTarget.NamedArgument = argument;

        DataRecord IRecordBuilder<DataRecord>.Build() => BuildTarget;
    }
}
