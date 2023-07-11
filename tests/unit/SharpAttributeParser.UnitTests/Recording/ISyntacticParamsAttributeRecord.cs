namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using System.Collections.Generic;

public interface ISyntacticParamsAttributeRecord
{
    public abstract OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> ValueSyntax { get; }
    public abstract bool ValueSyntaxRecorded { get; }
}
