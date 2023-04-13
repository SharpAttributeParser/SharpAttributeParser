namespace SharpAttributeParser.Tests.ISemanticAttributeParserCases;

using Microsoft.CodeAnalysis;

using Moq;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Xunit;

public class TryParse_NamedArray
{
    private static bool Target(ISemanticAttributeParser parser, ISemanticArgumentRecorder recorder, AttributeData attributeData) => parser.TryParse(recorder, attributeData);

    [Theory]
    [ClassData(typeof(Datasets.ParserSources))]
    [SuppressMessage("Minor Code Smell", "S1125: Boolean literals should not be redundant", Justification = "Prefer equality check over negation.")]
    public async Task NamedArray_FalseReturningRecorder_False(ISemanticAttributeParser parser)
    {
        var recorder = Mock.Of<ISemanticArgumentRecorder>(static (recorder) => recorder.TryRecordNamedArgument(It.IsAny<string>(), It.IsAny<IReadOnlyList<object?>?>()) == false);

        var source = """
            [Named(Values = null)]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData);

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
    public async Task NamedArray_DefaultLiteral_True_RecorderPopulated(ISemanticAttributeParser parser, SemanticNamedAttributeRecorder recorder)
    {
        Assert.Null(recorder.Values);

        var source = """
            [Named(Values = default)]
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
    public async Task NamedArray_DefaultExpression_True_RecorderPopulated(ISemanticAttributeParser parser, SemanticNamedAttributeRecorder recorder)
    {
        Assert.Null(recorder.Values);

        var source = """
            [Named(Values = default(object[]))]
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
}
