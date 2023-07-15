namespace SharpAttributeParser.SemanticAttributeParserCases;

using Microsoft.CodeAnalysis;

using Moq;

using System;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse
{
    private static bool Target(ISemanticAttributeParser parser, ISemanticArgumentRecorder recorder, AttributeData attributeData) => parser.TryParse(recorder, attributeData);

    [Theory]
    [ClassData(typeof(ParserSources))]
    public void NullRecorder_ArgumentNullException(ISemanticAttributeParser parser)
    {
        var exception = Record.Exception(() => Target(parser, null!, Mock.Of<AttributeData>()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public void NullAttributeData_ArgumentNullException(ISemanticAttributeParser parser)
    {
        var exception = Record.Exception(() => Target(parser, new SemanticGenericAttributeRecorder(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NonExistingAttribute_False_NotRecorded(ISemanticAttributeParser parser)
    {
        var source = """
            [NonExisting]
            public sealed class Foo { }
            """;

        await Generic_FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NonExistingConstructor_False_NotRecorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Generic<string, int>(4)]
            public sealed class Foo { }
            """;

        await Generic_FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NonExistingNamedParameter_True_Recorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Generic<string, int>(NonExisting = 4)]
            public sealed class Foo { }
            """;

        await Generic_TrueAndRecorded(parser, source, expected);

        static ExpectedResult expected(Compilation compilation)
        {
            var stringType = compilation.GetSpecialType(SpecialType.System_String);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            return new(stringType, intType);
        }
    }

    [AssertionMethod]
    private static async Task Generic_TrueAndRecorded(ISemanticAttributeParser parser, string source, Func<Compilation, ExpectedResult> expectedDelegate)
    {
        SemanticGenericAttributeRecorder recorder = new();

        var (compilation, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var expected = expectedDelegate(compilation);

        var result = Target(parser, recorder, attributeData);

        Assert.True(result);

        Assert.Equal(expected.T1, recorder.T1);
        Assert.Equal(expected.T2, recorder.T2);

        Assert.True(recorder.T1Recorded);
        Assert.True(recorder.T2Recorded);
    }

    [AssertionMethod]
    private static async Task Generic_FalseAndNotRecorded(ISemanticAttributeParser parser, string source)
    {
        SemanticGenericAttributeRecorder recorder = new();

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData);

        Assert.False(result);

        Assert.False(recorder.T1Recorded);
        Assert.False(recorder.T2Recorded);
    }

    private sealed class ExpectedResult
    {
        public ITypeSymbol T1 { get; }
        public ITypeSymbol T2 { get; }

        public ExpectedResult(ITypeSymbol t1, ITypeSymbol t2)
        {
            T1 = t1;
            T2 = t2;
        }
    }
}
