namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;

/// <summary>Represents the <see cref="Location"/> of an array-valued argument.</summary>
public sealed class CollectionLocation
{
    /// <summary>Represents the <see cref="Location"/> of an array-valued argument as <see cref="Location.None"/>.</summary>
    public static CollectionLocation None { get; } = new(Location.None, Array.Empty<Location>());

    /// <summary>Constructs a <see cref="CollectionLocation"/> of an empty array-valued argument.</summary>
    /// <param name="collection">The <see cref="Location"/> of the argument.</param>
    /// <returns>The constructed <see cref="CollectionLocation"/>.</returns>
    /// <exception cref="ArgumentNullException"/>
    public static CollectionLocation Empty(Location collection)
    {
        if (collection is null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        if (collection == Location.None)
        {
            return None;
        }

        return new(collection, Array.Empty<Location>());
    }

    /// <summary>Instantiates a <see cref="CollectionLocation"/>, representing the <see cref="Location"/> of an array-valued argument.</summary>
    /// <param name="collection"><inheritdoc cref="Collection" path="/summary"/></param>
    /// <param name="elements"><inheritdoc cref="Elements" path="/summary"/></param>
    /// <exception cref="ArgumentNullException"/>
    public static CollectionLocation Create(Location collection, IReadOnlyList<Location> elements)
    {
        if (collection is null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        if (elements is null)
        {
            throw new ArgumentNullException(nameof(elements));
        }

        if (collection == Location.None && elements.Count is 0)
        {
            return None;
        }

        return new(collection, elements);
    }

    /// <summary>The <see cref="Location"/> of the entire argument.</summary>
    public Location Collection { get; }

    /// <summary>The <see cref="Location"/> of each element in the argument.</summary>
    public IReadOnlyList<Location> Elements { get; }

    private CollectionLocation(Location collection, IReadOnlyList<Location> elements)
    {
        Collection = collection ?? throw new ArgumentNullException(nameof(collection));
        Elements = elements ?? throw new ArgumentNullException(nameof(elements));
    }
}
