namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.Collections.Generic;

public interface ISyntacticParamsAttributeRecord : ISemanticParamsAttributeRecord
{
    public abstract IReadOnlyList<ExpressionSyntax> ValueSyntax { get; }
}
