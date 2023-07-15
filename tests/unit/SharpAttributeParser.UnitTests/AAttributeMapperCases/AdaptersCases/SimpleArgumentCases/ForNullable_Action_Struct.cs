﻿namespace SharpAttributeParser.AAttributeMapperCases.AdaptersCases.SimpleArgumentCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Xunit;

public sealed class ForNullable_Action_Struct
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
        var recorder = Mapper<int>.Target(Data<int?>.Recorder);

        var exception = Record.Exception(() => recorder(null!, 3, ExpressionSyntaxFactory.Create()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidRecorder_NullSyntax_ArgumentExceptionWhenUsed()
    {
        var recorder = Mapper<int>.Target(Data<int?>.Recorder);

        var exception = Record.Exception(() => recorder(new Data<int?>(), 3, (ExpressionSyntax)null!));

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

    [AssertionMethod]
    private static void TrueAndRecorded<T>(T? expected, object? value, ExpressionSyntax syntax) where T : struct
    {
        var recorder = Mapper<T>.Target(Data<T?>.Recorder);

        Data<T?> data = new();

        var outcome = recorder(data, value, syntax);

        Assert.True(outcome);

        Assert.Equal(expected, data.Value);
        Assert.Equal(syntax, data.ValueSyntax);
        Assert.True(data.ValueRecorded);
    }

    [AssertionMethod]
    private static void FalseAndNotRecorded<T>(object? value, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) where T : struct
    {
        var recorder = Mapper<T>.Target(Data<T?>.Recorder);

        Data<T?> data = new();

        var outcome = recorder(data, value, syntax);

        Assert.False(outcome);

        Assert.False(data.ValueRecorded);
    }

    [AssertionMethod]
    private static void FalseAndNotRecordedDueToSyntax<T>(object? value, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) where T : struct
    {
        var recorder = Mapper<T>.Target(Data<T?>.Recorder);

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
    private sealed class Mapper<T> : AAttributeMapper<Data<T?>> where T : struct
    {
        public static Func<Data<T?>, object?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> Target(Action<Data<T?>, T?, ExpressionSyntax> recorder) => Adapters.SimpleArgument.ForNullable(recorder).Invoke;
    }

    private sealed class Data<T>
    {
        public static Action<Data<T>, T?, ExpressionSyntax> Recorder => (dataRecord, argument, syntax) =>
        {
            dataRecord.Value = argument;
            dataRecord.ValueSyntax = syntax;
            dataRecord.ValueRecorded = true;
        };

        public T? Value { get; set; }
        public ExpressionSyntax? ValueSyntax { get; set; }
        public bool ValueRecorded { get; set; }
    }
}