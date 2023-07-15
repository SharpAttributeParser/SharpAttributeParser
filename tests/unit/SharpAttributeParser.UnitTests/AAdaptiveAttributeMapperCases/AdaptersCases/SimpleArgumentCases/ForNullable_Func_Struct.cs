﻿namespace SharpAttributeParser.AAdaptiveAttributeMapperCases.AdaptersCases.SimpleArgumentCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Xunit;

public sealed class ForNullable_Func_Struct
{
    [Fact]
    public void NullSharedDelegate_ArgumentNullExceptionWhenUsed()
    {
        var exception = Record.Exception(() => Mapper<int>.Target(null!, Data<int?>.TrueSemanticRecorder));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullSemanticDelegate_ArgumentNullExceptionWhenUsed()
    {
        var exception = Record.Exception(() => Mapper<int>.Target(Data<int?>.TrueSharedRecorder, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidSharedRecorder_NullDataRecord_ArgumentNullExceptionWhenUsed()
    {
        var (recorder, _) = Mapper<int>.Target(Data<int?>.TrueSharedRecorder, Data<int?>.TrueSemanticRecorder);

        var exception = Record.Exception(() => recorder(null!, 3, ExpressionSyntaxFactory.Create()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidSharedRecorder_NullSyntax_ArgumentExceptionWhenUsed()
    {
        var (recorder, _) = Mapper<int>.Target(Data<int?>.TrueSharedRecorder, Data<int?>.TrueSemanticRecorder);

        var exception = Record.Exception(() => recorder(new Data<int?>(), 3, (ExpressionSyntax)null!));

        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public void ValidSemanticRecorder_NullDataRecord_ArgumentNullExceptionWhenUsed()
    {
        var (_, recorder) = Mapper<int>.Target(Data<int?>.TrueSharedRecorder, Data<int?>.TrueSemanticRecorder);

        var exception = Record.Exception(() => recorder(null!, 3));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullSyntaxCollection_FalseAndNotRecorded()
    {
        var syntax = OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(null!);

        FalseAndNotRecordedDueToSyntax<int>(3, syntax);
    }

    [Fact]
    public void SyntaxCollection_FalseAndNotRecorded()
    {
        var syntax = OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(ExpressionSyntaxFactory.CreateCollection());

        FalseAndNotRecordedDueToSyntax<int>(3, syntax);
    }

    [Fact]
    public void Syntax_TrueAndRecorded()
    {
        var syntax = ExpressionSyntaxFactory.Create();

        TrueAndRecorded<int>(3, 3, syntax);
    }

    [Fact]
    public void Enum_SameType_TrueAndRecorded()
    {
        var value = StringComparison.OrdinalIgnoreCase;

        TrueAndRecorded<StringComparison>(value, value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void Enum_DifferentEnumType_FalseAndNotRecorded()
    {
        var value = StringSplitOptions.TrimEntries;

        FalseAndNotRecorded<StringComparison>(value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void Enum_Null_TrueAndRecorded()
    {
        StringComparison? value = null;

        TrueAndRecorded(value, value, ExpressionSyntaxFactory.Create());
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

        TrueAndRecorded<int>(value, value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void Int_IntCastToObject_TrueAndRecorded()
    {
        object value = 3;

        TrueAndRecorded<int>((int)value, value, ExpressionSyntaxFactory.Create());
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
    public void NullableInt_Null_TrueAndRecorded()
    {
        int? value = null;

        TrueAndRecorded(value, value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void FalseReturningRecorder_FalseAndRecorded()
    {
        var (sharedRecorder, semanticRecorder) = Mapper<int>.Target(Data<int?>.FalseSharedRecorder, Data<int?>.FalseSemanticRecorder);

        var value = 3;
        var syntax = ExpressionSyntaxFactory.Create();

        Data<int?> sharedData = new();
        Data<int?> semanticData = new();

        var sharedOutcome = sharedRecorder(sharedData, value, syntax);
        var semanticOutcome = semanticRecorder(semanticData, value);

        Assert.False(sharedOutcome);
        Assert.False(semanticOutcome);

        Assert.Equal(value, sharedData.Value);
        Assert.Equal(syntax, sharedData.ValueSyntax);
        Assert.True(sharedData.ValueRecorded);

        Assert.Equal(value, semanticData.Value);
        Assert.True(semanticData.ValueRecorded);
    }

    [AssertionMethod]
    private static void TrueAndRecorded<T>(T? expected, object? value, ExpressionSyntax syntax) where T : struct
    {
        var (sharedRecorder, semanticRecorder) = Mapper<T>.Target(Data<T?>.TrueSharedRecorder, Data<T?>.TrueSemanticRecorder);

        Data<T?> sharedData = new();
        Data<T?> semanticData = new();

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
    private static void FalseAndNotRecorded<T>(object? value, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) where T : struct
    {
        var (sharedRecorder, semanticRecorder) = Mapper<T>.Target(Data<T?>.TrueSharedRecorder, Data<T?>.TrueSemanticRecorder);

        Data<T?> sharedData = new();
        Data<T?> semanticData = new();

        var sharedOutcome = sharedRecorder(sharedData, value, syntax);
        var semanticOutcome = semanticRecorder(semanticData, value);

        Assert.False(sharedOutcome);
        Assert.False(semanticOutcome);

        Assert.False(sharedData.ValueRecorded);
        Assert.False(semanticData.ValueRecorded);
    }

    [AssertionMethod]
    private static void FalseAndNotRecordedDueToSyntax<T>(object? value, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) where T : struct
    {
        var (recorder, _) = Mapper<T>.Target(Data<T?>.TrueSharedRecorder, Data<T?>.TrueSemanticRecorder);

        Data<T?> trueData = new();
        Data<T?> falseData = new();

        var trueOutcome = recorder(trueData, value, ExpressionSyntaxFactory.Create());
        var falseOutcome = recorder(falseData, value, syntax);

        Assert.True(trueOutcome);
        Assert.False(falseOutcome);

        Assert.True(trueData.ValueRecorded);
        Assert.False(falseData.ValueRecorded);
    }

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used to expose static member of ASemanticAttributeMapper.")]
    private sealed class Mapper<T> : AAdaptiveAttributeMapper<Data<T?>, Data<T?>> where T : struct
    {
        public static (Func<Data<T?>, object?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool>, Func<Data<T?>, object?, bool>) Target(Func<Data<T?>, T?, ExpressionSyntax, bool> sharedRecorder, Func<Data<T?>, T?, bool> semanticRecorder)
        {
            var recorders = Adapters.SimpleArgument.ForNullable(sharedRecorder, semanticRecorder);

            return (recorders.Shared.Invoke, recorders.Semantic.Invoke);
        }
    }

    private sealed class Data<T>
    {
        public static Func<Data<T>, T?, ExpressionSyntax, bool> TrueSharedRecorder => (dataRecord, argument, syntax) =>
        {
            Recorder(dataRecord, argument, syntax);

            return true;
        };

        public static Func<Data<T>, T?, ExpressionSyntax, bool> FalseSharedRecorder => (dataRecord, argument, syntax) =>
        {
            Recorder(dataRecord, argument, syntax);

            return false;
        };

        public static Func<Data<T>, T?, bool> TrueSemanticRecorder => (dataRecord, argument) =>
        {
            Recorder(dataRecord, argument);

            return true;
        };

        public static Func<Data<T>, T?, bool> FalseSemanticRecorder => (dataRecord, argument) =>
        {
            Recorder(dataRecord, argument);

            return false;
        };

        private static void Recorder(Data<T> dataRecord, T? argument, ExpressionSyntax syntax)
        {
            dataRecord.Value = argument;
            dataRecord.ValueSyntax = syntax;
            dataRecord.ValueRecorded = true;
        }

        private static void Recorder(Data<T> dataRecord, T? argument)
        {
            dataRecord.Value = argument;
            dataRecord.ValueRecorded = true;
        }

        public T? Value { get; set; }
        public ExpressionSyntax? ValueSyntax { get; set; }
        public bool ValueRecorded { get; set; }
    }
}
