namespace SharpAttributeParser.SyntacticAttributeRecorderFactoryCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Xunit;

public sealed class Create_Data
{
    private static ISyntacticAttributeRecorder<TData> Target<TData>(ISyntacticAttributeRecorderFactory factory, ISyntacticAttributeMapper<TData> mapper, TData dataRecord) => factory.Create(mapper, dataRecord);

    [Theory]
    [ClassData(typeof(FactorySources))]
    public void NullMapper_ArgumentNullException(ISyntacticAttributeRecorderFactory factory)
    {
        var exception = Record.Exception(() => Target(factory, null!, string.Empty));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(FactorySources))]
    public void NullDataRecord_ArgumentNullException(ISyntacticAttributeRecorderFactory factory)
    {
        var exception = Record.Exception(() => Target(factory, Mock.Of<ISyntacticAttributeMapper<string>>(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(FactorySources))]
    public void Valid_ProducedRecorderUsesProvidedMapperAndDataRecord(ISyntacticAttributeRecorderFactory factory)
    {
        DataRecord dataRecord = new();

        var typeParameter = Mock.Of<ITypeParameterSymbol>();
        var constructorParameter = Mock.Of<IParameterSymbol>();
        var namedParameter = string.Empty;

        var typeArgument = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
        var constructorArgument = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
        var constructorParamsArgument = Array.Empty<ExpressionSyntax>();
        var namedArgument = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);

        Mock<ISyntacticAttributeMapper<DataRecord>> mapperMock = new();

        mapperMock.Setup(static (mapper) => mapper.TryMapTypeParameter(It.IsAny<ITypeParameterSymbol>(), It.IsAny<DataRecord>())).Returns<ITypeParameterSymbol, DataRecord>(tryMapTypeParameter);
        mapperMock.Setup(static (mapper) => mapper.TryMapConstructorParameter(It.IsAny<IParameterSymbol>(), It.IsAny<DataRecord>())).Returns<IParameterSymbol, DataRecord>(tryMapConstructorParameter);
        mapperMock.Setup(static (mapper) => mapper.TryMapNamedParameter(It.IsAny<string>(), It.IsAny<DataRecord>())).Returns<string, DataRecord>(tryMapNamedParameter);

        var recorder = Target(factory, mapperMock.Object, dataRecord);

        recorder.TryRecordTypeArgumentSyntax(typeParameter, typeArgument);
        recorder.TryRecordConstructorArgumentSyntax(constructorParameter, constructorArgument);
        recorder.TryRecordConstructorParamsArgumentSyntax(constructorParameter, constructorParamsArgument);
        recorder.TryRecordNamedArgumentSyntax(namedParameter, namedArgument);

        var result = recorder.GetRecord();

        Assert.Equal(dataRecord, result);

        mapperMock.Verify((mapper) => mapper.TryMapTypeParameter(typeParameter, dataRecord), Times.AtLeastOnce);
        mapperMock.Verify((mapper) => mapper.TryMapConstructorParameter(constructorParameter, dataRecord), Times.AtLeast(2));
        mapperMock.Verify((mapper) => mapper.TryMapNamedParameter(namedParameter, dataRecord), Times.AtLeastOnce);

        Assert.Equal(typeArgument, result.TypeArgumentSyntax, ReferenceEqualityComparer.Instance);
        Assert.Equal(constructorArgument, result.ConstructorArgumentSyntax, ReferenceEqualityComparer.Instance);
        Assert.Equal<IReadOnlyList<ExpressionSyntax>>(constructorParamsArgument, result.ConstructorParamsArgumentSyntax, ReferenceEqualityComparer.Instance);
        Assert.Equal(namedArgument, result.NamedArgumentSyntax, ReferenceEqualityComparer.Instance);

        ISyntacticAttributeArgumentRecorder? tryMapTypeParameter(ITypeParameterSymbol parameter, DataRecord dataRecord) => new SyntacticAttributeArgumentRecorder((syntax) =>
        {
            dataRecord.TypeArgumentSyntax = syntax.AsT0;

            return true;
        });

        ISyntacticAttributeArgumentRecorder? tryMapConstructorParameter(IParameterSymbol parameter, DataRecord dataRecord) => new SyntacticAttributeArgumentRecorder((syntax) =>
        {
            syntax.Switch
            (
                (syntax) => dataRecord.ConstructorArgumentSyntax = syntax,
                (elementSyntax) => dataRecord.ConstructorParamsArgumentSyntax = elementSyntax
            );

            return true;
        });

        ISyntacticAttributeArgumentRecorder? tryMapNamedParameter(string parameterName, DataRecord dataRecord) => new SyntacticAttributeArgumentRecorder((syntax) =>
        {
            dataRecord.NamedArgumentSyntax = syntax.AsT0;

            return true;
        });
    }

    [SuppressMessage("Design", "CA1034: Nested types should not be visible", Justification = "Type should not be used elsewhere, but Moq requires it to be public.")]
    public sealed class DataRecord
    {
        public ExpressionSyntax? TypeArgumentSyntax { get; set; }
        public ExpressionSyntax? ConstructorArgumentSyntax { get; set; }
        public IReadOnlyList<ExpressionSyntax>? ConstructorParamsArgumentSyntax { get; set; }
        public ExpressionSyntax? NamedArgumentSyntax { get; set; }
    }
}
