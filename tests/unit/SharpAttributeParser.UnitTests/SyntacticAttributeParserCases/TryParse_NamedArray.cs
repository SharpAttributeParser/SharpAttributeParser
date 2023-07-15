namespace SharpAttributeParser.SyntacticAttributeParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_NamedArray
{
    private static bool Target(ISyntacticAttributeParser parser, ISyntacticArgumentRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => parser.TryParse(recorder, attributeData, attributeSyntax);

    [Theory]
    [ClassData(typeof(ParserSources))]
    [SuppressMessage("Minor Code Smell", "S1125: Boolean literals should not be redundant", Justification = "Prefer equality check over negation.")]
    public async Task FalseReturningRecorder_False(ISyntacticAttributeParser parser)
    {
        var recorder = Mock.Of<ISyntacticArgumentRecorder>(static (recorder) => recorder.TryRecordNamedArgument(It.IsAny<string>(), It.IsAny<IReadOnlyList<object?>?>(), It.IsAny<CollectionLocation>()) == false);

        var source = """
            [Named(ArrayValue = null)]
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
            [Named(4)]
            public sealed class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task ErrorArgument_True_NotRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Named(ArrayValue = (object[])4)]
            public sealed class Foo { }
            """;

        await TrueAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NullLiteral_True_Recorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Named(ArrayValue = null)]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax syntax) => new()
        {
            Value = null,
            ValueLocation = ExpectedLocation.ArrayArgument(syntax, 0)
        };
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task DefaultLiteral_True_Recorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Named(ArrayValue = default)]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax syntax) => new()
        {
            Value = null,
            ValueLocation = ExpectedLocation.ArrayArgument(syntax, 0)
        };
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task DefaultExpression_True_Recorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Named(ArrayValue = default(object[]))]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax syntax) => new()
        {
            Value = null,
            ValueLocation = ExpectedLocation.ArrayArgument(syntax, 0)
        };
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task Empty_True_Recorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Named(ArrayValue = new object[0])]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax syntax) => new()
        {
            Value = Array.Empty<object?>(),
            ValueLocation = ExpectedLocation.ArrayArgument(syntax, 0)
        };
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task Values_True_Recorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Named(ArrayValue = new object[] { "42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 } })]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax syntax)
        {
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            return new()
            {
                Value = new object?[] { "42", null, intType, "Foo", 42, (double)42, new object[] { "42", 42 } },
                ValueLocation = ExpectedLocation.ArrayArgument(syntax, 0)
            };
        }
    }

    [AssertionMethod]
    private static async Task TrueAndIdenticalToExpected(ISyntacticAttributeParser parser, string source, Func<Compilation, AttributeSyntax, ExpectedResult> expectedDelegate)
    {
        SyntacticNamedAttributeRecorder recorder = new();

        var (compilation, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var expected = expectedDelegate(compilation, attributeSyntax);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Equal(expected.Value, recorder.ArrayValue);
        Assert.True(recorder.ArrayValueRecorded);
        Assert.Equal(expected.ValueLocation!.Collection, recorder.ArrayValueLocation!.Collection);
        Assert.Equal(expected.ValueLocation.Elements, recorder.ArrayValueLocation.Elements);
    }

    [AssertionMethod]
    private static async Task ExpectedOutcomeAndNotRecorded(ISyntacticAttributeParser parser, string source, bool expected)
    {
        SyntacticNamedAttributeRecorder recorder = new();

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.Equal(expected, result);

        Assert.False(recorder.ArrayValueRecorded);
    }

    [AssertionMethod]
    private static async Task TrueAndNotRecorded(ISyntacticAttributeParser parser, string source) => await ExpectedOutcomeAndNotRecorded(parser, source, true);

    [AssertionMethod]
    private static async Task FalseAndNotRecorded(ISyntacticAttributeParser parser, string source) => await ExpectedOutcomeAndNotRecorded(parser, source, false);

    private sealed class ExpectedResult
    {
        public IReadOnlyList<object?>? Value { get; init; }
        public CollectionLocation? ValueLocation { get; init; }
    }
}
