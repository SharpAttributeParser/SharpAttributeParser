namespace SharpAttributeParser.SemanticParserCases;

using Microsoft.CodeAnalysis;

using Moq;

using System;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_SimpleNamed
{
    private bool Target(ISemanticRecorder recorder, AttributeData attributeData) => Context.Parser.TryParse(recorder, attributeData);

    private readonly ParserContext Context;

    public TryParse_SimpleNamed()
    {
        Context = ParserContext.Create();
    }

    [Fact]
    public async Task ErrorArgument_TrueButNotRecorded()
    {
        var source = """
            [Named(SimpleValue = (string)4)]
            public class Foo { }
            """;

        await TrueButNotRecorded(source);
    }

    [Fact]
    public async Task NullLiteral_ReturnsTrueAndRecords()
    {
        var source = """
            [Named(SimpleValue = null)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new() { Value = null };
    }

    [Fact]
    public async Task DefaultLiteral_ReturnsTrueAndRecords()
    {
        var source = """
            [Named(SimpleValue = default)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new() { Value = null };
    }

    [Fact]
    public async Task DefaultExpression_ReturnsTrueAndRecords()
    {
        var source = """
            [Named(SimpleValue = default(object[]))]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new() { Value = null };
    }

    [Fact]
    public async Task IntEnumValue_ReturnsTrueAndRecords()
    {
        var source = """
            [Named(SimpleValue = IntEnum.Some)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new() { Value = (int)IntEnum.Some };
    }

    [Fact]
    public async Task UIntEnumValues_ReturnsTrueAndRecords()
    {
        var source = """
            [Named(SimpleValue = UIntEnum.Some)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new() { Value = (uint)UIntEnum.Some };
    }

    [Fact]
    public async Task LongEnumValues_ReturnsTrueAndRecords()
    {
        var source = """
            [Named(SimpleValue = LongEnum.Some)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new() { Value = (long)LongEnum.Some };
    }

    [Fact]
    public async Task ULongEnumValues_ReturnsTrueAndRecords()
    {
        var source = """
            [Named(SimpleValue = ULongEnum.Some)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new() { Value = (ulong)ULongEnum.Some };
    }

    [Fact]
    public async Task ShortEnumValues_ReturnsTrueAndRecords()
    {
        var source = """
            [Named(SimpleValue = ShortEnum.Some)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new() { Value = (short)ShortEnum.Some };
    }

    [Fact]
    public async Task UShortEnumValues_ReturnsTrueAndRecords()
    {
        var source = """
            [Named(SimpleValue = UShortEnum.Some)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new() { Value = (ushort)UShortEnum.Some };
    }

    [Fact]
    public async Task ByteEnumValues_ReturnsTrueAndRecords()
    {
        var source = """
            [Named(SimpleValue = ByteEnum.Some)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new() { Value = (byte)ByteEnum.Some };
    }

    [Fact]
    public async Task SByteEnumValues_ReturnsTrueAndRecords()
    {
        var source = """
            [Named(SimpleValue = SByteEnum.Some)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new() { Value = (sbyte)SByteEnum.Some };
    }

    [AssertionMethod]
    private async Task ReturnsTrueAndRecords(string source, Func<Compilation, ExpectedArguments> expectedArgumentsDelegate)
    {
        var (compilation, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var expectedArguments = expectedArgumentsDelegate(compilation);

        var outcome = Target(Context.Recorder, attributeData);

        Assert.True(outcome);

        Context.RecorderMock.Verify((recorder) => recorder.NamedArgument.TryRecordArgument(nameof(NamedAttribute.SimpleValue), It.Is(expectedArguments.Value, SequenceEqualityComparer.Instance)), Times.Exactly(expectedArguments.ValueSetCount));
    }

    [AssertionMethod]
    private async Task TrueButNotRecorded(string source)
    {
        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var outcome = Target(Context.Recorder, attributeData);

        Assert.True(outcome);

        Context.RecorderMock.Verify(static (recorder) => recorder.NamedArgument.TryRecordArgument(It.IsAny<string>(), It.IsAny<object?>()), Times.Never);
    }

    private sealed class ExpectedArguments
    {
        public object? Value
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

        private readonly object? RawValue;
    }
}
