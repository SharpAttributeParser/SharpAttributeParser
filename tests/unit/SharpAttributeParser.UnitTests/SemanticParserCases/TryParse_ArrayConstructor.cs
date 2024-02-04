namespace SharpAttributeParser.SemanticParserCases;

using Microsoft.CodeAnalysis;

using Moq;

using SharpAttributeParser.Logging;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_ArrayConstructor
{
    private bool Target(ISemanticRecorder recorder, AttributeData attributeData) => Context.Parser.TryParse(recorder, attributeData);

    private readonly ParserContext Context = ParserContext.Create();

    [Fact]
    public async Task ErrorArguments_ReturnsFalseAndLogs()
    {
        var source = """
            [ArrayConstructor((object[])4, (object[])5)]
            public class Foo { }
            """;

        await ReturnsFalseAndLogs(source, static (logger) => logger.Constructor.InvalidConstructorArgument());
    }

    [Fact]
    public async Task NullLiterals_ReturnsTrueAndRecords()
    {
        var source = """
            [ArrayConstructor(null, null)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new(null, null);
    }

    [Fact]
    public async Task DefaultLiterals_ReturnsTrueAndRecords()
    {
        var source = """
            [ArrayConstructor(default, default)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new(null, null);
    }

    [Fact]
    public async Task DefaultExpressions_ReturnsTrueAndRecords()
    {
        var source = """
            [ArrayConstructor(default(object[]), default(object[]))]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new(null, null);
    }

    [Fact]
    public async Task EmptyArrays_ReturnsTrueAndRecords()
    {
        var source = """
            [ArrayConstructor(new object[] { }, new object[] { })]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new(Array.Empty<object?>(), Array.Empty<object?>());
    }

    [Fact]
    public async Task PopulatedArrays_ReturnsTrueAndRecords()
    {
        var source = """
            [ArrayConstructor(new object[] { "42", null, typeof(int), nameof(Foo) }, new object[] { ((42)), (double)(float)42, new object[] { "42", 42 } })]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation)
        {
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            return new(new object?[] { "42", null, intType, "Foo" }, new object?[] { 42, (double)42, new object[] { "42", 42 } });
        }
    }

    [Fact]
    public async Task Labelled_ReturnsTrueAndRecords()
    {
        var source = """
            [ArrayConstructor(valueA: new object[] { "42", null, typeof(int), nameof(Foo) }, valueB: new object[] { ((42)), (double)(float)42, new object[] { "42", 42 } })]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation)
        {
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            return new(new object?[] { "42", null, intType, "Foo" }, new object?[] { 42, (double)42, new object[] { "42", 42 } });
        }
    }

    [Fact]
    public async Task ReverseOrder_ReturnsTrueAndRecords()
    {
        var source = """
            [ArrayConstructor(valueB: new object?[] { 43 }, valueA: new object?[] { 42 })]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new(new object[] { 42 }, new object[] { 43 });
    }

    [AssertionMethod]
    private async Task ReturnsTrueAndRecords(string source, Func<Compilation, ExpectedArguments> expectedArgumentsDelegate)
    {
        var (compilation, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var constructorParameters = attributeData.AttributeConstructor!.Parameters;

        var expectedArguments = expectedArgumentsDelegate(compilation);

        var outcome = Target(Context.Recorder, attributeData);

        Assert.True(outcome);

        Context.RecorderMock.Verify((recorder) => recorder.Constructor.TryRecordArgument(constructorParameters[0], It.Is(expectedArguments.ValueA, SequenceEqualityComparer.Instance)), Times.Once);
        Context.RecorderMock.Verify((recorder) => recorder.Constructor.TryRecordArgument(constructorParameters[1], It.Is(expectedArguments.ValueB, SequenceEqualityComparer.Instance)), Times.Once);
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
        public IReadOnlyList<object?>? ValueA { get; }
        public IReadOnlyList<object?>? ValueB { get; }

        public ExpectedArguments(IReadOnlyList<object?>? valueA, IReadOnlyList<object?>? valueB)
        {
            ValueA = valueA;
            ValueB = valueB;
        }
    }
}
