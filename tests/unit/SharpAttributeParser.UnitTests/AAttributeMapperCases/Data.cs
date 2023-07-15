namespace SharpAttributeParser.AAttributeMapperCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using System.Collections.Generic;

internal sealed class Data
{
    public ITypeSymbol? T1 { get; set; }
    public ExpressionSyntax? T1Syntax { get; set; }
    public bool T1Recorded { get; set; }

    public ITypeSymbol? T2 { get; set; }
    public ExpressionSyntax? T2Syntax { get; set; }
    public bool T2Recorded { get; set; }

    public object? ValueA { get; set; }
    public OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> ValueASyntax { get; set; }
    public bool ValueARecorded { get; set; }

    public object? ValueB { get; set; }
    public OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> ValueBSyntax { get; set; }
    public bool ValueBRecorded { get; set; }
}
