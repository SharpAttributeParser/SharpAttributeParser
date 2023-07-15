namespace SharpAttributeParser.AttributeRecorderFactoryCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Xunit;

public sealed class Create_Data
{
    private static IAttributeRecorder<TData> Target<TData>(IAttributeRecorderFactory factory, IAttributeMapper<TData> mapper, TData dataRecord) => factory.Create(mapper, dataRecord);

    [Theory]
    [ClassData(typeof(FactorySources))]
    public void NullMapper_ArgumentNullException(IAttributeRecorderFactory factory)
    {
        var exception = Record.Exception(() => Target(factory, null!, string.Empty));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(FactorySources))]
    public void NullDataRecord_ArgumentNullException(IAttributeRecorderFactory factory)
    {
        var exception = Record.Exception(() => Target(factory, Mock.Of<IAttributeMapper<string>>(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(FactorySources))]
    public void Valid_ProducedRecorderUsesProvidedMapperAndDataRecord(IAttributeRecorderFactory factory)
    {
        DataRecord dataRecord = new();

        var typeParameter = Mock.Of<ITypeParameterSymbol>();
        var constructorParameter = Mock.Of<IParameterSymbol>();
        var namedParameter = string.Empty;

        var typeArgument = Mock.Of<ITypeSymbol>();
        var constructorArgument = Mock.Of<object>();
        var constructorParamsArgument = Mock.Of<object>();
        var namedArgument = Mock.Of<object>();

        var typeArgumentSyntax = ExpressionSyntaxFactory.Create();
        var constructorArgumentSyntax = ExpressionSyntaxFactory.Create();
        var constructorParamsArgumentSyntax = Array.Empty<ExpressionSyntax>();
        var namedArgumentSyntax = ExpressionSyntaxFactory.Create();

        Mock<IAttributeMapper<DataRecord>> mapperMock = new();

        mapperMock.Setup(static (mapper) => mapper.TryMapTypeParameter(It.IsAny<ITypeParameterSymbol>(), It.IsAny<DataRecord>())).Returns<ITypeParameterSymbol, DataRecord>(tryMapTypeParameter);
        mapperMock.Setup(static (mapper) => mapper.TryMapConstructorParameter(It.IsAny<IParameterSymbol>(), It.IsAny<DataRecord>())).Returns<IParameterSymbol, DataRecord>(tryMapConstructorParameter);
        mapperMock.Setup(static (mapper) => mapper.TryMapNamedParameter(It.IsAny<string>(), It.IsAny<DataRecord>())).Returns<string, DataRecord>(tryMapNamedParameter);

        var recorder = Target(factory, mapperMock.Object, dataRecord);

        recorder.TryRecordTypeArgument(typeParameter, typeArgument, typeArgumentSyntax);
        recorder.TryRecordConstructorArgument(constructorParameter, constructorArgument, constructorArgumentSyntax);
        recorder.TryRecordConstructorParamsArgument(constructorParameter, constructorParamsArgument, constructorParamsArgumentSyntax);
        recorder.TryRecordNamedArgument(namedParameter, namedArgument, namedArgumentSyntax);

        var result = recorder.GetRecord();

        Assert.Equal(dataRecord, result);

        mapperMock.Verify((mapper) => mapper.TryMapTypeParameter(typeParameter, dataRecord), Times.AtLeastOnce);
        mapperMock.Verify((mapper) => mapper.TryMapConstructorParameter(constructorParameter, dataRecord), Times.AtLeast(2));
        mapperMock.Verify((mapper) => mapper.TryMapNamedParameter(namedParameter, dataRecord), Times.AtLeastOnce);

        Assert.Equal(typeArgument, result.TypeArgument);
        Assert.Equal(constructorArgument, result.ConstructorArgument);
        Assert.Equal(constructorParamsArgument, result.ConstructorParamsArgument);
        Assert.Equal(namedArgument, result.NamedArgument);

        Assert.Equal(typeArgumentSyntax, result.TypeArgumentSyntax);
        Assert.Equal(constructorArgumentSyntax, result.ConstructorArgumentSyntax);
        Assert.Equal<IReadOnlyList<ExpressionSyntax>>(constructorParamsArgumentSyntax, result.ConstructorParamsArgumentSyntax);
        Assert.Equal(namedArgumentSyntax, result.NamedArgumentSyntax);

        IAttributeArgumentRecorder? tryMapTypeParameter(ITypeParameterSymbol parameter, DataRecord dataRecord) => new AttributeArgumentRecorder((argument, syntax) =>
        {
            dataRecord.TypeArgument = argument;
            dataRecord.TypeArgumentSyntax = syntax.AsT0;

            return true;
        });

        IAttributeConstructorArgumentRecorder? tryMapConstructorParameter(IParameterSymbol parameter, DataRecord dataRecord) => new AttributeArgumentRecorder((argument, syntax) =>
        {
            syntax.Switch
            (
                (syntax) =>
                {
                    dataRecord.ConstructorArgument = argument;
                    dataRecord.ConstructorArgumentSyntax = syntax;
                },
                (elementSyntax) =>
                {
                    dataRecord.ConstructorParamsArgument = argument;
                    dataRecord.ConstructorParamsArgumentSyntax = elementSyntax;
                }
            );

            return true;
        });

        IAttributeArgumentRecorder? tryMapNamedParameter(string parameterName, DataRecord dataRecord) => new AttributeArgumentRecorder((argument, syntax) =>
        {
            dataRecord.NamedArgument = argument;
            dataRecord.NamedArgumentSyntax = syntax.AsT0;

            return true;
        });
    }

    [SuppressMessage("Design", "CA1034: Nested types should not be visible", Justification = "Type should not be used elsewhere, but Moq requires it to be public.")]
    public sealed class DataRecord
    {
        public object? TypeArgument { get; set; }
        public object? ConstructorArgument { get; set; }
        public object? ConstructorParamsArgument { get; set; }
        public object? NamedArgument { get; set; }

        public ExpressionSyntax? TypeArgumentSyntax { get; set; }
        public ExpressionSyntax? ConstructorArgumentSyntax { get; set; }
        public IReadOnlyList<ExpressionSyntax>? ConstructorParamsArgumentSyntax { get; set; }
        public ExpressionSyntax? NamedArgumentSyntax { get; set; }
    }
}
