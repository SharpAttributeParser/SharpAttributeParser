namespace SharpAttributeParser.Tests.ISemanticAttributeParserCases;

using Microsoft.CodeAnalysis;

using Moq;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Xunit;

public class TryParse_ConstructorArray
{
    private static bool Target(ISemanticAttributeParser parser, ISemanticArgumentRecorder recorder, AttributeData attributeData) => parser.TryParse(recorder, attributeData);

    [Theory]
    [ClassData(typeof(Datasets.ParserSources))]
    [SuppressMessage("Minor Code Smell", "S1125: Boolean literals should not be redundant", Justification = "Prefer equality check over negation.")]
    public async Task ConstructorArray_FalseReturningRecorder_False(ISemanticAttributeParser parser)
    {
        var recorder = Mock.Of<ISemanticArgumentRecorder>(static (recorder) => recorder.TryRecordConstructorArgument(It.IsAny<IParameterSymbol>(), It.IsAny<IReadOnlyList<object?>?>()) == false);

        var source = """
            [ArrayConstructor(null)]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData);

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
    public async Task ConstructorArray_DefaultLiteral_True_RecorderPopulated(ISemanticAttributeParser parser, SemanticArrayConstructorAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);

        var source = """
            [ArrayConstructor(default)]
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
    public async Task ConstructorArray_DefaultExpression_True_RecorderPopulated(ISemanticAttributeParser parser, SemanticArrayConstructorAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);

        var source = """
            [ArrayConstructor(default(object[]))]
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
    [ClassData(typeof(Datasets.ArrayConstructorAttributeSources))]
    public async Task ConstructorArray_Empty_True_RecorderPopulated(ISemanticAttributeParser parser, SemanticArrayConstructorAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);

        var source = """
            [ArrayConstructor(new object[] { })]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData);

        Assert.True(result);

        Assert.Empty(recorder.Value!);
    }

    [Theory]
    [ClassData(typeof(Datasets.ArrayConstructorAttributeSources))]
    public async Task ConstructorArray_Labelled_True_RecorderPopulated(ISemanticAttributeParser parser, SemanticArrayConstructorAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);

        var source = """
            [ArrayConstructor(value: new object[] { "42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 } })]
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
}
