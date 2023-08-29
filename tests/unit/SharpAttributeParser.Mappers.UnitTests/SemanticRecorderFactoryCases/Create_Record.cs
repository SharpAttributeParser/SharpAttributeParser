namespace SharpAttributeParser.Mappers.SemanticRecorderFactoryCases;

using Microsoft.CodeAnalysis;

using Moq;

using System;

using Xunit;

public sealed class Create_Record
{
    private static ISemanticRecorder<TRecord> Target<TRecord>(ISemanticRecorderFactory factory, ISemanticMapper<TRecord> mapper, TRecord dataRecord) => factory.Create(mapper, dataRecord);

    [Fact]
    public void NullMapper_ArgumentNullException()
    {
        var context = FactoryContext.Create();

        var exception = Record.Exception(() => Target(context.Factory, null!, Mock.Of<object>()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullRecord_ArgumentNullException()
    {
        var context = FactoryContext.Create();

        var exception = Record.Exception(() => Target(context.Factory, Mock.Of<ISemanticMapper<object>>(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidMapperAndRecord_ConstructedRecorderUsesMapperAndRecord()
    {
        var typeParameter = Mock.Of<ITypeParameterSymbol>();
        var constructorParameter = Mock.Of<IParameterSymbol>();
        var namedParameterName = string.Empty;

        var typeArgument = Mock.Of<ITypeSymbol>();
        var constructorArgument = Mock.Of<object>();
        var namedArgument = Mock.Of<object>();

        var dataRecord = Mock.Of<object>();

        Mock<ISemanticMapper<object>> mapperMock = new() { DefaultValue = DefaultValue.Mock };

        var context = FactoryContext.Create();

        var recorder = Target(context.Factory, mapperMock.Object, dataRecord);

        recorder.TypeArgument.TryRecordArgument(typeParameter, typeArgument);
        recorder.ConstructorArgument.TryRecordArgument(constructorParameter, constructorArgument);
        recorder.NamedArgument.TryRecordArgument(namedParameterName, namedArgument);

        var retrievedDataRecord = recorder.GetRecord();

        mapperMock.Verify((mapper) => mapper.TryMapTypeParameter(typeParameter, dataRecord)!.TryRecordArgument(typeArgument), Times.Once);
        mapperMock.Verify((mapper) => mapper.TryMapConstructorParameter(constructorParameter, dataRecord)!.TryRecordArgument(constructorArgument), Times.Once);
        mapperMock.Verify((mapper) => mapper.TryMapNamedParameter(namedParameterName, dataRecord)!.TryRecordArgument(namedArgument), Times.Once);

        Assert.Same(dataRecord, retrievedDataRecord);
    }
}
