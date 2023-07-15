﻿namespace SharpAttributeParser.AAttributeMapperCases.AdaptersCases.ArrayArgumentCases.ParamsCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Xunit;

public sealed class ForNullableElements_Action_Struct
{
    [Fact]
    public void NullDelegate_ArgumentNullException()
    {
        var exception = Record.Exception(() => Mapper<int>.Target(null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidRecorder_NullDataRecord_ArgumentNullExceptionWhenUsed()
    {
        var recorder = Mapper<int>.Target(Data<int?>.Recorder);

        var exception = Record.Exception(() => recorder(null!, new[] { 3, 4 }, ExpressionSyntaxFactory.Create()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidRecorder_NullSyntax_ArgumentExceptionWhenUsed()
    {
        var recorder = Mapper<int>.Target(Data<int?>.Recorder);

        var exception = Record.Exception(() => recorder(new Data<int?>(), new[] { 3, 4 }, (ExpressionSyntax)null!));

        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public void ValidRecorder_NullSyntaxCollection_ArgumentExceptionWhenUsed()
    {
        var recorder = Mapper<int>.Target(Data<int?>.Recorder);

        var exception = Record.Exception(() => recorder(new Data<int?>(), new[] { 3, 4 }, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(null!)));

        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public void SyntaxCollection_TrueAndRecorded()
    {
        var value = new int?[] { 3, 4 };

        var syntax = OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(ExpressionSyntaxFactory.CreateCollection());

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
        var value = new StringComparison?[] { StringComparison.CurrentCulture, null };

        TrueAndRecorded(value, value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void Enum_EnumsCastToObjects_TrueAndRecorded()
    {
        var value = new object?[] { StringComparison.CurrentCulture, null };

        TrueAndRecorded(value.Select(static (value) => (StringComparison?)value), value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void Enum_DifferentTypeCastToObjects_FalseAndNotRecorded()
    {
        var value = new object?[] { StringSplitOptions.RemoveEmptyEntries, null };

        FalseAndNotRecorded<StringComparison>(value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void Enum_DifferentEnumType_FalseAndNotRecorded()
    {
        var value = new StringSplitOptions?[] { StringSplitOptions.RemoveEmptyEntries, null };

        FalseAndNotRecorded<StringComparison>(value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void Enum_Int_FalseAndNotRecorded()
    {
        var value = new int?[] { 3, null };

        FalseAndNotRecorded<StringComparison>(value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void Int_SameType_TrueAndRecorded()
    {
        var value = new int?[] { 3, null };

        TrueAndRecorded(value, value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void Int_Enum_FalseAndNotRecorded()
    {
        var value = new StringComparison?[] { StringComparison.CurrentCulture, null };

        FalseAndNotRecorded<int>(value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void Int_Double_FalseAndNotRecorded()
    {
        var value = new double?[] { 3.14, null };

        FalseAndNotRecorded<int>(value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void Int_String_FalseAndNotRecorded()
    {
        var value = new[] { "CurrentCulture", null };

        FalseAndNotRecorded<int>(value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void Int_NullCollection_FalseAndNotRecorded()
    {
        IReadOnlyList<int?>? value = null;

        FalseAndNotRecorded<int>(value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void Double_Int_FalseAndNotRecorded()
    {
        var value = new int?[] { 3, null };

        FalseAndNotRecorded<double>(value, ExpressionSyntaxFactory.Create());
    }

    [AssertionMethod]
    private static void TrueAndRecorded<T>(IEnumerable<T?>? expected, object? value, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) where T : struct
    {
        var recorder = Mapper<T>.Target(Data<T?>.Recorder);

        Data<T?> data = new();

        var outcome = recorder(data, value, syntax);

        Assert.True(outcome);

        Assert.Equal<IEnumerable<T?>>(expected, data.Value);
        OneOfAssertions.Equal(syntax, data.ValueSyntax);
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

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used to expose static member of ASemanticAttributeMapper.")]
    private sealed class Mapper<T> : AAttributeMapper<Data<T?>> where T : struct
    {
        public static Func<Data<T?>, object?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> Target(Action<Data<T?>, IReadOnlyList<T?>, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> recorder) => Adapters.ArrayArgument.Params.ForNullableElements(recorder).Invoke;
    }

    private sealed class Data<T>
    {
        public static Action<Data<T>, IReadOnlyList<T?>, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> Recorder => (dataRecord, argument, syntax) =>
        {
            dataRecord.Value = argument;
            dataRecord.ValueSyntax = syntax;
            dataRecord.ValueRecorded = true;
        };

        public IReadOnlyList<T?>? Value { get; set; }
        public OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> ValueSyntax { get; set; }
        public bool ValueRecorded { get; set; }
    }
}
