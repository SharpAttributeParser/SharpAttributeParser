namespace SharpAttributeParser.ASplitAttributeMapperCases.AdaptersCases.SimpleArgumentCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Xunit;

public sealed class ForNullable_Action_Struct
{
    [Fact]
    public void NullSemanticDelegate_ArgumentNullExceptionWhenUsed()
    {
        var exception = Record.Exception(() => Mapper<int>.Target(null!, SyntacticData.Recorder));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullSyntacticDelegate_ArgumentNullExceptionWhenUsed()
    {
        var exception = Record.Exception(() => Mapper<int>.Target(SemanticData<int?>.Recorder, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidSemanticRecorder_NullDataRecord_ArgumentNullExceptionWhenUsed()
    {
        var (recorder, _) = Mapper<int>.Target(SemanticData<int?>.Recorder, SyntacticData.Recorder);

        var exception = Record.Exception(() => recorder(null!, 3));

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
    public void NullSyntaxCollection_FalseAndNotRecorded()
    {
        var syntax = OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(null!);

        FalseAndNotRecorded(syntax);
    }

    [Fact]
    public void SyntaxCollection_FalseAndNotRecorded()
    {
        var syntax = OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(ExpressionSyntaxFactory.CreateCollection());

        FalseAndNotRecorded(syntax);
    }

    [Fact]
    public void Syntax_TrueAndRecorded()
    {
        var syntax = ExpressionSyntaxFactory.Create();

        TrueAndRecorded(syntax);
    }

    [Fact]
    public void Enum_SameType_TrueAndRecorded()
    {
        var value = StringComparison.OrdinalIgnoreCase;

        TrueAndRecorded<StringComparison>(value, value);
    }

    [Fact]
    public void Enum_DifferentEnumType_FalseAndNotRecorded()
    {
        var value = StringSplitOptions.TrimEntries;

        FalseAndNotRecorded<StringComparison>(value);
    }

    [Fact]
    public void Enum_Null_TrueAndRecorded()
    {
        StringComparison? value = null;

        TrueAndRecorded(value, value);
    }

    [Fact]
    public void Enum_Int_FalseAndNotRecorded()
    {
        var value = 3;

        FalseAndNotRecorded<StringComparison>(value);
    }

    [Fact]
    public void Int_SameType_TrueAndRecorded()
    {
        var value = 3;

        TrueAndRecorded<int>(value, value);
    }

    [Fact]
    public void Int_IntCastToObject_TrueAndRecorded()
    {
        object value = 3;

        TrueAndRecorded<int>((int)value, value);
    }

    [Fact]
    public void Int_Enum_FalseAndNotRecorded()
    {
        var value = StringComparison.Ordinal;

        FalseAndNotRecorded<int>(value);
    }

    [Fact]
    public void Int_Double_FalseAndNotRecorded()
    {
        var value = 3.14;

        FalseAndNotRecorded<int>(value);
    }

    [Fact]
    public void Int_String_FalseAndNotRecorded()
    {
        var value = "3";

        FalseAndNotRecorded<int>(value);
    }

    [Fact]
    public void NullableInt_Null_TrueAndRecorded()
    {
        int? value = null;

        TrueAndRecorded(value, value);
    }

    [AssertionMethod]
    private static void TrueAndRecorded<T>(T? expected, object? value) where T : struct
    {
        var (recorder, _) = Mapper<T>.Target(SemanticData<T?>.Recorder, SyntacticData.Recorder);

        SemanticData<T?> data = new();

        var outcome = recorder(data, value);

        Assert.True(outcome);

        Assert.Equal(expected, data.Value);
        Assert.True(data.ValueRecorded);
    }

    [AssertionMethod]
    private static void TrueAndRecorded(ExpressionSyntax syntax)
    {
        var (_, recorder) = Mapper<int>.Target(SemanticData<int?>.Recorder, SyntacticData.Recorder);

        SyntacticData data = new();

        var outcome = recorder(data, syntax);

        Assert.True(outcome);

        Assert.Equal(syntax, data.ValueSyntax);
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

    [AssertionMethod]
    private static void FalseAndNotRecorded(OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
    {
        var (_, recorder) = Mapper<int>.Target(SemanticData<int?>.Recorder, SyntacticData.Recorder);

        SyntacticData data = new();

        var outcome = recorder(data, syntax);

        Assert.False(outcome);

        Assert.False(data.ValueSyntaxRecorded);
    }

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used to expose static member of ASyntacticAttributeMapper.")]
    private sealed class Mapper<T> : ASplitAttributeMapper<SemanticData<T?>, SyntacticData> where T : struct
    {
        public static (Func<SemanticData<T?>, object?, bool>, Func<SyntacticData, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool>) Target(Action<SemanticData<T?>, T?> semanticRecorder, Action<SyntacticData, ExpressionSyntax> syntacticRecorder)
        {
            var recorders = Adapters.SimpleArgument.ForNullable(semanticRecorder, syntacticRecorder);

            return (recorders.Semantic.Invoke, recorders.Syntactic.Invoke);
        }
    }

    private sealed class SemanticData<T>
    {
        public static Action<SemanticData<T>, T> Recorder => (dataRecord, argument) =>
        {
            dataRecord.Value = argument;
            dataRecord.ValueRecorded = true;
        };

        public T? Value { get; set; }
        public bool ValueRecorded { get; set; }
    }

    private sealed class SyntacticData
    {
        public static Action<SyntacticData, ExpressionSyntax> Recorder => (dataRecord, syntax) =>
        {
            dataRecord.ValueSyntax = syntax;
            dataRecord.ValueSyntaxRecorded = true;
        };

        public ExpressionSyntax? ValueSyntax { get; set; }
        public bool ValueSyntaxRecorded { get; set; }
    }
}
