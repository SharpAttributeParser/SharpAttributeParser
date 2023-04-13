namespace SharpAttributeParser.Tests.ISyntacticAttributeParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Xunit;

public class TryParse_ConstructorSingle
{
    private static bool Target(ISyntacticAttributeParser parser, ISyntacticArgumentRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => parser.TryParse(recorder, attributeData, attributeSyntax);

    [Theory]
    [ClassData(typeof(Datasets.ParserSources))]
    [SuppressMessage("Minor Code Smell", "S1125: Boolean literals should not be redundant", Justification = "Prefer equality check over negation.")]
    public async Task ConstructorSingle_FalseReturningRecorder_False(ISyntacticAttributeParser parser)
    {
        var recorder = Mock.Of<ISyntacticArgumentRecorder>(static (recorder) => recorder.TryRecordConstructorArgument(It.IsAny<IParameterSymbol>(), It.IsAny<object?>(), It.IsAny<Location>()) == false);

        var source = """
            [SingleConstructor(null)]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.False(result);
    }

    [Theory]
    [ClassData(typeof(Datasets.SingleConstructorAttributeSources))]
    public async Task ConstructorSingle_NullLiteral_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticSingleConstructorAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueLocation);

        var source = """
            [SingleConstructor(null)]
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
    [ClassData(typeof(Datasets.SingleConstructorAttributeSources))]
    public async Task ConstructorSingle_DefaultLiteral_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticSingleConstructorAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueLocation);

        var source = """
            [SingleConstructor(default)]
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
    [ClassData(typeof(Datasets.SingleConstructorAttributeSources))]
    public async Task ConstructorSingle_DefaultExpression_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticSingleConstructorAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueLocation);

        var source = """
            [SingleConstructor(default(object))]
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
    [ClassData(typeof(Datasets.SingleConstructorAttributeSources))]
    public async Task ConstructorSingle_Value_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticSingleConstructorAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueLocation);

        var source = """
            [SingleConstructor("42")]
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
    [ClassData(typeof(Datasets.SingleConstructorAttributeSources))]
    public async Task ConstructorSingle_Labelled_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticSingleConstructorAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueLocation);

        var source = """
            [SingleConstructor(value: "42")]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var expectedLocation = ExpectedLocation.SingleArgument(attributeSyntax, 0);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Equal("42", recorder.Value);
        Assert.Equal(expectedLocation, recorder.ValueLocation);
    }
}
