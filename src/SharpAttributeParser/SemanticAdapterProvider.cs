namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;

/// <inheritdoc cref="ISemanticAdapterProvider"/>
internal sealed class SemanticAdapterProvider : ISemanticAdapterProvider
{
    DSemanticGenericRecorder ISemanticAdapterProvider.For(Action<ITypeSymbol> recorder)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return wrapper;

        bool wrapper(ITypeSymbol value)
        {
            recorder(value);

            return true;
        }
    }

    DSemanticSingleRecorder ISemanticAdapterProvider.For<T>(Func<T, bool> recorder) => For(recorder ?? throw new ArgumentNullException(nameof(recorder)));
    DSemanticArrayRecorder ISemanticAdapterProvider.For<T>(Func<IReadOnlyList<T>, bool> recorder) => For(recorder ?? throw new ArgumentNullException(nameof(recorder)));

    DSemanticSingleRecorder ISemanticAdapterProvider.For<T>(Action<T> recorder)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return For<T>(wrapper);

        bool wrapper(T value)
        {
            recorder(value);

            return true;
        }
    }

    DSemanticArrayRecorder ISemanticAdapterProvider.For<T>(Action<IReadOnlyList<T>> recorder)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return For<T>(wrapper);

        bool wrapper(IReadOnlyList<T> value)
        {
            recorder(value);

            return true;
        }
    }

    DSemanticSingleRecorder ISemanticAdapterProvider.ForNullable<T>(Func<T?, bool> recorder) where T : class => ForNullable(recorder ?? throw new ArgumentNullException(nameof(recorder)));
    DSemanticSingleRecorder ISemanticAdapterProvider.ForNullable<T>(Func<T?, bool> recorder) where T : struct => ForNullable(recorder ?? throw new ArgumentNullException(nameof(recorder)));

    DSemanticArrayRecorder ISemanticAdapterProvider.ForNullable<T>(Func<IReadOnlyList<T?>?, bool> recorder) where T : class => ForNullable(recorder ?? throw new ArgumentNullException(nameof(recorder)));
    DSemanticArrayRecorder ISemanticAdapterProvider.ForNullable<T>(Func<IReadOnlyList<T?>?, bool> recorder) where T : struct => ForNullable(recorder ?? throw new ArgumentNullException(nameof(recorder)));

    DSemanticSingleRecorder ISemanticAdapterProvider.ForNullable<T>(Action<T?> recorder) where T : class
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return ForNullable<T>(wrapper);

        bool wrapper(T? value)
        {
            recorder(value);

            return true;
        }
    }

    DSemanticSingleRecorder ISemanticAdapterProvider.ForNullable<T>(Action<T?> recorder) where T : struct
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return ForNullable<T>(wrapper);

        bool wrapper(T? value)
        {
            recorder(value);

            return true;
        }
    }

    DSemanticArrayRecorder ISemanticAdapterProvider.ForNullable<T>(Action<IReadOnlyList<T?>?> recorder) where T : class
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return ForNullable<T>(wrapper);

        bool wrapper(IReadOnlyList<T?>? value)
        {
            recorder(value);

            return true;
        }
    }

    DSemanticArrayRecorder ISemanticAdapterProvider.ForNullable<T>(Action<IReadOnlyList<T?>?> recorder) where T : struct
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return ForNullable<T>(wrapper);

        bool wrapper(IReadOnlyList<T?>? value)
        {
            recorder(value);

            return true;
        }
    }

    DSemanticArrayRecorder ISemanticAdapterProvider.ForNullableElements<T>(Func<IReadOnlyList<T?>, bool> recorder) where T : class => ForNullableElements(recorder ?? throw new ArgumentNullException(nameof(recorder)));
    DSemanticArrayRecorder ISemanticAdapterProvider.ForNullableElements<T>(Func<IReadOnlyList<T?>, bool> recorder) where T : struct => ForNullableElements(recorder ?? throw new ArgumentNullException(nameof(recorder)));

    DSemanticArrayRecorder ISemanticAdapterProvider.ForNullableElements<T>(Action<IReadOnlyList<T?>> recorder) where T : class
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return ForNullableElements<T>(wrapper);

        bool wrapper(IReadOnlyList<T?> value)
        {
            recorder(value);

            return true;
        }
    }

    DSemanticArrayRecorder ISemanticAdapterProvider.ForNullableElements<T>(Action<IReadOnlyList<T?>> recorder) where T : struct
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return ForNullableElements<T>(wrapper);

        bool wrapper(IReadOnlyList<T?> value)
        {
            recorder(value);

            return true;
        }
    }

    DSemanticArrayRecorder ISemanticAdapterProvider.ForNullableCollection<T>(Func<IReadOnlyList<T>?, bool> recorder) => ForNullableCollection(recorder ?? throw new ArgumentNullException(nameof(recorder)));

    DSemanticArrayRecorder ISemanticAdapterProvider.ForNullableCollection<T>(Action<IReadOnlyList<T>?> recorder)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

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

            if (value is not T tValue)
            {
                return false;
            }

            return recorder(tValue);
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

            if (value is IReadOnlyList<T> tList)
            {
                return recorder(tList);
            }

            var converted = new T[value.Count];

            for (var i = 0; i < converted.Length; i++)
            {
                if (value[i] is null)
                {
                    return false;
                }

                if (value[i] is not T tValue)
                {
                    return false;
                }

                converted[i] = tValue;
            }

            return recorder(converted);
        }
    }

    private static DSemanticSingleRecorder ForNullable<T>(Func<T?, bool> recorder) where T : class
    {
        return wrapper;

        bool wrapper(object? value)
        {
            if (value is null)
            {
                return recorder(null);
            }

            if (value is not T tValue)
            {
                return false;
            }

            return recorder(tValue);
        }
    }

    private static DSemanticSingleRecorder ForNullable<T>(Func<T?, bool> recorder) where T : struct
    {
        return wrapper;

        bool wrapper(object? value)
        {
            if (value is null)
            {
                return recorder(null);
            }

            if (value is not T tValue)
            {
                return false;
            }

            return recorder(tValue);
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

            if (value is IReadOnlyList<T?> tList)
            {
                return recorder(tList);
            }

            var converted = new T?[value.Count];

            for (var i = 0; i < converted.Length; i++)
            {
                if (value[i] is null)
                {
                    converted[i] = null;

                    continue;
                }

                if (value[i] is not T tValue)
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

            if (value is IReadOnlyList<T?> tList)
            {
                return recorder(tList);
            }

            var converted = new T?[value.Count];

            for (var i = 0; i < converted.Length; i++)
            {
                if (value[i] is null)
                {
                    converted[i] = null;

                    continue;
                }

                if (value[i] is not T tValue)
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
}
