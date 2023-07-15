namespace SharpAttributeParser.SyntacticAttributeParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_Combined
{
    private static bool Target(ISyntacticAttributeParser parser, ISyntacticArgumentRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => parser.TryParse(recorder, attributeData, attributeSyntax);

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task Params_Recorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Combined<string, int>("42", new object[0], "42", 42, NamedSingleValue = typeof(int), NamedArrayValue = new object[] { 42, "42" })]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expectedResult);

        static ExpectedResult expectedResult(Compilation compilation, AttributeSyntax syntax)
        {
            var stringType = compilation.GetSpecialType(SpecialType.System_String);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            var t1Location = ExpectedLocation.TypeArgument(syntax, 0);
            var t2Location = ExpectedLocation.TypeArgument(syntax, 1);
            var singleValueLocation = ExpectedLocation.SingleArgument(syntax, 0);
            var arrayValueLocation = ExpectedLocation.ArrayArgument(syntax, 1);
            var paramsValueLocation = ExpectedLocation.ParamsArgument(syntax, 2, 2);
            var namedSingleValueLocation = ExpectedLocation.SingleArgument(syntax, 4);
            var namedArrayValueLocation = ExpectedLocation.ArrayArgument(syntax, 5);

            return new ExpectedResult(stringType, intType, t1Location, t2Location)
            {
                SingleValue = "42",
                ArrayValue = Array.Empty<object>(),
                ParamsValue = new object[] { "42", 42 },
                NamedSingleValue = intType,
                NamedArrayValue = new object[] { 42, "42" },
                SingleValueLocation = singleValueLocation,
                NamedSingleValueLocation = namedSingleValueLocation,
                ArrayValueLocation = arrayValueLocation,
                ParamsValueLocation = paramsValueLocation,
                NamedArrayValueLocation = namedArrayValueLocation
            };
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task EmptyParams_Recorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Combined<string, int>("42", new object[0], NamedSingleValue = typeof(int), NamedArrayValue = new object[] { 42, "42" })]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expectedResult);

        static ExpectedResult expectedResult(Compilation compilation, AttributeSyntax syntax)
        {
            var stringType = compilation.GetSpecialType(SpecialType.System_String);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            var t1Location = ExpectedLocation.TypeArgument(syntax, 0);
            var t2Location = ExpectedLocation.TypeArgument(syntax, 1);
            var singleValueLocation = ExpectedLocation.SingleArgument(syntax, 0);
            var arrayValueLocation = ExpectedLocation.ArrayArgument(syntax, 1);
            var namedSingleValueLocation = ExpectedLocation.SingleArgument(syntax, 2);
            var namedArrayValueLocation = ExpectedLocation.ArrayArgument(syntax, 3);

            return new ExpectedResult(stringType, intType, t1Location, t2Location)
            {
                SingleValue = "42",
                ArrayValue = Array.Empty<object>(),
                ParamsValue = Array.Empty<object>(),
                NamedSingleValue = intType,
                NamedArrayValue = new object[] { 42, "42" },
                SingleValueLocation = singleValueLocation,
                NamedSingleValueLocation = namedSingleValueLocation,
                ArrayValueLocation = arrayValueLocation,
                ParamsValueLocation = CollectionLocation.None,
                NamedArrayValueLocation = namedArrayValueLocation
            };
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task OneElementParams_Recorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Combined<string, int>("42", new object[0], 42, NamedSingleValue = typeof(int), NamedArrayValue = new object[] { 42, "42" })]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expectedResult);

        static ExpectedResult expectedResult(Compilation compilation, AttributeSyntax syntax)
        {
            var stringType = compilation.GetSpecialType(SpecialType.System_String);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            var t1Location = ExpectedLocation.TypeArgument(syntax, 0);
            var t2Location = ExpectedLocation.TypeArgument(syntax, 1);
            var singleValueLocation = ExpectedLocation.SingleArgument(syntax, 0);
            var arrayValueLocation = ExpectedLocation.ArrayArgument(syntax, 1);
            var paramsValueLocation = ExpectedLocation.ParamsArgument(syntax, 2, 1);
            var namedSingleValueLocation = ExpectedLocation.SingleArgument(syntax, 3);
            var namedArrayValueLocation = ExpectedLocation.ArrayArgument(syntax, 4);

            return new ExpectedResult(stringType, intType, t1Location, t2Location)
            {
                SingleValue = "42",
                ArrayValue = Array.Empty<object>(),
                ParamsValue = new object[] { 42 },
                NamedSingleValue = intType,
                NamedArrayValue = new object[] { 42, "42" },
                SingleValueLocation = singleValueLocation,
                NamedSingleValueLocation = namedSingleValueLocation,
                ArrayValueLocation = arrayValueLocation,
                ParamsValueLocation = paramsValueLocation,
                NamedArrayValueLocation = namedArrayValueLocation
            };
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task Array_Recorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Combined<string, int>("42", new object[0], new object[] { "42", 42 }, NamedSingleValue = typeof(int), NamedArrayValue = new object[] { 42, "42" })]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expectedResult);

        static ExpectedResult expectedResult(Compilation compilation, AttributeSyntax syntax)
        {
            var stringType = compilation.GetSpecialType(SpecialType.System_String);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            var t1Location = ExpectedLocation.TypeArgument(syntax, 0);
            var t2Location = ExpectedLocation.TypeArgument(syntax, 1);
            var singleValueLocation = ExpectedLocation.SingleArgument(syntax, 0);
            var arrayValueLocation = ExpectedLocation.ArrayArgument(syntax, 1);
            var paramsValueLocation = ExpectedLocation.ArrayArgument(syntax, 2);
            var namedSingleValueLocation = ExpectedLocation.SingleArgument(syntax, 3);
            var namedArrayValueLocation = ExpectedLocation.ArrayArgument(syntax, 4);

            return new ExpectedResult(stringType, intType, t1Location, t2Location)
            {
                SingleValue = "42",
                ArrayValue = Array.Empty<object>(),
                ParamsValue = new object[] { "42", 42 },
                NamedSingleValue = intType,
                NamedArrayValue = new object[] { 42, "42" },
                SingleValueLocation = singleValueLocation,
                NamedSingleValueLocation = namedSingleValueLocation,
                ArrayValueLocation = arrayValueLocation,
                ParamsValueLocation = paramsValueLocation,
                NamedArrayValueLocation = namedArrayValueLocation
            };
        }
    }

    [AssertionMethod]
    private static async Task TrueAndIdenticalToExpected(ISyntacticAttributeParser parser, string source, Func<Compilation, AttributeSyntax, ExpectedResult> expectedDelegate)
    {
        SyntacticCombinedAttributeRecorder recorder = new();

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

        Assert.Equal(expected.SingleValue, recorder.SingleValue);
        Assert.True(recorder.SingleValueRecorded);
        Assert.Equal(expected.SingleValueLocation, recorder.SingleValueLocation);

        Assert.Equal(expected.NamedSingleValue, recorder.NamedSingleValue);
        Assert.True(recorder.NamedSingleValueRecorded);
        Assert.Equal(expected.NamedSingleValueLocation, recorder.NamedSingleValueLocation);

        Assert.Equal(expected.ArrayValue, recorder.ArrayValue);
        Assert.True(recorder.ArrayValueRecorded);
        Assert.Equal(expected.ArrayValueLocation?.Collection, recorder.ArrayValueLocation?.Collection);
        Assert.Equal(expected.ArrayValueLocation?.Elements, recorder.ArrayValueLocation?.Elements);

        Assert.Equal(expected.ParamsValue, recorder.ParamsValue);
        Assert.True(recorder.ParamsValueRecorded);
        Assert.Equal(expected.ParamsValueLocation?.Collection, recorder.ParamsValueLocation?.Collection);
        Assert.Equal(expected.ParamsValueLocation?.Elements, recorder.ParamsValueLocation?.Elements);

        Assert.Equal(expected.NamedArrayValue, recorder.NamedArrayValue);
        Assert.True(recorder.NamedArrayValueRecorded);
        Assert.Equal(expected.NamedArrayValueLocation?.Collection, recorder.NamedArrayValueLocation?.Collection);
        Assert.Equal(expected.NamedArrayValueLocation?.Elements, recorder.NamedArrayValueLocation?.Elements);
    }

    private sealed class ExpectedResult
    {
        public ITypeSymbol T1 { get; }
        public ITypeSymbol T2 { get; }

        public Location T1Location { get; }
        public Location T2Location { get; }

        public object? SingleValue { get; init; }
        public Location? SingleValueLocation { get; init; }

        public IReadOnlyList<object?>? ArrayValue { get; init; }
        public CollectionLocation? ArrayValueLocation { get; init; }

        public IReadOnlyList<object?>? ParamsValue { get; init; }
        public CollectionLocation? ParamsValueLocation { get; init; }

        public object? NamedSingleValue { get; init; }
        public Location? NamedSingleValueLocation { get; init; }

        public IReadOnlyList<object?>? NamedArrayValue { get; init; }
        public CollectionLocation? NamedArrayValueLocation { get; init; }

        public ExpectedResult(ITypeSymbol t1, ITypeSymbol t2, Location t1Location, Location t2Location)
        {
            T1 = t1;
            T2 = t2;

            T1Location = t1Location;
            T2Location = t2Location;
        }
    }
}
