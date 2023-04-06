namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;

/// <inheritdoc cref="ISemanticAdapterProvider"/>
internal sealed class SemanticAdapterProvider : ISemanticAdapterProvider
{
    Func<ITypeSymbol, bool> ISemanticAdapterProvider.For(Action<ITypeSymbol> recorder)
    {
        return wrapper;

        bool wrapper(ITypeSymbol value)
        {
            recorder(value);

            return true;
        }
    }

    Func<object?, bool> ISemanticAdapterProvider.For<T>(Func<T, bool> recorder) => For(recorder);
    Func<IReadOnlyList<object?>?, bool> ISemanticAdapterProvider.For<T>(Func<IReadOnlyList<T>, bool> recorder) => For(recorder);

    Func<object?, bool> ISemanticAdapterProvider.For<T>(Action<T> recorder)
    {
        return For<T>(wrapper);

        bool wrapper(T value)
        {
            recorder(value);

            return true;
        }
    }

    Func<IReadOnlyList<object?>?, bool> ISemanticAdapterProvider.For<T>(Action<IReadOnlyList<T>> recorder)
    {
        return For<T>(wrapper);

        bool wrapper(IReadOnlyList<T> value)
        {
            recorder(value);

            return true;
        }
    }

    Func<object?, bool> ISemanticAdapterProvider.ForNullable<T>(Func<T?, bool> recorder) where T : class => ForNullable(recorder);
    Func<object?, bool> ISemanticAdapterProvider.ForNullable<T>(Func<T?, bool> recorder) where T : struct => ForNullable(recorder);

    Func<IReadOnlyList<object?>?, bool> ISemanticAdapterProvider.ForNullable<T>(Func<IReadOnlyList<T?>?, bool> recorder) where T : class => ForNullable(recorder);
    Func<IReadOnlyList<object?>?, bool> ISemanticAdapterProvider.ForNullable<T>(Func<IReadOnlyList<T?>?, bool> recorder) where T : struct => ForNullable(recorder);

    Func<object?, bool> ISemanticAdapterProvider.ForNullable<T>(Action<T?> recorder) where T : class
    {
        return ForNullable<T>(wrapper);

        bool wrapper(T? value)
        {
            recorder(value);

            return true;
        }
    }

    Func<object?, bool> ISemanticAdapterProvider.ForNullable<T>(Action<T?> recorder) where T : struct
    {
        return ForNullable<T>(wrapper);

        bool wrapper(T? value)
        {
            recorder(value);

            return true;
        }
    }

    Func<IReadOnlyList<object?>?, bool> ISemanticAdapterProvider.ForNullable<T>(Action<IReadOnlyList<T?>?> recorder) where T : class
    {
        return ForNullable<T>(wrapper);

        bool wrapper(IReadOnlyList<T?>? value)
        {
            recorder(value);

            return true;
        }
    }

    Func<IReadOnlyList<object?>?, bool> ISemanticAdapterProvider.ForNullable<T>(Action<IReadOnlyList<T?>?> recorder) where T : struct
    {
        return ForNullable<T>(wrapper);

        bool wrapper(IReadOnlyList<T?>? value)
        {
            recorder(value);

            return true;
        }
    }

    Func<IReadOnlyList<object?>?, bool> ISemanticAdapterProvider.ForNullableElements<T>(Func<IReadOnlyList<T?>, bool> recorder) where T : class => ForNullableElements(recorder);
    Func<IReadOnlyList<object?>?, bool> ISemanticAdapterProvider.ForNullableElements<T>(Func<IReadOnlyList<T?>, bool> recorder) where T : struct => ForNullableElements(recorder);

    Func<IReadOnlyList<object?>?, bool> ISemanticAdapterProvider.ForNullableElements<T>(Action<IReadOnlyList<T?>> recorder) where T : class
    {
        return ForNullableElements<T>(wrapper);

        bool wrapper(IReadOnlyList<T?> value)
        {
            recorder(value);

            return true;
        }
    }

    Func<IReadOnlyList<object?>?, bool> ISemanticAdapterProvider.ForNullableElements<T>(Action<IReadOnlyList<T?>> recorder) where T : struct
    {
        return ForNullableElements<T>(wrapper);

        bool wrapper(IReadOnlyList<T?> value)
        {
            recorder(value);

            return true;
        }
    }

    Func<IReadOnlyList<object?>?, bool> ISemanticAdapterProvider.ForNullableCollection<T>(Func<IReadOnlyList<T>?, bool> recorder) => ForNullableCollection(recorder);

    Func<IReadOnlyList<object?>?, bool> ISemanticAdapterProvider.ForNullableCollection<T>(Action<IReadOnlyList<T>?> recorder)
    {
        return ForNullableCollection<T>(wrapper);

        bool wrapper(IReadOnlyList<T>? value)
        {
            recorder(value);

            return true;
        }
    }

    private static Func<object?, bool> For<T>(Func<T, bool> recorder)
    {
        return wrapper;

        bool wrapper(object? value)
        {
            if (value is null)
            {
                return false;
            }

            var (success, tValue) = TryConvert<T>(value);

            if (success)
            {
                return recorder(tValue!);
            }

            return false;
        }
    }

    private static Func<IReadOnlyList<object?>?, bool> For<T>(Func<IReadOnlyList<T>, bool> recorder)
    {
        return wrapper;

        bool wrapper(IReadOnlyList<object?>? value)
        {
            if (value is null)
            {
                return false;
            }

            var converted = new T[value.Count];

            for (var i = 0; i < converted.Length; i++)
            {
                if (value[i] is null)
                {
                    return false;
                }

                var (success, tValue) = TryConvert<T>(value[i]);

                if (success is false)
                {
                    return false;
                }

                converted[i] = tValue!;
            }

            return recorder(converted);
        }
    }

    private static Func<object?, bool> ForNullable<T>(Func<T?, bool> recorder) where T : class
    {
        return wrapper;

        bool wrapper(object? value)
        {
            var (success, tValue) = TryConvert<T>(value);

            if (success)
            {
                return recorder(tValue);
            }

            return false;
        }
    }

    private static Func<object?, bool> ForNullable<T>(Func<T?, bool> recorder) where T : struct
    {
        return wrapper;

        bool wrapper(object? value)
        {
            var (success, tValue) = TryConvert<T?>(value);

            if (success)
            {
                return recorder(tValue);
            }

            return false;
        }
    }

    private static Func<IReadOnlyList<object?>?, bool> ForNullable<T>(Func<IReadOnlyList<T?>?, bool> recorder) where T : class
    {
        return wrapper;

        bool wrapper(IReadOnlyList<object?>? value)
        {
            if (value is null)
            {
                return recorder(null);
            }

            var converted = new T?[value.Count];

            for (var i = 0; i < converted.Length; i++)
            {
                var (success, tValue) = TryConvert<T>(value[i]);

                if (success is false)
                {
                    return false;
                }

                converted[i] = tValue;
            }

            return recorder(converted);
        }
    }

    private static Func<IReadOnlyList<object?>?, bool> ForNullable<T>(Func<IReadOnlyList<T?>?, bool> recorder) where T : struct
    {
        return wrapper;

        bool wrapper(IReadOnlyList<object?>? value)
        {
            if (value is null)
            {
                return recorder(null);
            }

            var converted = new T?[value.Count];

            for (var i = 0; i < converted.Length; i++)
            {
                var (success, tValue) = TryConvert<T?>(value[i]);

                if (success is false)
                {
                    return false;
                }

                converted[i] = tValue;
            }

            return recorder(converted);
        }
    }

    private static Func<IReadOnlyList<object?>?, bool> ForNullableElements<T>(Func<IReadOnlyList<T?>, bool> recorder) where T : class
    {
        return ForNullable<T>(wrapper);

        bool wrapper(IReadOnlyList<T?>? value)
        {
            if (value is null)
            {
                return false;
            }

            return recorder(value);
        }
    }

    private static Func<IReadOnlyList<object?>?, bool> ForNullableElements<T>(Func<IReadOnlyList<T?>, bool> recorder) where T : struct
    {
        return ForNullable<T>(wrapper);

        bool wrapper(IReadOnlyList<T?>? value)
        {
            if (value is null)
            {
                return false;
            }

            return recorder(value);
        }
    }

    private static Func<IReadOnlyList<object?>?, bool> ForNullableCollection<T>(Func<IReadOnlyList<T>?, bool> recorder)
    {
        return wrapper;

        bool wrapper(IReadOnlyList<object?>? value)
        {
            if (value is null)
            {
                return recorder(null);
            }

            return For(recorder)(value);
        }
    }

    private static (bool, T?) TryConvert<T>(object? value)
    {
        if (value is null)
        {
            return (true, default);
        }

        if (value is T tValue)
        {
            return (true, tValue);
        }

        if (Nullable.GetUnderlyingType(typeof(T)) is Type definiteT)
        {
            return TryConvertToNullable<T>(definiteT, value);
        }

        try
        {
            return (true, (T)value);
        }
        catch (InvalidCastException)
        {
            return (false, default);
        }
    }

    private static (bool, T?) TryConvertToNullable<T>(Type definiteT, object value)
    {
        if (value.GetType().IsEnum)
        {
            try
            {
                var definite = Convert.ChangeType(value, definiteT);

                return (true, (T)definite);
            }
            catch (InvalidCastException)
            {
                return (false, default);
            }
        }

        if (definiteT.IsEnum && value.GetType().IsPrimitive)
        {
            try
            {
                var definite = Enum.Parse(definiteT, value.ToString(), true);

                return (true, (T)definite);
            }
            catch (ArgumentException)
            {
                return (false, default);
            }
        }

        return (false, default);
    }
}
