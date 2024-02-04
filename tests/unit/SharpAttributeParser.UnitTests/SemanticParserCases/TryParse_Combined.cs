namespace SharpAttributeParser.SemanticParserCases;

using Microsoft.CodeAnalysis;

using Moq;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_Combined
{
    private bool Target(ISemanticRecorder recorder, AttributeData attributeData) => Context.Parser.TryParse(recorder, attributeData);

    private readonly ParserContext Context = ParserContext.Create();

    [Fact]
    public async Task ParamsWithNamed_ReturnsTrueAndRecords()
    {
        var source = """
            [Combined<string, int>("42", new object[0], "42", 42, SimpleNamedValue = typeof(int), ArrayNamedValue = new object[] { 42, "42" })]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation)
        {
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            return new("42", Array.Empty<object>(), new object[] { "42", 42 })
            {
                SimpleNamedValue = intType,
                ArrayNamedValue = new object[] { 42, "42" }
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

        static ExpectedArguments expectedArguments(Compilation compilation) => new("42", Array.Empty<object>(), new object[] { "42", 42 });
    }

    [Fact]
    public async Task EmptyParamsWithNamed_ReturnsTrueAndRecords()
    {
        var source = """
            [Combined<string, int>("42", new object[0], SimpleNamedValue = typeof(int), ArrayNamedValue = new object[] { 42, "42" })]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation)
        {
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            return new("42", Array.Empty<object>(), Array.Empty<object>())
            {
                SimpleNamedValue = intType,
                ArrayNamedValue = new object[] { 42, "42" }
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

        static ExpectedArguments expectedArguments(Compilation compilation) => new("42", Array.Empty<object>(), Array.Empty<object>());
    }

    [Fact]
    public async Task OneParamsElement_ReturnsTrueAndRecords()
    {
        var source = """
            [Combined<string, int>("42", new object[0], 42, SimpleNamedValue = typeof(int), ArrayNamedValue = new object[] { 42, "42" })]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation)
        {
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            return new("42", Array.Empty<object>(), new object[] { 42 })
            {
                SimpleNamedValue = intType,
                ArrayNamedValue = new object[] { 42, "42" }
            };
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

        static ExpectedArguments expectedArguments(Compilation compilation)
        {
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            return new("42", Array.Empty<object>(), new object[] { "42", 42 })
            {
                SimpleNamedValue = intType,
                ArrayNamedValue = new object[] { 42, "42" }
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

        static ExpectedArguments expectedArguments(Compilation compilation) => new("42", Array.Empty<object>(), new object[] { "42", 42 });
    }

    [AssertionMethod]
    private async Task ReturnsTrueAndRecords(string source, Func<Compilation, ExpectedArguments> expectedArgumentsDelegate)
    {
        var (compilation, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var typeParameters = attributeData.AttributeClass!.TypeParameters;
        var typeArguments = attributeData.AttributeClass.TypeArguments;

        var constructorParameters = attributeData.AttributeConstructor!.Parameters;

        var expectedArguments = expectedArgumentsDelegate(compilation);

        var outcome = Target(Context.Recorder, attributeData);

        Assert.True(outcome);

        Context.RecorderMock.Verify((recorder) => recorder.Type.TryRecordArgument(typeParameters[0], typeArguments[0]), Times.Once);
        Context.RecorderMock.Verify((recorder) => recorder.Type.TryRecordArgument(typeParameters[1], typeArguments[1]), Times.Once);

        Context.RecorderMock.Verify((recorder) => recorder.Constructor.TryRecordArgument(constructorParameters[0], It.Is(expectedArguments.SimpleValue, SequenceEqualityComparer.Instance)), Times.Once);
        Context.RecorderMock.Verify((recorder) => recorder.Constructor.TryRecordArgument(constructorParameters[1], It.Is(expectedArguments.ArrayValue, SequenceEqualityComparer.Instance)), Times.Once);
        Context.RecorderMock.Verify((recorder) => recorder.Constructor.TryRecordArgument(constructorParameters[2], It.Is(expectedArguments.ParamsValue, SequenceEqualityComparer.Instance)), Times.Once);

        Context.RecorderMock.Verify((recorder) => recorder.Named.TryRecordArgument(nameof(CombinedAttribute<object, object>.SimpleNamedValue), It.Is(expectedArguments.SimpleNamedValue, SequenceEqualityComparer.Instance)), Times.Exactly(expectedArguments.SimpleNamedValueSetCount));
        Context.RecorderMock.Verify((recorder) => recorder.Named.TryRecordArgument(nameof(CombinedAttribute<object, object>.ArrayNamedValue), It.Is(expectedArguments.ArrayNamedValue, SequenceEqualityComparer.Instance)), Times.Exactly(expectedArguments.ArrayNamedValueSetCount));
    }

    private sealed class ExpectedArguments
    {
        public object? SimpleValue { get; }
        public IReadOnlyList<object?>? ArrayValue { get; }
        public IReadOnlyList<object?>? ParamsValue { get; }

        public object? SimpleNamedValue
        {
            get => RawSimpleNamedValue;
            init
            {
                RawSimpleNamedValue = value;
                SimpleNamedValueSet = true;
            }
        }

        public IReadOnlyList<object?>? ArrayNamedValue
        {
            get => RawArrayNamedValue;
            init
            {
                RawArrayNamedValue = value;
                ArrayNamedValueSet = true;
            }
        }

        public int SimpleNamedValueSetCount => SimpleNamedValueSet ? 1 : 0;
        public int ArrayNamedValueSetCount => ArrayNamedValueSet ? 1 : 0;

        private readonly bool SimpleNamedValueSet;
        private readonly bool ArrayNamedValueSet;

        private readonly object? RawSimpleNamedValue;
        private readonly IReadOnlyList<object?>? RawArrayNamedValue;

        public ExpectedArguments(object? simpleValue, IReadOnlyList<object?>? arrayValue, IReadOnlyList<object?>? paramsValue)
        {
            SimpleValue = simpleValue;
            ArrayValue = arrayValue;
            ParamsValue = paramsValue;
        }
    }
}
