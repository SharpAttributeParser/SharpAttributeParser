namespace SharpAttributeParser;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;

/// <summary>Records the argument of some attribute parameter, together with syntactical information about the argument.</summary>
public interface IAttributeArgumentRecorder
{
    /// <summary>Records the provided attribute argument.</summary>
    /// <param name="argument">The recorded argument.</param>
    /// <param name="syntax">The <see cref="ExpressionSyntax"/> syntactically describing the argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    public abstract bool RecordArgument(object? argument, ExpressionSyntax syntax);
}
