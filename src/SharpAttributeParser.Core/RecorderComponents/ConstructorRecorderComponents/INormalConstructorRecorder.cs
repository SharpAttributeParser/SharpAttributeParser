namespace SharpAttributeParser.RecorderComponents.ConstructorRecorderComponents;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>Records the normal arguments of constructor parameters, together with syntactic information about the arguments.</summary>
public interface INormalConstructorRecorder
{
    /// <summary>Attempts to record an argument of a constructor parameter, together with syntactic information about the argument.</summary>
    /// <param name="parameter">The constructor parameter.</param>
    /// <param name="argument">The argument of the constructor parameter.</param>
    /// <param name="syntax">The syntactic description of the argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    public abstract bool TryRecordArgument(IParameterSymbol parameter, object? argument, ExpressionSyntax syntax);
}
