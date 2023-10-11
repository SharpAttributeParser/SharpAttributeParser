namespace SharpAttributeParser;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static class AttributeSyntaxFactory
{
    private static readonly AttributeSyntax Syntax = SyntaxFactory.Attribute(SyntaxFactory.ParseName(string.Empty));

    public static AttributeSyntax Create() => Syntax;
}
