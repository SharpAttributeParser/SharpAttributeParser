namespace SharpAttributeParser.Tests.ISyntacticAttributeParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Xunit;

public class TryParse_NamedSingle
{
    private static bool Target(ISyntacticAttributeParser parser, ISyntacticArgumentRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => parser.TryParse(recorder, attributeData, attributeSyntax);

    [Theory]
    [ClassData(typeof(Datasets.ParserSources))]
    [SuppressMessage("Minor Code Smell", "S1125: Boolean literals should not be redundant", Justification = "Prefer equality check over negation.")]
    public async Task NamedSingle_FalseReturningRecorder_False(ISyntacticAttributeParser parser)
    {
        var recorder = Mock.Of<ISyntacticArgumentRecorder>(static (recorder) => recorder.TryRecordNamedArgument(It.IsAny<string>(), It.IsAny<object?>(), It.IsAny<Location>()) == false);

        var source = """
            [Named(Value = null)]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.False(result);
    }

    [Theory]
    [ClassData(typeof(Datasets.NamedAttributeSources))]
    public async Task NamedSingle_NonExistingNamedArugment_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticNamedAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueLocation);

        var source = """
            [Named(NonExisting = 42, Value = null)]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var expectedLocation = ExpectedLocation.SingleArgument(attributeSyntax, 1);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Null(recorder.Value);
        Assert.Equal(expectedLocation, recorder.ValueLocation);
    }

    [Theory]
    [ClassData(typeof(Datasets.NamedAttributeSources))]
    public async Task NamedSingle_NullLiteral_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticNamedAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueLocation);

        var source = """
            [Named(Value = null)]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var expectedLocation = ExpectedLocation.SingleArgument(attributeSyntax, 0);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Null(recorder.Value);
        Assert.Equal(expectedLocation, recorder.ValueLocation);
    }

    [Theory]
    [ClassData(typeof(Datasets.NamedAttributeSources))]
    public async Task NamedSingle_DefaultLiteral_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticNamedAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueLocation);

        var source = """
            [Named(Value = default)]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var expectedLocation = ExpectedLocation.SingleArgument(attributeSyntax, 0);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Null(recorder.Value);
        Assert.Equal(expectedLocation, recorder.ValueLocation);
    }

    [Theory]
    [ClassData(typeof(Datasets.NamedAttributeSources))]
    public async Task NamedSingle_DefaultExpression_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticNamedAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueLocation);

        var source = """
            [Named(Value = default(object))]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var expectedLocation = ExpectedLocation.SingleArgument(attributeSyntax, 0);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Null(recorder.Value);
        Assert.Equal(expectedLocation, recorder.ValueLocation);
    }

    [Theory]
    [ClassData(typeof(Datasets.NamedAttributeSources))]
    public async Task NamedSingle_Value_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticNamedAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueLocation);

        var source = """
            [Named(Value = "42")]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var expectedLocation = ExpectedLocation.SingleArgument(attributeSyntax, 0);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Equal("42", recorder.Value);
        Assert.Equal(expectedLocation, recorder.ValueLocation);
    }

    [Theory]
    [ClassData(typeof(Datasets.NamedAttributeSources))]
    public async Task NamedSingle_NonExisting_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticNamedAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueLocation);

        var source = """
            [Named(NonExisting = 42, Value = "42")]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var expectedLocation = ExpectedLocation.SingleArgument(attributeSyntax, 1);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Equal("42", recorder.Value);
        Assert.Equal(expectedLocation, recorder.ValueLocation);
    }
}
