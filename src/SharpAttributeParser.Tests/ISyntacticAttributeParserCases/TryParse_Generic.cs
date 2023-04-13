namespace SharpAttributeParser.Tests.ISyntacticAttributeParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Xunit;

public class TryParse_Generic
{
    private static bool Target(ISyntacticAttributeParser parser, ISyntacticArgumentRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => parser.TryParse(recorder, attributeData, attributeSyntax);

    [Theory]
    [ClassData(typeof(Datasets.ParserSources))]
    [SuppressMessage("Minor Code Smell", "S1125: Boolean literals should not be redundant", Justification = "Prefer equality check over negation.")]
    public async Task Generic_FalseReturningRecorder_False(ISyntacticAttributeParser parser)
    {
        var recorder = Mock.Of<ISyntacticArgumentRecorder>(static (recorder) => recorder.TryRecordGenericArgument(It.IsAny<ITypeParameterSymbol>(), It.IsAny<ITypeSymbol>(), It.IsAny<Location>()) == false);

        var source = """
            [Generic<string>]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.False(result);
    }

    [Theory]
    [ClassData(typeof(Datasets.GenericAttributeSources))]
    public async Task Generic_WithoutBody_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticGenericAttributeRecorder recorder)
    {
        Assert.Null(recorder.T);
        Assert.Null(recorder.TLocation);

        var source = """
            [Generic<string>]
            public class Foo { }
            """;

        var (compilation, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var expectedType = compilation.GetSpecialType(SpecialType.System_String);
        var expectedLocation = ExpectedLocation.TypeArgument(attributeSyntax, 0);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Equal(expectedType, recorder.T);
        Assert.Equal(expectedLocation, recorder.TLocation);
    }

    [Theory]
    [ClassData(typeof(Datasets.GenericAttributeSources))]
    public async Task Generic_WithBody_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticGenericAttributeRecorder recorder)
    {
        Assert.Null(recorder.T);
        Assert.Null(recorder.TLocation);

        var source = """
            [Generic<string>()]
            public class Foo { }
            """;

        var (compilation, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var expectedType = compilation.GetSpecialType(SpecialType.System_String);
        var expectedLocation = ExpectedLocation.TypeArgument(attributeSyntax, 0);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Equal(expectedType, recorder.T);
        Assert.Equal(expectedLocation, recorder.TLocation);
    }

    [Theory]
    [ClassData(typeof(Datasets.GenericAttributeSources))]
    public async Task Generic_Tuple_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticGenericAttributeRecorder recorder)
    {
        Assert.Null(recorder.T);
        Assert.Null(recorder.TLocation);

        var source = """
            [Generic<(string, int)>]
            public class Foo { }
            """;

        var (compilation, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);

        var expectedType = compilation.GetTypeByMetadataName("System.ValueTuple`2")!.Construct(stringType, intType);
        var expectedLocation = ExpectedLocation.TypeArgument(attributeSyntax, 0);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Equal(expectedType, recorder.T);
        Assert.Equal(expectedLocation, recorder.TLocation);
    }

    [Theory]
    [ClassData(typeof(Datasets.GenericAttributeSources))]
    public async Task QualifiedGeneric_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticGenericAttributeRecorder recorder)
    {
        Assert.Null(recorder.T);

        var source = """
            [SharpAttributeParser.Tests.QualifiedGeneric<int>]
            public class Foo { }
            """;

        var (compilation, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var intType = compilation.GetSpecialType(SpecialType.System_Int32);

        var expectedLocation = ExpectedLocation.TypeArgument(attributeSyntax, 0);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Equal(intType, recorder.T);
        Assert.Equal(expectedLocation, recorder.TLocation);
    }
}
