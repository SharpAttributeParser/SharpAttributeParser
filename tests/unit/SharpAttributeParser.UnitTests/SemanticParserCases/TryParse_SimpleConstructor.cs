namespace SharpAttributeParser.SemanticParserCases;

using Microsoft.CodeAnalysis;

using Moq;

using SharpAttributeParser.Logging;

using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_SimpleConstructor
{
    private bool Target(ISemanticRecorder recorder, AttributeData attributeData) => Context.Parser.TryParse(recorder, attributeData);

    private ParserContext Context { get; } = ParserContext.Create();

    [Fact]
    public async Task ErrorArguments_ReturnsFalseAndLogs()
    {
        var source = """
            [SimpleConstructor((string)4, (string)5)]
            public class Foo { }
            """;

        await ReturnsFalseAndLogs(source, static (logger) => logger.ConstructorArgument.InvalidConstructorArgument());
    }

    [Fact]
    public async Task NullLiterals_ReturnsTrueAndRecords()
    {
        var source = """
            [SimpleConstructor(null, null)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new(null, null);
    }

    [Fact]
    public async Task DefaultLiterals_ReturnsTrueAndRecords()
    {
        var source = """
            [SimpleConstructor(default, default)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new(null, null);
    }

    [Fact]
    public async Task DefaultExpressions_ReturnsTrueAndRecords()
    {
        var source = """
            [SimpleConstructor(default(object), default(object))]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new(null, null);
    }

    [Fact]
    public async Task StringValues_ReturnsTrueAndRecords()
    {
        var source = """
            [SimpleConstructor("42", "43")]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new("42", "43");
    }

    [Fact]
    public async Task TypeValues_ReturnsTrueAndRecords()
    {
        var source = """
            [SimpleConstructor(typeof(int), typeof(string))]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new(compilation.GetSpecialType(SpecialType.System_Int32), compilation.GetSpecialType(SpecialType.System_String));
    }

    [Fact]
    public async Task IntEnumValues_ReturnsTrueAndRecords()
    {
        var source = """
            [SimpleConstructor(IntEnum.None, IntEnum.Some)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new((int)IntEnum.None, (int)IntEnum.Some);
    }

    [Fact]
    public async Task UIntEnumValues_ReturnsTrueAndRecords()
    {
        var source = """
            [SimpleConstructor(UIntEnum.None, UIntEnum.Some)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new((uint)UIntEnum.None, (uint)UIntEnum.Some);
    }

    [Fact]
    public async Task LongEnumValues_ReturnsTrueAndRecords()
    {
        var source = """
            [SimpleConstructor(LongEnum.None, LongEnum.Some)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new((long)LongEnum.None, (long)LongEnum.Some);
    }

    [Fact]
    public async Task ULongEnumValues_ReturnsTrueAndRecords()
    {
        var source = """
            [SimpleConstructor(ULongEnum.None, ULongEnum.Some)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new((ulong)ULongEnum.None, (ulong)ULongEnum.Some);
    }

    [Fact]
    public async Task ShortEnumValues_ReturnsTrueAndRecords()
    {
        var source = """
            [SimpleConstructor(ShortEnum.None, ShortEnum.Some)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new((short)ShortEnum.None, (short)ShortEnum.Some);
    }

    [Fact]
    public async Task UShortEnumValues_ReturnsTrueAndRecords()
    {
        var source = """
            [SimpleConstructor(UShortEnum.None, UShortEnum.Some)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new((ushort)UShortEnum.None, (ushort)UShortEnum.Some);
    }

    [Fact]
    public async Task ByteEnumValues_ReturnsTrueAndRecords()
    {
        var source = """
            [SimpleConstructor(ByteEnum.None, ByteEnum.Some)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new((byte)ByteEnum.None, (byte)ByteEnum.Some);
    }

    [Fact]
    public async Task SByteEnumValues_ReturnsTrueAndRecords()
    {
        var source = """
            [SimpleConstructor(SByteEnum.None, SByteEnum.Some)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new((sbyte)SByteEnum.None, (sbyte)SByteEnum.Some);
    }

    [Fact]
    public async Task ReverseOrder_ReturnsTrueAndRecords()
    {
        var source = """
            [SimpleConstructor(valueB: 43, valueA: 42)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new(42, 43);
    }

    [AssertionMethod]
    private async Task ReturnsTrueAndRecords(string source, Func<Compilation, ExpectedArguments> expectedArgumentsDelegate)
    {
        var (compilation, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var constructorParameters = attributeData.AttributeConstructor!.Parameters;

        var expectedArguments = expectedArgumentsDelegate(compilation);

        var outcome = Target(Context.Recorder, attributeData);

        Assert.True(outcome);

        Context.RecorderMock.Verify((recorder) => recorder.ConstructorArgument.TryRecordArgument(constructorParameters[0], It.Is(expectedArguments.ValueA, SequenceEqualityComparer.Instance)), Times.Once);
        Context.RecorderMock.Verify((recorder) => recorder.ConstructorArgument.TryRecordArgument(constructorParameters[1], It.Is(expectedArguments.ValueB, SequenceEqualityComparer.Instance)), Times.Once);
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
        public object? ValueA { get; }
        public object? ValueB { get; }

        public ExpectedArguments(object? valueA, object? valueB)
        {
            ValueA = valueA;
            ValueB = valueB;
        }
    }
}
