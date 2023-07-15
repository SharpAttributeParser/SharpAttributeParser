namespace SharpAttributeParser;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;

/// <summary>Records the argument of some attribute constructor parameter, together with syntactical information about the argument.</summary>
public interface IAttributeConstructorArgumentRecorder : IAttributeArgumentRecorder
{
    /// <summary>Records the provided attribute argument.</summary>
    /// <param name="argument">The recorded argument.</param>
    /// <param name="elementSyntax">The <see cref="ExpressionSyntax"/> syntactically describing each element of the <see langword="params"/>-argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    public abstract bool RecordParamsArgument(object? argument, IReadOnlyList<ExpressionSyntax> elementSyntax);
}
