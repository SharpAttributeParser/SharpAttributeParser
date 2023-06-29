namespace SharpAttributeParser.Tests.ISemanticAttributeParserCases;

using Microsoft.CodeAnalysis;

using System;
using System.Threading.Tasks;

using Xunit;

public class TryParse
{
    private static bool Target(ISemanticAttributeParser parser, ISemanticArgumentRecorder recorder, AttributeData attributeData) => parser.TryParse(recorder, attributeData);

    [Theory]
    [ClassData(typeof(Datasets.ParserSources))]
    public async Task NullRecorder_ArgumentNullException(ISemanticAttributeParser parser)
    {
        var source = """
            [Generic<string>]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var recorder = Datasets.GetNullRecorder();

        var exception = Record.Exception(() => Target(parser, recorder, attributeData));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(Datasets.GenericAttributeSources))]
    public void NullAttributeData_ArgumentNullException(ISemanticAttributeParser parser, ISemanticArgumentRecorder recorder)
    {
        var attributeData = Datasets.GetNullAttributeData();

        var exception = Record.Exception(() => Target(parser, recorder, attributeData));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(Datasets.GenericAttributeSources))]
    public async Task NonExistingAttribute_False_RecorderNotPopulated(ISemanticAttributeParser parser, SemanticGenericAttributeRecorder recorder)
    {
        var source = """
            [NonExisting]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData);

        Assert.False(result);

        Assert.Null(recorder.T);
    }

    [Theory]
    [ClassData(typeof(Datasets.GenericAttributeSources))]
    public async Task NonExistingConstructor_False_RecorderNotPopulated(ISemanticAttributeParser parser, SemanticGenericAttributeRecorder recorder)
    {
        var source = """
            [Generic<string>(4)]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData);

        Assert.False(result);

        Assert.Null(recorder.T);
    }

    [Theory]
    [ClassData(typeof(Datasets.SingleConstructorAttributeSources))]
    public async Task SyntacticalError_False_RecorderNotPopulated(ISemanticAttributeParser parser, SemanticSingleConstructorAttributeRecorder recorder)
    {
        var source = """
            [SingleConstructor(4?)]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData);

        Assert.False(result);

        Assert.Null(recorder.Value);
    }
}
