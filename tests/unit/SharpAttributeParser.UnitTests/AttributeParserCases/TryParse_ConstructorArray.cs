﻿namespace SharpAttributeParser.AttributeParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Recording;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_ArrayConstructor
{
    private IArrayConstructorAttributeRecorderFactory RecorderFactory { get; }

    public TryParse_ArrayConstructor(IArrayConstructorAttributeRecorderFactory recorderFactory)
    {
        RecorderFactory = recorderFactory;
    }

    private static bool Target(IAttributeParser parser, IAttributeRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => parser.TryParse(recorder, attributeData, attributeSyntax);

    [Theory]
    [ClassData(typeof(ParserSources))]
    [SuppressMessage("Minor Code Smell", "S1125: Boolean literals should not be redundant", Justification = "Prefer equality check over negation.")]
    public async Task FalseReturningRecorder_False(IAttributeParser parser)
    {
        var recorder = Mock.Of<IAttributeRecorder>(static (recorder) => recorder.TryRecordConstructorArgument(It.IsAny<IParameterSymbol>(), It.IsAny<IReadOnlyList<object?>?>(), It.IsAny<ExpressionSyntax>()) == false);

        var source = """
            [ArrayConstructor(null)]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.False(result);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NonExsitingAttribute_FalseAndNotRecorded(IAttributeParser parser)
    {
        var source = """
            [NonExisting]
            public class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NonExsitingConstructor_FalseAndNotRecorded(IAttributeParser parser)
    {
        var source = """
            [ArrayConstructor(3, 4)]
            public class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task ErrorArgument_FalseAndNotRecorded(IAttributeParser parser)
    {
        var source = """
            [ArrayConstructor((object[])4)]
            public class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NullLiteral_TrueAndRecorded(IAttributeParser parser)
    {
        var source = """
            [ArrayConstructor(null)]
            public class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax attributeSyntax) => new(null, attributeSyntax.ArgumentList!.Arguments[0].Expression);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task DefaultLiteral_TrueAndRecorded(IAttributeParser parser)
    {
        var source = """
            [ArrayConstructor(default)]
            public class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax attributeSyntax) => new(null, attributeSyntax.ArgumentList!.Arguments[0].Expression);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task DefaultExpression_TrueAndRecorded(IAttributeParser parser)
    {
        var source = """
            [ArrayConstructor(default(object[]))]
            public class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax attributeSyntax) => new(null, attributeSyntax.ArgumentList!.Arguments[0].Expression);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task Empty_TrueAndRecorded(IAttributeParser parser)
    {
        var source = """
            [ArrayConstructor(new object[] { })]
            public class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax attributeSyntax) => new(Array.Empty<object?>(), attributeSyntax.ArgumentList!.Arguments[0].Expression);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task Values_TrueAndRecorded(IAttributeParser parser)
    {
        var source = """
            [ArrayConstructor(new object[] { "42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 } })]
            public class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax attributeSyntax)
        {
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            return new(new object?[] { "42", null, intType, "Foo", 42, (double)42, new object[] { "42", 42 } }, attributeSyntax.ArgumentList!.Arguments[0].Expression);
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task Labelled_TrueAndRecorded(IAttributeParser parser)
    {
        var source = """
            [ArrayConstructor(value: new object[] { "42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 } })]
            public class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax attributeSyntax)
        {
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            return new(new object?[] { "42", null, intType, "Foo", 42, (double)42, new object[] { "42", 42 } }, attributeSyntax.ArgumentList!.Arguments[0].Expression);
        }
    }

    [AssertionMethod]
    private async Task TrueAndIdenticalToExpected(IAttributeParser parser, string source, Func<Compilation, AttributeSyntax, ExpectedResult> expectedDelegate)
    {
        var recorder = RecorderFactory.Create();

        var (compilation, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var expected = expectedDelegate(compilation, attributeSyntax);

        var outcome = Target(parser, recorder, attributeData, attributeSyntax);
        var result = recorder.GetRecord();

        Assert.True(outcome);

        Assert.Equal(expected.Value, result.Value);
        Assert.Equal(expected.ValueSyntax, result.ValueSyntax);
        Assert.True(result.ValueRecorded);
    }

    [AssertionMethod]
    private async Task FalseAndNotRecorded(IAttributeParser parser, string source)
    {
        var recorder = RecorderFactory.Create();

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var outcome = Target(parser, recorder, attributeData, attributeSyntax);
        var result = recorder.GetRecord();

        Assert.False(outcome);

        Assert.False(result.ValueRecorded);
    }

    private sealed class ExpectedResult
    {
        public IReadOnlyList<object?>? Value { get; }
        public ExpressionSyntax ValueSyntax { get; }

        public ExpectedResult(IReadOnlyList<object?>? value, ExpressionSyntax valueSyntax)
        {
            Value = value;
            ValueSyntax = valueSyntax;
        }
    }
}