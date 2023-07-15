namespace SharpAttributeParser;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.Collections.Generic;

internal static class ExpressionSyntaxFactory
{
    private static ExpressionSyntax Syntax { get; } = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);

    public static ExpressionSyntax Create() => Syntax;
    public static IReadOnlyList<ExpressionSyntax> CreateCollection() => new[] { Create() };
}
