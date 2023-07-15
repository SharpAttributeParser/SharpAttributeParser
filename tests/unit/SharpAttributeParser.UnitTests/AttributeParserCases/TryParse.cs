namespace SharpAttributeParser.AttributeParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Recording;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse
{
    private IGenericAttributeRecorderFactory RecorderFactory { get; }

    public TryParse(IGenericAttributeRecorderFactory recorderFactory)
    {
        RecorderFactory = recorderFactory;
    }

    private static bool Target(IAttributeParser parser, IAttributeRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => parser.TryParse(recorder, attributeData, attributeSyntax);

    [Theory]
    [ClassData(typeof(ParserSources))]
    public void NullRecorder_ArgumentNullException(IAttributeParser parser)
    {
        var exception = Record.Exception(() => Target(parser, null!, Mock.Of<AttributeData>(), AttributeSyntaxFactory.Create()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public void NullAttributeData_ArgumentNullException(IAttributeParser parser)
    {
        var exception = Record.Exception(() => Target(parser, RecorderFactory.Create(), null!, AttributeSyntaxFactory.Create()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public void NullAttributeSyntax_ArgumentNullException(IAttributeParser parser)
    {
        var exception = Record.Exception(() => Target(parser, RecorderFactory.Create(), Mock.Of<AttributeData>(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NonExistingAttribute_FalseAndNotRecorded(IAttributeParser parser)
    {
        var source = """
            [NonExisting]
            public class Foo { }
            """;

        await Generic_FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NonExistingConstructor_FalseAndNotRecorded(IAttributeParser parser)
    {
        var source = """
            [Generic<string, int>(4)]
            public class Foo { }
            """;

        await Generic_FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NonExistingNamedParameter_TrueAndRecorded(IAttributeParser parser)
    {
        var source = """
            [Generic<string, int>(NonExisting = 4)]
            public class Foo { }
            """;

        await Generic_TrueAndRecorded(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax attributeSyntax)
        {
            var stringType = compilation.GetSpecialType(SpecialType.System_String);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            IReadOnlyList<ExpressionSyntax> typeArgumentSyntaxes = ((GenericNameSyntax)attributeSyntax.Name).TypeArgumentList.Arguments;

            return new(stringType, typeArgumentSyntaxes[0], intType, typeArgumentSyntaxes[1]);
        }
    }

    [AssertionMethod]
    private async Task Generic_TrueAndRecorded(IAttributeParser parser, string source, Func<Compilation, AttributeSyntax, ExpectedResult> expectedDelegate)
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
    private async Task Generic_FalseAndNotRecorded(IAttributeParser parser, string source)
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
