namespace SharpAttributeParser;

using System;

/// <inheritdoc cref="ISemanticArgumentAdapter{TData}"/>
internal sealed class SemanticArgumentAdapter<TData> : ISemanticArgumentAdapter<TData>
{
    ISemanticCollectionArgumentAdapter<TData> ISemanticArgumentAdapter<TData>.Collections { get; } = new SemanticCollectionArgumentAdapter<TData>();

    DSemanticAttributeParameterMapping<TData> ISemanticArgumentAdapter<TData>.For<T>(Func<TData, T, bool> recorder)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return For(recorder);
    }

    DSemanticAttributeParameterMapping<TData> ISemanticArgumentAdapter<TData>.For<T>(Action<TData, T> recorder)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return For<T>(wrapper);

        bool wrapper(TData data, T value)
        {
            recorder(data, value);

            return true;
        }
    }

    DSemanticAttributeParameterMapping<TData> ISemanticArgumentAdapter<TData>.ForNullable<T>(Func<TData, T?, bool> recorder) where T : class
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return ForNullable(recorder);
    }

    DSemanticAttributeParameterMapping<TData> ISemanticArgumentAdapter<TData>.ForNullable<T>(Action<TData, T?> recorder) where T : class
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return ForNullable<T>(wrapper);

        bool wrapper(TData data, T? value)
        {
            recorder(data, value);

            return true;
        }
    }

    DSemanticAttributeParameterMapping<TData> ISemanticArgumentAdapter<TData>.ForNullable<T>(Func<TData, T?, bool> recorder) where T : struct
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return ForNullable(recorder);
    }

    DSemanticAttributeParameterMapping<TData> ISemanticArgumentAdapter<TData>.ForNullable<T>(Action<TData, T?> recorder) where T : struct
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return ForNullable<T>(wrapper);

        bool wrapper(TData data, T? value)
        {
            recorder(data, value);

            return true;
        }
    }

    private static DSemanticAttributeParameterMapping<TData> For<T>(Func<TData, T, bool> recorder)
    {
        return wrapper;

        DSemanticAttributeArgumentRecorder wrapper(TData data) => (value) =>
        {
            if (value is null)
            {
                return false;
            }

            if (typeof(T).IsArray)
            {
                return false;
            }

            if (value is not T tValue)
            {
                return false;
            }

            return recorder(data, tValue);
        };
    }

    private static DSemanticAttributeParameterMapping<TData> ForNullable<T>(Func<TData, T?, bool> recorder) where T : class
    {
        return wrapper;

        DSemanticAttributeArgumentRecorder wrapper(TData data) => (value) =>
        {
            if (value is null)
            {
                return recorder(data, null);
            }

            if (value is not T tValue)
            {
                return false;
            }

            return recorder(data, tValue);
        };
    }

    private static DSemanticAttributeParameterMapping<TData> ForNullable<T>(Func<TData, T?, bool> recorder) where T : struct
    {
        return wrapper;

        DSemanticAttributeArgumentRecorder wrapper(TData data) => (value) =>
        {
            if (value is null)
            {
                return recorder(data, null);
            }

            if (value is not T tValue)
            {
                return false;
            }

            return recorder(data, tValue);
        };
    }
}
