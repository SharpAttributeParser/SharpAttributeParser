namespace SharpAttributeParser;

using OneOf;
using OneOf.Types;

using System.Collections.Generic;
using System.Linq;

/// <summary>Provides common functionality related to converted <see cref="object"/> to arrays.</summary>
internal static class CommonArrayConverters
{
    /// <summary>Attempts to convert the provided <see cref="object"/> to a non-nullable array of <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">The type of the elements of the converted array.</typeparam>
    /// <param name="argument">The <see cref="object"/> that is converted to an array of <typeparamref name="T"/>.</param>
    /// <returns>The converted array, or <see cref="Error"/> if the attempt was unsuccessful.</returns>
    public static OneOf<Error, IReadOnlyList<T>> NonNullable<T>(object? argument)
    {
        if (argument is null)
        {
            return new Error();
        }

        if (argument is IReadOnlyList<T> tListArgument)
        {
            if (argument.GetType().GetElementType() != typeof(T))
            {
                return new Error();
            }

            if (tListArgument.Any(static (argumentElement) => argumentElement is null))
            {
                return new Error();
            }

            return OneOf<Error, IReadOnlyList<T>>.FromT1(tListArgument);
        }

        if (argument is IReadOnlyList<object> objectListArgument)
        {
            return NonNullable<T>(objectListArgument);
        }

        return new Error();
    }

    /// <summary>Attempts to convert the provided <see cref="object"/> to a array of <typeparamref name="T"/>, where both the collection itself and the elements are nullable.</summary>
    /// <typeparam name="T">The type of the elements of the converted array.</typeparam>
    /// <param name="argument">The <see cref="object"/> that is converted to an array of <typeparamref name="T"/>.</param>
    /// <returns>The converted array, or <see cref="Error"/> if the attempt was unsuccessful.</returns>
    public static OneOf<Error, IReadOnlyList<T?>?> NullableClass<T>(object? argument) where T : class
    {
        if (argument is null)
        {
            return OneOf<Error, IReadOnlyList<T?>?>.FromT1(null);
        }

        if (argument is IReadOnlyList<T> tListArgument)
        {
            return OneOf<Error, IReadOnlyList<T?>?>.FromT1(tListArgument);
        }

        if (argument is IReadOnlyList<object> objectListArgument)
        {
            return Nullable<T>(objectListArgument);
        }

        return new Error();
    }

    /// <summary>Attempts to convert the provided <see cref="object"/> to a array of <typeparamref name="T"/>, where both the collection itself and the elements are nullable.</summary>
    /// <typeparam name="T">The type of the elements of the converted array.</typeparam>
    /// <param name="argument">The <see cref="object"/> that is converted to an array of <typeparamref name="T"/>.</param>
    /// <returns>The converted array, or <see cref="Error"/> if the attempt was unsuccessful.</returns>
    public static OneOf<Error, IReadOnlyList<T?>?> NullableStruct<T>(object? argument) where T : struct
    {
        if (argument is null)
        {
            return OneOf<Error, IReadOnlyList<T?>?>.FromT1(null);
        }

        if (argument is IReadOnlyList<T?> tListArgument)
        {
            return OneOf<Error, IReadOnlyList<T?>?>.FromT1(tListArgument);
        }

        if (argument is IReadOnlyList<T> nonNullTListArgument)
        {
            if (argument.GetType().GetElementType() != typeof(T))
            {
                return new Error();
            }

            return OneOf<Error, IReadOnlyList<T?>?>.FromT1(Nullable(nonNullTListArgument));
        }

        if (argument is IReadOnlyList<object> objectListArgument)
        {
            return Nullable<T?>(objectListArgument);
        }

        return new Error();
    }

    /// <summary>Attempts to convert the provided <see cref="object"/> to a array of <typeparamref name="T"/>, where the collection itself is nullable.</summary>
    /// <typeparam name="T">The type of the elements of the converted array.</typeparam>
    /// <param name="argument">The <see cref="object"/> that is converted to an array of <typeparamref name="T"/>.</param>
    /// <returns>The converted array, or <see cref="Error"/> if the attempt was unsuccessful.</returns>
    public static OneOf<Error, IReadOnlyList<T>?> NullableCollection<T>(object? argument)
    {
        if (argument is null)
        {
            return OneOf<Error, IReadOnlyList<T>?>.FromT1(null);
        }

        return NonNullable<T>(argument).MapT1<IReadOnlyList<T>?>(static (array) => array);
    }

    /// <summary>Attempts to convert the provided <see cref="object"/> to a array of <typeparamref name="T"/>, where the elements are nullable.</summary>
    /// <typeparam name="T">The type of the elements of the converted array.</typeparam>
    /// <param name="argument">The <see cref="object"/> that is converted to an array of <typeparamref name="T"/>.</param>
    /// <returns>The converted array, or <see cref="Error"/> if the attempt was unsuccessful.</returns>
    public static OneOf<Error, IReadOnlyList<T?>> NullableClassElements<T>(object? argument) where T : class
    {
        if (argument is null)
        {
            return new Error();
        }

        return NullableClass<T>(argument).MapT1(static (array) => array!);
    }

    /// <summary>Attempts to convert the provided <see cref="object"/> to a array of <typeparamref name="T"/>, where the elements are nullable.</summary>
    /// <typeparam name="T">The type of the elements of the converted array.</typeparam>
    /// <param name="argument">The <see cref="object"/> that is converted to an array of <typeparamref name="T"/>.</param>
    /// <returns>The converted array, or <see cref="Error"/> if the attempt was unsuccessful.</returns>
    public static OneOf<Error, IReadOnlyList<T?>> NullableStructElements<T>(object? argument) where T : struct
    {
        if (argument is null)
        {
            return new Error();
        }

        return NullableStruct<T>(argument).MapT1(static (array) => array!);
    }

    private static OneOf<Error, IReadOnlyList<T>> NonNullable<T>(IReadOnlyList<object> argument)
    {
        var converted = new T[argument.Count];

        for (var i = 0; i < argument.Count; i++)
        {
            if (argument[i] is not T tElement)
            {
                return new Error();
            }

            converted[i] = tElement;
        }

        return OneOf<Error, IReadOnlyList<T>>.FromT1(converted);
    }

    private static OneOf<Error, IReadOnlyList<T?>?> Nullable<T>(IReadOnlyList<object> argument)
    {
        var converted = new T?[argument.Count];

        for (var i = 0; i < argument.Count; i++)
        {
            if (argument[i] is null)
            {
                converted[i] = default;

                continue;
            }

            if (argument[i] is not T tElement)
            {
                return new Error();
            }

            converted[i] = tElement;
        }

        return OneOf<Error, IReadOnlyList<T?>?>.FromT1(converted);
    }

    private static IReadOnlyList<T?> Nullable<T>(IReadOnlyList<T> argument) where T : struct
    {
        var nullableArgument = new T?[argument.Count];

        for (var i = 0; i < argument.Count; i++)
        {
            nullableArgument[i] = argument[i];
        }

        return nullableArgument;
    }
}
