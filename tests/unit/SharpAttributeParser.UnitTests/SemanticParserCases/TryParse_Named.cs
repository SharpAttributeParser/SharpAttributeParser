namespace SharpAttributeParser.SemanticParserCases;

using Microsoft.CodeAnalysis;

using Moq;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_Named
{
    private bool Target(ISemanticRecorder recorder, AttributeData attributeData) => Context.Parser.TryParse(recorder, attributeData);

    private ParserContext Context { get; } = ParserContext.Create();

    [Fact]
    public async Task ExpectedOrder_ReturnsTrueAndRecords()
    {
        var source = """
            [Named(SimpleValue = default, ArrayValue = default)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new() { SimpleValue = null, ArrayValue = null };
    }

    [Fact]
    public async Task ReverseOrder_ReturnsTrueAndRecords()
    {
        var source = """
            [Named(ArrayValue = default, SimpleValue = default)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new() { SimpleValue = null, ArrayValue = null };
    }

    [AssertionMethod]
    private async Task ReturnsTrueAndRecords(string source, Func<Compilation, ExpectedArguments> expectedArgumentsDelegate)
    {
        var (compilation, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var expectedArguments = expectedArgumentsDelegate(compilation);

        var outcome = Target(Context.Recorder, attributeData);

        Assert.True(outcome);

        Context.RecorderMock.Verify((recorder) => recorder.NamedArgument.TryRecordArgument(nameof(NamedAttribute.SimpleValue), It.Is(expectedArguments.SimpleValue, SequenceEqualityComparer.Instance)), Times.Exactly(expectedArguments.SimpleValueSetCount));
        Context.RecorderMock.Verify((recorder) => recorder.NamedArgument.TryRecordArgument(nameof(NamedAttribute.ArrayValue), It.Is(expectedArguments.ArrayValue, SequenceEqualityComparer.Instance)), Times.Exactly(expectedArguments.ArrayValueSetCount));
    }

    private sealed class ExpectedArguments
    {
        public object? SimpleValue
        {
            get => RawSimpleValue;
            init
            {
                RawSimpleValue = value;
                SimpleValueSet = true;
            }
        }

        public IReadOnlyList<object?>? ArrayValue
        {
            get => RawArrayValue;
            init
            {
                RawArrayValue = value;
                ArrayValueSet = true;
            }
        }

        public int SimpleValueSetCount => SimpleValueSet ? 1 : 0;
        public int ArrayValueSetCount => ArrayValueSet ? 1 : 0;

        private bool SimpleValueSet { get; init; }
        private bool ArrayValueSet { get; init; }

        private object? RawSimpleValue { get; init; }
        private IReadOnlyList<object?>? RawArrayValue { get; init; }
    }
}
