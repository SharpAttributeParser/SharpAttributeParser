namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.Collections.Generic;

public interface ISemanticNamedAttributeRecord
{
    public abstract object? SimpleValue { get; }
    public abstract bool SimpleValueRecorded { get; }

    public abstract IReadOnlyList<object?>? ArrayValue { get; }
    public abstract bool ArrayValueRecorded { get; }
}

public interface ISyntacticNamedAttributeRecord : ISemanticNamedAttributeRecord
{
    public abstract ExpressionSyntax SimpleValueSyntax { get; }
    public abstract ExpressionSyntax ArrayValueSyntax { get; }
}
