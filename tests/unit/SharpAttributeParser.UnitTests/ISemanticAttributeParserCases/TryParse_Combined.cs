namespace SharpAttributeParser.Tests.ISemanticAttributeParserCases;

using Microsoft.CodeAnalysis;

using System.Threading.Tasks;

using Xunit;

public class TryParse_Combined
{
    private static bool Target(ISemanticAttributeParser parser, ISemanticArgumentRecorder recorder, AttributeData attributeData) => parser.TryParse(recorder, attributeData);

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

        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);

        var result = Target(parser, recorder, attributeData);

        Assert.True(result);

        Assert.Equal(stringType, recorder.T1);
        Assert.Equal(intType, recorder.T2);
        Assert.Equal("42", recorder.Value!);
        Assert.Empty(recorder.ArrayValues!);
        Assert.Equal(new object[] { "42", 42 }, recorder.ParamsValues);
        Assert.Equal(intType, recorder.NamedValue);
        Assert.Equal(new object[] { 42, "42" }, recorder.NamedValues);
    }

    [Theory]
    [ClassData(typeof(Datasets.CombinedAttributeSources))]
    public async Task Combined_EmptyParams_RecorderPopulated(ISemanticAttributeParser parser, SemanticCombinedAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ArrayValues);
        Assert.Null(recorder.ParamsValues);
        Assert.Null(recorder.NamedValue);
        Assert.Null(recorder.NamedValues);

        var source = """
            [Combined<string, int>("42", new object[0], NamedValue = typeof(int), NamedValues = new object[] { 42, "42" })]
            public class Foo { }
            """;

        var (compilation, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);

        var result = Target(parser, recorder, attributeData);

        Assert.True(result);

        Assert.Equal(stringType, recorder.T1);
        Assert.Equal(intType, recorder.T2);
        Assert.Equal("42", recorder.Value!);
        Assert.Empty(recorder.ArrayValues!);
        Assert.Empty(recorder.ParamsValues!);
        Assert.Equal(intType, recorder.NamedValue);
        Assert.Equal(new object[] { 42, "42" }, recorder.NamedValues);
    }

    [Theory]
    [ClassData(typeof(Datasets.CombinedAttributeSources))]
    public async Task Combined_OneParamsElement_RecorderPopulated(ISemanticAttributeParser parser, SemanticCombinedAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ArrayValues);
        Assert.Null(recorder.ParamsValues);
        Assert.Null(recorder.NamedValue);
        Assert.Null(recorder.NamedValues);

        var source = """
            [Combined<string, int>("42", new object[0], 42, NamedValue = typeof(int), NamedValues = new object[] { 42, "42" })]
            public class Foo { }
            """;

        var (compilation, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);

        var result = Target(parser, recorder, attributeData);

        Assert.True(result);

        Assert.Equal(stringType, recorder.T1);
        Assert.Equal(intType, recorder.T2);
        Assert.Equal("42", recorder.Value!);
        Assert.Empty(recorder.ArrayValues!);
        Assert.Equal(new object[] { 42 }, recorder.ParamsValues!);
        Assert.Equal(intType, recorder.NamedValue);
        Assert.Equal(new object[] { 42, "42" }, recorder.NamedValues);
    }

    [Theory]
    [ClassData(typeof(Datasets.CombinedAttributeSources))]
    public async Task Combined_Array_RecorderPopulated(ISemanticAttributeParser parser, SemanticCombinedAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ArrayValues);
        Assert.Null(recorder.ParamsValues);
        Assert.Null(recorder.NamedValue);
        Assert.Null(recorder.NamedValues);

        var source = """
            [Combined<string, int>("42", new object[0], new object[] { "42", 42 }, NamedValue = typeof(int), NamedValues = new object[] { 42, "42" })]
            public class Foo { }
            """;

        var (compilation, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);

        var result = Target(parser, recorder, attributeData);

        Assert.True(result);

        Assert.Equal(stringType, recorder.T1);
        Assert.Equal(intType, recorder.T2);
        Assert.Equal("42", recorder.Value!);
        Assert.Empty(recorder.ArrayValues!);
        Assert.Equal(new object[] { "42", 42 }, recorder.ParamsValues);
        Assert.Equal(intType, recorder.NamedValue);
        Assert.Equal(new object[] { 42, "42" }, recorder.NamedValues);
    }
}
