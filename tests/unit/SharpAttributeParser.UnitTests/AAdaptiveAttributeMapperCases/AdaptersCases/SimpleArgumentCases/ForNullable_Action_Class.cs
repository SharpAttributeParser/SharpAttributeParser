namespace SharpAttributeParser.AAdaptiveAttributeMapperCases.AdaptersCases.SimpleArgumentCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Xunit;

public sealed class ForNullable_Action_Class
{
    [Fact]
    public void NullSharedDelegate_ArgumentNullExceptionWhenUsed()
    {
        var exception = Record.Exception(() => Mapper<string>.Target(null!, Data<string>.SemanticRecorder));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullSemanticDelegate_ArgumentNullExceptionWhenUsed()
    {
        var exception = Record.Exception(() => Mapper<string>.Target(Data<string>.SharedRecorder, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidSharedRecorder_NullDataRecord_ArgumentNullExceptionWhenUsed()
    {
        var (recorder, _) = Mapper<string>.Target(Data<string>.SharedRecorder, Data<string>.SemanticRecorder);

        var exception = Record.Exception(() => recorder(null!, "3", ExpressionSyntaxFactory.Create()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidSharedRecorder_NullSyntax_ArgumentExceptionWhenUsed()
    {
        var (recorder, _) = Mapper<string>.Target(Data<string>.SharedRecorder, Data<string>.SemanticRecorder);

        var exception = Record.Exception(() => recorder(new Data<string>(), "3", (ExpressionSyntax)null!));

        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public void ValidSemanticRecorder_NullDataRecord_ArgumentNullExceptionWhenUsed()
    {
        var (_, recorder) = Mapper<string>.Target(Data<string>.SharedRecorder, Data<string>.SemanticRecorder);

        var exception = Record.Exception(() => recorder(null!, "3"));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullSyntaxCollection_FalseAndNotRecorded()
    {
        var syntax = OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(null!);

        FalseAndNotRecordedDueToSyntax<string>("3", syntax);
    }

    [Fact]
    public void SyntaxCollection_FalseAndNotRecorded()
    {
        var syntax = OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(ExpressionSyntaxFactory.CreateCollection());

        FalseAndNotRecordedDueToSyntax<string>("3", syntax);
    }

    [Fact]
    public void Syntax_TrueAndRecorded()
    {
        var syntax = ExpressionSyntaxFactory.Create();

        TrueAndRecorded("3", "3", syntax);
    }

    [Fact]
    public void NullableIntArray_NullElement_TrueAndRecorded()
    {
        var value = new int?[] { 3, null };

        TrueAndRecorded(value, value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void IntArray_NullCollection_TrueAndRecorded()
    {
        IReadOnlyList<int?>? value = null;

        TrueAndRecorded(value, value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void String_SameType_TrueAndRecorded()
    {
        var value = "3";

        TrueAndRecorded(value, value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void String_StringCastToObject_TrueAndRecorded()
    {
        object value = "3";

        TrueAndRecorded((string)value, value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void String_Enum_FalseAndNotRecorded()
    {
        var value = StringComparison.OrdinalIgnoreCase;

        FalseAndNotRecorded<string>(value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void String_Null_TrueAndRecorded()
    {
        string? value = null;

        TrueAndRecorded(value, value, ExpressionSyntaxFactory.Create());
    }

    [AssertionMethod]
    private static void TrueAndRecorded<T>(T? expected, object? value, ExpressionSyntax syntax) where T : class
    {
        var (sharedRecorder, semanticRecorder) = Mapper<T>.Target(Data<T>.SharedRecorder, Data<T>.SemanticRecorder);

        Data<T> sharedData = new();
        Data<T> semanticData = new();

        var sharedOutcome = sharedRecorder(sharedData, value, syntax);
        var semanticOutcome = semanticRecorder(semanticData, value);

        Assert.True(sharedOutcome);
        Assert.True(semanticOutcome);

        Assert.Equal(expected, sharedData.Value);
        Assert.Equal(syntax, sharedData.ValueSyntax);
        Assert.True(sharedData.ValueRecorded);

        Assert.Equal(expected, semanticData.Value);
        Assert.True(semanticData.ValueRecorded);
    }

    [AssertionMethod]
    private static void FalseAndNotRecorded<T>(object? value, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) where T : class
    {
        var (sharedRecorder, semanticRecorder) = Mapper<T>.Target(Data<T>.SharedRecorder, Data<T>.SemanticRecorder);

        Data<T> sharedData = new();
        Data<T> semanticData = new();

        var sharedOutcome = sharedRecorder(semanticData, value, syntax);
        var semanticOutcome = semanticRecorder(semanticData, value);

        Assert.False(sharedOutcome);
        Assert.False(semanticOutcome);

        Assert.False(sharedData.ValueRecorded);
        Assert.False(semanticData.ValueRecorded);
    }

    [AssertionMethod]
    private static void FalseAndNotRecordedDueToSyntax<T>(object? value, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) where T : class
    {
        var (recorder, _) = Mapper<T>.Target(Data<T>.SharedRecorder, Data<T>.SemanticRecorder);

        Data<T> trueData = new();
        Data<T> falseData = new();

        var trueOutcome = recorder(trueData, value, ExpressionSyntaxFactory.Create());
        var falseOutcome = recorder(falseData, value, syntax);

        Assert.True(trueOutcome);
        Assert.False(falseOutcome);

        Assert.True(trueData.ValueRecorded);
        Assert.False(falseData.ValueRecorded);
    }

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used to expose static member of ASemanticAttributeMapper.")]
    private sealed class Mapper<T> : AAdaptiveAttributeMapper<Data<T>, Data<T>> where T : class
    {
        public static (Func<Data<T>, object?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool>, Func<Data<T>, object?, bool>) Target(Action<Data<T>, T?, ExpressionSyntax> sharedRecorder, Action<Data<T>, T?> semanticRecorder)
        {
            var recorders = Adapters.SimpleArgument.ForNullable(sharedRecorder, semanticRecorder);

            return (recorders.Shared.Invoke, recorders.Semantic.Invoke);
        }
    }

    private sealed class Data<T>
    {
        public static Action<Data<T>, T?, ExpressionSyntax> SharedRecorder => (dataRecord, argument, syntax) =>
        {
            dataRecord.Value = argument;
            dataRecord.ValueSyntax = syntax;
            dataRecord.ValueRecorded = true;
        };

        public static Action<Data<T>, T?> SemanticRecorder => (dataRecord, argument) =>
        {
            dataRecord.Value = argument;
            dataRecord.ValueRecorded = true;
        };

        public T? Value { get; set; }
        public ExpressionSyntax? ValueSyntax { get; set; }
        public bool ValueRecorded { get; set; }
    }
}
