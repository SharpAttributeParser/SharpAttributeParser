namespace SharpAttributeParser;

using System;

/// <summary>Provides adapters that may be applied to semantically parsed attribute arguments before they are recorded.</summary>
/// <typeparam name="TData">The type representing the recorded arguments.</typeparam>
public interface ISemanticArgumentAdapter<TData>
{
    /// <summary>Provides adapters related to collections.</summary>
    public abstract ISemanticCollectionArgumentAdapter<TData> Collections { get; }

    /// <summary>Produces a recorder which ensures that the argument is of type <typeparamref name="T"/> before attempting to record it.</summary>
    /// <typeparam name="T">The expected type of the argument.</typeparam>
    /// <param name="recorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/>.</param>
    /// <returns>A mapping from some parameter to the produced recorder.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract DSemanticAttributeParameterMapping<TData> For<T>(Func<TData, T, bool> recorder) where T : notnull;

    /// <summary>Produces a recorder which ensures that the argument is of type <typeparamref name="T"/> before attempting to record it - and which returns <see langword="true"/> if this is the case.</summary>
    /// <typeparam name="T">The expected type of the argument.</typeparam>
    /// <param name="recorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/>.</param>
    /// <returns>A mapping from some parameter to the produced recorder.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract DSemanticAttributeParameterMapping<TData> For<T>(Action<TData, T> recorder) where T : notnull;

    /// <summary>Produces a recorder which ensures that the argument is of type <typeparamref name="T"/>, or <see langword="null"/>, before attempting to record it.</summary>
    /// <typeparam name="T">The expected type of the argument.</typeparam>
    /// <param name="recorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/> or <see langword="null"/>.</param>
    /// <returns>A mapping from some parameter to the produced recorder.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract DSemanticAttributeParameterMapping<TData> ForNullable<T>(Func<TData, T?, bool> recorder) where T : class;

    /// <summary>Produces a recorder which ensures that the argument is of type <typeparamref name="T"/>, or <see langword="null"/>, before attempting to record it - and which returns <see langword="true"/> if this is the case.</summary>
    /// <typeparam name="T">The expected type of the argument.</typeparam>
    /// <param name="recorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/> or <see langword="null"/>.</param>
    /// <returns>A mapping from some parameter to the produced recorder.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract DSemanticAttributeParameterMapping<TData> ForNullable<T>(Action<TData, T?> recorder) where T : class;

    /// <summary>Produces a recorder which ensures that the argument is of type <typeparamref name="T"/>, or <see langword="null"/>, before attempting to record it.</summary>
    /// <typeparam name="T">The expected type of the argument.</typeparam>
    /// <param name="recorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/> or <see langword="null"/>.</param>
    /// <returns>A mapping from some parameter to the produced recorder.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract DSemanticAttributeParameterMapping<TData> ForNullable<T>(Func<TData, T?, bool> recorder) where T : struct;

    /// <summary>Produces a recorder which ensures that the argument is of type <typeparamref name="T"/>, or <see langword="null"/>, before attempting to record it - and which returns <see langword="true"/> if this is the case.</summary>
    /// <typeparam name="T">The expected type of the argument.</typeparam>
    /// <param name="recorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/> or <see langword="null"/>.</param>
    /// <returns>A mapping from some parameter to the produced recorder.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract DSemanticAttributeParameterMapping<TData> ForNullable<T>(Action<TData, T?> recorder) where T : struct;
}
