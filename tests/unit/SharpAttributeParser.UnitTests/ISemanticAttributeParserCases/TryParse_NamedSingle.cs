namespace SharpAttributeParser.Tests.ISemanticAttributeParserCases;

using Microsoft.CodeAnalysis;

using Moq;

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Xunit;

public class TryParse_NamedSingle
{
    private static bool Target(ISemanticAttributeParser parser, ISemanticArgumentRecorder recorder, AttributeData attributeData) => parser.TryParse(recorder, attributeData);

    [Theory]
    [ClassData(typeof(Datasets.ParserSources))]
    [SuppressMessage("Minor Code Smell", "S1125: Boolean literals should not be redundant", Justification = "Prefer equality check over negation.")]
    public async Task NamedSingle_FalseReturningRecorder_False(ISemanticAttributeParser parser)
    {
        var recorder = Mock.Of<ISemanticArgumentRecorder>(static (recorder) => recorder.TryRecordNamedArgument(It.IsAny<string>(), It.IsAny<object?>()) == false);

        var source = """
            [Named(Value = null)]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData);

        Assert.False(result);
    }

    [Theory]
    [ClassData(typeof(Datasets.NamedAttributeSources))]
    public async Task NamedSingle_NonExistingNamedArugment_True_RecorderPopulated(ISemanticAttributeParser parser, SemanticNamedAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);

        var source = """
            [Named(NonExisting = 42, Value = null)]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData);

        Assert.True(result);

        Assert.Null(recorder.Value);
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
    public async Task NamedSingle_DefaultLiteral_True_RecorderPopulated(ISemanticAttributeParser parser, SemanticNamedAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);

        var source = """
            [Named(Value = default)]
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
    public async Task NamedSingle_DefaultExpression_True_RecorderPopulated(ISemanticAttributeParser parser, SemanticNamedAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);

        var source = """
            [Named(Value = default(object[]))]
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
}
