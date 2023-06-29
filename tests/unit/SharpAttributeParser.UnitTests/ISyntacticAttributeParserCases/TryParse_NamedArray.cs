namespace SharpAttributeParser.Tests.ISyntacticAttributeParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Xunit;

public class TryParse_NamedArray
{
    private static bool Target(ISyntacticAttributeParser parser, ISyntacticArgumentRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => parser.TryParse(recorder, attributeData, attributeSyntax);

    [Theory]
    [ClassData(typeof(Datasets.ParserSources))]
    [SuppressMessage("Minor Code Smell", "S1125: Boolean literals should not be redundant", Justification = "Prefer equality check over negation.")]
    public async Task NamedArray_FalseReturningRecorder_False(ISyntacticAttributeParser parser)
    {
        var recorder = Mock.Of<ISyntacticArgumentRecorder>(static (recorder) => recorder.TryRecordNamedArgument(It.IsAny<string>(), It.IsAny<IReadOnlyList<object?>?>(), It.IsAny<Location>(), It.IsAny<IReadOnlyList<Location>>()) == false);

        var source = """
            [Named(Values = null)]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.False(result);
    }

    [Theory]
    [ClassData(typeof(Datasets.NamedAttributeSources))]
    public async Task NamedArray_NullLiteral_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticNamedAttributeRecorder recorder)
    {
        Assert.Null(recorder.Values);
        Assert.Null(recorder.ValuesCollectionLocations);
        Assert.Null(recorder.ValuesElementLocations);

        var source = """
            [Named(Values = null)]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var (expectedCollectionLocation, expectedElementLocations) = ExpectedLocation.ArrayArgument(attributeSyntax, 0);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Null(recorder.Values);
        Assert.Equal(expectedCollectionLocation, recorder.ValuesCollectionLocations);
        Assert.Equal(expectedElementLocations, recorder.ValuesElementLocations);
    }

    [Theory]
    [ClassData(typeof(Datasets.NamedAttributeSources))]
    public async Task NamedArray_DefaultLiteral_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticNamedAttributeRecorder recorder)
    {
        Assert.Null(recorder.Values);
        Assert.Null(recorder.ValuesCollectionLocations);
        Assert.Null(recorder.ValuesElementLocations);

        var source = """
            [Named(Values = default)]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var (expectedCollectionLocation, expectedElementLocations) = ExpectedLocation.ArrayArgument(attributeSyntax, 0);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Null(recorder.Values);
        Assert.Equal(expectedCollectionLocation, recorder.ValuesCollectionLocations);
        Assert.Equal(expectedElementLocations, recorder.ValuesElementLocations);
    }

    [Theory]
    [ClassData(typeof(Datasets.NamedAttributeSources))]
    public async Task NamedArray_DefaultExpression_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticNamedAttributeRecorder recorder)
    {
        Assert.Null(recorder.Values);
        Assert.Null(recorder.ValuesCollectionLocations);
        Assert.Null(recorder.ValuesElementLocations);

        var source = """
            [Named(Values = default(object[]))]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var (expectedCollectionLocation, expectedElementLocations) = ExpectedLocation.ArrayArgument(attributeSyntax, 0);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Null(recorder.Values);
        Assert.Equal(expectedCollectionLocation, recorder.ValuesCollectionLocations);
        Assert.Equal(expectedElementLocations, recorder.ValuesElementLocations);
    }

    [Theory]
    [ClassData(typeof(Datasets.NamedAttributeSources))]
    public async Task NamedArray_Values_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticNamedAttributeRecorder recorder)
    {
        Assert.Null(recorder.Values);
        Assert.Null(recorder.ValuesCollectionLocations);
        Assert.Null(recorder.ValuesElementLocations);

        var source = """
            [Named(Values = new object[] { "42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 } })]
            public class Foo { }
            """;

        var (compilation, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var intType = compilation.GetSpecialType(SpecialType.System_Int32);

        var (expectedCollectionLocation, expectedElementLocations) = ExpectedLocation.ArrayArgument(attributeSyntax, 0);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Equal(7, recorder.Values!.Count);
        Assert.Equal("42", recorder.Values[0]);
        Assert.Null(recorder.Values[1]);
        Assert.Equal(intType, recorder.Values[2]);
        Assert.Equal("Foo", recorder.Values[3]);
        Assert.Equal(42, recorder.Values[4]);
        Assert.Equal((double)42, recorder.Values[5]);
        Assert.Equal(new object[] { "42", 42 }, (object?[])recorder.Values[6]!);

        Assert.Equal(expectedCollectionLocation, recorder.ValuesCollectionLocations);
        Assert.Equal(expectedElementLocations, recorder.ValuesElementLocations);
    }
}
