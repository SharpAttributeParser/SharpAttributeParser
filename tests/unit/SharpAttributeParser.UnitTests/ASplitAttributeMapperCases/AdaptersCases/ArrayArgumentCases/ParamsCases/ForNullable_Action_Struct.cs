﻿namespace SharpAttributeParser.ASplitAttributeMapperCases.AdaptersCases.ArrayArgumentCases.ParamsCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Xunit;

public sealed class ForNullable_Action_Struct
{
    [Fact]
    public void NullSemanticDelegate_ArgumentNullException()
    {
        var exception = Record.Exception(() => Mapper<int>.Target(null!, SyntacticData.Recorder));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullSyntacticDelegate_ArgumentNullException()
    {
        var exception = Record.Exception(() => Mapper<int>.Target(SemanticData<int?>.Recorder, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidSemanticRecorder_NullDataRecord_ArgumentNullExceptionWhenUsed()
    {
        var (recorder, _) = Mapper<int>.Target(SemanticData<int?>.Recorder, SyntacticData.Recorder);

        var exception = Record.Exception(() => recorder(null!, new[] { 3, 4 }));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidSyntacticRecorder_NullDataRecord_ArgumentNullExceptionWhenUsed()
    {
        var (_, recorder) = Mapper<int>.Target(SemanticData<int?>.Recorder, SyntacticData.Recorder);

        var exception = Record.Exception(() => recorder(null!, ExpressionSyntaxFactory.Create()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidSyntacticRecorder_NullSyntax_ArgumentExceptionWhenUsed()
    {
        var (_, recorder) = Mapper<int>.Target(SemanticData<int?>.Recorder, SyntacticData.Recorder);

        var exception = Record.Exception(() => recorder(new SyntacticData(), (ExpressionSyntax)null!));

        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public void ValidSyntacticRecorder_NullSyntaxCollection_ArgumentExceptionWhenUsed()
    {
        var (_, recorder) = Mapper<int>.Target(SemanticData<int?>.Recorder, SyntacticData.Recorder);

        var exception = Record.Exception(() => recorder(new SyntacticData(), OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(null!)));

        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public void SyntaxCollection_TrueAndRecorded()
    {
        var syntax = OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(ExpressionSyntaxFactory.CreateCollection());

        TrueAndRecorded(syntax);
    }

    [Fact]
    public void Syntax_TrueAndRecorded()
    {
        var syntax = ExpressionSyntaxFactory.Create();

        TrueAndRecorded(syntax);
    }

    [Fact]
    public void Enum_NotArrayType_FalseAndNotRecorded()
    {
        var value = "3";

        FalseAndNotRecorded<StringComparison>(value);
    }

    [Fact]
    public void Enum_SameType_TrueAndRecorded()
    {
        var value = new StringComparison?[] { StringComparison.CurrentCulture, null };

        TrueAndRecorded(value, value);
    }

    [Fact]
    public void Enum_EnumsCastToObjects_TrueAndRecorded()
    {
        var value = new object?[] { StringComparison.CurrentCulture, null };

        TrueAndRecorded(value.Select(static (value) => (StringComparison?)value), value);
    }

    [Fact]
    public void Enum_DifferentTypeCastToObjects_FalseAndNotRecorded()
    {
        var value = new object?[] { StringSplitOptions.RemoveEmptyEntries, null };

        FalseAndNotRecorded<StringComparison>(value);
    }

    [Fact]
    public void Enum_DifferentEnumType_FalseAndNotRecorded()
    {
        var value = new StringSplitOptions?[] { StringSplitOptions.RemoveEmptyEntries, null };

        FalseAndNotRecorded<StringComparison>(value);
    }

    [Fact]
    public void Enum_Int_FalseAndNotRecorded()
    {
        var value = new int?[] { 3, null };

        FalseAndNotRecorded<StringComparison>(value);
    }

    [Fact]
    public void Int_SameType_TrueAndRecorded()
    {
        var value = new int?[] { 3, null };

        TrueAndRecorded(value, value);
    }

    [Fact]
    public void Int_Enum_FalseAndNotRecorded()
    {
        var value = new StringComparison?[] { StringComparison.CurrentCulture, null };

        FalseAndNotRecorded<int>(value);
    }

    [Fact]
    public void Int_Double_FalseAndNotRecorded()
    {
        var value = new double?[] { 3.14, null };

        FalseAndNotRecorded<int>(value);
    }

    [Fact]
    public void Int_String_FalseAndNotRecorded()
    {
        var value = new[] { "CurrentCulture", null };

        FalseAndNotRecorded<int>(value);
    }

    [Fact]
    public void Int_NullCollection_TrueAndRecorded()
    {
        IReadOnlyList<int?>? value = null;

        TrueAndRecorded(value, value);
    }

    [Fact]
    public void Double_Int_FalseAndNotRecorded()
    {
        var value = new int?[] { 3, null };

        FalseAndNotRecorded<double>(value);
    }

    [AssertionMethod]
    private static void TrueAndRecorded<T>(IEnumerable<T?>? expected, object? value) where T : struct
    {
        var (recorder, _) = Mapper<T>.Target(SemanticData<T?>.Recorder, SyntacticData.Recorder);

        SemanticData<T?> data = new();

        var outcome = recorder(data, value);

        Assert.True(outcome);

        Assert.Equal<IEnumerable<T?>>(expected, data.Value);
        Assert.True(data.ValueRecorded);
    }

    [AssertionMethod]
    private static void TrueAndRecorded(OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
    {
        var (_, recorder) = Mapper<int>.Target(SemanticData<int?>.Recorder, SyntacticData.Recorder);

        SyntacticData data = new();

        var outcome = recorder(data, syntax);

        Assert.True(outcome);

        OneOfAssertions.Equal(syntax, data.ValueSyntax);
        Assert.True(data.ValueSyntaxRecorded);
    }

    [AssertionMethod]
    private static void FalseAndNotRecorded<T>(object? value) where T : struct
    {
        var (recorder, _) = Mapper<T>.Target(SemanticData<T?>.Recorder, SyntacticData.Recorder);

        SemanticData<T?> data = new();

        var outcome = recorder(data, value);

        Assert.False(outcome);

        Assert.False(data.ValueRecorded);
    }

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used to expose static member of ASemanticAttributeMapper.")]
    private sealed class Mapper<T> : ASplitAttributeMapper<SemanticData<T?>, SyntacticData> where T : struct
    {
        public static (Func<SemanticData<T?>, object?, bool>, Func<SyntacticData, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool>) Target(Action<SemanticData<T?>, IReadOnlyList<T?>?> semanticRecorder, Action<SyntacticData, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> syntacticRecorder)
        {
            var recorders = Adapters.ArrayArgument.Params.ForNullable(semanticRecorder, syntacticRecorder);

            return (recorders.Semantic.Invoke, recorders.Syntactic.Invoke);
        }
    }

    private sealed class SemanticData<T>
    {
        public static Action<SemanticData<T>, IReadOnlyList<T?>?> Recorder => (dataRecord, argument) =>
        {
            dataRecord.Value = argument;
            dataRecord.ValueRecorded = true;
        };

        public IReadOnlyList<T?>? Value { get; set; }
        public bool ValueRecorded { get; set; }
    }

    private sealed class SyntacticData
    {
        public static Action<SyntacticData, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> Recorder => (dataRecord, syntax) =>
        {
            dataRecord.ValueSyntax = syntax;
            dataRecord.ValueSyntaxRecorded = true;
        };

        public OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> ValueSyntax { get; set; }
        public bool ValueSyntaxRecorded { get; set; }
    }
}
