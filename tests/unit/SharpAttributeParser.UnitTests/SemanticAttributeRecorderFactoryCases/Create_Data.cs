namespace SharpAttributeParser.SemanticAttributeRecorderFactoryCases;

using Microsoft.CodeAnalysis;

using Moq;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Xunit;

public sealed class Create_Data
{
    private static ISemanticAttributeRecorder<TData> Target<TData>(ISemanticAttributeRecorderFactory factory, ISemanticAttributeMapper<TData> mapper, TData dataRecord) => factory.Create(mapper, dataRecord);

    [Theory]
    [ClassData(typeof(FactorySources))]
    public void NullMapper_ArgumentNullException(ISemanticAttributeRecorderFactory factory)
    {
        var exception = Record.Exception(() => Target(factory, null!, string.Empty));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(FactorySources))]
    public void NullDataRecord_ArgumentNullException(ISemanticAttributeRecorderFactory factory)
    {
        var exception = Record.Exception(() => Target(factory, Mock.Of<ISemanticAttributeMapper<string>>(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(FactorySources))]
    public void Valid_ProducedRecorderUsesProvidedMapperAndDataRecord(ISemanticAttributeRecorderFactory factory)
    {
        DataRecord dataRecord = new();

        var typeParameter = Mock.Of<ITypeParameterSymbol>();
        var constructorParameter = Mock.Of<IParameterSymbol>();
        var namedParameter = string.Empty;

        var typeArgument = Mock.Of<ITypeSymbol>();
        var constructorArgument = Mock.Of<object>();
        var namedArgument = Mock.Of<object>();

        Mock<ISemanticAttributeMapper<DataRecord>> mapperMock = new();

        mapperMock.Setup(static (mapper) => mapper.TryMapTypeParameter(It.IsAny<DataRecord>(), It.IsAny<ITypeParameterSymbol>())).Returns<DataRecord, ITypeParameterSymbol>(tryMapTypeParameter);
        mapperMock.Setup(static (mapper) => mapper.TryMapConstructorParameter(It.IsAny<DataRecord>(), It.IsAny<IParameterSymbol>())).Returns<DataRecord, IParameterSymbol>(tryMapConstructorParameter);
        mapperMock.Setup(static (mapper) => mapper.TryMapNamedParameter(It.IsAny<DataRecord>(), It.IsAny<string>())).Returns<DataRecord, string>(tryMapNamedParameter);

        var recorder = Target(factory, mapperMock.Object, dataRecord);

        recorder.TryRecordTypeArgument(typeParameter, typeArgument);
        recorder.TryRecordConstructorArgument(constructorParameter, constructorArgument);
        recorder.TryRecordNamedArgument(namedParameter, namedArgument);

        var result = recorder.GetResult();

        Assert.Equal(dataRecord, result);

        mapperMock.Verify((mapper) => mapper.TryMapTypeParameter(dataRecord, typeParameter), Times.AtLeastOnce);
        mapperMock.Verify((mapper) => mapper.TryMapConstructorParameter(dataRecord, constructorParameter), Times.AtLeastOnce);
        mapperMock.Verify((mapper) => mapper.TryMapNamedParameter(dataRecord, namedParameter), Times.AtLeastOnce);

        Assert.Equal(typeArgument, result.TypeArgument, ReferenceEqualityComparer.Instance);
        Assert.Equal(constructorArgument, result.ConstructorArgument, ReferenceEqualityComparer.Instance);
        Assert.Equal(namedArgument, result.NamedArgument, ReferenceEqualityComparer.Instance);

        DSemanticAttributeArgumentRecorder? tryMapTypeParameter(DataRecord dataRecord, ITypeParameterSymbol parameter) => (argument) =>
        {
            dataRecord.TypeArgument = argument;

            return true;
        };

        DSemanticAttributeArgumentRecorder? tryMapConstructorParameter(DataRecord dataRecord, IParameterSymbol parameter) => (argument) =>
        {
            dataRecord.ConstructorArgument = argument;

            return true;
        };

        DSemanticAttributeArgumentRecorder? tryMapNamedParameter(DataRecord dataRecord, string parameterName) => (argument) =>
        {
            dataRecord.NamedArgument = argument;

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
}
