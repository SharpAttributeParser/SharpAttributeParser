namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;

/// <summary>Determines the <see cref="Location"/> of attribute arguments, when an attribute is being parsed syntactically.</summary>
public interface IArgumentLocator
{
    /// <summary>Determines the <see cref="Location"/> of a generic type argument, expressed according to the provided <see cref="TypeSyntax"/>.</summary>
    /// <param name="type">The <see cref="TypeSyntax"/> describing how the argument was expressed.</param>
    /// <returns>The <see cref="Location"/> of the argument.</returns>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract Location TypeArgument(TypeSyntax type);

    /// <summary>Determines the <see cref="Location"/> of a non-array-valued argument, expressed according to the provided <see cref="ExpressionSyntax"/>.</summary>
    /// <param name="expression">The <see cref="ExpressionSyntax"/> describing how the argument was expressed.</param>
    /// <returns>The <see cref="Location"/> of the argument.</returns>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract Location SingleArgument(ExpressionSyntax expression);

    /// <summary>Determines the <see cref="Location"/> of an array-valued argument, expressed according to the provided <see cref="ExpressionSyntax"/>.</summary>
    /// <param name="expression">The <see cref="ExpressionSyntax"/> describing how the argument was expressed.</param>
    /// <remarks>If the elements were specified as a <see langword="params"/> array, use <see cref="ParamsArguments(IReadOnlyList{ExpressionSyntax})"/> instead.</remarks>
    /// <returns>The <see cref="Location"/> of the entire argument, and of each element in the collection.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract (Location Collection, IReadOnlyList<Location> Elements) ArrayArgument(ExpressionSyntax expression);

    /// <summary>Determines the <see cref="Location"/> of an array-valued argument, expressed as a <see langword="params"/> array according to the provided <see cref="ExpressionSyntax"/>.</summary>
    /// <param name="expressions">The <see cref="ExpressionSyntax"/> describing how each element of the <see langword="params"/> array was expressed.</param>
    /// <returns>The <see cref="Location"/> of the entire argument, and of each element in the collection.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract (Location Collection, IReadOnlyList<Location> Elements) ParamsArguments(IReadOnlyList<ExpressionSyntax> expressions);

    /// <summary>Determines the <see cref="Location"/> of an array-valued argument, expressed as a <see langword="params"/> array according to the provided <see cref="ExpressionSyntax"/>.</summary>
    /// <param name="expressions">The <see cref="ExpressionSyntax"/> describing how each element of the <see langword="params"/> array was expressed.</param>
    /// <returns>The <see cref="Location"/> of the entire argument, and of each element in the collection.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract (Location Collection, IReadOnlyList<Location> Elements) ParamsArguments(params ExpressionSyntax[] expressions);
}
