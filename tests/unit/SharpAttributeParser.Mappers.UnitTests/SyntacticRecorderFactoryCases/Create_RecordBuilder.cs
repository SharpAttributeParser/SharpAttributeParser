namespace SharpAttributeParser.Mappers.SyntacticRecorderFactoryCases;

using Microsoft.CodeAnalysis;

using Moq;

using System;

using Xunit;

public sealed class Create_RecordBuilder
{
    private static ISyntacticRecorder<TRecord> Target<TRecord, TRecordBuilder>(ISyntacticRecorderFactory factory, ISyntacticMapper<TRecordBuilder> mapper, TRecordBuilder dataRecord) where TRecordBuilder : IRecordBuilder<TRecord> => factory.Create<TRecord, TRecordBuilder>(mapper, dataRecord);

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

        var exception = Record.Exception(() => Target<object, IRecordBuilder<object>>(context.Factory, Mock.Of<ISyntacticMapper<IRecordBuilder<object>>>(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidMapperAndRecord_ConstructedRecorderUsesMapperAndRecord()
    {
        var typeParameter = Mock.Of<ITypeParameterSymbol>();
        var constructorParameter = Mock.Of<IParameterSymbol>();
        var namedParameterName = string.Empty;

        var typeSyntax = ExpressionSyntaxFactory.Create();
        var constructorSyntax = ExpressionSyntaxFactory.Create();
        var namedSyntax = ExpressionSyntaxFactory.Create();

        var dataRecord = Mock.Of<object>();
        Mock<IRecordBuilder<object>> recordBuilderMock = new();

        Mock<ISyntacticMapper<object>> mapperMock = new() { DefaultValue = DefaultValue.Mock };

        var context = FactoryContext.Create();

        recordBuilderMock.Setup(static (recordBuilder) => recordBuilder.Build()).Returns(dataRecord);

        var recorder = Target<object, IRecordBuilder<object>>(context.Factory, mapperMock.Object, recordBuilderMock.Object);

        recorder.TypeArgument.TryRecordArgument(typeParameter, typeSyntax);
        recorder.ConstructorArgument.TryRecordArgument(constructorParameter, constructorSyntax);
        recorder.NamedArgument.TryRecordArgument(namedParameterName, namedSyntax);

        var retrievedDataRecord = recorder.GetRecord();

        mapperMock.Verify((mapper) => mapper.TryMapTypeParameter(typeParameter, recordBuilderMock.Object)!.TryRecordArgument(typeSyntax), Times.Once);
        mapperMock.Verify((mapper) => mapper.TryMapConstructorParameter(constructorParameter, recordBuilderMock.Object)!.TryRecordArgument(constructorSyntax), Times.Once);
        mapperMock.Verify((mapper) => mapper.TryMapNamedParameter(namedParameterName, recordBuilderMock.Object)!.TryRecordArgument(namedSyntax), Times.Once);

        Assert.Same(dataRecord, retrievedDataRecord);

        recordBuilderMock.Verify((recordBuilder) => recordBuilder.Build(), Times.Once);
    }
}
