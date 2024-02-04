namespace SharpAttributeParser.SyntacticParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using OneOf;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_Combined
{
    private bool Target(ISyntacticRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => Context.Parser.TryParse(recorder, attributeData, attributeSyntax);

    private readonly ParserContext Context = ParserContext.Create();

    [Fact]
    public async Task ParamsWithNamed_ReturnsTrueAndRecords()
    {
        var source = """
            [Combined<string, int>("42", new object[0], "42", 42, SimpleNamedValue = typeof(int), ArrayNamedValue = new object[] { 42, "42" })]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax)
        {
            IReadOnlyList<ExpressionSyntax> typeArguments = ((GenericNameSyntax)attributeSyntax.Name).TypeArgumentList.Arguments;
            IReadOnlyList<ExpressionSyntax> arguments = attributeSyntax.ArgumentList!.Arguments.Select(static (argumentSyntax) => argumentSyntax.Expression).ToList();

            return new ExpectedArguments(typeArguments[0], typeArguments[1], arguments[0], arguments[1], new[] { arguments[2], arguments[3] })
            {
                SimpleNamedValueSyntax = arguments[4],
                ArrayNamedValueSyntax = arguments[5]
            };
        }
    }

    [Fact]
    public async Task ParamsWithoutNamed_ReturnsTrueAndRecords()
    {
        var source = """
            [Combined<string, int>("42", new object[0], "42", 42)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax)
        {
            IReadOnlyList<ExpressionSyntax> typeArguments = ((GenericNameSyntax)attributeSyntax.Name).TypeArgumentList.Arguments;
            IReadOnlyList<ExpressionSyntax> arguments = attributeSyntax.ArgumentList!.Arguments.Select(static (argumentSyntax) => argumentSyntax.Expression).ToList();

            return new ExpectedArguments(typeArguments[0], typeArguments[1], arguments[0], arguments[1], new[] { arguments[2], arguments[3] });
        }
    }

    [Fact]
    public async Task EmptyParamsWithNamed_ReturnsTrueAndRecords()
    {
        var source = """
            [Combined<string, int>("42", new object[0], SimpleNamedValue = typeof(int), ArrayNamedValue = new object[] { 42, "42" })]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax)
        {
            IReadOnlyList<ExpressionSyntax> typeArguments = ((GenericNameSyntax)attributeSyntax.Name).TypeArgumentList.Arguments;
            IReadOnlyList<ExpressionSyntax> arguments = attributeSyntax.ArgumentList!.Arguments.Select(static (argumentSyntax) => argumentSyntax.Expression).ToList();

            return new ExpectedArguments(typeArguments[0], typeArguments[1], arguments[0], arguments[1], Array.Empty<ExpressionSyntax>())
            {
                SimpleNamedValueSyntax = arguments[2],
                ArrayNamedValueSyntax = arguments[3]
            };
        }
    }

    [Fact]
    public async Task EmptyParamsWithoutNamed_ReturnsTrueAndRecords()
    {
        var source = """
            [Combined<string, int>("42", new object[0])]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax)
        {
            IReadOnlyList<ExpressionSyntax> typeArguments = ((GenericNameSyntax)attributeSyntax.Name).TypeArgumentList.Arguments;
            IReadOnlyList<ExpressionSyntax> arguments = attributeSyntax.ArgumentList!.Arguments.Select(static (argumentSyntax) => argumentSyntax.Expression).ToList();

            return new ExpectedArguments(typeArguments[0], typeArguments[1], arguments[0], arguments[1], Array.Empty<ExpressionSyntax>());
        }
    }

    [Fact]
    public async Task OneParamsElement_ReturnsTrueAndRecords()
    {
        var source = """
            [Combined<string, int>("42", new object[0], 42, SimpleNamedValue = typeof(int), ArrayNamedValue = new object[] { 42, "42" })]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax)
        {
            IReadOnlyList<ExpressionSyntax> typeArguments = ((GenericNameSyntax)attributeSyntax.Name).TypeArgumentList.Arguments;
            IReadOnlyList<ExpressionSyntax> arguments = attributeSyntax.ArgumentList!.Arguments.Select(static (argumentSyntax) => argumentSyntax.Expression).ToList();

            return new ExpectedArguments(typeArguments[0], typeArguments[1], arguments[0], arguments[1], new[] { arguments[2] })
            {
                SimpleNamedValueSyntax = arguments[3],
                ArrayNamedValueSyntax = arguments[4]
            };
        }
    }

    [Fact]
    public async Task OneArrayValuedParamsElement_ReturnsTrueAndRecords()
    {
        var source = """
            [Combined<string, int>("42", new object[0], new int[0])]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax)
        {
            IReadOnlyList<ExpressionSyntax> typeArguments = ((GenericNameSyntax)attributeSyntax.Name).TypeArgumentList.Arguments;
            IReadOnlyList<ExpressionSyntax> arguments = attributeSyntax.ArgumentList!.Arguments.Select(static (argumentSyntax) => argumentSyntax.Expression).ToList();

            return new ExpectedArguments(typeArguments[0], typeArguments[1], arguments[0], arguments[1], new[] { arguments[2] });
        }
    }

    [Fact]
    public async Task ArrayWithNamed_ReturnsTrueAndRecords()
    {
        var source = """
            [Combined<string, int>("42", new object[0], new object[] { "42", 42 }, SimpleNamedValue = typeof(int), ArrayNamedValue = new object[] { 42, "42" })]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax)
        {
            var typeArguments = ((GenericNameSyntax)attributeSyntax.Name).TypeArgumentList.Arguments;
            IReadOnlyList<ExpressionSyntax> arguments = attributeSyntax.ArgumentList!.Arguments.Select(static (argumentSyntax) => argumentSyntax.Expression).ToList();

            return new ExpectedArguments(typeArguments[0], typeArguments[1], arguments[0], arguments[1], arguments[2])
            {
                SimpleNamedValueSyntax = arguments[3],
                ArrayNamedValueSyntax = arguments[4]
            };
        }
    }

    [Fact]
    public async Task ArrayWithoutNamed_ReturnsTrueAndRecords()
    {
        var source = """
            [Combined<string, int>("42", new object[0], new object[] { "42", 42 })]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax)
        {
            var typeArguments = ((GenericNameSyntax)attributeSyntax.Name).TypeArgumentList.Arguments;
            IReadOnlyList<ExpressionSyntax> arguments = attributeSyntax.ArgumentList!.Arguments.Select(static (argumentSyntax) => argumentSyntax.Expression).ToList();

            return new ExpectedArguments(typeArguments[0], typeArguments[1], arguments[0], arguments[1], arguments[2]);
        }
    }

    [AssertionMethod]
    private async Task ReturnsTrueAndRecords(string source, Func<AttributeSyntax, ExpectedArguments> expectedArgumentsDelegate)
    {
        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var typeParameters = attributeData.AttributeClass!.TypeParameters;

        var constructorParameters = attributeData.AttributeConstructor!.Parameters;

        var expectedArguments = expectedArgumentsDelegate(attributeSyntax);

        var outcome = Target(Context.Recorder, attributeData, attributeSyntax);

        Assert.True(outcome);

        Context.RecorderMock.Verify((recorder) => recorder.Type.TryRecordArgument(typeParameters[0], expectedArguments.T1Syntax), Times.Once);
        Context.RecorderMock.Verify((recorder) => recorder.Type.TryRecordArgument(typeParameters[1], expectedArguments.T2Syntax), Times.Once);

        Context.RecorderMock.Verify((recorder) => recorder.Constructor.TryRecordArgument(constructorParameters[0], expectedArguments.SimpleValueSyntax!), Times.Once);
        Context.RecorderMock.Verify((recorder) => recorder.Constructor.TryRecordArgument(constructorParameters[1], It.Is<ExpressionSyntax>(expectedArguments.ArrayValueSyntax!, SequenceEqualityComparer.Instance)), Times.Once);

        expectedArguments.ParamsValueSyntax.Switch
        (
            (syntax) => Context.RecorderMock.Verify((recorder) => recorder.Constructor.TryRecordArgument(constructorParameters[2], syntax), Times.Once),
            (elementSyntax) => Context.RecorderMock.Verify((recorder) => recorder.Constructor.TryRecordParamsArgument(constructorParameters[2], It.Is<IReadOnlyList<ExpressionSyntax>>(elementSyntax, SequenceEqualityComparer.Instance)), Times.Once)
        );

        Context.RecorderMock.Verify((recorder) => recorder.Named.TryRecordArgument(nameof(CombinedAttribute<object, object>.SimpleNamedValue), expectedArguments.SimpleNamedValueSyntax!), Times.Exactly(expectedArguments.SimpleNamedValueSyntaxSetCount));
        Context.RecorderMock.Verify((recorder) => recorder.Named.TryRecordArgument(nameof(CombinedAttribute<object, object>.ArrayNamedValue), expectedArguments.ArrayNamedValueSyntax!), Times.Exactly(expectedArguments.ArrayNamedValueSyntaxSetCount));
    }

    private sealed class ExpectedArguments
    {
        public ExpressionSyntax T1Syntax { get; }
        public ExpressionSyntax T2Syntax { get; }

        public ExpressionSyntax? SimpleValueSyntax { get; }
        public ExpressionSyntax? ArrayValueSyntax { get; }
        public OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> ParamsValueSyntax { get; }

        public ExpressionSyntax? SimpleNamedValueSyntax
        {
            get => RawSimpleNamedValueSyntax;
            init
            {
                RawSimpleNamedValueSyntax = value;
                SimpleNamedValueSyntaxSet = true;
            }
        }

        public ExpressionSyntax? ArrayNamedValueSyntax
        {
            get => RawArrayNamedValueSyntax;
            init
            {
                RawArrayNamedValueSyntax = value;
                ArrayNamedValueSyntaxSet = true;
            }
        }

        public int SimpleNamedValueSyntaxSetCount => SimpleNamedValueSyntaxSet ? 1 : 0;
        public int ArrayNamedValueSyntaxSetCount => ArrayNamedValueSyntaxSet ? 1 : 0;

        private readonly bool SimpleNamedValueSyntaxSet;
        private readonly bool ArrayNamedValueSyntaxSet;

        private readonly ExpressionSyntax? RawSimpleNamedValueSyntax;
        private readonly ExpressionSyntax? RawArrayNamedValueSyntax;

        public ExpectedArguments(ExpressionSyntax t1syntax, ExpressionSyntax t2syntax, ExpressionSyntax simpleValueSyntax, ExpressionSyntax arrayValueSyntax, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> paramsValueSyntax)
        {
            T1Syntax = t1syntax;
            T2Syntax = t2syntax;

            SimpleValueSyntax = simpleValueSyntax;
            ArrayValueSyntax = arrayValueSyntax;
            ParamsValueSyntax = paramsValueSyntax;
        }
    }
}
