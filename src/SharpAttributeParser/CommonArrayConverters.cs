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

        if (argument is not IReadOnlyList<T> tListArgument)
        {
            if (argument is not IReadOnlyList<object> objectListArgument)
            {
                return new Error();
            }

            var converted = new T[objectListArgument.Count];

            for (var i = 0; i < objectListArgument.Count; i++)
            {
                if (objectListArgument[i] is not T tElement)
                {
                    return new Error();
                }

                converted[i] = tElement;
            }

            return OneOf<Error, IReadOnlyList<T>>.FromT1(converted);
        }

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

    /// <summary>Attempts to convert the provided <see cref="object"/> to a array of <typeparamref name="T"/>, where both the collection itself and the elements are nullable.</summary>
    /// <typeparam name="T">The type of the elements of the converted array.</typeparam>
    /// <param name="argument">The <see cref="object"/> that is converted to an array of <typeparamref name="T"/>.</param>
    /// <returns>The converted array, or <see cref="Error"/> if the attempt was unsuccessful.</returns>
    public static OneOf<Error, IReadOnlyList<T?>?> Nullable<T>(object? argument)
    {
        if (argument is null)
        {
            return OneOf<Error, IReadOnlyList<T?>?>.FromT1(null);
        }

        if (argument is not IReadOnlyList<T> tListArgument)
        {
            if (argument is not IReadOnlyList<object> objectListArgument)
            {
                return new Error();
            }

            var converted = new T?[objectListArgument.Count];

            for (var i = 0; i < objectListArgument.Count; i++)
            {
                if (objectListArgument[i] is null)
                {
                    converted[i] = default;

                    continue;
                }

                if (objectListArgument[i] is not T tElement)
                {
                    return new Error();
                }

                converted[i] = tElement;
            }

            return OneOf<Error, IReadOnlyList<T?>?>.FromT1(converted);
        }

        if (argument.GetType().GetElementType() != typeof(T))
        {
            return new Error();
        }

        return OneOf<Error, IReadOnlyList<T?>?>.FromT1(tListArgument);
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
    public static OneOf<Error, IReadOnlyList<T?>> NullableElements<T>(object? argument)
    {
        if (argument is null)
        {
            return new Error();
        }

        return Nullable<T>(argument).MapT1(static (array) => array!);
    }
}
