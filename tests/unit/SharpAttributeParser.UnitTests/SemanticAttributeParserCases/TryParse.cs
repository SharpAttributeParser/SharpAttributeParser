namespace SharpAttributeParser.SemanticAttributeParserCases;

using Microsoft.CodeAnalysis;

using Moq;

using SharpAttributeParser.Recording;

using System;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse
{
    private ISemanticGenericAttributeRecorderFactory RecorderFactory { get; }

    public TryParse(ISemanticGenericAttributeRecorderFactory recorderFactory)
    {
        RecorderFactory = recorderFactory;
    }

    private static bool Target(ISemanticAttributeParser parser, ISemanticAttributeRecorder recorder, AttributeData attributeData) => parser.TryParse(recorder, attributeData);

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
        var exception = Record.Exception(() => Target(parser, RecorderFactory.Create(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NonExistingAttribute_FalseAndNotRecorded(ISemanticAttributeParser parser)
    {
        var source = """
            [NonExisting]
            public class Foo { }
            """;

        await Generic_FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NonExistingConstructor_FalseAndNotRecorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Generic<string, int>(4)]
            public class Foo { }
            """;

        await Generic_FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NonExistingNamedParameter_TrueAndRecorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Generic<string, int>(NonExisting = 4)]
            public class Foo { }
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
    private async Task Generic_TrueAndRecorded(ISemanticAttributeParser parser, string source, Func<Compilation, ExpectedResult> expectedDelegate)
    {
        var recorder = RecorderFactory.Create();

        var (compilation, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var expected = expectedDelegate(compilation);

        var outcome = Target(parser, recorder, attributeData);
        var result = recorder.GetRecord();

        Assert.True(outcome);

        Assert.Equal(expected.T1, result.T1);
        Assert.True(result.T1Recorded);

        Assert.Equal(expected.T2, result.T2);
        Assert.True(result.T2Recorded);
    }

    [AssertionMethod]
    private async Task Generic_FalseAndNotRecorded(ISemanticAttributeParser parser, string source)
    {
        var recorder = RecorderFactory.Create();

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var outcome = Target(parser, recorder, attributeData);
        var result = recorder.GetRecord();

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
