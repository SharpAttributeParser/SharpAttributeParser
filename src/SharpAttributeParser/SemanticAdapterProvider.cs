namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;

/// <inheritdoc cref="ISemanticAdapterProvider"/>
internal sealed class SemanticAdapterProvider : ISemanticAdapterProvider
{
    DSemanticGenericRecorder ISemanticAdapterProvider.For(Action<ITypeSymbol> recorder)
    {
        return wrapper;

        bool wrapper(ITypeSymbol value)
        {
            recorder(value);

            return true;
        }
    }

    DSemanticSingleRecorder ISemanticAdapterProvider.For<T>(Func<T, bool> recorder) => For(recorder);
    DSemanticArrayRecorder ISemanticAdapterProvider.For<T>(Func<IReadOnlyList<T>, bool> recorder) => For(recorder);

    DSemanticSingleRecorder ISemanticAdapterProvider.For<T>(Action<T> recorder)
    {
        return For<T>(wrapper);

        bool wrapper(T value)
        {
            recorder(value);

            return true;
        }
    }

    DSemanticArrayRecorder ISemanticAdapterProvider.For<T>(Action<IReadOnlyList<T>> recorder)
    {
        return For<T>(wrapper);

        bool wrapper(IReadOnlyList<T> value)
        {
            recorder(value);

            return true;
        }
    }

    DSemanticSingleRecorder ISemanticAdapterProvider.ForNullable<T>(Func<T?, bool> recorder) where T : class => ForNullable(recorder);
    DSemanticSingleRecorder ISemanticAdapterProvider.ForNullable<T>(Func<T?, bool> recorder) where T : struct => ForNullable(recorder);

    DSemanticArrayRecorder ISemanticAdapterProvider.ForNullable<T>(Func<IReadOnlyList<T?>?, bool> recorder) where T : class => ForNullable(recorder);
    DSemanticArrayRecorder ISemanticAdapterProvider.ForNullable<T>(Func<IReadOnlyList<T?>?, bool> recorder) where T : struct => ForNullable(recorder);

    DSemanticSingleRecorder ISemanticAdapterProvider.ForNullable<T>(Action<T?> recorder) where T : class
    {
        return ForNullable<T>(wrapper);

        bool wrapper(T? value)
        {
            recorder(value);

            return true;
        }
    }

    DSemanticSingleRecorder ISemanticAdapterProvider.ForNullable<T>(Action<T?> recorder) where T : struct
    {
        return ForNullable<T>(wrapper);

        bool wrapper(T? value)
        {
            recorder(value);

            return true;
        }
    }

    DSemanticArrayRecorder ISemanticAdapterProvider.ForNullable<T>(Action<IReadOnlyList<T?>?> recorder) where T : class
    {
        return ForNullable<T>(wrapper);

        bool wrapper(IReadOnlyList<T?>? value)
        {
            recorder(value);

            return true;
        }
    }

    DSemanticArrayRecorder ISemanticAdapterProvider.ForNullable<T>(Action<IReadOnlyList<T?>?> recorder) where T : struct
    {
        return ForNullable<T>(wrapper);

        bool wrapper(IReadOnlyList<T?>? value)
        {
            recorder(value);

            return true;
        }
    }

    DSemanticArrayRecorder ISemanticAdapterProvider.ForNullableElements<T>(Func<IReadOnlyList<T?>, bool> recorder) where T : class => ForNullableElements(recorder);
    DSemanticArrayRecorder ISemanticAdapterProvider.ForNullableElements<T>(Func<IReadOnlyList<T?>, bool> recorder) where T : struct => ForNullableElements(recorder);

    DSemanticArrayRecorder ISemanticAdapterProvider.ForNullableElements<T>(Action<IReadOnlyList<T?>> recorder) where T : class
    {
        return ForNullableElements<T>(wrapper);

        bool wrapper(IReadOnlyList<T?> value)
        {
            recorder(value);

            return true;
        }
    }

    DSemanticArrayRecorder ISemanticAdapterProvider.ForNullableElements<T>(Action<IReadOnlyList<T?>> recorder) where T : struct
    {
        return ForNullableElements<T>(wrapper);

        bool wrapper(IReadOnlyList<T?> value)
        {
            recorder(value);

            return true;
        }
    }

    DSemanticArrayRecorder ISemanticAdapterProvider.ForNullableCollection<T>(Func<IReadOnlyList<T>?, bool> recorder) => ForNullableCollection(recorder);

    DSemanticArrayRecorder ISemanticAdapterProvider.ForNullableCollection<T>(Action<IReadOnlyList<T>?> recorder)
    {
        return ForNullableCollection<T>(wrapper);

        bool wrapper(IReadOnlyList<T>? value)
        {
            recorder(value);

            return true;
        }
    }

    private static DSemanticSingleRecorder For<T>(Func<T, bool> recorder)
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

    private static DSemanticArrayRecorder For<T>(Func<IReadOnlyList<T>, bool> recorder)
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

    private static DSemanticSingleRecorder ForNullable<T>(Func<T?, bool> recorder) where T : class
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

    private static DSemanticSingleRecorder ForNullable<T>(Func<T?, bool> recorder) where T : struct
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

    private static DSemanticArrayRecorder ForNullable<T>(Func<IReadOnlyList<T?>?, bool> recorder) where T : class
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

    private static DSemanticArrayRecorder ForNullable<T>(Func<IReadOnlyList<T?>?, bool> recorder) where T : struct
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

    private static DSemanticArrayRecorder ForNullableElements<T>(Func<IReadOnlyList<T?>, bool> recorder) where T : class
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

    private static DSemanticArrayRecorder ForNullableElements<T>(Func<IReadOnlyList<T?>, bool> recorder) where T : struct
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

    private static DSemanticArrayRecorder ForNullableCollection<T>(Func<IReadOnlyList<T>?, bool> recorder)
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
