namespace SharpAttributeParser.ASyntacticAttributeMapperCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using System.Collections.Generic;

internal sealed class Mapper : ASyntacticAttributeMapper<Data>
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

    protected override IEnumerable<(OneOf<int, string> IndexOrName, DTypeArgumentSyntaxRecorder Recorder)> AddTypeParameterMappings()
    {
        yield return (IndexT1, RecordT1Syntax);
        yield return (TypeNameT1, RecordT1Syntax);
        yield return (IndexT2, RecordT2Syntax);
        yield return (TypeNameT2, RecordT2Syntax);
    }

    protected override IEnumerable<(string Name, DArgumentSyntaxRecorder Recorder)> AddParameterMappings()
    {
        yield return (NameValueA, RecordValueASyntax);
        yield return (NameValueB, RecordValueBSyntax);
    }

    private static bool RecordT1Syntax(Data dataRecord, ExpressionSyntax syntax)
    {
        dataRecord.T1Syntax = syntax;
        dataRecord.T1SyntaxRecorded = true;

        return true;
    }

    private static bool RecordT2Syntax(Data dataRecord, ExpressionSyntax syntax)
    {
        dataRecord.T2Syntax = syntax;
        dataRecord.T2SyntaxRecorded = true;

        return true;
    }

    private static bool RecordValueASyntax(Data dataRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
    {
        dataRecord.ValueASyntax = syntax;
        dataRecord.ValueASyntaxRecorded = true;

        return true;
    }

    private static bool RecordValueBSyntax(Data dataRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
    {
        dataRecord.ValueBSyntax = syntax;
        dataRecord.ValueBSyntaxRecorded = true;

        return true;
    }
}
