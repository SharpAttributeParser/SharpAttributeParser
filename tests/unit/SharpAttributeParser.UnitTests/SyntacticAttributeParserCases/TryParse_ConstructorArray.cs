namespace SharpAttributeParser.SyntacticAttributeParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_ConstructorArray
{
    private static bool Target(ISyntacticAttributeParser parser, ISyntacticArgumentRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => parser.TryParse(recorder, attributeData, attributeSyntax);

    [Theory]
    [ClassData(typeof(ParserSources))]
    [SuppressMessage("Minor Code Smell", "S1125: Boolean literals should not be redundant", Justification = "Prefer equality check over negation.")]
    public async Task FalseReturningRecorder_False(ISyntacticAttributeParser parser)
    {
        var recorder = Mock.Of<ISyntacticArgumentRecorder>(static (recorder) => recorder.TryRecordConstructorArgument(It.IsAny<IParameterSymbol>(), It.IsAny<IReadOnlyList<object?>?>(), It.IsAny<CollectionLocation>()) == false);

        var source = """
            [ConstructorArray(null)]
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
            [ConstructorArray(3, 4)]
            public sealed class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task ErrorArgument_False_NotRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [ConstructorArray((object[])4)]
            public sealed class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NullLiteral_True_Recorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [ConstructorArray(null)]
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
            [ConstructorArray(default)]
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
            [ConstructorArray(default(object[]))]
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
            [ConstructorArray(new object[] { })]
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
            [ConstructorArray(new object[] { "42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 } })]
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

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task Labelled_True_Recorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [ConstructorArray(value: new object[] { "42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 } })]
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
        SyntacticConstructorArrayAttributeRecorder recorder = new();

        var (compilation, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var expected = expectedDelegate(compilation, attributeSyntax);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Equal(expected.Value, recorder.Value);
        Assert.True(recorder.ValueRecorded);
        Assert.Equal(expected.ValueLocation!.Collection, recorder.ValueLocation!.Collection);
        Assert.Equal(expected.ValueLocation.Elements, recorder.ValueLocation.Elements);
    }

    [AssertionMethod]
    private static async Task FalseAndNotRecorded(ISyntacticAttributeParser parser, string source)
    {
        SyntacticConstructorArrayAttributeRecorder recorder = new();

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.False(result);

        Assert.False(recorder.ValueRecorded);
    }

    private sealed class ExpectedResult
    {
        public IReadOnlyList<object?>? Value { get; init; }
        public CollectionLocation? ValueLocation { get; init; }
    }
}
