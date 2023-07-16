namespace SharpAttributeParser.AttributeRecorderFactoryCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Xunit;

public sealed class Create_Record_RecordBuilder
{
    private static IAttributeRecorder<TRecord> Target<TRecord, TRecordBuilder>(IAttributeRecorderFactory factory, IAttributeMapper<TRecordBuilder> mapper, TRecordBuilder recordBuilder) where TRecordBuilder : IRecordBuilder<TRecord> => factory.Create<TRecord, TRecordBuilder>(mapper, recordBuilder);

    [Theory]
    [ClassData(typeof(FactorySources))]
    public void NullMapper_ArgumentNullException(IAttributeRecorderFactory factory)
    {
        var exception = Record.Exception(() => Target<string, IRecordBuilder<string>>(factory, null!, Mock.Of<IRecordBuilder<string>>()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(FactorySources))]
    public void NullDataRecord_ArgumentNullException(IAttributeRecorderFactory factory)
    {
        var exception = Record.Exception(() => Target<string, IRecordBuilder<string>>(factory, Mock.Of<IAttributeMapper<IRecordBuilder<string>>>(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(FactorySources))]
    public void Valid_ProducedRecorderUsesProvidedMapperAndDataRecord(IAttributeRecorderFactory factory)
    {
        DataRecord dataRecord = new();
        DataRecordBuilder dataRecordBuilder = new(dataRecord);

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

        Mock<IAttributeMapper<DataRecordBuilder>> mapperMock = new();

        mapperMock.Setup(static (mapper) => mapper.TryMapTypeParameter(It.IsAny<ITypeParameterSymbol>(), It.IsAny<DataRecordBuilder>())).Returns<ITypeParameterSymbol, DataRecordBuilder>(tryMapTypeParameter);
        mapperMock.Setup(static (mapper) => mapper.TryMapConstructorParameter(It.IsAny<IParameterSymbol>(), It.IsAny<DataRecordBuilder>())).Returns<IParameterSymbol, DataRecordBuilder>(tryMapConstructorParameter);
        mapperMock.Setup(static (mapper) => mapper.TryMapNamedParameter(It.IsAny<string>(), It.IsAny<DataRecordBuilder>())).Returns<string, DataRecordBuilder>(tryMapNamedParameter);

        var recorder = Target<DataRecord, DataRecordBuilder>(factory, mapperMock.Object, dataRecordBuilder);

        recorder.TryRecordTypeArgument(typeParameter, typeArgument, typeArgumentSyntax);
        recorder.TryRecordConstructorArgument(constructorParameter, constructorArgument, constructorArgumentSyntax);
        recorder.TryRecordConstructorParamsArgument(constructorParameter, constructorParamsArgument, constructorParamsArgumentSyntax);
        recorder.TryRecordNamedArgument(namedParameter, namedArgument, namedArgumentSyntax);

        var result = recorder.GetRecord();

        Assert.Equal(dataRecord, result);

        mapperMock.Verify((mapper) => mapper.TryMapTypeParameter(typeParameter, dataRecordBuilder), Times.AtLeastOnce);
        mapperMock.Verify((mapper) => mapper.TryMapConstructorParameter(constructorParameter, dataRecordBuilder), Times.AtLeast(2));
        mapperMock.Verify((mapper) => mapper.TryMapNamedParameter(namedParameter, dataRecordBuilder), Times.AtLeastOnce);

        Assert.Equal(typeArgument, result.TypeArgument);
        Assert.Equal(constructorArgument, result.ConstructorArgument);
        Assert.Equal(constructorParamsArgument, result.ConstructorParamsArgument);
        Assert.Equal(namedArgument, result.NamedArgument);

        Assert.Equal(typeArgumentSyntax, result.TypeArgumentSyntax);
        Assert.Equal(constructorArgumentSyntax, result.ConstructorArgumentSyntax);
        Assert.Equal<IReadOnlyList<ExpressionSyntax>>(constructorParamsArgumentSyntax, result.ConstructorParamsArgumentSyntax);
        Assert.Equal(namedArgumentSyntax, result.NamedArgumentSyntax);

        IAttributeArgumentRecorder? tryMapTypeParameter(ITypeParameterSymbol parameter, DataRecordBuilder recordBuilder) => new AttributeArgumentRecorder((argument, syntax) =>
        {
            recordBuilder.WithTypeArgument(argument, syntax.AsT0);

            return true;
        });

        IAttributeConstructorArgumentRecorder? tryMapConstructorParameter(IParameterSymbol parameter, DataRecordBuilder recordBuilder) => new AttributeArgumentRecorder((argument, syntax) =>
        {
            syntax.Switch
            (
                (syntax) => recordBuilder.WithConstructorArgument(argument, syntax),
                (elementSyntax) => recordBuilder.WithConstructorParamsArgument(argument, elementSyntax)
            );

            return true;
        });

        IAttributeArgumentRecorder? tryMapNamedParameter(string parameterName, DataRecordBuilder recordBuilder) => new AttributeArgumentRecorder((argument, syntax) =>
        {
            recordBuilder.WithNamedArgument(argument, syntax.AsT0);

            return true;
        });
    }

    [SuppressMessage("Design", "CA1034: Nested types should not be visible", Justification = "Type should not be used elsewhere, but DataRecordBuilder requires it to be public.")]
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

    [SuppressMessage("Design", "CA1034: Nested types should not be visible", Justification = "Type should not be used elsewhere, but Moq requires it to be public.")]
    public sealed class DataRecordBuilder : IRecordBuilder<DataRecord>
    {
        private DataRecord BuildTarget { get; }

        public DataRecordBuilder(DataRecord buildTarget)
        {
            BuildTarget = buildTarget;
        }

        public void WithTypeArgument(object? argument, ExpressionSyntax syntax)
        {
            BuildTarget.TypeArgument = argument;
            BuildTarget.TypeArgumentSyntax = syntax;
        }

        public void WithConstructorArgument(object? argument, ExpressionSyntax syntax)
        {
            BuildTarget.ConstructorArgument = argument;
            BuildTarget.ConstructorArgumentSyntax = syntax;
        }

        public void WithConstructorParamsArgument(object? argument, IReadOnlyList<ExpressionSyntax> elementSyntax)
        {
            BuildTarget.ConstructorParamsArgument = argument;
            BuildTarget.ConstructorParamsArgumentSyntax = elementSyntax;
        }

        public void WithNamedArgument(object? argument, ExpressionSyntax syntax)
        {
            BuildTarget.NamedArgument = argument;
            BuildTarget.NamedArgumentSyntax = syntax;
        }

        DataRecord IRecordBuilder<DataRecord>.Build() => BuildTarget;
    }
}
