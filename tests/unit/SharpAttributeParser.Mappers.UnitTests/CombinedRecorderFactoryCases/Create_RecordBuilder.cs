namespace SharpAttributeParser.Mappers.CombinedRecorderFactoryCases;

using Microsoft.CodeAnalysis;

using Moq;

using System;

using Xunit;

public sealed class Create_RecordBuilder
{
    private static ICombinedRecorder<TRecord> Target<TRecord, TRecordBuilder>(ICombinedRecorderFactory factory, ICombinedMapper<TRecordBuilder> mapper, TRecordBuilder dataRecord) where TRecordBuilder : IRecordBuilder<TRecord> => factory.Create<TRecord, TRecordBuilder>(mapper, dataRecord);

    [Fact]
    public void NullMapper_ArgumentNullException()
    {
        var context = FactoryContext.Create();

        var exception = Record.Exception(() => Target<object, IRecordBuilder<object>>(context.Factory, null!, Mock.Of<IRecordBuilder<object>>()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullRecord_ArgumentNullException()
    {
        var context = FactoryContext.Create();

        var exception = Record.Exception(() => Target<object, IRecordBuilder<object>>(context.Factory, Mock.Of<ICombinedMapper<IRecordBuilder<object>>>(), null!));

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

        var typeSyntax = ExpressionSyntaxFactory.Create();
        var constructorSyntax = ExpressionSyntaxFactory.Create();
        var namedSyntax = ExpressionSyntaxFactory.Create();

        var dataRecord = Mock.Of<object>();
        Mock<IRecordBuilder<object>> recordBuilderMock = new();

        Mock<ICombinedMapper<object>> mapperMock = new() { DefaultValue = DefaultValue.Mock };

        var context = FactoryContext.Create();

        recordBuilderMock.Setup(static (recordBuilder) => recordBuilder.Build()).Returns(dataRecord);

        var recorder = Target<object, IRecordBuilder<object>>(context.Factory, mapperMock.Object, recordBuilderMock.Object);

        recorder.TypeArgument.TryRecordArgument(typeParameter, typeArgument, typeSyntax);
        recorder.ConstructorArgument.TryRecordArgument(constructorParameter, constructorArgument, constructorSyntax);
        recorder.NamedArgument.TryRecordArgument(namedParameterName, namedArgument, namedSyntax);

        var retrievedDataRecord = recorder.GetRecord();

        mapperMock.Verify((mapper) => mapper.TryMapTypeParameter(typeParameter, recordBuilderMock.Object)!.TryRecordArgument(typeArgument, typeSyntax), Times.Once);
        mapperMock.Verify((mapper) => mapper.TryMapConstructorParameter(constructorParameter, recordBuilderMock.Object)!.TryRecordArgument(constructorArgument, constructorSyntax), Times.Once);
        mapperMock.Verify((mapper) => mapper.TryMapNamedParameter(namedParameterName, recordBuilderMock.Object)!.TryRecordArgument(namedArgument, namedSyntax), Times.Once);

        Assert.Same(dataRecord, retrievedDataRecord);

        recordBuilderMock.Verify((recordBuilder) => recordBuilder.Build(), Times.Once);
    }
}
