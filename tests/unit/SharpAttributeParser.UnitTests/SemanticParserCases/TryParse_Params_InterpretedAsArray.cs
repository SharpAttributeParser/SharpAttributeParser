namespace SharpAttributeParser.SemanticParserCases;

using Microsoft.CodeAnalysis;

using Moq;

using SharpAttributeParser.Logging;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_Params_InterpretedAsArray
{
    private bool Target(ISemanticRecorder recorder, AttributeData attributeData) => Context.Parser.TryParse(recorder, attributeData);

    private readonly ParserContext Context = ParserContext.Create();

    [Fact]
    public async Task ErrorArgument_ReturnsFalseAndLogs()
    {
        var source = """
            [Params((object[])4)]
            public class Foo { }
            """;

        await ReturnsFalseAndLogs(source, static (logger) => logger.ConstructorArgument.InvalidConstructorArgument());
    }

    [Fact]
    public async Task NullLiteral_ReturnsTrueAndRecords()
    {
        var source = """
            [Params(null)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new(null);
    }

    [Fact]
    public async Task DefaultLiteral_ReturnsTrueAndRecords()
    {
        var source = """
            [Params(default)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new(null);
    }

    [Fact]
    public async Task DefaultExpression_ReturnsTrueAndRecords()
    {
        var source = """
            [Params(default(object[]))]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new(null);
    }

    [Fact]
    public async Task ObjectArrayCastedToObject_ReturnsTrueAndRecords()
    {
        var source = """
            [Params((object)(new object[] { 4 }))]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new(new object?[] { 4 });
    }

    [Fact]
    public async Task IntArrayCastedToObjectArray_ReturnsTrueAndRecords()
    {
        var source = """
            [Params((object[])(new int[] { 4 }))]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new(new object?[] { 4 });
    }

    [Fact]
    public async Task PopulatedArray_ReturnsTrueAndRecords()
    {
        var source = """
            [Params(new object[] { "42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 } })]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation)
        {
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            return new(new object?[] { "42", null, intType, "Foo", 42, (double)42, new object[] { "42", 42 } });
        }
    }

    [Fact]
    public async Task EmptyArray_ReturnsTrueAndRecords()
    {
        var source = """
            [Params(new object[0])]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new(Array.Empty<object?>());
    }

    [Fact]
    public async Task Labelled_ReturnsTrueAndRecords()
    {
        var source = """
            [Params(value: new object[] { "42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 } })]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation)
        {
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            return new(new object?[] { "42", null, intType, "Foo", 42, (double)42, new object[] { "42", 42 } });
        }
    }

    [AssertionMethod]
    private async Task ReturnsTrueAndRecords(string source, Func<Compilation, ExpectedArguments> expectedArgumentsDelegate)
    {
        var (compilation, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var constructorParameters = attributeData.AttributeConstructor!.Parameters;

        var expectedArguments = expectedArgumentsDelegate(compilation);

        var outcome = Target(Context.Recorder, attributeData);

        Assert.True(outcome);

        Context.RecorderMock.Verify((recorder) => recorder.ConstructorArgument.TryRecordArgument(constructorParameters[0], It.Is(expectedArguments.Value, SequenceEqualityComparer.Instance)), Times.Once);
    }

    [AssertionMethod]
    private async Task ReturnsFalseAndLogs(string source, Expression<Action<ISemanticParserLogger<SemanticParser>>> loggerExpression)
    {
        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var outcome = Target(Context.Recorder, attributeData);

        Assert.False(outcome);

        Context.LoggerMock.Verify(loggerExpression, Times.Once);
    }

    private sealed class ExpectedArguments
    {
        public IReadOnlyList<object?>? Value { get; }

        public ExpectedArguments(IReadOnlyList<object?>? value)
        {
            Value = value;
        }
    }
}
