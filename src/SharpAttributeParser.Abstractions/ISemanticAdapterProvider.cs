namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;

/// <summary>Provides adapters that may be applied to semantically parsed arguments before they are recorded.</summary>
public interface ISemanticAdapterProvider
{
    /// <summary>Returns <see langword="true"/> for any matching type-parameter.</summary>
    /// <param name="recorder">Responsible for recording the argument of a type-parameter.</param>
    /// <returns>A recorder that returns <see langword="true"/> if the parameter name matches a registered name, and which wraps the provided recorder.</returns>
    public abstract Func<ITypeSymbol, bool> For(Action<ITypeSymbol> recorder);

    /// <summary>Ensures that the argument is of a certain type, <typeparamref name="T"/>, before attempting to record it.</summary>
    /// <typeparam name="T">The expected type of the argument.</typeparam>
    /// <param name="recorder">Reponsible for recording the argument, if it is of type <typeparamref name="T"/>.</param>
    /// <returns>A recorder capable of handling an argument of any type, and which wraps the provided recorder.</returns>
    public abstract Func<object?, bool> For<T>(Func<T, bool> recorder) where T : notnull;

    /// <summary>Ensures that all elements of the argument are of a certain type, <typeparamref name="T"/>, before attempting to record them.</summary>
    /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
    /// <param name="recorder">Reponsible for recording the argument, if all elements are of type <typeparamref name="T"/>.</param>
    /// <returns>A recorder capable of handling an argument of any type, and which wraps the provided recorder.</returns>
    public abstract Func<IReadOnlyList<object?>?, bool> For<T>(Func<IReadOnlyList<T>, bool> recorder) where T : notnull;

    /// <summary>Ensures that the argument is of a certain type, <typeparamref name="T"/>, before attempting to record it - and returns <see langword="true"/> if this is the case.</summary>
    /// <typeparam name="T">The expected type of the argument.</typeparam>
    /// <param name="recorder">Reponsible for recording the argument, if it is of type <typeparamref name="T"/>.</param>
    /// <returns>A recorder capable of handling an argument of any type, and which wraps the provided recorder.</returns>
    public abstract Func<object?, bool> For<T>(Action<T> recorder) where T : notnull;

    /// <summary>Ensures that all elements of the argument are of a certain type, <typeparamref name="T"/>, before attempting to record them - and returns <see langword="true"/> if this is the case.</summary>
    /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
    /// <param name="recorder">Reponsible for recording the argument, if all elements are of type <typeparamref name="T"/>.</param>
    /// <returns>A recorder capable of handling an argument of any type, and which wraps the provided recorder.</returns>
    public abstract Func<IReadOnlyList<object?>?, bool> For<T>(Action<IReadOnlyList<T>> recorder) where T : notnull;

    /// <summary>Ensures that the argument is of a certain type, <typeparamref name="T"/>, or <see langword="null"/>, before attempting to record it.</summary>
    /// <typeparam name="T">The expected type of the argument.</typeparam>
    /// <param name="recorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/> or <see langword="null"/>.</param>
    /// <returns>A recorder capable of handling an argument of any type, and which wraps the provided recorder.</returns>
    public abstract Func<object?, bool> ForNullable<T>(Func<T?, bool> recorder) where T : class;

    /// <summary>Ensures that the argument is of a certain type, <typeparamref name="T"/>, or <see langword="null"/>, before attempting to record it.</summary>
    /// <typeparam name="T">The expected type of the argument.</typeparam>
    /// <param name="recorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/> or <see langword="null"/>.</param>
    /// <returns>A recorder capable of handling an argument of any type, and which wraps the provided recorder.</returns>
    public abstract Func<object?, bool> ForNullable<T>(Func<T?, bool> recorder) where T : struct;

    /// <summary>Ensures that all elements of the argument are of a certain type, <typeparamref name="T"/>, or <see langword="null"/>, before attempting to record them.</summary>
    /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
    /// <param name="recorder">Responsible for recording the argument, if all elements are of type <typeparamref name="T"/> or <see langword="null"/>.</param>
    /// <returns>A recorder capable of handling an argument of any type, and which wraps the provided recorder.</returns>
    public abstract Func<IReadOnlyList<object?>?, bool> ForNullable<T>(Func<IReadOnlyList<T?>?, bool> recorder) where T : class;

    /// <summary>Ensures that all elements of the argument are of a certain type, <typeparamref name="T"/>, or <see langword="null"/>, before attempting to record them.</summary>
    /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
    /// <param name="recorder">Responsible for recording the argument, if all elements are of type <typeparamref name="T"/> or <see langword="null"/>.</param>
    /// <returns>A recorder capable of handling an argument of any type, and which wraps the provided recorder.</returns>
    public abstract Func<IReadOnlyList<object?>?, bool> ForNullable<T>(Func<IReadOnlyList<T?>?, bool> recorder) where T : struct;

    /// <summary>Ensures that the argument is of a certain type, <typeparamref name="T"/>, or <see langword="null"/>, before attempting to record it - and returns <see langword="true"/> if this is the case.</summary>
    /// <typeparam name="T">The expected type of the argument.</typeparam>
    /// <param name="recorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/> or <see langword="null"/>.</param>
    /// <returns>A recorder capable of handling an argument of any type, and which wraps the provided recorder.</returns>
    public abstract Func<object?, bool> ForNullable<T>(Action<T?> recorder) where T : class;

    /// <summary>Ensures that the argument is of a certain type, <typeparamref name="T"/>, or <see langword="null"/>, before attempting to record it - and returns <see langword="true"/> if this is the case.</summary>
    /// <typeparam name="T">The expected type of the argument.</typeparam>
    /// <param name="recorder">Responsible for recording the argument, if it is of type <typeparamref name="T"/> or <see langword="null"/>.</param>
    /// <returns>A recorder capable of handling an argument of any type, and which wraps the provided recorder.</returns>
    public abstract Func<object?, bool> ForNullable<T>(Action<T?> recorder) where T : struct;

    /// <summary>Ensures that all elements of the argument are of a certain type, <typeparamref name="T"/>, or <see langword="null"/>, before attempting to record them - and returns <see langword="true"/> if this is the case.</summary>
    /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
    /// <param name="recorder">Responsible for recording the argument, if all elements are of type <typeparamref name="T"/> or <see langword="null"/>.</param>
    /// <returns>A recorder capable of handling an argument of any type, and which wraps the provided recorder.</returns>
    public abstract Func<IReadOnlyList<object?>?, bool> ForNullable<T>(Action<IReadOnlyList<T?>?> recorder) where T : class;

    /// <summary>Ensures that all elements of the argument are of a certain type, <typeparamref name="T"/>, or <see langword="null"/>, before attempting to record them - and returns <see langword="true"/> if this is the case.</summary>
    /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
    /// <param name="recorder">Responsible for recording the argument, if all elements are of type <typeparamref name="T"/> or <see langword="null"/>.</param>
    /// <returns>A recorder capable of handling an argument of any type, and which wraps the provided recorder.</returns>
    public abstract Func<IReadOnlyList<object?>?, bool> ForNullable<T>(Action<IReadOnlyList<T?>?> recorder) where T : struct;

    /// <summary>Ensures that all elements of the argument are of a certain type, <typeparamref name="T"/>, or <see langword="null"/>, before attempting to record them.</summary>
    /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
    /// <param name="recorder">Responsible for recording the argument, if all elements are of type <typeparamref name="T"/> or <see langword="null"/>.</param>
    /// <returns>A recorder capable of handling an argument of any type, and which wraps the provided recorder.</returns>
    public abstract Func<IReadOnlyList<object?>?, bool> ForNullableElements<T>(Func<IReadOnlyList<T?>, bool> recorder) where T : class;

    /// <summary>Ensures that all elements of the argument are of a certain type, <typeparamref name="T"/>, or <see langword="null"/>, before attempting to record them.</summary>
    /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
    /// <param name="recorder">Responsible for recording the argument, if all elements are of type <typeparamref name="T"/> or <see langword="null"/>.</param>
    /// <returns>A recorder capable of handling an argument of any type, and which wraps the provided recorder.</returns>
    public abstract Func<IReadOnlyList<object?>?, bool> ForNullableElements<T>(Func<IReadOnlyList<T?>, bool> recorder) where T : struct;

    /// <summary>Ensures that all elements of the argument are of a certain type, <typeparamref name="T"/>, or <see langword="null"/>, before attempting to record them and returns <see langword="true"/> if this is the case.</summary>
    /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
    /// <param name="recorder">Responsible for recording the argument, if all elements are of type <typeparamref name="T"/> or <see langword="null"/>.</param>
    /// <returns>A recorder capable of handling an argument of any type, and which wraps the provided recorder.</returns>
    public abstract Func<IReadOnlyList<object?>?, bool> ForNullableElements<T>(Action<IReadOnlyList<T?>> recorder) where T : class;

    /// <summary>Ensures that all elements of the argument are of a certain type, <typeparamref name="T"/>, or <see langword="null"/>, before attempting to record them and returns <see langword="true"/> if this is the case.</summary>
    /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
    /// <param name="recorder">Responsible for recording the argument, if all elements are of type <typeparamref name="T"/> or <see langword="null"/>.</param>
    /// <returns>A recorder capable of handling an argument of any type, and which wraps the provided recorder.</returns>
    public abstract Func<IReadOnlyList<object?>?, bool> ForNullableElements<T>(Action<IReadOnlyList<T?>> recorder) where T : struct;

    /// <summary>Ensures that all elements of the argument are of a certain type, <typeparamref name="T"/>, before attempting to record them.</summary>
    /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
    /// <param name="recorder">Responsible for recording the argument, if all elements are of type <typeparamref name="T"/>.</param>
    /// <returns>A recorder capable of handling an argument of any type, and which wraps the provided recorder.</returns>
    public abstract Func<IReadOnlyList<object?>?, bool> ForNullableCollection<T>(Func<IReadOnlyList<T>?, bool> recorder) where T : notnull;

    /// <summary>Ensures that all elements of the argument are of a certain type, <typeparamref name="T"/>, before attempting to record them - and returns <see langword="true"/> if this is the case.</summary>
    /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
    /// <param name="recorder">Responsible for recording the argument, if all elements are of type <typeparamref name="T"/>.</param>
    /// <returns>A recorder capable of handling an argument of any type, and which wraps the provided recorder.</returns>
    public abstract Func<IReadOnlyList<object?>?, bool> ForNullableCollection<T>(Action<IReadOnlyList<T>?> recorder) where T : notnull;
}
