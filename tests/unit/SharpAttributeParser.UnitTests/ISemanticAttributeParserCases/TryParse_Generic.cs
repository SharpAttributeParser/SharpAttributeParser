namespace SharpAttributeParser.Tests.ISemanticAttributeParserCases;

using Microsoft.CodeAnalysis;

using Moq;

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Xunit;

public class TryParse_Generic
{
    private static bool Target(ISemanticAttributeParser parser, ISemanticArgumentRecorder recorder, AttributeData attributeData) => parser.TryParse(recorder, attributeData);

    [Theory]
    [ClassData(typeof(Datasets.ParserSources))]
    [SuppressMessage("Minor Code Smell", "S1125: Boolean literals should not be redundant", Justification = "Prefer equality check over negation.")]
    public async Task Generic_FalseReturningRecorder_False(ISemanticAttributeParser parser)
    {
        var recorder = Mock.Of<ISemanticArgumentRecorder>(static (recorder) => recorder.TryRecordGenericArgument(It.IsAny<ITypeParameterSymbol>(), It.IsAny<ITypeSymbol>()) == false);

        var source = """
            [Generic<string>]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData);

        Assert.False(result);
    }

    [Theory]
    [ClassData(typeof(Datasets.GenericAttributeSources))]
    public async Task Generic_WithoutBody_True_RecorderPopulated(ISemanticAttributeParser parser, SemanticGenericAttributeRecorder recorder)
    {
        Assert.Null(recorder.T);

        var source = """
            [Generic<string>]
            public class Foo { }
            """;

        var (compilation, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var expectedType = compilation.GetSpecialType(SpecialType.System_String);

        var result = Target(parser, recorder, attributeData);

        Assert.True(result);

        Assert.Equal(expectedType, recorder.T);
    }

    [Theory]
    [ClassData(typeof(Datasets.GenericAttributeSources))]
    public async Task Generic_WithBody_True_RecorderPopulated(ISemanticAttributeParser parser, SemanticGenericAttributeRecorder recorder)
    {
        Assert.Null(recorder.T);

        var source = """
            [Generic<string>()]
            public class Foo { }
            """;

        var (compilation, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var expectedType = compilation.GetSpecialType(SpecialType.System_String);

        var result = Target(parser, recorder, attributeData);

        Assert.True(result);

        Assert.Equal(expectedType, recorder.T);
    }

    [Theory]
    [ClassData(typeof(Datasets.GenericAttributeSources))]
    public async Task Generic_Tuple_True_RecorderPopulated(ISemanticAttributeParser parser, SemanticGenericAttributeRecorder recorder)
    {
        Assert.Null(recorder.T);

        var source = """
            [Generic<(string, int)>]
            public class Foo { }
            """;

        var (compilation, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);

        var expectedType = compilation.GetTypeByMetadataName("System.ValueTuple`2")!.Construct(stringType, intType);

        var result = Target(parser, recorder, attributeData);

        Assert.True(result);

        Assert.Equal(expectedType, recorder.T);
    }

    [Theory]
    [ClassData(typeof(Datasets.GenericAttributeSources))]
    public async Task QualifiedGeneric_True_RecorderPopulated(ISemanticAttributeParser parser, SemanticGenericAttributeRecorder recorder)
    {
        Assert.Null(recorder.T);

        var source = """
            [SharpAttributeParser.Tests.QualifiedGeneric<int>]
            public class Foo { }
            """;

        var (compilation, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var intType = compilation.GetSpecialType(SpecialType.System_Int32);

        var result = Target(parser, recorder, attributeData);

        Assert.True(result);

        Assert.Equal(intType, recorder.T);
    }
}
