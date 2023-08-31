namespace SharpAttributeParser;

using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>Records syntactic information about the arguments of named parameters.</summary>
public interface ISyntacticNamedArgumentRecorder
{
    /// <summary>Attempts to record syntactic information about an argument of a named parameter.</summary>
    /// <param name="parameterName">The name of the named parameter.</param>
    /// <param name="syntax">The syntactic description of the argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether syntactic information was successfully recorded.</returns>
    public abstract bool TryRecordArgument(string parameterName, ExpressionSyntax syntax);
}
