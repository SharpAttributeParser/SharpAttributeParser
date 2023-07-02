namespace SharpAttributeParser.SyntacticAttributeParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_Generic
{
    private static bool Target(ISyntacticAttributeParser parser, ISyntacticArgumentRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => parser.TryParse(recorder, attributeData, attributeSyntax);

    [Theory]
    [ClassData(typeof(ParserSources))]
    [SuppressMessage("Minor Code Smell", "S1125: Boolean literals should not be redundant", Justification = "Prefer equality check over negation.")]
    public async Task FalseReturningRecorder_False(ISyntacticAttributeParser parser)
    {
        var recorder = Mock.Of<ISyntacticArgumentRecorder>(static (recorder) => recorder.TryRecordGenericArgument(It.IsAny<ITypeParameterSymbol>(), It.IsAny<ITypeSymbol>(), It.IsAny<Location>()) == false);

        var source = """
            [Generic<string, int>]
            public sealed class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.False(result);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NonExsitingAttribute_False_NotRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [NonExisting]
            public sealed class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NonExsitingConstructor_False_NotRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Generic<string, int>(4)]
            public sealed class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task ErrorArgument_False_NotRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Generic<NonExisting, int>]
            public sealed class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task WithoutBody_True_Recorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Generic<string, int>]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax syntax)
        {
            var stringType = compilation.GetSpecialType(SpecialType.System_String);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            var t1Location = ExpectedLocation.TypeArgument(syntax, 0);
            var t2Location = ExpectedLocation.TypeArgument(syntax, 1);

            return new(stringType, intType, t1Location, t2Location);
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task WithBody_True_Recorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Generic<string, int>()]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax syntax)
        {
            var stringType = compilation.GetSpecialType(SpecialType.System_String);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            var t1Location = ExpectedLocation.TypeArgument(syntax, 0);
            var t2Location = ExpectedLocation.TypeArgument(syntax, 1);

            return new(stringType, intType, t1Location, t2Location);
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task Tuple_True_Recorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Generic<(string, int), (int, string)>]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax syntax)
        {
            var stringType = compilation.GetSpecialType(SpecialType.System_String);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            var valueTupleType = compilation.GetTypeByMetadataName("System.ValueTuple`2")!;

            var t1 = valueTupleType.Construct(stringType, intType);
            var t2 = valueTupleType.Construct(intType, stringType);

            var t1Location = ExpectedLocation.TypeArgument(syntax, 0);
            var t2Location = ExpectedLocation.TypeArgument(syntax, 1);

            return new(t1, t2, t1Location, t2Location);
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task QualifiedTrue_Recorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [SharpAttributeParser.QualifiedGeneric<string, int>]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

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
    private static async Task TrueAndIdenticalToExpected(ISyntacticAttributeParser parser, string source, Func<Compilation, AttributeSyntax, ExpectedResult> expectedDelegate)
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
    private static async Task FalseAndNotRecorded(ISyntacticAttributeParser parser, string source)
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
