namespace SharpAttributeParser.Tests.ISemanticAttributeParserCases;

using Microsoft.CodeAnalysis;

using System.Threading.Tasks;

using Xunit;

public class TryParse_Params
{
    private static bool Target(ISemanticAttributeParser parser, ISemanticArgumentRecorder recorder, AttributeData attributeData) => parser.TryParse(recorder, attributeData);

    [Theory]
    [ClassData(typeof(Datasets.ParamsAttributeSources))]
    public async Task Params_NullLiteral_True_RecorderPopulated(ISemanticAttributeParser parser, SemanticParamsAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);

        var source = """
            [Params(null)]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData);

        Assert.True(result);

        Assert.Null(recorder.Value);
        Assert.True(recorder.ValueRecorded);
    }

    [Theory]
    [ClassData(typeof(Datasets.ParamsAttributeSources))]
    public async Task Params_DefaultLiteral_True_RecorderPopulated(ISemanticAttributeParser parser, SemanticParamsAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);

        var source = """
            [Params(default)]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData);

        Assert.True(result);

        Assert.Null(recorder.Value);
        Assert.True(recorder.ValueRecorded);
    }

    [Theory]
    [ClassData(typeof(Datasets.ParamsAttributeSources))]
    public async Task Params_DefaultExpression_SameType_True_RecorderPopulated(ISemanticAttributeParser parser, SemanticParamsAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);

        var source = """
            [Params(default(object[]))]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData);

        Assert.True(result);

        Assert.Null(recorder.Value);
        Assert.True(recorder.ValueRecorded);
    }

    [Theory]
    [ClassData(typeof(Datasets.ParamsAttributeSources))]
    public async Task Params_DefaultExpression_DifferentType_True_RecorderPopulated(ISemanticAttributeParser parser, SemanticParamsAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);

        var source = """
            [Params(default(object))]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData);

        Assert.True(result);

        Assert.Equal(1, recorder.Value!.Count);
        Assert.Null(recorder.Value[0]);
        Assert.True(recorder.ValueRecorded);
    }

    [Theory]
    [ClassData(typeof(Datasets.ParamsAttributeSources))]
    public async Task Params_OneParamsValue_True_RecorderPopulated(ISemanticAttributeParser parser, SemanticParamsAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);

        var source = """
            [Params("42")]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData);

        Assert.True(result);

        Assert.Equal(1, recorder.Value!.Count);
        Assert.Equal("42", recorder.Value[0]);
    }

    [Theory]
    [ClassData(typeof(Datasets.ParamsAttributeSources))]
    public async Task Params_OneArrayValuedParamsValue_True_RecorderPopulated(ISemanticAttributeParser parser, SemanticParamsAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);

        var source = """
            [Params(new int[] { 4 })]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData);

        Assert.True(result);

        Assert.Equal(1, recorder.Value!.Count);
        Assert.Equal(new int[] { 4 }, recorder.Value[0]);
    }

    [Theory]
    [ClassData(typeof(Datasets.ParamsAttributeSources))]
    public async Task Params_MultipleParamsValues_True_RecorderPopulated(ISemanticAttributeParser parser, SemanticParamsAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);

        var source = """
            [Params("42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 })]
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
    [ClassData(typeof(Datasets.ParamsAttributeSources))]
    public async Task Params_ArrayValues_True_RecorderPopulated(ISemanticAttributeParser parser, SemanticParamsAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);

        var source = """
            [Params(new object[] { "42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 } })]
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
    [ClassData(typeof(Datasets.ParamsAttributeSources))]
    public async Task Params_Empty_True_RecorderPopulated(ISemanticAttributeParser parser, SemanticParamsAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);

        var source = """
            [Params]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData);

        Assert.True(result);

        Assert.Empty(recorder.Value!);
        Assert.True(recorder.ValueRecorded);
    }

    [Theory]
    [ClassData(typeof(Datasets.ParamsAttributeSources))]
    public async Task Params_Labelled_True_RecorderPopulated(ISemanticAttributeParser parser, SemanticParamsAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);

        var source = """
            [Params(value: new object[] { "42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 } })]
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
