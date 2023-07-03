namespace SharpAttributeParser;

using System;
using System.Collections.Generic;

/// <inheritdoc cref="ISemanticCollectionArgumentAdapter{TData}"/>
internal sealed class SemanticCollectionArgumentAdapter<TData> : ISemanticCollectionArgumentAdapter<TData>
{
    DSemanticAttributeParameterMapping<TData> ISemanticCollectionArgumentAdapter<TData>.For<T>(Func<TData, IReadOnlyList<T>, bool> recorder)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return For(recorder);
    }

    DSemanticAttributeParameterMapping<TData> ISemanticCollectionArgumentAdapter<TData>.For<T>(Action<TData, IReadOnlyList<T>> recorder)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return For<T>(wrapper);

        bool wrapper(TData data, IReadOnlyList<T> value)
        {
            recorder(data, value);

            return true;
        }
    }

    DSemanticAttributeParameterMapping<TData> ISemanticCollectionArgumentAdapter<TData>.ForNullable<T>(Func<TData, IReadOnlyList<T?>?, bool> recorder) where T : class
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return ForNullable(recorder);
    }

    DSemanticAttributeParameterMapping<TData> ISemanticCollectionArgumentAdapter<TData>.ForNullable<T>(Action<TData, IReadOnlyList<T?>?> recorder) where T : class
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return ForNullable<T>(wrapper);

        bool wrapper(TData data, IReadOnlyList<T?>? value)
        {
            recorder(data, value);

            return true;
        }
    }

    DSemanticAttributeParameterMapping<TData> ISemanticCollectionArgumentAdapter<TData>.ForNullable<T>(Func<TData, IReadOnlyList<T?>?, bool> recorder)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return ForNullable(recorder);
    }

    DSemanticAttributeParameterMapping<TData> ISemanticCollectionArgumentAdapter<TData>.ForNullable<T>(Action<TData, IReadOnlyList<T?>?> recorder)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return ForNullable<T>(wrapper);

        bool wrapper(TData data, IReadOnlyList<T?>? value)
        {
            recorder(data, value);

            return true;
        }
    }

    DSemanticAttributeParameterMapping<TData> ISemanticCollectionArgumentAdapter<TData>.ForNullableCollection<T>(Func<TData, IReadOnlyList<T>?, bool> recorder)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return ForNullableCollection(recorder);
    }

    DSemanticAttributeParameterMapping<TData> ISemanticCollectionArgumentAdapter<TData>.ForNullableCollection<T>(Action<TData, IReadOnlyList<T>?> recorder)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return ForNullableCollection<T>(wrapper);

        bool wrapper(TData data, IReadOnlyList<T>? value)
        {
            recorder(data, value);

            return true;
        }
    }

    DSemanticAttributeParameterMapping<TData> ISemanticCollectionArgumentAdapter<TData>.ForNullableElements<T>(Func<TData, IReadOnlyList<T?>, bool> recorder) where T : class
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return ForNullableElements(recorder);
    }

    DSemanticAttributeParameterMapping<TData> ISemanticCollectionArgumentAdapter<TData>.ForNullableElements<T>(Action<TData, IReadOnlyList<T?>> recorder) where T : class
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return ForNullableElements<T>(wrapper);

        bool wrapper(TData data, IReadOnlyList<T?> value)
        {
            recorder(data, value);

            return true;
        }
    }

    DSemanticAttributeParameterMapping<TData> ISemanticCollectionArgumentAdapter<TData>.ForNullableElements<T>(Func<TData, IReadOnlyList<T?>, bool> recorder)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return ForNullableElements(recorder);
    }

    DSemanticAttributeParameterMapping<TData> ISemanticCollectionArgumentAdapter<TData>.ForNullableElements<T>(Action<TData, IReadOnlyList<T?>> recorder)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return ForNullableElements<T>(wrapper);

        bool wrapper(TData data, IReadOnlyList<T?> value)
        {
            recorder(data, value);

            return true;
        }
    }

    private static DSemanticAttributeParameterMapping<TData> For<T>(Func<TData, IReadOnlyList<T>, bool> recorder)
    {
        return wrapper;

        DSemanticAttributeArgumentRecorder wrapper(TData data) => (value) =>
        {
            if (value is null)
            {
                return false;
            }

            if (value is IReadOnlyList<T> tList)
            {
                return recorder(data, tList);
            }

            if (value is not IReadOnlyList<object> objectList)
            {
                return false;
            }

            var converted = new T[objectList.Count];

            for (var i = 0; i < converted.Length; i++)
            {
                if (objectList[i] is null)
                {
                    return false;
                }

                if (objectList[i] is not T tValue)
                {
                    return false;
                }

                converted[i] = tValue;
            }

            return recorder(data, converted);
        };
    }

    private static DSemanticAttributeParameterMapping<TData> ForNullable<T>(Func<TData, IReadOnlyList<T?>?, bool> recorder) where T : class
    {
        return wrapper;

        DSemanticAttributeArgumentRecorder wrapper(TData data) => (value) =>
        {
            if (value is null)
            {
                return recorder(data, null);
            }

            if (value is IReadOnlyList<T?> tList)
            {
                return recorder(data, tList);

            }

            if (value is not IReadOnlyList<object?> objectList)
            {
                return false;
            }

            var converted = new T?[objectList.Count];

            for (var i = 0; i < converted.Length; i++)
            {
                if (objectList[i] is null)
                {
                    converted[i] = null;

                    continue;
                }

                if (objectList[i] is not T tValue)
                {
                    return false;
                }

                converted[i] = tValue;
            }

            return recorder(data, converted);
        };
    }

    private static DSemanticAttributeParameterMapping<TData> ForNullable<T>(Func<TData, IReadOnlyList<T?>?, bool> recorder) where T : struct
    {
        return wrapper;

        DSemanticAttributeArgumentRecorder wrapper(TData data) => (value) =>
        {
            if (value is null)
            {
                return recorder(data, null);
            }

            if (value is IReadOnlyList<T?> tList)
            {
                return recorder(data, tList);
            }

            if (value is not IReadOnlyList<object?> objectList)
            {
                return false;
            }

            var converted = new T?[objectList.Count];

            for (var i = 0; i < converted.Length; i++)
            {
                if (objectList[i] is null)
                {
                    converted[i] = null;

                    continue;
                }

                if (objectList[i] is not T tValue)
                {
                    return false;
                }

                converted[i] = tValue;
            }

            return recorder(data, converted);
        };
    }

    private static DSemanticAttributeParameterMapping<TData> ForNullableElements<T>(Func<TData, IReadOnlyList<T?>, bool> recorder) where T : class
    {
        return ForNullable<T>(wrapper);

        bool wrapper(TData data, IReadOnlyList<T?>? value)
        {
            if (value is null)
            {
                return false;
            }

            return recorder(data, value);
        }
    }

    private static DSemanticAttributeParameterMapping<TData> ForNullableElements<T>(Func<TData, IReadOnlyList<T?>, bool> recorder) where T : struct
    {
        return ForNullable<T>(wrapper);

        bool wrapper(TData data, IReadOnlyList<T?>? value)
        {
            if (value is null)
            {
                return false;
            }

            return recorder(data, value);
        }
    }

    private static DSemanticAttributeParameterMapping<TData> ForNullableCollection<T>(Func<TData, IReadOnlyList<T>?, bool> recorder)
    {
        return wrapper;

        DSemanticAttributeArgumentRecorder wrapper(TData data) => (value) =>
        {
            if (value is null)
            {
                return recorder(data, null);
            }

            return For(recorder)(data)(value);
        };
    }
}
