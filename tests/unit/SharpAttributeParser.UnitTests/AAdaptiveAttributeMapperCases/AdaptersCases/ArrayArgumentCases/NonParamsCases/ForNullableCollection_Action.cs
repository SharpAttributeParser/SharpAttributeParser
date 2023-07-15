namespace SharpAttributeParser.AAdaptiveAttributeMapperCases.AdaptersCases.ArrayArgumentCases.NonParamsCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Xunit;

public sealed class ForNullableCollection_Action
{
    [Fact]
    public void NullSharedDelegate_ArgumentNullException()
    {
        var exception = Record.Exception(() => Mapper<int>.Target(null!, Data<int>.SemanticRecorder));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullSemanticDelegate_ArgumentNullException()
    {
        var exception = Record.Exception(() => Mapper<int>.Target(Data<int>.SharedRecorder, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidSharedRecorder_NullDataRecord_ArgumentNullExceptionWhenUsed()
    {
        var (recorder, _) = Mapper<int>.Target(Data<int>.SharedRecorder, Data<int>.SemanticRecorder);

        var exception = Record.Exception(() => recorder(null!, new[] { 3, 4 }, ExpressionSyntaxFactory.Create()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidSharedRecorder_NullSyntax_ArgumentExceptionWhenUsed()
    {
        var (recorder, _) = Mapper<int>.Target(Data<int>.SharedRecorder, Data<int>.SemanticRecorder);

        var exception = Record.Exception(() => recorder(new Data<int>(), new[] { 3, 4 }, (ExpressionSyntax)null!));

        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public void ValidSemanticRecorder_NullDataRecord_ArgumentNullExceptionWhenUsed()
    {
        var (_, recorder) = Mapper<int>.Target(Data<int>.SharedRecorder, Data<int>.SemanticRecorder);

        var exception = Record.Exception(() => recorder(null!, new[] { 3, 4 }));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullSyntaxCollection_FalseAndNotRecorded()
    {
        var syntax = OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(null!);

        FalseAndNotRecordedDueToSyntax<int>(new[] { 3, 4 }, syntax);
    }

    [Fact]
    public void SyntaxCollection_FalseAndNotRecorded()
    {
        var syntax = OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(ExpressionSyntaxFactory.CreateCollection());

        FalseAndNotRecordedDueToSyntax<int>(new[] { 3, 4 }, syntax);
    }

    [Fact]
    public void Syntax_TrueAndRecorded()
    {
        var value = new[] { 3, 4 };

        var syntax = ExpressionSyntaxFactory.Create();

        TrueAndRecorded(value, value, syntax);
    }

    [Fact]
    public void Enum_NotArrayType_FalseAndNotRecorded()
    {
        var value = "3";

        FalseAndNotRecorded<StringComparison>(value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void Enum_SameType_TrueAndRecorded()
    {
        var value = new[] { StringComparison.CurrentCulture, StringComparison.InvariantCultureIgnoreCase };

        TrueAndRecorded(value, value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void Enum_DifferentEnumType_FalseAndNotRecorded()
    {
        var value = new[] { StringSplitOptions.RemoveEmptyEntries, StringSplitOptions.TrimEntries };

        FalseAndNotRecorded<StringComparison>(value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void Enum_Int_FalseAndNotRecorded()
    {
        var value = new[] { 3, 4 };

        FalseAndNotRecorded<StringSplitOptions>(value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void Int_NullableIntWithValues_FalseAndNotRecorded()
    {
        var value = new int?[] { 3, 4 };

        FalseAndNotRecorded<int>(value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void Int_SameType_TrueAndRecorded()
    {
        var value = new[] { 3, 4 };

        TrueAndRecorded(value, value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void Int_IntsCastToObjects_TrueAndRecorded()
    {
        var value = new object[] { 3, 4 };

        TrueAndRecorded(value.Select(static (value) => (int)value), value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void Int_Enum_FalseAndNotRecorded()
    {
        var value = new[] { StringComparison.CurrentCulture, StringComparison.InvariantCultureIgnoreCase };

        FalseAndNotRecorded<int>(value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void Int_Double_FalseAndNotRecorded()
    {
        var value = new[] { 3.14, 4.14 };

        FalseAndNotRecorded<int>(value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void Int_String_FalseAndNotRecorded()
    {
        var value = new[] { "CurrentCulture", "InvariantCultureIgnoreCase" };

        FalseAndNotRecorded<int>(value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void Int_NullCollection_TrueAndRecorded()
    {
        IReadOnlyList<int>? value = null;

        TrueAndRecorded(value, value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void Double_Int_FalseAndNotRecorded()
    {
        var value = new[] { 3, 4 };

        FalseAndNotRecorded<double>(value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void String_SameType_TrueAndRecorded()
    {
        var value = new[] { "3", "4" };

        TrueAndRecorded(value, value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void String_DifferentType_FalseAndNotRecorded()
    {
        var value = new object[] { "3", StringComparison.OrdinalIgnoreCase };

        FalseAndNotRecorded<string>(value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void String_NullElement_FalseAndNotRecorded()
    {
        var value = new[] { "3", null };

        FalseAndNotRecorded<string>(value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void String_NullCollection_TrueAndRecorded()
    {
        IReadOnlyList<string>? value = null;

        TrueAndRecorded(value, value, ExpressionSyntaxFactory.Create());
    }

    [AssertionMethod]
    private static void TrueAndRecorded<T>(IEnumerable<T>? expected, object? value, ExpressionSyntax syntax) where T : notnull
    {
        var (sharedRecorder, semanticRecorder) = Mapper<T>.Target(Data<T>.SharedRecorder, Data<T>.SemanticRecorder);

        Data<T> sharedData = new();
        Data<T> semanticData = new();

        var sharedOutcome = sharedRecorder(sharedData, value, syntax);
        var semanticOutcome = semanticRecorder(semanticData, value);

        Assert.True(sharedOutcome);
        Assert.True(semanticOutcome);

        Assert.Equal<IEnumerable<T>>(expected, sharedData.Value);
        Assert.Equal(syntax, sharedData.ValueSyntax);
        Assert.True(sharedData.ValueRecorded);

        Assert.Equal<IEnumerable<T>>(expected, semanticData.Value);
        Assert.True(semanticData.ValueRecorded);
    }

    [AssertionMethod]
    private static void FalseAndNotRecorded<T>(object? value, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) where T : notnull
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

    [AssertionMethod]
    private static void FalseAndNotRecordedDueToSyntax<T>(object? value, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) where T : notnull
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
    private sealed class Mapper<T> : AAdaptiveAttributeMapper<Data<T>, Data<T>> where T : notnull
    {
        public static (Func<Data<T>, object?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool>, Func<Data<T>, object?, bool>) Target(Action<Data<T>, IReadOnlyList<T>?, ExpressionSyntax> sharedRecorder, Action<Data<T>, IReadOnlyList<T>?> semanticRecorder)
        {
            var recorders = Adapters.ArrayArgument.NonParams.ForNullableCollection(sharedRecorder, semanticRecorder);

            return (recorders.Shared.Invoke, recorders.Semantic.Invoke);
        }
    }

    private sealed class Data<T>
    {
        public static Action<Data<T>, IReadOnlyList<T>?, ExpressionSyntax> SharedRecorder => (dataRecord, argument, syntax) =>
        {
            dataRecord.Value = argument;
            dataRecord.ValueSyntax = syntax;
            dataRecord.ValueRecorded = true;
        };

        public static Action<Data<T>, IReadOnlyList<T>?> SemanticRecorder => (dataRecord, argument) =>
        {
            dataRecord.Value = argument;
            dataRecord.ValueRecorded = true;
        };

        public IReadOnlyList<T>? Value { get; set; }
        public ExpressionSyntax? ValueSyntax { get; set; }
        public bool ValueRecorded { get; set; }
    }
}
