namespace SharpAttributeParser.Tests.IArgumentLocatorCases;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

internal static class Datasets
{
    public static ExpressionSyntax GetNullSingleArgumentExpression() => null!;
    public static ExpressionSyntax GetNullArrayArgumentExpression() => null!;
    public static ExpressionSyntax[] GetNullParamsArgumentExpression() => null!;
    public static TypeSyntax GetNullTypeArgument() => null!;

    public static ExpressionSyntax[] GetNullContainingParamsArgumentExpression() => new ExpressionSyntax[]
    {
        SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression),
        null!
    };

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used as test input.")]
    public sealed class Sources : ATestDataset<IArgumentLocator>
    {
        protected override IEnumerable<IArgumentLocator> GetSamples() => new[]
        {
            DependencyInjection.GetRequiredService<IArgumentLocator>()
        };
    }
}
