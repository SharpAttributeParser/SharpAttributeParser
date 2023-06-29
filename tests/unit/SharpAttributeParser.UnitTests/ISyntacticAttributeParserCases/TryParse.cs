namespace SharpAttributeParser.Tests.ISyntacticAttributeParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Threading.Tasks;

using Xunit;

public class TryParse
{
    private static bool Target(ISyntacticAttributeParser parser, ISyntacticArgumentRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => parser.TryParse(recorder, attributeData, attributeSyntax);

    [Theory]
    [ClassData(typeof(Datasets.ParserSources))]
    public async Task NullRecorder_ArgumentNullException(ISyntacticAttributeParser parser)
    {
        var source = """
            [Generic<string>]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var recorder = Datasets.GetNullRecorder();

        var exception = Record.Exception(() => Target(parser, recorder, attributeData, attributeSyntax));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(Datasets.GenericAttributeSources))]
    public async Task NullAttributeData_ArgumentNullException(ISyntacticAttributeParser parser, ISyntacticArgumentRecorder recorder)
    {
        var source = """
            [Generic<string>]
            public class Foo { }
            """;

        var (_, _, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");
        var attributeData = Datasets.GetNullAttributeData();

        var exception = Record.Exception(() => Target(parser, recorder, attributeData, attributeSyntax));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(Datasets.GenericAttributeSources))]
    public async Task NullAttributeSyntax_ArgumentNullException(ISyntacticAttributeParser parser, ISyntacticArgumentRecorder recorder)
    {
        var source = """
            [Generic<string>]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");
        var attributeSyntax = Datasets.GetNullAttributeSyntax();

        var exception = Record.Exception(() => Target(parser, recorder, attributeData, attributeSyntax));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(Datasets.GenericAttributeSources))]
    public async Task NonExistingAttribute_False_RecorderNotPopulated(ISyntacticAttributeParser parser, SyntacticGenericAttributeRecorder recorder)
    {
        var source = """
            [NonExisting]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.False(result);

        Assert.Null(recorder.T);
        Assert.Null(recorder.TLocation);
    }

    [Theory]
    [ClassData(typeof(Datasets.GenericAttributeSources))]
    public async Task NonExistingConstructor_False_RecorderNotPopulated(ISyntacticAttributeParser parser, SyntacticGenericAttributeRecorder recorder)
    {
        var source = """
            [Generic<string>(4)]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.False(result);

        Assert.Null(recorder.T);
        Assert.Null(recorder.TLocation);
    }

    [Theory]
    [ClassData(typeof(Datasets.SingleConstructorAttributeSources))]
    public async Task SyntacticalError_False_RecorderNotPopulated(ISyntacticAttributeParser parser, SyntacticSingleConstructorAttributeRecorder recorder)
    {
        var source = """
            [SingleConstructor(4?)]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.False(result);

        Assert.Null(recorder.Value);
    }
}
