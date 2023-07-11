namespace SharpAttributeParser.ASyntacticAttributeMapperCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using System.Collections.Generic;

internal sealed class Data
{
    public ExpressionSyntax? T1Syntax { get; set; }
    public bool T1SyntaxRecorded { get; set; }

    public ExpressionSyntax? T2Syntax { get; set; }
    public bool T2SyntaxRecorded { get; set; }

    public OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> ValueASyntax { get; set; }
    public bool ValueASyntaxRecorded { get; set; }

    public OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> ValueBSyntax { get; set; }
    public bool ValueBSyntaxRecorded { get; set; }
}
