namespace SharpAttributeParser.SyntacticAttributeRecorderFactoryCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Xunit;

public sealed class Create_Data_DataBuilder
{
    private static ISyntacticAttributeRecorder<TData> Target<TData, TDataBuilder>(ISyntacticAttributeRecorderFactory factory, ISyntacticAttributeMapper<TDataBuilder> mapper, TDataBuilder dataBuilder) where TDataBuilder : IRecordBuilder<TData> => factory.Create<TData, TDataBuilder>(mapper, dataBuilder);

    [Theory]
    [ClassData(typeof(FactorySources))]
    public void NullMapper_ArgumentNullException(ISyntacticAttributeRecorderFactory factory)
    {
        var exception = Record.Exception(() => Target<string, IRecordBuilder<string>>(factory, null!, Mock.Of<IRecordBuilder<string>>()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(FactorySources))]
    public void NullDataRecord_ArgumentNullException(ISyntacticAttributeRecorderFactory factory)
    {
        var exception = Record.Exception(() => Target<string, IRecordBuilder<string>>(factory, Mock.Of<ISyntacticAttributeMapper<IRecordBuilder<string>>>(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(FactorySources))]
    public void Valid_ProducedRecorderUsesProvidedMapperAndDataRecord(ISyntacticAttributeRecorderFactory factory)
    {
        DataRecord dataRecord = new();
        DataRecordBuilder dataRecordBuilder = new(dataRecord);

        var typeParameter = Mock.Of<ITypeParameterSymbol>();
        var constructorParameter = Mock.Of<IParameterSymbol>();
        var namedParameter = string.Empty;

        var typeArgument = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
        var constructorArgument = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
        var constructorParamsArgument = Array.Empty<ExpressionSyntax>();
        var namedArgument = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);

        Mock<ISyntacticAttributeMapper<DataRecordBuilder>> mapperMock = new();

        mapperMock.Setup(static (mapper) => mapper.TryMapTypeParameter(It.IsAny<ITypeParameterSymbol>(), It.IsAny<DataRecordBuilder>())).Returns<ITypeParameterSymbol, DataRecordBuilder>(tryMapTypeParameter);
        mapperMock.Setup(static (mapper) => mapper.TryMapConstructorParameter(It.IsAny<IParameterSymbol>(), It.IsAny<DataRecordBuilder>())).Returns<IParameterSymbol, DataRecordBuilder>(tryMapConstructorParameter);
        mapperMock.Setup(static (mapper) => mapper.TryMapNamedParameter(It.IsAny<string>(), It.IsAny<DataRecordBuilder>())).Returns<string, DataRecordBuilder>(tryMapNamedParameter);

        var recorder = Target<DataRecord, DataRecordBuilder>(factory, mapperMock.Object, dataRecordBuilder);

        recorder.TryRecordTypeArgumentSyntax(typeParameter, typeArgument);
        recorder.TryRecordConstructorArgumentSyntax(constructorParameter, constructorArgument);
        recorder.TryRecordConstructorParamsArgumentSyntax(constructorParameter, constructorParamsArgument);
        recorder.TryRecordNamedArgumentSyntax(namedParameter, namedArgument);

        var result = recorder.GetRecord();

        Assert.Equal(dataRecord, result);

        mapperMock.Verify((mapper) => mapper.TryMapTypeParameter(typeParameter, dataRecordBuilder), Times.AtLeastOnce);
        mapperMock.Verify((mapper) => mapper.TryMapConstructorParameter(constructorParameter, dataRecordBuilder), Times.AtLeast(2));
        mapperMock.Verify((mapper) => mapper.TryMapNamedParameter(namedParameter, dataRecordBuilder), Times.AtLeastOnce);

        Assert.Equal(typeArgument, result.TypeArgumentSyntax, ReferenceEqualityComparer.Instance);
        Assert.Equal(constructorArgument, result.ConstructorArgumentSyntax, ReferenceEqualityComparer.Instance);
        Assert.Equal<IReadOnlyList<ExpressionSyntax>>(constructorParamsArgument, result.ConstructorParamsArgumentSyntax, ReferenceEqualityComparer.Instance);
        Assert.Equal(namedArgument, result.NamedArgumentSyntax, ReferenceEqualityComparer.Instance);

        ISyntacticAttributeArgumentRecorder? tryMapTypeParameter(ITypeParameterSymbol parameter, DataRecordBuilder dataBuilder) => new SyntacticAttributeArgumentRecorder((syntax) =>
        {
            dataRecord.TypeArgumentSyntax = syntax.AsT0;

            return true;
        });

        ISyntacticAttributeConstructorArgumentRecorder? tryMapConstructorParameter(IParameterSymbol parameter, DataRecordBuilder dataBuilder) => new SyntacticAttributeArgumentRecorder((syntax) =>
        {
            syntax.Switch
            (
                (syntax) => dataRecord.ConstructorArgumentSyntax = syntax,
                (elementSyntax) => dataRecord.ConstructorParamsArgumentSyntax = elementSyntax
            );

            return true;
        });

        ISyntacticAttributeArgumentRecorder? tryMapNamedParameter(string parameterName, DataRecordBuilder dataBuilder) => new SyntacticAttributeArgumentRecorder((syntax) =>
        {
            dataRecord.NamedArgumentSyntax = syntax.AsT0;

            return true;
        });
    }

    [SuppressMessage("Design", "CA1034: Nested types should not be visible", Justification = "Type should not be used elsewhere, but DataRecordBuilder requires it to be public.")]
    public sealed class DataRecord
    {
        public ExpressionSyntax? TypeArgumentSyntax { get; set; }
        public ExpressionSyntax? ConstructorArgumentSyntax { get; set; }
        public IReadOnlyList<ExpressionSyntax>? ConstructorParamsArgumentSyntax { get; set; }
        public ExpressionSyntax? NamedArgumentSyntax { get; set; }
    }

    [SuppressMessage("Design", "CA1034: Nested types should not be visible", Justification = "Type should not be used elsewhere, but Moq requires it to be public.")]
    public sealed class DataRecordBuilder : IRecordBuilder<DataRecord>
    {
        private DataRecord BuildTarget { get; }

        public DataRecordBuilder(DataRecord buildTarget)
        {
            BuildTarget = buildTarget;
        }

        public void WithTypeArgumentSyntax(ExpressionSyntax syntax) => BuildTarget.TypeArgumentSyntax = syntax;
        public void WithConstructorArgumentSyntax(ExpressionSyntax syntax) => BuildTarget.ConstructorArgumentSyntax = syntax;
        public void WithConstructorParamsArgumentSyntax(IReadOnlyList<ExpressionSyntax> elementSyntax) => BuildTarget.ConstructorParamsArgumentSyntax = elementSyntax;
        public void WithNamedArgumentSyntax(ExpressionSyntax syntax) => BuildTarget.NamedArgumentSyntax = syntax;

        DataRecord IRecordBuilder<DataRecord>.Build() => BuildTarget;
    }
}
