namespace SharpAttributeParser.SemanticAttributeParserCases;

using Microsoft.CodeAnalysis;

using Moq;

using SharpAttributeParser.Recording;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_Generic
{
    private ISemanticGenericAttributeRecorderFactory RecorderFactory { get; }

    public TryParse_Generic(ISemanticGenericAttributeRecorderFactory recorderFactory)
    {
        RecorderFactory = recorderFactory;
    }

    private static bool Target(ISemanticAttributeParser parser, ISemanticAttributeRecorder recorder, AttributeData attributeData) => parser.TryParse(recorder, attributeData);

    [Theory]
    [ClassData(typeof(ParserSources))]
    [SuppressMessage("Minor Code Smell", "S1125: Boolean literals should not be redundant", Justification = "Prefer equality check over negation.")]
    public async Task FalseReturningRecorder_False(ISemanticAttributeParser parser)
    {
        var recorder = Mock.Of<ISemanticAttributeRecorder>(static (recorder) => recorder.TryRecordTypeArgument(It.IsAny<ITypeParameterSymbol>(), It.IsAny<ITypeSymbol>()) == false);

        var source = """
            [Generic<string, int>]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData);

        Assert.False(result);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NonExsitingAttribute_FalseAndNotRecorded(ISemanticAttributeParser parser)
    {
        var source = """
            [NonExisting]
            public class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NonExsitingConstructor_FalseAndNotRecorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Generic<string, int>(4)]
            public class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task ErrorArgument_FalseAndNotRecorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Generic<NonExisting, int>]
            public class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task WithoutBody_TrueAndRecorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Generic<string, int>]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation)
        {
            var stringType = compilation.GetSpecialType(SpecialType.System_String);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            return new(stringType, intType);
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task WithBody_TrueAndRecorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Generic<string, int>()]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation)
        {
            var stringType = compilation.GetSpecialType(SpecialType.System_String);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            return new(stringType, intType);
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task Tuple_TrueAndRecorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Generic<(string, int), (int, string)>]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation)
        {
            var stringType = compilation.GetSpecialType(SpecialType.System_String);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            var valueTupleType = compilation.GetTypeByMetadataName("System.ValueTuple`2")!;

            var t1 = valueTupleType.Construct(stringType, intType);
            var t2 = valueTupleType.Construct(intType, stringType);

            return new(t1, t2);
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task Qualified_TrueAndRecorded(ISemanticAttributeParser parser)
    {
        var source = """
            [SharpAttributeParser.QualifiedGeneric<string, int>]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation)
        {
            var stringType = compilation.GetSpecialType(SpecialType.System_String);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            return new(stringType, intType);
        }
    }

    [AssertionMethod]
    private async Task TrueAndRecordedAsExpected(ISemanticAttributeParser parser, string source, Func<Compilation, ExpectedResult> expectedDelegate)
    {
        var recorder = RecorderFactory.Create();

        var (compilation, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var expected = expectedDelegate(compilation);

        var outcome = Target(parser, recorder, attributeData);
        var result = recorder.GetResult();

        Assert.True(outcome);

        Assert.Equal(expected.T1, result.T1);
        Assert.True(result.T1Recorded);

        Assert.Equal(expected.T2, result.T2);
        Assert.True(result.T2Recorded);
    }

    [AssertionMethod]
    private async Task FalseAndNotRecorded(ISemanticAttributeParser parser, string source)
    {
        var recorder = RecorderFactory.Create();

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var outcome = Target(parser, recorder, attributeData);
        var result = recorder.GetResult();

        Assert.False(outcome);

        Assert.False(result.T1Recorded);
        Assert.False(result.T2Recorded);
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
