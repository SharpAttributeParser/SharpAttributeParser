namespace SharpAttributeParser.Tests.ISyntacticAttributeParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Xunit;

public class TryParse_ConstructorArray
{
    private static bool Target(ISyntacticAttributeParser parser, ISyntacticArgumentRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => parser.TryParse(recorder, attributeData, attributeSyntax);

    [Theory]
    [ClassData(typeof(Datasets.ParserSources))]
    [SuppressMessage("Minor Code Smell", "S1125: Boolean literals should not be redundant", Justification = "Prefer equality check over negation.")]
    public async Task ConstructorArray_FalseReturningRecorder_False(ISyntacticAttributeParser parser)
    {
        var recorder = Mock.Of<ISyntacticArgumentRecorder>(static (recorder) => recorder.TryRecordConstructorArgument(It.IsAny<IParameterSymbol>(), It.IsAny<IReadOnlyList<object?>?>(), It.IsAny<Location>(), It.IsAny<IReadOnlyList<Location>>()) == false);

        var source = """
            [ArrayConstructor(null)]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.False(result);
    }

    [Theory]
    [ClassData(typeof(Datasets.ArrayConstructorAttributeSources))]
    public async Task ConstructorArray_NullLiteral_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticArrayConstructorAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueCollectionLocation);
        Assert.Null(recorder.ValueElementLocations);

        var source = """
            [ArrayConstructor(null)]
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
    [ClassData(typeof(Datasets.ArrayConstructorAttributeSources))]
    public async Task ConstructorArray_DefaultLiteral_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticArrayConstructorAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueCollectionLocation);
        Assert.Null(recorder.ValueElementLocations);

        var source = """
            [ArrayConstructor(default)]
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
    [ClassData(typeof(Datasets.ArrayConstructorAttributeSources))]
    public async Task ConstructorArray_DefaultExpression_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticArrayConstructorAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueCollectionLocation);
        Assert.Null(recorder.ValueElementLocations);

        var source = """
            [ArrayConstructor(default(object[]))]
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
    [ClassData(typeof(Datasets.ArrayConstructorAttributeSources))]
    public async Task ConstructorArray_Values_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticArrayConstructorAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueCollectionLocation);
        Assert.Null(recorder.ValueElementLocations);

        var source = """
            [ArrayConstructor(new object[] { "42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 } })]
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
    [ClassData(typeof(Datasets.ArrayConstructorAttributeSources))]
    public async Task ConstructorArray_Empty_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticArrayConstructorAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueCollectionLocation);
        Assert.Null(recorder.ValueElementLocations);

        var source = """
            [ArrayConstructor(value: new object[] { })]
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
    [ClassData(typeof(Datasets.ArrayConstructorAttributeSources))]
    public async Task ConstructorArray_Labelled_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticArrayConstructorAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueCollectionLocation);
        Assert.Null(recorder.ValueElementLocations);

        var source = """
            [ArrayConstructor(value: new object[] { "42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 } })]
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
