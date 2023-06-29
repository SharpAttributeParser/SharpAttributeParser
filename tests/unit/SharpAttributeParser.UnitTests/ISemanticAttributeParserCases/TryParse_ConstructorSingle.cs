namespace SharpAttributeParser.Tests.ISemanticAttributeParserCases;

using Microsoft.CodeAnalysis;

using Moq;

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Xunit;

public class TryParse_ConstructorSingle
{
    private static bool Target(ISemanticAttributeParser parser, ISemanticArgumentRecorder recorder, AttributeData attributeData) => parser.TryParse(recorder, attributeData);

    [Theory]
    [ClassData(typeof(Datasets.ParserSources))]
    [SuppressMessage("Minor Code Smell", "S1125: Boolean literals should not be redundant", Justification = "Prefer equality check over negation.")]
    public async Task ConstructorSingle_FalseReturningRecorder_False(ISemanticAttributeParser parser)
    {
        var recorder = Mock.Of<ISemanticArgumentRecorder>(static (recorder) => recorder.TryRecordConstructorArgument(It.IsAny<IParameterSymbol>(), It.IsAny<object?>()) == false);

        var source = """
            [SingleConstructor(null)]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData);

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
    public async Task ConstructorSingle_DefaultLiteral_True_RecorderPopulated(ISemanticAttributeParser parser, SemanticSingleConstructorAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);

        var source = """
            [SingleConstructor(default)]
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
    public async Task ConstructorSingle_DefaultExpression_True_RecorderPopulated(ISemanticAttributeParser parser, SemanticSingleConstructorAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);

        var source = """
            [SingleConstructor(default(object))]
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
}
