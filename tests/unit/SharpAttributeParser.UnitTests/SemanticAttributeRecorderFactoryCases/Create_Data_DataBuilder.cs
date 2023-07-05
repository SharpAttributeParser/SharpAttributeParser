namespace SharpAttributeParser.SemanticAttributeRecorderFactoryCases;

using Microsoft.CodeAnalysis;

using Moq;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Xunit;

public sealed class Create_Data_DataBuilder
{
    private static ISemanticAttributeRecorder<TData> Target<TData, TDataBuilder>(ISemanticAttributeRecorderFactory factory, ISemanticAttributeMapper<TDataBuilder> mapper, TDataBuilder dataBuilder) where TDataBuilder : IAttributeDataBuilder<TData> => factory.Create<TData, TDataBuilder>(mapper, dataBuilder);

    [Theory]
    [ClassData(typeof(FactorySources))]
    public void NullMapper_ArgumentNullException(ISemanticAttributeRecorderFactory factory)
    {
        var exception = Record.Exception(() => Target<string, IAttributeDataBuilder<string>>(factory, null!, Mock.Of<IAttributeDataBuilder<string>>()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(FactorySources))]
    public void NullDataRecord_ArgumentNullException(ISemanticAttributeRecorderFactory factory)
    {
        var exception = Record.Exception(() => Target<string, IAttributeDataBuilder<string>>(factory, Mock.Of<ISemanticAttributeMapper<IAttributeDataBuilder<string>>>(), null!));

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

        mapperMock.Setup(static (mapper) => mapper.TryMapTypeParameter(It.IsAny<DataRecordBuilder>(), It.IsAny<ITypeParameterSymbol>())).Returns<DataRecordBuilder, ITypeParameterSymbol>(tryMapTypeParameter);
        mapperMock.Setup(static (mapper) => mapper.TryMapConstructorParameter(It.IsAny<DataRecordBuilder>(), It.IsAny<IParameterSymbol>())).Returns<DataRecordBuilder, IParameterSymbol>(tryMapConstructorParameter);
        mapperMock.Setup(static (mapper) => mapper.TryMapNamedParameter(It.IsAny<DataRecordBuilder>(), It.IsAny<string>())).Returns<DataRecordBuilder, string>(tryMapNamedParameter);

        var recorder = Target<DataRecord, DataRecordBuilder>(factory, mapperMock.Object, dataRecordBuilder);

        recorder.TryRecordTypeArgument(typeParameter, typeArgument);
        recorder.TryRecordConstructorArgument(constructorParameter, constructorArgument);
        recorder.TryRecordNamedArgument(namedParameter, namedArgument);

        var result = recorder.GetResult();

        Assert.Equal(dataRecord, result);

        mapperMock.Verify((mapper) => mapper.TryMapTypeParameter(dataRecordBuilder, typeParameter), Times.AtLeastOnce);
        mapperMock.Verify((mapper) => mapper.TryMapConstructorParameter(dataRecordBuilder, constructorParameter), Times.AtLeastOnce);
        mapperMock.Verify((mapper) => mapper.TryMapNamedParameter(dataRecordBuilder, namedParameter), Times.AtLeastOnce);

        Assert.Equal(typeArgument, result.TypeArgument, ReferenceEqualityComparer.Instance);
        Assert.Equal(constructorArgument, result.ConstructorArgument, ReferenceEqualityComparer.Instance);
        Assert.Equal(namedArgument, result.NamedArgument, ReferenceEqualityComparer.Instance);

        DSemanticAttributeArgumentRecorder? tryMapTypeParameter(DataRecordBuilder dataBuilder, ITypeParameterSymbol parameter) => (argument) =>
        {
            dataBuilder.WithTypeArgument(argument);

            return true;
        };

        DSemanticAttributeArgumentRecorder? tryMapConstructorParameter(DataRecordBuilder dataBuilder, IParameterSymbol parameter) => (argument) =>
        {
            dataBuilder.WithConstructorArgument(argument);

            return true;
        };

        DSemanticAttributeArgumentRecorder? tryMapNamedParameter(DataRecordBuilder dataBuilder, string parameterName) => (argument) =>
        {
            dataBuilder.WithNamedArgument(argument);

            return true;
        };
    }

    [SuppressMessage("Design", "CA1034: Nested types should not be visible", Justification = "Type should not be used elsewhere, but Moq requires it to be public.")]
    public sealed class DataRecord
    {
        public object? TypeArgument { get; set; }
        public object? ConstructorArgument { get; set; }
        public object? NamedArgument { get; set; }
    }

    [SuppressMessage("Design", "CA1034: Nested types should not be visible", Justification = "Type should not be used elsewhere, but Moq requires it to be public.")]
    public sealed class DataRecordBuilder : IAttributeDataBuilder<DataRecord>
    {
        private DataRecord BuildTarget { get; }

        public DataRecordBuilder(DataRecord buildTarget)
        {
            BuildTarget = buildTarget;
        }

        public void WithTypeArgument(object? typeArgument) => BuildTarget.TypeArgument = typeArgument;
        public void WithConstructorArgument(object? typeArgument) => BuildTarget.ConstructorArgument = typeArgument;
        public void WithNamedArgument(object? typeArgument) => BuildTarget.NamedArgument = typeArgument;

        DataRecord IAttributeDataBuilder<DataRecord>.Build() => BuildTarget;
    }
}
