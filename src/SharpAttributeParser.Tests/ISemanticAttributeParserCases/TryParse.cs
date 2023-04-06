namespace SharpAttributeParser.Tests.ISemanticAttributeParserCases;

using Microsoft.CodeAnalysis;

using Moq;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xunit;

public class TryParse
{
    private static bool Target(ISemanticAttributeParser parser, ISemanticArgumentRecorder recorder, AttributeData attributeData) => parser.TryParse(recorder, attributeData);

    [Theory]
    [ClassData(typeof(Datasets.ParserSources))]
    public async Task NullRecorder_ArgumentNullException(ISemanticAttributeParser parser)
    {
        var source = """
            [Generic<string>]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var recorder = Datasets.GetNullRecorder();

        var exception = Record.Exception(() => Target(parser, recorder, attributeData));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(Datasets.GenericAttributeSources))]
    public void NullAttributeData_ArgumentNullException(ISemanticAttributeParser parser, ISemanticArgumentRecorder recorder)
    {
        var attributeData = Datasets.GetNullAttributeData();

        var exception = Record.Exception(() => Target(parser, recorder, attributeData));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(Datasets.GenericAttributeSources))]
    public async Task NonExistingAttribute_False_RecorderNotPopulated(ISemanticAttributeParser parser, SemanticGenericAttributeRecorder recorder)
    {
        var source = """
            [NonExisting]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData);

        Assert.False(result);

        Assert.Null(recorder.T);
    }

    [Theory]
    [ClassData(typeof(Datasets.GenericAttributeSources))]
    public async Task NonExistingConstructor_False_RecorderNotPopulated(ISemanticAttributeParser parser, SemanticGenericAttributeRecorder recorder)
    {
        var source = """
            [Generic<string>(4)]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData);

        Assert.False(result);

        Assert.Null(recorder.T);
    }

    [Theory]
    [ClassData(typeof(Datasets.SingleConstructorAttributeSources))]
    public async Task SyntacticalError_False_RecorderNotPopulated(ISemanticAttributeParser parser, SemanticSingleConstructorAttributeRecorder recorder)
    {
        var source = """
            [SingleConstructor(4?)]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData);

        Assert.False(result);

        Assert.Null(recorder.Value);
    }

    [Theory]
    [ClassData(typeof(Datasets.ParserSources))]
    public async Task Generic_FalseReturningRecorder_False(ISemanticAttributeParser parser)
    {
        Mock<ISemanticArgumentRecorder> recorderMock = new();

        recorderMock.Setup(static (recorder) => recorder.TryRecordGenericArgument(It.IsAny<ITypeParameterSymbol>(), It.IsAny<ITypeSymbol>())).Returns(false);

        var source = """
            [Generic<string>]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorderMock.Object, attributeData);

        Assert.False(result);
    }

    [Theory]
    [ClassData(typeof(Datasets.GenericAttributeSources))]
    public async Task Generic_WithoutBody_True_RecorderPopulated(ISemanticAttributeParser parser, SemanticGenericAttributeRecorder recorder)
    {
        Assert.Null(recorder.T);

        var source = """
            [Generic<string>]
            public class Foo { }
            """;

        var (compilation, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var diag = compilation.GetDiagnostics();

        Assert.Empty(diag);

        var expectedType = compilation.GetSpecialType(SpecialType.System_String);

        var result = Target(parser, recorder, attributeData);

        Assert.True(result);

        Assert.Equal(expectedType, recorder.T);
    }

    [Theory]
    [ClassData(typeof(Datasets.GenericAttributeSources))]
    public async Task Generic_WithBody_True_RecorderPopulated(ISemanticAttributeParser parser, SemanticGenericAttributeRecorder recorder)
    {
        Assert.Null(recorder.T);

        var source = """
            [Generic<string>()]
            public class Foo { }
            """;

        var (compilation, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var expectedType = compilation.GetSpecialType(SpecialType.System_String);

        var result = Target(parser, recorder, attributeData);

        Assert.True(result);

        Assert.Equal(expectedType, recorder.T);
    }

    [Theory]
    [ClassData(typeof(Datasets.GenericAttributeSources))]
    public async Task Generic_Tuple_True_RecorderPopulated(ISemanticAttributeParser parser, SemanticGenericAttributeRecorder recorder)
    {
        Assert.Null(recorder.T);

        var source = """
            [Generic<(string, int)>]
            public class Foo { }
            """;

        var (compilation, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);

        var expectedType = compilation.GetTypeByMetadataName("System.ValueTuple`2")!.Construct(stringType, intType);

        var result = Target(parser, recorder, attributeData);

        Assert.True(result);

        Assert.Equal(expectedType, recorder.T);
    }

    [Theory]
    [ClassData(typeof(Datasets.ParserSources))]
    public async Task ConstructorSingle_FalseReturningRecorder_False(ISemanticAttributeParser parser)
    {
        Mock<ISemanticArgumentRecorder> recorderMock = new();

        recorderMock.Setup(static (recorder) => recorder.TryRecordConstructorArgument(It.IsAny<IParameterSymbol>(), It.IsAny<object?>())).Returns(false);

        var source = """
            [SingleConstructor(null)]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorderMock.Object, attributeData);

        Assert.False(result);
    }

    [Theory]
    [ClassData(typeof(Datasets.SingleConstructorAttributeSources))]
    public async Task ConstructorSingle_NullLiteral_True_RecorderPopulated(ISemanticAttributeParser parser, SemanticSingleConstructorAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);

        var source = """
            [SingleConstructor(null)]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData);

        Assert.True(result);

        Assert.Null(recorder.Value);
        Assert.True(recorder.ValueRecorded);
    }

    [Theory]
    [ClassData(typeof(Datasets.SingleConstructorAttributeSources))]
    public async Task ConstructorSingle_Value_True_RecorderPopulated(ISemanticAttributeParser parser, SemanticSingleConstructorAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);

        var source = """
            [SingleConstructor("42")]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData);

        Assert.True(result);

        Assert.Equal("42", recorder.Value);
    }

    [Theory]
    [ClassData(typeof(Datasets.ParserSources))]
    public async Task ConstructorArray_FalseReturningRecorder_False(ISemanticAttributeParser parser)
    {
        Mock<ISemanticArgumentRecorder> recorderMock = new();

        recorderMock.Setup(static (recorder) => recorder.TryRecordConstructorArgument(It.IsAny<IParameterSymbol>(), It.IsAny<IReadOnlyList<object?>?>())).Returns(false);

        var source = """
            [ArrayConstructor(null)]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorderMock.Object, attributeData);

        Assert.False(result);
    }

    [Theory]
    [ClassData(typeof(Datasets.ArrayConstructorAttributeSources))]
    public async Task ConstructorArray_NullLiteral_True_RecorderPopulated(ISemanticAttributeParser parser, SemanticArrayConstructorAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);

        var source = """
            [ArrayConstructor(null)]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData);

        Assert.True(result);

        Assert.Null(recorder.Value);
        Assert.True(recorder.ValueRecorded);
    }

    [Theory]
    [ClassData(typeof(Datasets.ArrayConstructorAttributeSources))]
    public async Task ConstructorArray_Values_True_RecorderPopulated(ISemanticAttributeParser parser, SemanticArrayConstructorAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);

        var source = """
            [ArrayConstructor(new object[] { "42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 } })]
            public class Foo { }
            """;

        var (compilation, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var intType = compilation.GetSpecialType(SpecialType.System_Int32);

        var result = Target(parser, recorder, attributeData);

        Assert.True(result);

        Assert.Equal(7, recorder.Value!.Count);
        Assert.Equal("42", recorder.Value[0]);
        Assert.Null(recorder.Value[1]);
        Assert.Equal(intType, recorder.Value[2]);
        Assert.Equal("Foo", recorder.Value[3]);
        Assert.Equal(42, recorder.Value[4]);
        Assert.Equal((double)42, recorder.Value[5]);
        Assert.Equal(new object[] { "42", 42 }, (object?[])recorder.Value[6]!);
    }

    [Theory]
    [ClassData(typeof(Datasets.ParserSources))]
    public async Task NamedSingle_FalseReturningRecorder_False(ISemanticAttributeParser parser)
    {
        Mock<ISemanticArgumentRecorder> recorderMock = new();

        recorderMock.Setup(static (recorder) => recorder.TryRecordNamedArgument(It.IsAny<string>(), It.IsAny<object?>())).Returns(false);

        var source = """
            [Named(Value = null)]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorderMock.Object, attributeData);

        Assert.False(result);
    }

    [Theory]
    [ClassData(typeof(Datasets.NamedAttributeSources))]
    public async Task NamedSingle_NullLiteral_True_RecorderPopulated(ISemanticAttributeParser parser, SemanticNamedAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);

        var source = """
            [Named(Value = null)]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData);

        Assert.True(result);

        Assert.Null(recorder.Value);
        Assert.True(recorder.ValueRecorded);
    }

    [Theory]
    [ClassData(typeof(Datasets.NamedAttributeSources))]
    public async Task NamedSingle_Value_True_RecorderPopulated(ISemanticAttributeParser parser, SemanticNamedAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);

        var source = """
            [Named(Value = "42")]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData);

        Assert.True(result);

        Assert.Equal("42", recorder.Value);
    }

    [Theory]
    [ClassData(typeof(Datasets.NamedAttributeSources))]
    public async Task NamedSingle_NonExisting_True_RecorderPopulated(ISemanticAttributeParser parser, SemanticNamedAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);

        var source = """
            [Named(NonExisting = 42, Value = "42")]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData);

        Assert.True(result);

        Assert.Equal("42", recorder.Value);
    }

    [Theory]
    [ClassData(typeof(Datasets.ParserSources))]
    public async Task NamedArray_FalseReturningRecorder_False(ISemanticAttributeParser parser)
    {
        Mock<ISemanticArgumentRecorder> recorderMock = new();

        recorderMock.Setup(static (recorder) => recorder.TryRecordNamedArgument(It.IsAny<string>(), It.IsAny<IReadOnlyList<object?>?>())).Returns(false);

        var source = """
            [Named(Values = null)]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorderMock.Object, attributeData);

        Assert.False(result);
    }

    [Theory]
    [ClassData(typeof(Datasets.NamedAttributeSources))]
    public async Task NamedArray_NullLiteral_True_RecorderPopulated(ISemanticAttributeParser parser, SemanticNamedAttributeRecorder recorder)
    {
        Assert.Null(recorder.Values);

        var source = """
            [Named(Values = null)]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData);

        Assert.True(result);

        Assert.Null(recorder.Values);
        Assert.True(recorder.ValuesRecorded);
    }

    [Theory]
    [ClassData(typeof(Datasets.NamedAttributeSources))]
    public async Task NamedArray_Values_True_RecorderPopulated(ISemanticAttributeParser parser, SemanticNamedAttributeRecorder recorder)
    {
        Assert.Null(recorder.Values);

        var source = """
            [Named(Values = new object[] { "42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 } })]
            public class Foo { }
            """;

        var (compilation, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var intType = compilation.GetSpecialType(SpecialType.System_Int32);

        var result = Target(parser, recorder, attributeData);

        Assert.True(result);

        Assert.Equal(7, recorder.Values!.Count);
        Assert.Equal("42", recorder.Values[0]);
        Assert.Null(recorder.Values[1]);
        Assert.Equal(intType, recorder.Values[2]);
        Assert.Equal("Foo", recorder.Values[3]);
        Assert.Equal(42, recorder.Values[4]);
        Assert.Equal((double)42, recorder.Values[5]);
        Assert.Equal(new object[] { "42", 42 }, (object?[])recorder.Values[6]!);
    }

    [Theory]
    [ClassData(typeof(Datasets.CombinedAttributeSources))]
    public async Task Combined_Params_RecorderPopulated(ISemanticAttributeParser parser, SemanticCombinedAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ArrayValues);
        Assert.Null(recorder.ParamsValues);
        Assert.Null(recorder.NamedValue);
        Assert.Null(recorder.NamedValues);

        var source = """
            [Combined<string, int>("42", new object[0], "42", 42, NamedValue = typeof(int), NamedValues = new object[] { 42, "42" })]
            public class Foo { }
            """;

        var (compilation, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var intType = compilation.GetSpecialType(SpecialType.System_Int32);

        var result = Target(parser, recorder, attributeData);

        Assert.True(result);

        Assert.Equal("42", recorder.Value!);
        Assert.Empty(recorder.ArrayValues!);
        Assert.Equal(new object[] { "42", 42 }, recorder.ParamsValues);
        Assert.Equal(intType, recorder.NamedValue);
        Assert.Equal(new object[] { 42, "42" }, recorder.NamedValues);
    }

    [Theory]
    [ClassData(typeof(Datasets.CombinedAttributeSources))]
    public async Task Combined_Array_RecorderPopulated(ISemanticAttributeParser parser, SemanticCombinedAttributeRecorder recoreder)
    {
        Assert.Null(recoreder.Value);
        Assert.Null(recoreder.ArrayValues);
        Assert.Null(recoreder.ParamsValues);
        Assert.Null(recoreder.NamedValue);
        Assert.Null(recoreder.NamedValues);

        var source = """
            [Combined<string, int>("42", new object[0], new object[] { "42", 42 }, NamedValue = typeof(int), NamedValues = new object[] { 42, "42" })]
            public class Foo { }
            """;

        var (compilation, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var intType = compilation.GetSpecialType(SpecialType.System_Int32);

        var result = Target(parser, recoreder, attributeData);

        Assert.True(result);

        Assert.Equal("42", recoreder.Value!);
        Assert.Empty(recoreder.ArrayValues!);
        Assert.Equal(new object[] { "42", 42 }, recoreder.ParamsValues);
        Assert.Equal(intType, recoreder.NamedValue);
        Assert.Equal(new object[] { 42, "42" }, recoreder.NamedValues);
    }
}
