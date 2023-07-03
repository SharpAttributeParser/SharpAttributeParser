namespace SharpAttributeParser;

using System;
using System.Collections.Generic;

/// <summary>Provides adapters that may be applied to collections of semantically parsed arguments before they are recorded.</summary>
/// <typeparam name="TData">The type representing the recorded arguments.</typeparam>
public interface ISemanticCollectionArgumentAdapter<TData>
{
    /// <summary>Produces a recorder which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before attempting to record it.</summary>
    /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
    /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
    /// <returns>A mapping from some parameter to the produced recorder.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract DSemanticAttributeParameterMapping<TData> For<T>(Func<TData, IReadOnlyList<T>, bool> recorder) where T : notnull;

    /// <summary>Produces a recorder which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before attempting to record it - and which returns <see langword="true"/> if this is the case.</summary>
    /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
    /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
    /// <returns>A mapping from some parameter to the produced recorder.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract DSemanticAttributeParameterMapping<TData> For<T>(Action<TData, IReadOnlyList<T>> recorder) where T : notnull;

    /// <summary>Produces a recorder which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before attempting to record it.</summary>
    /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
    /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
    /// <returns>A mapping from some parameter to the produced recorder.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract DSemanticAttributeParameterMapping<TData> ForNullableCollection<T>(Func<TData, IReadOnlyList<T>?, bool> recorder) where T : notnull;

    /// <summary>Produces a recorder which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before attempting to record it - and which returns <see langword="true"/> if this is the case.</summary>
    /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
    /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
    /// <returns>A mapping from some parameter to the produced recorder.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract DSemanticAttributeParameterMapping<TData> ForNullableCollection<T>(Action<TData, IReadOnlyList<T>?> recorder) where T : notnull;

    /// <summary>Produces a recorder which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before attempting to record it.</summary>
    /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
    /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
    /// <returns>A mapping from some parameter to the produced recorder.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract DSemanticAttributeParameterMapping<TData> ForNullable<T>(Func<TData, IReadOnlyList<T?>?, bool> recorder) where T : class;

    /// <summary>Produces a recorder which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before attempting to record it - and which returns <see langword="true"/> if this is the case.</summary>
    /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
    /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
    /// <returns>A mapping from some parameter to the produced recorder.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract DSemanticAttributeParameterMapping<TData> ForNullable<T>(Action<TData, IReadOnlyList<T?>?> recorder) where T : class;

    /// <summary>Produces a recorder which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before attempting to record it.</summary>
    /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
    /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
    /// <returns>A mapping from some parameter to the produced recorder.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract DSemanticAttributeParameterMapping<TData> ForNullable<T>(Func<TData, IReadOnlyList<T?>?, bool> recorder) where T : struct;

    /// <summary>Produces a recorder which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before attempting to record it - and which returns <see langword="true"/> if this is the case.</summary>
    /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
    /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
    /// <returns>A mapping from some parameter to the produced recorder.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract DSemanticAttributeParameterMapping<TData> ForNullable<T>(Action<TData, IReadOnlyList<T?>?> recorder) where T : struct;

    /// <summary>Produces a recorder which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before attempting to record it.</summary>
    /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
    /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
    /// <returns>A mapping from some parameter to the produced recorder.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract DSemanticAttributeParameterMapping<TData> ForNullableElements<T>(Func<TData, IReadOnlyList<T?>, bool> recorder) where T : class;

    /// <summary>Produces a recorder which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before attempting to record it - and which returns <see langword="true"/> if this is the case.</summary>
    /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
    /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
    /// <returns>A mapping from some parameter to the produced recorder.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract DSemanticAttributeParameterMapping<TData> ForNullableElements<T>(Action<TData, IReadOnlyList<T?>> recorder) where T : class;

    /// <summary>Produces a recorder which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before attempting to record it.</summary>
    /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
    /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
    /// <returns>A mapping from some parameter to the produced recorder.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract DSemanticAttributeParameterMapping<TData> ForNullableElements<T>(Func<TData, IReadOnlyList<T?>, bool> recorder) where T : struct;

    /// <summary>Produces a recorder which ensures that the argument is a collection with elements of type <typeparamref name="T"/> before attempting to record it - and which returns <see langword="true"/> if this is the case.</summary>
    /// <typeparam name="T">The expected type of the elements of the argument.</typeparam>
    /// <param name="recorder">Responsible for recording the argument, if it is a collection with elements of type <typeparamref name="T"/>.</param>
    /// <returns>A mapping from some parameter to the produced recorder.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract DSemanticAttributeParameterMapping<TData> ForNullableElements<T>(Action<TData, IReadOnlyList<T?>> recorder) where T : struct;
}
