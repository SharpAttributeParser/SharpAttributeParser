namespace SharpAttributeParser.SyntacticAttributeParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using System;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse
{
    private static bool Target(ISyntacticAttributeParser parser, ISyntacticArgumentRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => parser.TryParse(recorder, attributeData, attributeSyntax);

    [Theory]
    [ClassData(typeof(ParserSources))]
    public void NullRecorder_ArgumentNullException(ISyntacticAttributeParser parser)
    {
        var exception = Record.Exception(() => Target(parser, null!, Mock.Of<AttributeData>(), AttributeSyntaxFactory.Create()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public void NullAttributeData_ArgumentNullException(ISyntacticAttributeParser parser)
    {
        var exception = Record.Exception(() => Target(parser, new SyntacticGenericAttributeRecorder(), null!, AttributeSyntaxFactory.Create()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public void NullAttributeSyntax_ArgumentNullException(ISyntacticAttributeParser parser)
    {
        var exception = Record.Exception(() => Target(parser, new SyntacticGenericAttributeRecorder(), Mock.Of<AttributeData>(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NonExistingAttribute_False_NotRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [NonExisting]
            public sealed class Foo { }
            """;

        await Generic_FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NonExistingConstructor_False_NotRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Generic<string, int>(4)]
            public sealed class Foo { }
            """;

        await Generic_FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NonExistingNamedParameter_True_Recorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Generic<string, int>(NonExisting = 4)]
            public sealed class Foo { }
            """;

        await Generic_TrueAndRecorded(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax syntax)
        {
            var stringType = compilation.GetSpecialType(SpecialType.System_String);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            var t1Location = ExpectedLocation.TypeArgument(syntax, 0);
            var t2Location = ExpectedLocation.TypeArgument(syntax, 1);

            return new(stringType, intType, t1Location, t2Location);
        }
    }

    [AssertionMethod]
    private static async Task Generic_TrueAndRecorded(ISyntacticAttributeParser parser, string source, Func<Compilation, AttributeSyntax, ExpectedResult> expectedDelegate)
    {
        SyntacticGenericAttributeRecorder recorder = new();

        var (compilation, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var expected = expectedDelegate(compilation, attributeSyntax);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Equal(expected.T1, recorder.T1);
        Assert.True(recorder.T1Recorded);
        Assert.Equal(expected.T1Location, recorder.T1Location);

        Assert.Equal(expected.T2, recorder.T2);
        Assert.True(recorder.T2Recorded);
        Assert.Equal(expected.T2Location, recorder.T2Location);
    }

    [AssertionMethod]
    private static async Task Generic_FalseAndNotRecorded(ISyntacticAttributeParser parser, string source)
    {
        SyntacticGenericAttributeRecorder recorder = new();

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.False(result);

        Assert.False(recorder.T1Recorded);
        Assert.False(recorder.T2Recorded);
    }

    private sealed class ExpectedResult
    {
        public ITypeSymbol T1 { get; }
        public ITypeSymbol T2 { get; }

        public Location T1Location { get; }
        public Location T2Location { get; }

        public ExpectedResult(ITypeSymbol t1, ITypeSymbol t2, Location t1Location, Location t2Location)
        {
            T1 = t1;
            T2 = t2;

            T1Location = t1Location;
            T2Location = t2Location;
        }
    }
}
