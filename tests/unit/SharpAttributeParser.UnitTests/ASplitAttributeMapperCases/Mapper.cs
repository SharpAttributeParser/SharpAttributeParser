namespace SharpAttributeParser.ASplitAttributeMapperCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using System.Collections.Generic;

internal sealed class Mapper : ASplitAttributeMapper<SemanticData, SyntacticData>
{
    public static int IndexNone => IndexT1 + IndexT2;
    public static int IndexT1 => 1;
    public static int IndexT2 => 2;

    public static string TypeNameNone => $"{TypeNameT1}{TypeNameT2}";
    public static string TypeNameT1 => "T1";
    public static string TypeNameT2 => "T2";

    public static string NameNone => $"{NameValueA}{NameValueB}";
    public static string NameValueA => "A";
    public static string NameValueB => "B";

    protected override IEnumerable<(OneOf<int, string> IndexOrName, ITypeArgumentRecorderProvider Recorders)> AddTypeParameterMappings()
    {
        yield return (IndexT1, new TypeArgumentRecorderProvider(RecordT1, RecordT1));
        yield return (TypeNameT1, new TypeArgumentRecorderProvider(RecordT1, RecordT1));
        yield return (IndexT2, new TypeArgumentRecorderProvider(RecordT2, RecordT2));
        yield return (TypeNameT2, new TypeArgumentRecorderProvider(RecordT2, RecordT2));
    }

    protected override IEnumerable<(string Name, IArgumentRecorderProvider Recorders)> AddParameterMappings()
    {
        yield return (NameValueA, new ArgumentRecorderProvider(RecordValueA, RecordValueA));
        yield return (NameValueB, new ArgumentRecorderProvider(RecordValueB, RecordValueB));
    }

    private static bool RecordT1(SemanticData dataRecord, ITypeSymbol argument)
    {
        dataRecord.T1 = argument;
        dataRecord.T1Recorded = true;

        return true;
    }

    private static bool RecordT1(SyntacticData dataRecord, ExpressionSyntax syntax)
    {
        dataRecord.T1Syntax = syntax;
        dataRecord.T1SyntaxRecorded = true;

        return true;
    }

    private static bool RecordT2(SemanticData dataRecord, ITypeSymbol argument)
    {
        dataRecord.T2 = argument;
        dataRecord.T2Recorded = true;

        return true;
    }

    private static bool RecordT2(SyntacticData dataRecord, ExpressionSyntax syntax)
    {
        dataRecord.T2Syntax = syntax;
        dataRecord.T2SyntaxRecorded = true;

        return true;
    }

    private static bool RecordValueA(SemanticData dataRecord, object? argument)
    {
        dataRecord.ValueA = argument;
        dataRecord.ValueARecorded = true;

        return true;
    }

    private static bool RecordValueA(SyntacticData dataRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
    {
        dataRecord.ValueASyntax = syntax;
        dataRecord.ValueASyntaxRecorded = true;

        return true;
    }

    private static bool RecordValueB(SemanticData dataRecord, object? argument)
    {
        dataRecord.ValueB = argument;
        dataRecord.ValueBRecorded = true;

        return true;
    }

    private static bool RecordValueB(SyntacticData dataRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
    {
        dataRecord.ValueBSyntax = syntax;
        dataRecord.ValueBSyntaxRecorded = true;

        return true;
    }
}
