namespace SharpAttributeParser.ASplitAttributeMapperCases.AdaptersCases.ArrayArgumentCases.NonParamsCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Xunit;

public sealed class ForNullable_Func_Class
{
    [Fact]
    public void NullSemanticDelegate_ArgumentNullException()
    {
        var exception = Record.Exception(() => Mapper<string>.Target(null!, SyntacticData.TrueRecorder));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullSyntacticDelegate_ArgumentNullException()
    {
        var exception = Record.Exception(() => Mapper<string>.Target(SemanticData<string>.TrueRecorder, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidSemanticRecorder_NullDataRecord_ArgumentNullExceptionWhenUsed()
    {
        var (recorder, _) = Mapper<string>.Target(SemanticData<string>.TrueRecorder, SyntacticData.TrueRecorder);

        var exception = Record.Exception(() => recorder(null!, new[] { "3", "4" }));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidSyntacticRecorder_NullDataRecord_ArgumentNullExceptionWhenUsed()
    {
        var (_, recorder) = Mapper<string>.Target(SemanticData<string>.TrueRecorder, SyntacticData.TrueRecorder);

        var exception = Record.Exception(() => recorder(null!, ExpressionSyntaxFactory.Create()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidSyntacticRecorder_NullSyntax_ArgumentExceptionWhenUsed()
    {
        var (_, recorder) = Mapper<string>.Target(SemanticData<string>.TrueRecorder, SyntacticData.TrueRecorder);

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
    public void String_NotArrayType_FalseAndNotRecorded()
    {
        var value = "3";

        FalseAndNotRecorded<string>(value);
    }

    [Fact]
    public void String_SameType_TrueAndRecorded()
    {
        var value = new[] { "3", null };

        TrueAndRecorded(value, value);
    }

    [Fact]
    public void String_StringsCastToObjects_TrueAndRecorded()
    {
        var value = new object?[] { "3", null };

        TrueAndRecorded(value.Select(static (value) => (string?)value), value);
    }

    [Fact]
    public void String_DifferentType_FalseAndNotRecorded()
    {
        var value = new object[] { "3", StringComparison.OrdinalIgnoreCase };

        FalseAndNotRecorded<string>(value);
    }

    [Fact]
    public void String_NullCollection_TrueAndRecorded()
    {
        IReadOnlyList<string>? value = null;

        TrueAndRecorded(value, value);
    }

    [Fact]
    public void FalseReturningRecorder_FalseAndRecorded()
    {
        var (semanticRecorder, syntacticRecorder) = Mapper<string>.Target(SemanticData<string>.FalseRecorder, SyntacticData.FalseRecorder);

        var value = new[] { "3", "4" };
        var syntax = ExpressionSyntaxFactory.Create();

        SemanticData<string> semanticData = new();
        SyntacticData syntacticData = new();

        var sharedOutcome = semanticRecorder(semanticData, value);
        var semanticOutcome = syntacticRecorder(syntacticData, syntax);

        Assert.False(sharedOutcome);
        Assert.False(semanticOutcome);

        Assert.Equal(value, semanticData.Value);
        Assert.True(semanticData.ValueRecorded);

        Assert.Equal(syntax, syntacticData.ValueSyntax);
        Assert.True(syntacticData.ValueSyntaxRecorded);
    }

    [AssertionMethod]
    private static void TrueAndRecorded<T>(IEnumerable<T?>? expected, object? value) where T : class
    {
        var (recorder, _) = Mapper<T>.Target(SemanticData<T>.TrueRecorder, SyntacticData.TrueRecorder);

        SemanticData<T> data = new();

        var outcome = recorder(data, value);

        Assert.True(outcome);

        Assert.Equal<IEnumerable<T?>>(expected, data.Value);
        Assert.True(data.ValueRecorded);
    }

    [AssertionMethod]
    private static void TrueAndRecorded(ExpressionSyntax syntax)
    {
        var (_, recorder) = Mapper<string>.Target(SemanticData<string>.TrueRecorder, SyntacticData.TrueRecorder);

        SyntacticData data = new();

        var outcome = recorder(data, syntax);

        Assert.True(outcome);

        Assert.Equal(syntax, data.ValueSyntax);
        Assert.True(data.ValueSyntaxRecorded);
    }

    [AssertionMethod]
    private static void FalseAndNotRecorded<T>(object? value) where T : class
    {
        var (recorder, _) = Mapper<T>.Target(SemanticData<T>.TrueRecorder, SyntacticData.TrueRecorder);

        SemanticData<T> data = new();

        var outcome = recorder(data, value);

        Assert.False(outcome);

        Assert.False(data.ValueRecorded);
    }

    [AssertionMethod]
    private static void FalseAndNotRecorded(OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
    {
        var (_, recorder) = Mapper<string>.Target(SemanticData<string>.TrueRecorder, SyntacticData.TrueRecorder);

        SyntacticData data = new();

        var outcome = recorder(data, syntax);

        Assert.False(outcome);

        Assert.False(data.ValueSyntaxRecorded);
    }

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used to expose static member of ASemanticAttributeMapper.")]
    private sealed class Mapper<T> : ASplitAttributeMapper<SemanticData<T>, SyntacticData> where T : class
    {
        public static (Func<SemanticData<T>, object?, bool>, Func<SyntacticData, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool>) Target(Func<SemanticData<T>, IReadOnlyList<T?>?, bool> semanticRecorder, Func<SyntacticData, ExpressionSyntax, bool> syntacticRecorder)
        {
            var recorders = Adapters.ArrayArgument.NonParams.ForNullable(semanticRecorder, syntacticRecorder);

            return (recorders.Semantic.Invoke, recorders.Syntactic.Invoke);
        }
    }

    private sealed class SemanticData<T>
    {
        public static Func<SemanticData<T>, IReadOnlyList<T?>?, bool> TrueRecorder => (dataRecord, argument) =>
        {
            Recorder(dataRecord, argument);

            return true;
        };

        public static Func<SemanticData<T>, IReadOnlyList<T?>?, bool> FalseRecorder => (dataRecord, argument) =>
        {
            Recorder(dataRecord, argument);

            return false;
        };

        public static void Recorder(SemanticData<T> dataRecord, IReadOnlyList<T?>? argument)
        {
            dataRecord.Value = argument;
            dataRecord.ValueRecorded = true;
        }

        public IReadOnlyList<T?>? Value { get; set; }
        public bool ValueRecorded { get; set; }
    }

    private sealed class SyntacticData
    {
        public static Func<SyntacticData, ExpressionSyntax, bool> TrueRecorder => (dataRecord, syntax) =>
        {
            Recorder(dataRecord, syntax);

            return true;
        };

        public static Func<SyntacticData, ExpressionSyntax, bool> FalseRecorder => (dataRecord, syntax) =>
        {
            Recorder(dataRecord, syntax);

            return false;
        };

        public static void Recorder(SyntacticData dataRecord, ExpressionSyntax syntax)
        {
            dataRecord.ValueSyntax = syntax;
            dataRecord.ValueSyntaxRecorded = true;
        }

        public ExpressionSyntax? ValueSyntax { get; set; }
        public bool ValueSyntaxRecorded { get; set; }
    }
}
