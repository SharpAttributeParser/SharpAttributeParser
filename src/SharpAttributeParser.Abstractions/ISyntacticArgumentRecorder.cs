namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;

/// <summary>Responsible for recording the parsed arguments of an attribute, when parsed syntactically.</summary>
public interface ISyntacticArgumentRecorder
{
    /// <summary>Attempts to record the argument of a type parameter.</summary>
    /// <param name="parameter">The parameter with which the argument is associated.</param>
    /// <param name="value">The value of the argument.</param>
    /// <param name="location">The <see cref="Location"/> of the argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract bool TryRecordGenericArgument(ITypeParameterSymbol parameter, ITypeSymbol value, Location location);

    /// <summary>Attempts to record the argument of a constructor parameter.</summary>
    /// <param name="parameter">The parameter with which the argument is associated.</param>
    /// <param name="value">The value of the constructor argument.</param>
    /// <param name="location">The <see cref="Location"/> of the argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract bool TryRecordConstructorArgument(IParameterSymbol parameter, object? value, Location location);

    /// <summary>Attempts to record the array-valued argument of a constructor parameter.</summary>
    /// <param name="parameter">The parameter with which the argument is associated.</param>
    /// <param name="value">The value of the argument.</param>
    /// <param name="collectionLocation">The <see cref="Location"/> of the entire argument.</param>
    /// <param name="elementLocations">The <see cref="Location"/> of the individual elements in the array-valued argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract bool TryRecordConstructorArgument(IParameterSymbol parameter, IReadOnlyList<object?>? value, Location collectionLocation, IReadOnlyList<Location> elementLocations);

    /// <summary>Attempts to record the argument of a named parameter.</summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="value">The value of the argument.</param>
    /// <param name="location">The <see cref="Location"/> of the argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract bool TryRecordNamedArgument(string parameterName, object? value, Location location);

    /// <summary>Attempts to record the array-valued argument of a named parameter.</summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="value">The value of the argument.</param>
    /// <param name="collectionLocation">The <see cref="Location"/> of the entire argument.</param>
    /// <param name="elementLocations">The <see cref="Location"/> of the individual elements in the array-valued argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract bool TryRecordNamedArgument(string parameterName, IReadOnlyList<object?>? value, Location collectionLocation, IReadOnlyList<Location> elementLocations);
}
