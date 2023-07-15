namespace SharpAttributeParser.AAttributeMapperCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using System.Collections.Generic;

internal sealed class Mapper : AAttributeMapper<Data>
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

    protected override IEnumerable<(OneOf<int, string> IndexOrName, DTypeArgumentRecorder Recorder)> AddTypeParameterMappings()
    {
        yield return (IndexT1, RecordT1);
        yield return (TypeNameT1, RecordT1);
        yield return (IndexT2, RecordT2);
        yield return (TypeNameT2, RecordT2);
    }

    protected override IEnumerable<(string Name, DArgumentRecorder Recorder)> AddParameterMappings()
    {
        yield return (NameValueA, RecordValueA);
        yield return (NameValueB, RecordValueB);
    }

    private static bool RecordT1(Data dataRecord, ITypeSymbol argument, ExpressionSyntax syntax)
    {
        dataRecord.T1 = argument;
        dataRecord.T1Syntax = syntax;
        dataRecord.T1Recorded = true;

        return true;
    }

    private static bool RecordT2(Data dataRecord, ITypeSymbol argument, ExpressionSyntax syntax)
    {
        dataRecord.T2 = argument;
        dataRecord.T2Syntax = syntax;
        dataRecord.T2Recorded = true;

        return true;
    }

    private static bool RecordValueA(Data dataRecord, object? argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
    {
        dataRecord.ValueA = argument;
        dataRecord.ValueASyntax = syntax;
        dataRecord.ValueARecorded = true;

        return true;
    }

    private static bool RecordValueB(Data dataRecord, object? argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
    {
        dataRecord.ValueB = argument;
        dataRecord.ValueBSyntax = syntax;
        dataRecord.ValueBRecorded = true;

        return true;
    }
}
