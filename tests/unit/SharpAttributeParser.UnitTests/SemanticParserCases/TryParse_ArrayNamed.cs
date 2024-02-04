namespace SharpAttributeParser.SemanticParserCases;

using Microsoft.CodeAnalysis;

using Moq;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_ArrayNamed
{
    private bool Target(ISemanticRecorder recorder, AttributeData attributeData) => Context.Parser.TryParse(recorder, attributeData);

    private readonly ParserContext Context = ParserContext.Create();

    [Fact]
    public async Task ErrorArgument_TrueButNotRecorded()
    {
        var source = """
            [Named(ArrayValue = (object[])4)]
            public class Foo { }
            """;

        await TrueButNotRecorded(source);
    }

    [Fact]
    public async Task NullLiteral_ReturnsTrueAndRecords()
    {
        var source = """
            [Named(ArrayValue = null)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new() { Value = null };
    }

    [Fact]
    public async Task DefaultLiteral_ReturnsTrueAndRecords()
    {
        var source = """
            [Named(ArrayValue = default)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new() { Value = null };
    }

    [Fact]
    public async Task DefaultExpression_ReturnsTrueAndRecords()
    {
        var source = """
            [Named(ArrayValue = default(object[]))]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new() { Value = null };
    }

    [Fact]
    public async Task EmptyArray_ReturnsTrueAndRecords()
    {
        var source = """
            [Named(ArrayValue = new object[0])]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new() { Value = Array.Empty<object?>() };
    }

    [Fact]
    public async Task PopulatedArray_ReturnsTrueAndRecords()
    {
        var source = """
            [Named(ArrayValue = new object[] { "42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 } })]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation)
        {
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            return new() { Value = new object?[] { "42", null, intType, "Foo", 42, (double)42, new object[] { "42", 42 } } };
        }
    }

    [AssertionMethod]
    private async Task ReturnsTrueAndRecords(string source, Func<Compilation, ExpectedArguments> expectedArgumentsDelegate)
    {
        var (compilation, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var expectedArguments = expectedArgumentsDelegate(compilation);

        var outcome = Target(Context.Recorder, attributeData);

        Assert.True(outcome);

        Context.RecorderMock.Verify((recorder) => recorder.Named.TryRecordArgument(nameof(NamedAttribute.ArrayValue), It.Is(expectedArguments.Value, SequenceEqualityComparer.Instance)), Times.Exactly(expectedArguments.ValueSetCount));
    }

    [AssertionMethod]
    private async Task TrueButNotRecorded(string source)
    {
        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var outcome = Target(Context.Recorder, attributeData);

        Assert.True(outcome);

        Context.RecorderMock.Verify(static (recorder) => recorder.Named.TryRecordArgument(It.IsAny<string>(), It.IsAny<object?>()), Times.Never);
    }

    private sealed class ExpectedArguments
    {
        public IReadOnlyList<object?>? Value
        {
            get => RawValue;
            init
            {
                RawValue = value;
                ValueSet = true;
            }
        }

        public int ValueSetCount => ValueSet ? 1 : 0;

        private readonly bool ValueSet;

        private readonly IReadOnlyList<object?>? RawValue;
    }
}
