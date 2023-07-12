namespace SharpAttributeParser;

using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>Records syntactical information about the argument of some attribute parameter.</summary>
public interface ISyntacticAttributeArgumentRecorder
{
    /// <summary>Records syntactical information about an attribute argument.</summary>
    /// <param name="syntax">The <see cref="ExpressionSyntax"/> syntactically describing the argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether the syntactical information was successfully recorded.</returns>
    public abstract bool RecordArgumentSyntax(ExpressionSyntax syntax);
}
