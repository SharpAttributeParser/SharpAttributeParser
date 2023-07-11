namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

internal interface ISyntacticArrayConstructorAttributeRecordBuilder : IRecordBuilder<ISyntacticArrayConstructorAttributeRecord>
{
    public abstract void WithValueSyntax(ExpressionSyntax syntax);
}
