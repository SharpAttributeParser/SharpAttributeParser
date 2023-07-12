namespace SharpAttributeParser;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.Collections.Generic;

/// <summary>Records syntactical information about the argument of some attribute constructor parameter.</summary>
public interface ISyntacticAttributeConstructorArgumentRecorder : ISyntacticAttributeArgumentRecorder
{
    /// <summary>Records syntactical information about an attribute constructor argument.</summary>
    /// <param name="elementSyntax">The <see cref="ExpressionSyntax"/> syntactically describing each element of the <see langword="params"/>-argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether the syntactical information was successfully recorded.</returns>
    public abstract bool RecordParamsArgumentSyntax(IReadOnlyList<ExpressionSyntax> elementSyntax);
}
