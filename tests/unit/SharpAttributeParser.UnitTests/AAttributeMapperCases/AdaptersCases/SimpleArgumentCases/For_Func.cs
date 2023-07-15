namespace SharpAttributeParser.AAttributeMapperCases.AdaptersCases.SimpleArgumentCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Xunit;

public sealed class For_Func
{
    [Fact]
    public void NullDelegate_ArgumentNullExceptionWhenUsed()
    {
        var exception = Record.Exception(() => Mapper<int>.Target(null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidRecorder_NullDataRecord_ArgumentNullExceptionWhenUsed()
    {
        var recorder = Mapper<int>.Target(Data<int>.TrueRecorder);

        var exception = Record.Exception(() => recorder(null!, 3, ExpressionSyntaxFactory.Create()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidRecorder_NullSyntax_ArgumentExceptionWhenUsed()
    {
        var recorder = Mapper<int>.Target(Data<int>.TrueRecorder);

        var exception = Record.Exception(() => recorder(new Data<int>(), 3, (ExpressionSyntax)null!));

        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public void ValidRecorder_NullSyntaxCollection_FalseAndNotRecorded()
    {
        var syntax = OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(null!);

        FalseAndNotRecordedDueToSyntax<int>(3, syntax);
    }

    [Fact]
    public void ValidRecorder_SyntaxCollection_FalseAndNotRecorded()
    {
        var syntax = OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(ExpressionSyntaxFactory.CreateCollection());

        FalseAndNotRecordedDueToSyntax<int>(3, syntax);
    }

    [Fact]
    public void Enum_DifferentEnumType_FalseAndNotRecorded()
    {
        var value = StringSplitOptions.TrimEntries;

        FalseAndNotRecorded<StringComparison>(value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void Enum_Int_FalseAndNotRecorded()
    {
        var value = 3;

        FalseAndNotRecorded<StringComparison>(value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void Int_SameType_TrueAndRecorded()
    {
        var value = 3;

        TrueAndRecorded(value, value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void Int_IntCastToObject_TrueAndRecorded()
    {
        object value = 3;

        TrueAndRecorded((int)value, value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void Int_Enum_FalseAndNotRecorded()
    {
        var value = StringComparison.Ordinal;

        FalseAndNotRecorded<int>(value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void Int_Double_FalseAndNotRecorded()
    {
        var value = 3.14;

        FalseAndNotRecorded<int>(value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void Int_String_FalseAndNotRecorded()
    {
        var value = "3";

        FalseAndNotRecorded<int>(value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void Int_NullableIntWithValue_TrueAndRecorded()
    {
        int? value = 3;

        TrueAndRecorded(value.Value, value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void Int_Null_FalseAndNotRecorded()
    {
        int? value = null;

        FalseAndNotRecorded<int>(value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void Double_Int_FalseAndNotRecorded()
    {
        var value = 3;

        FalseAndNotRecorded<double>(value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void NullableIntArray_NullElement_TrueAndRecorded()
    {
        var value = new int?[] { 3, null };

        TrueAndRecorded(value, value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void NullableIntArray_NullCollection_FalseAndNotRecorded()
    {
        IReadOnlyList<int?>? value = null;

        FalseAndNotRecorded<IReadOnlyList<int?>>(value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void String_SameType_TrueAndRecorded()
    {
        var value = "3";

        TrueAndRecorded(value, value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void String_Enum_FalseAndNotRecorded()
    {
        var value = StringComparison.OrdinalIgnoreCase;

        FalseAndNotRecorded<string>(value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void String_Null_FalseAndNotRecorded()
    {
        string? value = null;

        FalseAndNotRecorded<string>(value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void FalseReturningRecorder_FalseAndRecorded()
    {
        var recorder = Mapper<int>.Target(Data<int>.FalseRecorder);

        var value = 3;
        var syntax = ExpressionSyntaxFactory.Create();

        Data<int> data = new();

        var outcome = recorder(data, value, syntax);

        Assert.False(outcome);

        Assert.Equal(value, data.Value);
        Assert.Equal(syntax, data.ValueSyntax);
        Assert.True(data.ValueRecorded);
    }

    [AssertionMethod]
    private static void TrueAndRecorded<T>(T expected, object? value, ExpressionSyntax syntax) where T : notnull
    {
        var recorder = Mapper<T>.Target(Data<T>.TrueRecorder);

        Data<T> data = new();

        var outcome = recorder(data, value, syntax);

        Assert.True(outcome);

        Assert.Equal(expected, data.Value);
        Assert.Equal(syntax, data.ValueSyntax);
        Assert.True(data.ValueRecorded);
    }

    [AssertionMethod]
    private static void FalseAndNotRecorded<T>(object? value, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) where T : notnull
    {
        var recorder = Mapper<T>.Target(Data<T>.TrueRecorder);

        Data<T> data = new();

        var outcome = recorder(data, value, syntax);

        Assert.False(outcome);

        Assert.False(data.ValueRecorded);
    }

    [AssertionMethod]
    private static void FalseAndNotRecordedDueToSyntax<T>(object? value, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) where T : notnull
    {
        var recorder = Mapper<T>.Target(Data<T>.TrueRecorder);

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
    private sealed class Mapper<T> : AAttributeMapper<Data<T>> where T : notnull
    {
        public static Func<Data<T>, object?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> Target(Func<Data<T>, T, ExpressionSyntax, bool> recorder) => Adapters.SimpleArgument.For(recorder).Invoke;
    }

    private sealed class Data<T>
    {
        public static Func<Data<T>, T, ExpressionSyntax, bool> TrueRecorder => (dataRecord, argument, syntax) =>
        {
            Recorder(dataRecord, argument, syntax);

            return true;
        };

        public static Func<Data<T>, T, ExpressionSyntax, bool> FalseRecorder => (dataRecord, argument, syntax) =>
        {
            Recorder(dataRecord, argument, syntax);

            return false;
        };

        private static void Recorder(Data<T> dataRecord, T argument, ExpressionSyntax syntax)
        {
            dataRecord.Value = argument;
            dataRecord.ValueSyntax = syntax;
            dataRecord.ValueRecorded = true;
        }

        public T? Value { get; set; }
        public ExpressionSyntax? ValueSyntax { get; set; }
        public bool ValueRecorded { get; set; }
    }
}
