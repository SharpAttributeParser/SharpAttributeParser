namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using System;

/// <summary>Records the arguments of an attribute.</summary>
public interface ISemanticAttributeRecorder
{
    /// <summary>Attempts to record the argument of a type-parameter.</summary>
    /// <param name="parameter">The <see cref="ITypeParameterSymbol"/> representing the type-parameter.</param>
    /// <param name="argument">The argument of the type-parameter.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract bool TryRecordTypeArgument(ITypeParameterSymbol parameter, ITypeSymbol argument);

    /// <summary>Attempts to record the argument of a constructor parameter.</summary>
    /// <param name="parameter">The <see cref="IParameterSymbol"/> representing the parameter.</param>
    /// <param name="argument">The argument of the parameter.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract bool TryRecordConstructorArgument(IParameterSymbol parameter, object? argument);

    /// <summary>Attempts to record the argument of a named parameter.</summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="argument">The argument of the parameter.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract bool TryRecordNamedArgument(string parameterName, object? argument);
}

/// <summary>Records the arguments of an attribute.</summary>
/// <typeparam name="TRecord">The type to which the <see cref="ISemanticAttributeRecorder"/> records arguments.</typeparam>
public interface ISemanticAttributeRecorder<out TRecord> : ISemanticAttributeRecorder
{
    /// <summary>Retrieves the <typeparamref name="TRecord"/>, representing the recorded arguments.</summary>
    /// <returns>The <typeparamref name="TRecord"/> representing the recorded arguments.</returns>
    /// <exception cref="InvalidOperationException"/>
    public abstract TRecord GetRecord();
}
