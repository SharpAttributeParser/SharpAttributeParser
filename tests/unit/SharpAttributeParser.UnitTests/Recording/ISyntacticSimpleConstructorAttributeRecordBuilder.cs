namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

internal interface ISyntacticSimpleConstructorAttributeRecordBuilder : IRecordBuilder<ISyntacticSimpleConstructorAttributeRecord>
{
    public abstract void WithValue(object? value, ExpressionSyntax syntax);
}
