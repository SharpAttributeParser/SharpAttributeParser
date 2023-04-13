namespace SharpAttributeParser.Tests.ISyntacticAttributeParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.Threading.Tasks;

using Xunit;

public class TryParse_Params
{
    private static bool Target(ISyntacticAttributeParser parser, ISyntacticArgumentRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => parser.TryParse(recorder, attributeData, attributeSyntax);

    [Theory]
    [ClassData(typeof(Datasets.ParamsAttributeSources))]
    public async Task Params_NullLiteral_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticParamsAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueCollectionLocation);
        Assert.Null(recorder.ValueElementLocations);

        var source = """
            [Params(null)]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var (expectedCollectionLocation, expectedElementLocations) = ExpectedLocation.ArrayArgument(attributeSyntax, 0);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Null(recorder.Value);
        Assert.Equal(expectedCollectionLocation, recorder.ValueCollectionLocation);
        Assert.Equal(expectedElementLocations, recorder.ValueElementLocations);
    }

    [Theory]
    [ClassData(typeof(Datasets.ParamsAttributeSources))]
    public async Task Params_DefaultLiteral_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticParamsAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueCollectionLocation);
        Assert.Null(recorder.ValueElementLocations);

        var source = """
            [Params(default)]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var (expectedCollectionLocation, expectedElementLocations) = ExpectedLocation.ArrayArgument(attributeSyntax, 0);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Null(recorder.Value);
        Assert.Equal(expectedCollectionLocation, recorder.ValueCollectionLocation);
        Assert.Equal(expectedElementLocations, recorder.ValueElementLocations);
    }

    [Theory]
    [ClassData(typeof(Datasets.ParamsAttributeSources))]
    public async Task Params_DefaultExpression_SameType_NotTreatedAsParams_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticParamsAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueCollectionLocation);
        Assert.Null(recorder.ValueElementLocations);

        var source = """
            [Params(default(object[]))]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var (expectedCollectionLocation, expectedElementLocations) = ExpectedLocation.ArrayArgument(attributeSyntax, 0);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Null(recorder.Value);
        Assert.Equal(expectedCollectionLocation, recorder.ValueCollectionLocation);
        Assert.Equal(expectedElementLocations, recorder.ValueElementLocations);
    }

    [Theory]
    [ClassData(typeof(Datasets.ParamsAttributeSources))]
    public async Task Params_DefaultExpression_DifferentType_TreatedAsParams_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticParamsAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueCollectionLocation);
        Assert.Null(recorder.ValueElementLocations);

        var source = """
            [Params(default(object))]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var (expectedCollectionLocation, expectedElementLocations) = ExpectedLocation.ParamsArgument(attributeSyntax, 0, 1);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Equal(1, recorder.Value!.Count);
        Assert.Null(recorder.Value[0]);
        Assert.Equal(expectedCollectionLocation, recorder.ValueCollectionLocation);
        Assert.Equal(expectedElementLocations, recorder.ValueElementLocations);
    }

    [Theory]
    [ClassData(typeof(Datasets.ParamsAttributeSources))]
    public async Task Params_OneParamsValue_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticParamsAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueCollectionLocation);
        Assert.Null(recorder.ValueElementLocations);

        var source = """
            [Params("42")]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var (expectedCollectionLocation, expectedElementLocations) = ExpectedLocation.ParamsArgument(attributeSyntax, 0, 1);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Equal(1, recorder.Value!.Count);
        Assert.Equal("42", recorder.Value[0]);

        Assert.Equal(expectedCollectionLocation, recorder.ValueCollectionLocation);
        Assert.Equal(expectedElementLocations, recorder.ValueElementLocations);
    }

    [Theory]
    [ClassData(typeof(Datasets.ParamsAttributeSources))]
    public async Task Params_MultipleParamsValues_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticParamsAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueCollectionLocation);
        Assert.Null(recorder.ValueElementLocations);

        var source = """
            [Params("42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 })]
            public class Foo { }
            """;

        var (compilation, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var intType = compilation.GetSpecialType(SpecialType.System_Int32);

        var (expectedCollectionLocation, expectedElementLocations) = ExpectedLocation.ParamsArgument(attributeSyntax, 0, 7);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Equal(7, recorder.Value!.Count);
        Assert.Equal("42", recorder.Value[0]);
        Assert.Null(recorder.Value[1]);
        Assert.Equal(intType, recorder.Value[2]);
        Assert.Equal("Foo", recorder.Value[3]);
        Assert.Equal(42, recorder.Value[4]);
        Assert.Equal((double)42, recorder.Value[5]);
        Assert.Equal(new object[] { "42", 42 }, (object?[])recorder.Value[6]!);

        Assert.Equal(expectedCollectionLocation, recorder.ValueCollectionLocation);
        Assert.Equal(expectedElementLocations, recorder.ValueElementLocations);
    }

    [Theory]
    [ClassData(typeof(Datasets.ParamsAttributeSources))]
    public async Task Params_OneArrayValueSameType_NotTreatedAsParams_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticParamsAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueCollectionLocation);
        Assert.Null(recorder.ValueElementLocations);

        var source = """
            [Params(new object[] { (new object[] { (object[])(new object[] { new object[] { 4 } }) }) })]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var (expectedCollectionLocation, expectedElementLocations) = ExpectedLocation.ArrayArgument(attributeSyntax, 0);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Equal(1, recorder.Value!.Count);
        Assert.Equal(4, ((object[])((object[])((object[])recorder.Value[0]!)[0])[0])[0]);

        Assert.Equal(expectedCollectionLocation, recorder.ValueCollectionLocation);
        Assert.Equal(expectedElementLocations, recorder.ValueElementLocations);
    }

    [Theory]
    [ClassData(typeof(Datasets.ParamsAttributeSources))]
    public async Task Params_OneArrayValueDifferentType_TreatedAsParams_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticParamsAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueCollectionLocation);
        Assert.Null(recorder.ValueElementLocations);

        var source = """
            [Params(new int[] { 4 })]
            public class Foo { }
            """;

        var (c, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var d = c.GetDiagnostics();
        Assert.Empty(d);
        var (expectedCollectionLocation, expectedElementLocations) = ExpectedLocation.ParamsArgument(attributeSyntax, 0, 1);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Equal(1, recorder.Value!.Count);
        Assert.Equal(new[] { 4 }, recorder.Value[0]);

        Assert.Equal(expectedCollectionLocation, recorder.ValueCollectionLocation);
        Assert.Equal(expectedElementLocations, recorder.ValueElementLocations);
    }

    [Theory]
    [ClassData(typeof(Datasets.ParamsAttributeSources))]
    public async Task Params_MultipleArrayValuesSameType_NotTreatedAsParams_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticParamsAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueCollectionLocation);
        Assert.Null(recorder.ValueElementLocations);

        var source = """
            [Params(new object[] { "42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 } })]
            public class Foo { }
            """;

        var (compilation, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var intType = compilation.GetSpecialType(SpecialType.System_Int32);

        var (expectedCollectionLocation, expectedElementLocations) = ExpectedLocation.ArrayArgument(attributeSyntax, 0);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Equal(7, recorder.Value!.Count);
        Assert.Equal("42", recorder.Value[0]);
        Assert.Null(recorder.Value[1]);
        Assert.Equal(intType, recorder.Value[2]);
        Assert.Equal("Foo", recorder.Value[3]);
        Assert.Equal(42, recorder.Value[4]);
        Assert.Equal((double)42, recorder.Value[5]);
        Assert.Equal(new object[] { "42", 42 }, (object?[])recorder.Value[6]!);

        Assert.Equal(expectedCollectionLocation, recorder.ValueCollectionLocation);
        Assert.Equal(expectedElementLocations, recorder.ValueElementLocations);
    }

    [Theory]
    [ClassData(typeof(Datasets.ParamsAttributeSources))]
    public async Task Params_MultipleArrayValuesDifferentType_TreatedAsParams_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticParamsAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueCollectionLocation);
        Assert.Null(recorder.ValueElementLocations);

        var source = """
            [Params(new int[] { 4, 5, 6 })]
            public class Foo { }
            """;

        var (c, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var d = c.GetDiagnostics();
        Assert.Empty(d);
        var (expectedCollectionLocation, expectedElementLocations) = ExpectedLocation.ParamsArgument(attributeSyntax, 0, 1);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Equal(1, recorder.Value!.Count);
        Assert.Equal(new[] { 4, 5, 6 }, recorder.Value[0]);

        Assert.Equal(expectedCollectionLocation, recorder.ValueCollectionLocation);
        Assert.Equal(expectedElementLocations, recorder.ValueElementLocations);
    }

    [Theory]
    [ClassData(typeof(Datasets.ParamsAttributeSources))]
    public async Task Params_EmptyArray_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticParamsAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueCollectionLocation);
        Assert.Null(recorder.ValueElementLocations);

        var source = """
            [Params(new object[0])]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var (expectedCollectionLocation, expectedElementLocations) = ExpectedLocation.ArrayArgument(attributeSyntax, 0);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Empty(recorder.Value!);

        Assert.Equal(expectedCollectionLocation, recorder.ValueCollectionLocation);
        Assert.Equal(expectedElementLocations, recorder.ValueElementLocations);
    }

    [Theory]
    [ClassData(typeof(Datasets.ParamsAttributeSources))]
    public async Task Params_Empty_WithoutBody_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticParamsAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueCollectionLocation);
        Assert.Null(recorder.ValueElementLocations);

        var source = """
            [Params]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Empty(recorder.Value!);

        Assert.Equal(Location.None, recorder.ValueCollectionLocation);
        Assert.Empty(recorder.ValueElementLocations!);
    }

    [Theory]
    [ClassData(typeof(Datasets.ParamsAttributeSources))]
    public async Task Params_Empty_WithBody_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticParamsAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueCollectionLocation);
        Assert.Null(recorder.ValueElementLocations);

        var source = """
            [Params()]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Empty(recorder.Value!);

        Assert.Equal(Location.None, recorder.ValueCollectionLocation);
        Assert.Empty(recorder.ValueElementLocations!);
    }

    [Theory]
    [ClassData(typeof(Datasets.ParamsAttributeSources))]
    public async Task Params_Labelled_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticParamsAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueCollectionLocation);
        Assert.Null(recorder.ValueElementLocations);

        var source = """
            [Params(value: new object[] { "42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 } })]
            public class Foo { }
            """;

        var (compilation, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var intType = compilation.GetSpecialType(SpecialType.System_Int32);

        var (expectedCollectionLocation, expectedElementLocations) = ExpectedLocation.ArrayArgument(attributeSyntax, 0);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Equal(7, recorder.Value!.Count);
        Assert.Equal("42", recorder.Value[0]);
        Assert.Null(recorder.Value[1]);
        Assert.Equal(intType, recorder.Value[2]);
        Assert.Equal("Foo", recorder.Value[3]);
        Assert.Equal(42, recorder.Value[4]);
        Assert.Equal((double)42, recorder.Value[5]);
        Assert.Equal(new object[] { "42", 42 }, (object?[])recorder.Value[6]!);

        Assert.Equal(expectedCollectionLocation, recorder.ValueCollectionLocation);
        Assert.Equal(expectedElementLocations, recorder.ValueElementLocations);
    }
}
