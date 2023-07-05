namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using System;

/// <summary>Responsible for recording the parsed arguments of an attribute.</summary>
public interface ISemanticAttributeRecorder
{
    /// <summary>Attempts to record the argument of a type-parameter.</summary>
    /// <param name="parameter">The parameter with which the argument is associated.</param>
    /// <param name="argument">The argument of the type-parameter.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract bool TryRecordTypeArgument(ITypeParameterSymbol parameter, ITypeSymbol argument);

    /// <summary>Attempts to record the argument of a constructor parameter.</summary>
    /// <param name="parameter">The parameter with which the argument is associated.</param>
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

/// <summary>Responsible for recording the parsed arguments of an attribute.</summary>
/// <typeparam name="TData">The type to which the <see cref="ISemanticAttributeRecorder"/> records attribute arguments.</typeparam>
public interface ISemanticAttributeRecorder<out TData> : ISemanticAttributeRecorder
{
    /// <summary>Retrieves the <typeparamref name="TData"/>, representing the recorded arguments.</summary>
    /// <returns>The <typeparamref name="TData"/> representing the recorded arguments.</returns>
    /// <exception cref="InvalidOperationException"/>
    public abstract TData GetResult();
}
