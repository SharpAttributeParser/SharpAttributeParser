namespace SharpAttributeParser;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.Collections.Generic;

/// <summary>Records syntactical information about the argument of some attribute parameter.</summary>
public interface ISyntacticAttributeArgumentRecorder
{
    /// <summary>Records syntactical information about an attribute argument.</summary>
    /// <param name="syntax">The <see cref="ExpressionSyntax"/>, syntactically describing the argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether the syntactical information was successfully recorded.</returns>
    public abstract bool RecordArgumentSyntax(ExpressionSyntax syntax);

    /// <summary>Records syntactical information about an attribute <see langword="params"/>-argument.</summary>
    /// <param name="elementSyntax">The <see cref="ExpressionSyntax"/>, syntactically describing each element in the <see langword="params"/>-argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether the syntactical information was successfully recorded.</returns>
    public abstract bool RecordParamsArgumentSyntax(IReadOnlyList<ExpressionSyntax> elementSyntax);
}
