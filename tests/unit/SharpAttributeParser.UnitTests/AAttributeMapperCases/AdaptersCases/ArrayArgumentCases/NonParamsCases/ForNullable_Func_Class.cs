namespace SharpAttributeParser.AAttributeMapperCases.AdaptersCases.ArrayArgumentCases.NonParamsCases;

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
    public void NullDelegate_ArgumentNullException()
    {
        var exception = Record.Exception(() => Mapper<string>.Target(null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidRecorder_NullDataRecord_ArgumentNullExceptionWhenUsed()
    {
        var recorder = Mapper<string>.Target(Data<string>.TrueRecorder);

        var exception = Record.Exception(() => recorder(null!, new[] { "3", "4" }, ExpressionSyntaxFactory.Create()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidRecorder_NullSyntax_ArgumentExceptionWhenUsed()
    {
        var recorder = Mapper<string>.Target(Data<string>.TrueRecorder);

        var exception = Record.Exception(() => recorder(new Data<string>(), new[] { "3", "4" }, (ExpressionSyntax)null!));

        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public void ValidRecorder_NullSyntaxCollection_FalseAndNotRecorded()
    {
        var syntax = OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(null!);

        FalseAndNotRecordedDueToSyntax<string>(new[] { "3", "4" }, syntax);
    }

    [Fact]
    public void ValidRecorder_SyntaxCollection_FalseAndNotRecorded()
    {
        var syntax = OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(ExpressionSyntaxFactory.CreateCollection());

        FalseAndNotRecordedDueToSyntax<string>(new[] { "3", "4" }, syntax);
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
    public void String_NullCollection_TrueAndRecorded()
    {
        IReadOnlyList<string>? value = null;

        TrueAndRecorded(value, value, ExpressionSyntaxFactory.Create());
    }

    [Fact]
    public void FalseReturningRecorder_FalseAndRecorded()
    {
        var recorder = Mapper<string>.Target(Data<string>.FalseRecorder);

        var value = new[] { "3", "4" };
        var syntax = ExpressionSyntaxFactory.Create();

        Data<string> data = new();

        var outcome = recorder(data, value, syntax);

        Assert.False(outcome);

        Assert.Equal(value, data.Value);
        Assert.Equal(syntax, data.ValueSyntax);
        Assert.True(data.ValueRecorded);
    }

    [AssertionMethod]
    private static void TrueAndRecorded<T>(IEnumerable<T?>? expected, object? value, ExpressionSyntax syntax) where T : class
    {
        var recorder = Mapper<T>.Target(Data<T>.TrueRecorder);

        Data<T> data = new();

        var outcome = recorder(data, value, syntax);

        Assert.True(outcome);

        Assert.Equal<IEnumerable<T?>>(expected, data.Value);
        Assert.Equal(syntax, data.ValueSyntax);
        Assert.True(data.ValueRecorded);
    }

    [AssertionMethod]
    private static void FalseAndNotRecorded<T>(object? value, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) where T : class
    {
        var recorder = Mapper<T>.Target(Data<T>.TrueRecorder);

        Data<T> data = new();

        var outcome = recorder(data, value, syntax);

        Assert.False(outcome);

        Assert.False(data.ValueRecorded);
    }

    [AssertionMethod]
    private static void FalseAndNotRecordedDueToSyntax<T>(object? value, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) where T : class
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

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used to expose static member of AAttributeMapper.")]
    private sealed class Mapper<T> : AAttributeMapper<Data<T>> where T : class
    {
        public static Func<Data<T>, object?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> Target(Func<Data<T>, IReadOnlyList<T?>?, ExpressionSyntax, bool> recorder) => Adapters.ArrayArgument.NonParams.ForNullable(recorder).Invoke;
    }

    private sealed class Data<T>
    {
        public static Func<Data<T>, IReadOnlyList<T?>?, ExpressionSyntax, bool> TrueRecorder => (dataRecord, argument, syntax) =>
        {
            Recorder(dataRecord, argument, syntax);

            return true;
        };

        public static Func<Data<T>, IReadOnlyList<T?>?, ExpressionSyntax, bool> FalseRecorder => (dataRecord, argument, syntax) =>
        {
            Recorder(dataRecord, argument, syntax);

            return false;
        };

        private static void Recorder(Data<T> dataRecord, IReadOnlyList<T?>? argument, ExpressionSyntax syntax)
        {
            dataRecord.Value = argument;
            dataRecord.ValueSyntax = syntax;
            dataRecord.ValueRecorded = true;
        }

        public IReadOnlyList<T?>? Value { get; set; }
        public ExpressionSyntax? ValueSyntax { get; set; }
        public bool ValueRecorded { get; set; }
    }
}
