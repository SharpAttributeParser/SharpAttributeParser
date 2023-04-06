﻿namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;

/// <inheritdoc cref="ISyntacticAdapterProvider"/>
internal sealed class SyntacticAdapterProvider : ISyntacticAdapterProvider
{
    private ISemanticAdapterProvider SemanticAdapters { get; }

    /// <summary>Instantiates a <see cref="SyntacticAdapterProvider"/>, providing adapters that may be applied to syntactically parsed arguments before they are recorded.</summary>
    /// <param name="semanticAdapters"><inheritdoc cref="ISemanticAdapterProvider" path="/summary"/></param>
    public SyntacticAdapterProvider(ISemanticAdapterProvider semanticAdapters)
    {
        SemanticAdapters = semanticAdapters;
    }

    Func<ITypeSymbol, Location, bool> ISyntacticAdapterProvider.For(Action<ITypeSymbol, Location> recorder)
    {
        return wrapper;

        bool wrapper(ITypeSymbol value, Location location)
        {
            return SemanticAdapters.For(innerWrapper)(value);

            void innerWrapper(ITypeSymbol value) => recorder(value, location);
        }
    }

    Func<object?, Location, bool> ISyntacticAdapterProvider.For<T>(Func<T, Location, bool> recorder)
    {
        return wrapper;

        bool wrapper(object? value, Location location)
        {
            return SemanticAdapters.For<T>(innerWrapper)(value);

            bool innerWrapper(T value) => recorder(value, location);
        }
    }

    Func<IReadOnlyList<object?>?, Location, IReadOnlyList<Location>, bool> ISyntacticAdapterProvider.For<T>(Func<IReadOnlyList<T>, Location, IReadOnlyList<Location>, bool> recorder)
    {
        return wrapper;

        bool wrapper(IReadOnlyList<object?>? value, Location collectionLocation, IReadOnlyList<Location> elementLocations)
        {
            return SemanticAdapters.For<T>(innerWrapper)(value);

            bool innerWrapper(IReadOnlyList<T> value) => recorder(value, collectionLocation, elementLocations);
        }
    }

    Func<object?, Location, bool> ISyntacticAdapterProvider.For<T>(Action<T, Location> recorder)
    {
        return wrapper;

        bool wrapper(object? value, Location location)
        {
            return SemanticAdapters.For<T>(innerWrapper)(value);

            void innerWrapper(T value) => recorder(value, location);
        }
    }

    Func<IReadOnlyList<object?>?, Location, IReadOnlyList<Location>, bool> ISyntacticAdapterProvider.For<T>(Action<IReadOnlyList<T>, Location, IReadOnlyList<Location>> recorder)
    {
        return wrapper;

        bool wrapper(IReadOnlyList<object?>? value, Location collectionLocation, IReadOnlyList<Location> elementLocations)
        {
            return SemanticAdapters.For<T>(innerWrapper)(value);

            void innerWrapper(IReadOnlyList<T> value) => recorder(value, collectionLocation, elementLocations);
        }
    }

    Func<object?, Location, bool> ISyntacticAdapterProvider.ForNullable<T>(Func<T?, Location, bool> recorder) where T : class
    {
        return wrapper;

        bool wrapper(object? value, Location location)
        {
            return SemanticAdapters.ForNullable<T>(innerWrapper)(value);

            bool innerWrapper(T? value) => recorder(value, location);
        }
    }

    Func<object?, Location, bool> ISyntacticAdapterProvider.ForNullable<T>(Func<T?, Location, bool> recorder) where T : struct
    {
        return wrapper;

        bool wrapper(object? value, Location location)
        {
            return SemanticAdapters.ForNullable<T>(innerWrapper)(value);

            bool innerWrapper(T? value) => recorder(value, location);
        }
    }

    Func<IReadOnlyList<object?>?, Location, IReadOnlyList<Location>, bool> ISyntacticAdapterProvider.ForNullable<T>(Func<IReadOnlyList<T?>?, Location, IReadOnlyList<Location>, bool> recorder) where T : class
    {
        return wrapper;

        bool wrapper(IReadOnlyList<object?>? value, Location collectionLocation, IReadOnlyList<Location> elementLocations)
        {
            return SemanticAdapters.ForNullable<T>(innerWrapper)(value);

            bool innerWrapper(IReadOnlyList<T?>? value) => recorder(value, collectionLocation, elementLocations);
        }
    }

    Func<IReadOnlyList<object?>?, Location, IReadOnlyList<Location>, bool> ISyntacticAdapterProvider.ForNullable<T>(Func<IReadOnlyList<T?>?, Location, IReadOnlyList<Location>, bool> recorder) where T : struct
    {
        return wrapper;

        bool wrapper(IReadOnlyList<object?>? value, Location collectionLocation, IReadOnlyList<Location> elementLocations)
        {
            return SemanticAdapters.ForNullable<T>(innerWrapper)(value);

            bool innerWrapper(IReadOnlyList<T?>? value) => recorder(value, collectionLocation, elementLocations);
        }
    }

    Func<object?, Location, bool> ISyntacticAdapterProvider.ForNullable<T>(Action<T?, Location> recorder) where T : class
    {
        return wrapper;

        bool wrapper(object? value, Location location)
        {
            return SemanticAdapters.ForNullable<T>(innerWrapper)(value);

            void innerWrapper(T? value) => recorder(value, location);
        }
    }

    Func<object?, Location, bool> ISyntacticAdapterProvider.ForNullable<T>(Action<T?, Location> recorder) where T : struct
    {
        return wrapper;

        bool wrapper(object? value, Location location)
        {
            return SemanticAdapters.ForNullable<T>(innerWrapper)(value);

            void innerWrapper(T? value) => recorder(value, location);
        }
    }

    Func<IReadOnlyList<object?>?, Location, IReadOnlyList<Location>, bool> ISyntacticAdapterProvider.ForNullable<T>(Action<IReadOnlyList<T?>?, Location, IReadOnlyList<Location>> recorder) where T : class
    {
        return wrapper;

        bool wrapper(IReadOnlyList<object?>? value, Location collectionLocation, IReadOnlyList<Location> elementLocations)
        {
            return SemanticAdapters.ForNullable<T>(innerWrapper)(value);

            void innerWrapper(IReadOnlyList<T?>? value) => recorder(value, collectionLocation, elementLocations);
        }
    }

    Func<IReadOnlyList<object?>?, Location, IReadOnlyList<Location>, bool> ISyntacticAdapterProvider.ForNullable<T>(Action<IReadOnlyList<T?>?, Location, IReadOnlyList<Location>> recorder) where T : struct
    {
        return wrapper;

        bool wrapper(IReadOnlyList<object?>? value, Location collectionLocation, IReadOnlyList<Location> elementLocations)
        {
            return SemanticAdapters.ForNullable<T>(innerWrapper)(value);

            void innerWrapper(IReadOnlyList<T?>? value) => recorder(value, collectionLocation, elementLocations);
        }
    }

    Func<IReadOnlyList<object?>?, Location, IReadOnlyList<Location>, bool> ISyntacticAdapterProvider.ForNullableElements<T>(Func<IReadOnlyList<T?>, Location, IReadOnlyList<Location>, bool> recorder) where T : class
    {
        return wrapper;

        bool wrapper(IReadOnlyList<object?>? value, Location collectionLocation, IReadOnlyList<Location> elementLocations)
        {
            return SemanticAdapters.ForNullableElements<T>(innerWrapper)(value);

            bool innerWrapper(IReadOnlyList<T?> value) => recorder(value, collectionLocation, elementLocations);
        }
    }

    Func<IReadOnlyList<object?>?, Location, IReadOnlyList<Location>, bool> ISyntacticAdapterProvider.ForNullableElements<T>(Func<IReadOnlyList<T?>, Location, IReadOnlyList<Location>, bool> recorder) where T : struct
    {
        return wrapper;

        bool wrapper(IReadOnlyList<object?>? value, Location collectionLocation, IReadOnlyList<Location> elementLocations)
        {
            return SemanticAdapters.ForNullableElements<T>(innerWrapper)(value);

            bool innerWrapper(IReadOnlyList<T?> value) => recorder(value, collectionLocation, elementLocations);
        }
    }

    Func<IReadOnlyList<object?>?, Location, IReadOnlyList<Location>, bool> ISyntacticAdapterProvider.ForNullableElements<T>(Action<IReadOnlyList<T?>, Location, IReadOnlyList<Location>> recorder) where T : class
    {
        return wrapper;

        bool wrapper(IReadOnlyList<object?>? value, Location collectionLocation, IReadOnlyList<Location> elementLocations)
        {
            return SemanticAdapters.ForNullableElements<T>(innerWrapper)(value);

            void innerWrapper(IReadOnlyList<T?> value) => recorder(value, collectionLocation, elementLocations);
        }
    }

    Func<IReadOnlyList<object?>?, Location, IReadOnlyList<Location>, bool> ISyntacticAdapterProvider.ForNullableElements<T>(Action<IReadOnlyList<T?>, Location, IReadOnlyList<Location>> recorder) where T : struct
    {
        return wrapper;

        bool wrapper(IReadOnlyList<object?>? value, Location collectionLocation, IReadOnlyList<Location> elementLocations)
        {
            return SemanticAdapters.ForNullableElements<T>(innerWrapper)(value);

            void innerWrapper(IReadOnlyList<T?> value) => recorder(value, collectionLocation, elementLocations);
        }
    }

    Func<IReadOnlyList<object?>?, Location, IReadOnlyList<Location>, bool> ISyntacticAdapterProvider.ForNullableCollection<T>(Func<IReadOnlyList<T>?, Location, IReadOnlyList<Location>, bool> recorder)
    {
        return wrapper;

        bool wrapper(IReadOnlyList<object?>? value, Location collectionLocation, IReadOnlyList<Location> elementLocations)
        {
            return SemanticAdapters.ForNullableCollection<T>(innerWrapper)(value);

            bool innerWrapper(IReadOnlyList<T>? value) => recorder(value, collectionLocation, elementLocations);
        }
    }

    Func<IReadOnlyList<object?>?, Location, IReadOnlyList<Location>, bool> ISyntacticAdapterProvider.ForNullableCollection<T>(Action<IReadOnlyList<T>?, Location, IReadOnlyList<Location>> recorder)
    {
        return wrapper;

        bool wrapper(IReadOnlyList<object?>? value, Location collectionLocation, IReadOnlyList<Location> elementLocations)
        {
            return SemanticAdapters.ForNullableCollection<T>(innerWrapper)(value);

            void innerWrapper(IReadOnlyList<T>? value) => recorder(value, collectionLocation, elementLocations);
        }
    }
}
