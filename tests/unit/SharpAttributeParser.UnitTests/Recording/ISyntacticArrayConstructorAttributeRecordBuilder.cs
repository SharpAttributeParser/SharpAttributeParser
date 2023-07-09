namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.Collections.Generic;

internal interface ISyntacticArrayConstructorAttributeRecordBuilder : IRecordBuilder<ISemanticArrayConstructorAttributeRecord>
{
    public abstract void WithValue(IReadOnlyList<object?>? value, ExpressionSyntax syntax);
}
