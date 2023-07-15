namespace SharpAttributeParser.AttributeParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Recording;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_Generic
{
    private IGenericAttributeRecorderFactory RecorderFactory { get; }

    public TryParse_Generic(IGenericAttributeRecorderFactory recorderFactory)
    {
        RecorderFactory = recorderFactory;
    }

    private static bool Target(IAttributeParser parser, IAttributeRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => parser.TryParse(recorder, attributeData, attributeSyntax);

    [Theory]
    [ClassData(typeof(ParserSources))]
    [SuppressMessage("Minor Code Smell", "S1125: Boolean literals should not be redundant", Justification = "Prefer equality check over negation.")]
    public async Task FalseReturningRecorder_False(IAttributeParser parser)
    {
        var recorder = Mock.Of<IAttributeRecorder>(static (recorder) => recorder.TryRecordTypeArgument(It.IsAny<ITypeParameterSymbol>(), It.IsAny<ITypeSymbol>(), It.IsAny<ExpressionSyntax>()) == false);

        var source = """
            [Generic<string, int>]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.False(result);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NonExsitingAttribute_FalseAndNotRecorded(IAttributeParser parser)
    {
        var source = """
            [NonExisting]
            public class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NonExsitingConstructor_FalseAndNotRecorded(IAttributeParser parser)
    {
        var source = """
            [Generic<string, int>(4)]
            public class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task ErrorArgument_FalseAndNotRecorded(IAttributeParser parser)
    {
        var source = """
            [Generic<NonExisting, int>]
            public class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task WithoutBody_TrueAndRecorded(IAttributeParser parser)
    {
        var source = """
            [Generic<string, int>]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax attributeSyntax)
        {
            var stringType = compilation.GetSpecialType(SpecialType.System_String);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            var typeArguments = ((GenericNameSyntax)attributeSyntax.Name).TypeArgumentList.Arguments;

            return new(stringType, typeArguments[0], intType, typeArguments[1]);
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task WithBody_TrueAndRecorded(IAttributeParser parser)
    {
        var source = """
            [Generic<string, int>()]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax attributeSyntax)
        {
            var stringType = compilation.GetSpecialType(SpecialType.System_String);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            var typeArguments = ((GenericNameSyntax)attributeSyntax.Name).TypeArgumentList.Arguments;

            return new(stringType, typeArguments[0], intType, typeArguments[1]);
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task Tuple_TrueAndRecorded(IAttributeParser parser)
    {
        var source = """
            [Generic<(string, int), (int, string)>]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax attributeSyntax)
        {
            var stringType = compilation.GetSpecialType(SpecialType.System_String);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            var valueTupleType = compilation.GetTypeByMetadataName("System.ValueTuple`2")!;

            var t1 = valueTupleType.Construct(stringType, intType);
            var t2 = valueTupleType.Construct(intType, stringType);

            var typeArguments = ((GenericNameSyntax)attributeSyntax.Name).TypeArgumentList.Arguments;

            return new(t1, typeArguments[0], t2, typeArguments[1]);
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task Qualified_TrueAndRecorded(IAttributeParser parser)
    {
        var source = """
            [SharpAttributeParser.QualifiedGeneric<string, int>]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax attributeSyntax)
        {
            var stringType = compilation.GetSpecialType(SpecialType.System_String);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            var typeArguments = ((GenericNameSyntax)((QualifiedNameSyntax)attributeSyntax.Name).Right).TypeArgumentList.Arguments;

            return new(stringType, typeArguments[0], intType, typeArguments[1]);
        }
    }

    [AssertionMethod]
    private async Task TrueAndRecordedAsExpected(IAttributeParser parser, string source, Func<Compilation, AttributeSyntax, ExpectedResult> expectedDelegate)
    {
        var recorder = RecorderFactory.Create();

        var (compilation, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var expected = expectedDelegate(compilation, attributeSyntax);

        var outcome = Target(parser, recorder, attributeData, attributeSyntax);
        var result = recorder.GetRecord();

        Assert.True(outcome);

        Assert.Equal(expected.T1, result.T1);
        Assert.Equal(expected.T1Syntax, result.T1Syntax);
        Assert.True(result.T1Recorded);

        Assert.Equal(expected.T2, result.T2);
        Assert.Equal(expected.T2Syntax, result.T2Syntax);
        Assert.True(result.T2Recorded);
    }

    [AssertionMethod]
    private async Task FalseAndNotRecorded(IAttributeParser parser, string source)
    {
        var recorder = RecorderFactory.Create();

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var outcome = Target(parser, recorder, attributeData, attributeSyntax);
        var result = recorder.GetRecord();

        Assert.False(outcome);

        Assert.False(result.T1Recorded);
        Assert.False(result.T2Recorded);
    }

    private sealed class ExpectedResult
    {
        public ITypeSymbol T1 { get; }
        public ExpressionSyntax T1Syntax { get; }

        public ITypeSymbol T2 { get; }
        public ExpressionSyntax T2Syntax { get; }

        public ExpectedResult(ITypeSymbol t1, ExpressionSyntax t1Syntax, ITypeSymbol t2, ExpressionSyntax t2Syntax)
        {
            T1 = t1;
            T1Syntax = t1Syntax;

            T2 = t2;
            T2Syntax = t2Syntax;
        }
    }
}
