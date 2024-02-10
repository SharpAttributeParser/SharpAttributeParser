namespace SharpAttributeParser.SyntacticParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Logging;

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_Syntactic
{
    private bool Target(ISyntacticRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => Context.Parser.TryParse(recorder, attributeData, attributeSyntax);

    private readonly ParserContext Context = ParserContext.Create();

    [Fact]
    public async Task NonGenericSyntax_ReturnsFalseAndLogs()
    {
        var semanticSource = """
            [Generic<int, string>]
            public class Foo { }
            """;

        var syntacticSource = """
            [Generic]
            public class Foo { }
            """;

        await Generic_ReturnsFalseAndLogs(semanticSource, syntacticSource, static (logger) => logger.Type.SyntaxNotRecognizedAsGenericExpression());
    }

    [Fact]
    public async Task TooFewGenericArguments_ReturnsFalseAndLogs()
    {
        var semanticSource = """
            [Generic<int, string>]
            public class Foo { }
            """;

        var syntacticSource = """
            [Generic<int>]
            public class Foo { }
            """;

        await Generic_ReturnsFalseAndLogs(semanticSource, syntacticSource, static (logger) => logger.Type.UnexpectedNumberOfSyntacticTypeArguments());
    }

    [Fact]
    public async Task TooManyGenericArguments_FalseAndLoggee()
    {
        var semanticSource = """
            [Generic<int, string>]
            public class Foo { }
            """;

        var syntacticSource = """
            [Generic<int, string, bool>]
            public class Foo { }
            """;

        await Generic_ReturnsFalseAndLogs(semanticSource, syntacticSource, static (logger) => logger.Type.UnexpectedNumberOfSyntacticTypeArguments());
    }

    [Fact]
    public async Task MissingRequiredArgument_ReturnsFalseAndLogs()
    {
        var semanticSource = """
            [SimpleConstructor(4, 5)]
            public class Foo { }
            """;

        var syntacticSource = """
            [SimpleConstructor]
            public class Foo { }
            """;

        await Generic_ReturnsFalseAndLogs(semanticSource, syntacticSource, static (logger) => logger.Constructor.MissingOneOrMoreRequiredConstructorArgument());
    }

    [Fact]
    public async Task OutOfOrderLabelledFollowedByUnlabelled_ReturnsFalseAndLogs()
    {
        var semanticSource = """
            [SimpleConstructor(valueB: 42, valueA: "42")]
            public class Foo { }
            """;

        var syntacticSource = """
            [SimpleConstructor(valueB: 42, "42")]
            public class Foo { }
            """;

        await Generic_ReturnsFalseAndLogs(semanticSource, syntacticSource, static (logger) => logger.Constructor.OutOfOrderLabelledConstructorArgumentsFollowedByUnlabelled());
    }

    [Fact]
    public async Task ConstructorArgumentsMixedWithNamedArguments_ReturnsFalseAndLogs()
    {
        var semanticSource = """
            [SimpleConstructor(42, "42")]
            public class Foo { }
            """;

        var syntacticSource = """
            [SimpleConstructor(42, Named = 42, "42")]
            public class Foo { }
            """;

        await Generic_ReturnsFalseAndLogs(semanticSource, syntacticSource, static (logger) => logger.Constructor.MissingOneOrMoreRequiredConstructorArgument());
    }

    [Fact]
    public async Task NonExistingConstructorArgument_ReturnsFalseAndLogs()
    {
        var semanticSource = """
            [SimpleConstructor(42, "42")]
            public class Foo { }
            """;

        var syntacticSource = """
            [SimpleConstructor(42, nonExisting: "42")]
            public class Foo { }
            """;

        await Generic_ReturnsFalseAndLogs(semanticSource, syntacticSource, static (logger) => logger.Constructor.MissingOneOrMoreRequiredConstructorArgument());
    }

    [Fact]
    public async Task DuplicateConstructorArgument_ReturnsFalseAndLogs()
    {
        var semanticSource = """
            [SimpleConstructor(42, "42")]
            public class Foo { }
            """;

        var syntacticSource = """
            [SimpleConstructor(42, valueA: 42)]
            public class Foo { }
            """;

        await Generic_ReturnsFalseAndLogs(semanticSource, syntacticSource, static (logger) => logger.Constructor.MissingOneOrMoreRequiredConstructorArgument());
    }

    [Fact]
    public async Task InOrderLabelledFollowedByUnlabelled_ReturnsTrueAndRecords()
    {
        var source = """
            [SimpleConstructor(valueA: 42, "42")]
            public class Foo { }
            """;

        await SimpleConstructor_ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedSimpleConstructorArguments expectedArguments(AttributeSyntax attributeSyntax)
        {
            var arguments = attributeSyntax.ArgumentList!.Arguments.Select(static (argumentSyntax) => argumentSyntax.Expression).ToList();

            return new(arguments[0], arguments[1]);
        }
    }

    [Fact]
    public async Task NonExistingNamedParameter_ReturnsTrueAndRecords()
    {
        var source = """
            [SimpleConstructor(42, "42", NonExisting = 4)]
            public class Foo { }
            """;

        await SimpleConstructor_ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedSimpleConstructorArguments expectedArguments(AttributeSyntax attributeSyntax)
        {
            var arguments = attributeSyntax.ArgumentList!.Arguments.Select(static (argumentSyntax) => argumentSyntax.Expression).ToList();

            return new(arguments[0], arguments[1]);
        }
    }

    [Fact]
    public async Task DuplicateNamedArgument_ReturnsTrueAndRecords()
    {
        var semanticSource = """
            [Named(SimpleValue = 42)]
            public class Foo { }
            """;

        var syntacticSource = """
            [Named(SimpleValue = 42, SimpleValue = "42")]
            public class Foo { }
            """;

        await Named_ReturnsTrueAndRecords(semanticSource, syntacticSource, expectedArguments);

        static ExpectedNamedResult expectedArguments(AttributeSyntax attributeSyntax)
        {
            var arguments = attributeSyntax.ArgumentList!.Arguments.Select(static (argumentSyntax) => argumentSyntax.Expression).ToList();

            return new() { SimpleValueSyntax = arguments[0] };
        }
    }

    [AssertionMethod]
    private async Task Generic_ReturnsFalseAndLogs(string semanticSource, string syntacticSource, Expression<Action<ISyntacticParserLogger<SyntacticParser>>> loggerExpression)
    {
        var (_, attributeData, _) = await CompilationStore.GetComponents(semanticSource, "Foo");
        var (_, _, attributeSyntax) = await CompilationStore.GetComponents(syntacticSource, "Foo");

        var outcome = Target(Context.Recorder, attributeData, attributeSyntax);

        Assert.False(outcome);

        Context.LoggerMock.Verify(loggerExpression, Times.Once);
    }

    [AssertionMethod]
    private async Task SimpleConstructor_ReturnsTrueAndRecords(string source, Func<AttributeSyntax, ExpectedSimpleConstructorArguments> expectedArgumentsDelegate)
    {
        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var constructorParameters = attributeData.AttributeConstructor!.Parameters;

        var expectedArguments = expectedArgumentsDelegate(attributeSyntax);

        var outcome = Target(Context.Recorder, attributeData, attributeSyntax);

        Assert.True(outcome);

        Context.RecorderMock.Verify((recorder) => recorder.Constructor.Normal.TryRecordArgument(constructorParameters[0], expectedArguments.ValueASyntax), Times.Once);
        Context.RecorderMock.Verify((recorder) => recorder.Constructor.Normal.TryRecordArgument(constructorParameters[1], expectedArguments.ValueBSyntax), Times.Once);
    }

    [AssertionMethod]
    private async Task Named_ReturnsTrueAndRecords(string semanticSource, string syntacticSource, Func<AttributeSyntax, ExpectedNamedResult> expectedArgumentsDelegate)
    {
        var (_, attributeData, _) = await CompilationStore.GetComponents(semanticSource, "Foo");
        var (_, _, attributeSyntax) = await CompilationStore.GetComponents(syntacticSource, "Foo");

        var expectedArguments = expectedArgumentsDelegate(attributeSyntax);

        var outcome = Target(Context.Recorder, attributeData, attributeSyntax);

        Assert.True(outcome);

        Context.RecorderMock.Verify((recorder) => recorder.Named.TryRecordArgument(nameof(NamedAttribute.SimpleValue), expectedArguments.SimpleValueSyntax!), Times.Exactly(expectedArguments.SimpleValueSyntaxSetCount));
    }

    private sealed class ExpectedSimpleConstructorArguments
    {
        public ExpressionSyntax ValueASyntax { get; }
        public ExpressionSyntax ValueBSyntax { get; }

        public ExpectedSimpleConstructorArguments(ExpressionSyntax valueASyntax, ExpressionSyntax valueBSyntax)
        {
            ValueASyntax = valueASyntax;
            ValueBSyntax = valueBSyntax;
        }
    }

    private sealed class ExpectedNamedResult
    {
        public ExpressionSyntax? SimpleValueSyntax
        {
            get => RawSimpleValueSyntax;
            init
            {
                RawSimpleValueSyntax = value;
                SimpleValueSyntaxSet = true;
            }
        }

        public int SimpleValueSyntaxSetCount => SimpleValueSyntaxSet ? 1 : 0;

        private readonly bool SimpleValueSyntaxSet;

        private readonly ExpressionSyntax? RawSimpleValueSyntax;
    }
}
