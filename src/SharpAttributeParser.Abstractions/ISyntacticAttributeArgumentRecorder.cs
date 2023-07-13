namespace SharpAttributeParser;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;

/// <summary>Records syntactical information about the argument of some attribute parameter.</summary>
public interface ISyntacticAttributeArgumentRecorder
{
    /// <summary>Records syntactical information about an attribute argument.</summary>
    /// <param name="syntax">The <see cref="ExpressionSyntax"/> syntactically describing the argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether the syntactical information was successfully recorded.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    public abstract bool RecordArgumentSyntax(ExpressionSyntax syntax);
}
