namespace SharpAttributeParser.AAdaptiveAttributeMapperCases.AdaptersCases.ArrayArgumentCases.ParamsCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Xunit;

public sealed class ForNullableElements_Action_Class
{
    [Fact]
    public void NullSharedDelegate_ArgumentNullException()
    {
        var exception = Record.Exception(() => Mapper<string>.Target(null!, Data<string>.SemanticRecorder));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullSemanticDelegate_ArgumentNullException()
    {
        var exception = Record.Exception(() => Mapper<string>.Target(Data<string>.SharedRecorder, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidSharedRecorder_NullDataRecord_ArgumentNullExceptionWhenUsed()
    {
        var (recorder, _) = Mapper<string>.Target(Data<string>.SharedRecorder, Data<string>.SemanticRecorder);

        var exception = Record.Exception(() => recorder(null!, new[] { "3", "4" }, ExpressionSyntaxFactory.Create()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidSharedRecorder_NullSyntax_ArgumentExceptionWhenUsed()
    {
        var (recorder, _) = Mapper<string>.Target(Data<string>.SharedRecorder, Data<string>.SemanticRecorder);

        var exception = Record.Exception(() => recorder(new Data<string>(), new[] { "3", "4" }, (ExpressionSyntax)null!));

        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public void ValidSharedRecorder_NullSyntaxCollection_ArgumentExceptionWhenUsed()
    {
        var (recorder, _) = Mapper<string>.Target(Data<string>.SharedRecorder, Data<string>.SemanticRecorder);

        var exception = Record.Exception(() => recorder(new Data<string>(), new[] { "3", "4" }, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(null!)));

        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public void ValidSemanticRecorder_NullDataRecord_ArgumentNullExceptionWhenUsed()
    {
        var (_, recorder) = Mapper<string>.Target(Data<string>.SharedRecorder, Data<string>.SemanticRecorder);

        var exception = Record.Exception(() => recorder(null!, new[] { "3", "4" }));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Syntax_TrueAndRecorded()
    {
        var value = new[] { "3", "4" };

        var syntax = ExpressionSyntaxFactory.Create();

        TrueAndRecorded(value, value, syntax);
    }

    [Fact]
    public void SyntaxCollection_TrueAndRecorded()
    {
        var value = new[] { "3", "4" };

        var syntax = OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(ExpressionSyntaxFactory.CreateCollection());

        TrueAndRecorded(value, value, syntax);
    }

    [Fact]
    public void String_NotArrayType_FalseAndNotRecorded()
    {
        var value = "3";

        FalseAndNotRecorded<string>(value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void String_SameType_TrueAndRecorded()
    {
        var value = new[] { "3", null };

        TrueAndRecorded(value, value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void String_StringsCastToObjects_TrueAndRecorded()
    {
        var value = new object?[] { "3", null };

        TrueAndRecorded(value.Select(static (value) => (string?)value), value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void String_DifferentType_FalseAndNotRecorded()
    {
        var value = new object[] { "3", StringComparison.OrdinalIgnoreCase };

        FalseAndNotRecorded<string>(value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void String_NullCollection_FalseAndNotRecorded()
    {
        IReadOnlyList<string>? value = null;

        FalseAndNotRecorded<string>(value, ExpressionSyntaxFactory.Create());
    }

    [AssertionMethod]
    private static void TrueAndRecorded<T>(IEnumerable<T?> expected, object? value, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) where T : class
    {
        var (sharedRecorder, semanticRecorder) = Mapper<T>.Target(Data<T>.SharedRecorder, Data<T>.SemanticRecorder);

        Data<T> sharedData = new();
        Data<T> semanticData = new();

        var sharedOutcome = sharedRecorder(sharedData, value, syntax);
        var semanticOutcome = semanticRecorder(semanticData, value);

        Assert.True(sharedOutcome);
        Assert.True(semanticOutcome);

        Assert.Equal<IEnumerable<T?>>(expected, sharedData.Value);
        OneOfAssertions.Equal(syntax, sharedData.ValueSyntax);
        Assert.True(sharedData.ValueRecorded);

        Assert.Equal<IEnumerable<T?>>(expected, semanticData.Value);
        Assert.True(semanticData.ValueRecorded);
    }

    [AssertionMethod]
    private static void FalseAndNotRecorded<T>(object? value, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) where T : class
    {
        var (sharedRecorder, semanticRecorder) = Mapper<T>.Target(Data<T>.SharedRecorder, Data<T>.SemanticRecorder);

        Data<T> sharedData = new();
        Data<T> semanticData = new();

        var sharedOutcome = sharedRecorder(sharedData, value, syntax);
        var semanticOutcome = semanticRecorder(semanticData, value);

        Assert.False(sharedOutcome);
        Assert.False(semanticOutcome);

        Assert.False(sharedData.ValueRecorded);
        Assert.False(semanticData.ValueRecorded);
    }

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used to expose static member of ASemanticAttributeMapper.")]
    private sealed class Mapper<T> : AAdaptiveAttributeMapper<Data<T>, Data<T>> where T : class
    {
        public static (Func<Data<T>, object?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool>, Func<Data<T>, object?, bool>) Target(Action<Data<T>, IReadOnlyList<T?>, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> sharedRecorder, Action<Data<T>, IReadOnlyList<T?>> semanticRecorder)
        {
            var recorders = Adapters.ArrayArgument.Params.ForNullableElements(sharedRecorder, semanticRecorder);

            return (recorders.Shared.Invoke, recorders.Semantic.Invoke);
        }
    }

    private sealed class Data<T>
    {
        public static Action<Data<T>, IReadOnlyList<T?>?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> SharedRecorder => (dataRecord, argument, syntax) =>
        {
            dataRecord.Value = argument;
            dataRecord.ValueSyntax = syntax;
            dataRecord.ValueRecorded = true;
        };

        public static Action<Data<T>, IReadOnlyList<T?>?> SemanticRecorder => (dataRecord, argument) =>
        {
            dataRecord.Value = argument;
            dataRecord.ValueRecorded = true;
        };

        public IReadOnlyList<T?>? Value { get; set; }
        public OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> ValueSyntax { get; set; }
        public bool ValueRecorded { get; set; }
    }
}
