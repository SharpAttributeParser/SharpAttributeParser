namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

internal interface ISyntacticNamedAttributeRecordBuilder : IRecordBuilder<ISyntacticNamedAttributeRecord>
{
    public abstract void WithSimpleValueSyntax(ExpressionSyntax syntax);
    public abstract void WithArrayValueSyntax(ExpressionSyntax syntax);
}
