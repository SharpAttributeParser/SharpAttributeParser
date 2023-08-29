﻿namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;

/// <summary>Records the arguments of type parameters, together with syntactic information about the arguments.</summary>
public interface ICombinedTypeArgumentRecorder
{
    /// <summary>Attempts to record an argument of a type parameter, together with syntactic information about the argument.</summary>
    /// <param name="parameter">The type parameter.</param>
    /// <param name="argument">The argument of the type parameter.</param>
    /// <param name="syntax">The syntactic description of the argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract bool TryRecordArgument(ITypeParameterSymbol parameter, ITypeSymbol argument, ExpressionSyntax syntax);
}