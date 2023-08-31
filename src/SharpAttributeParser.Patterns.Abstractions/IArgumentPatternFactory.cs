namespace SharpAttributeParser.Patterns;

using Microsoft.CodeAnalysis;

using System;

/// <summary>Handles creation of <see cref="IArgumentPattern{T}"/>.</summary>
public interface IArgumentPatternFactory
{
    /// <summary>Creates a pattern which ensures that arguments are of type <see cref="bool"/>.</summary>
    /// <returns>The created pattern.</returns>
    public abstract IArgumentPattern<bool> Bool();

    /// <summary>Creates a pattern which ensures that arguments are of type <see cref="byte"/>.</summary>
    /// <returns>The created pattern.</returns>
    public abstract IArgumentPattern<byte> Byte();

    /// <summary>Creates a pattern which ensures that arguments are of type <see cref="sbyte"/>.</summary>
    /// <returns>The created pattern.</returns>
    public abstract IArgumentPattern<sbyte> SByte();

    /// <summary>Creates a pattern which ensures that arguments are of type <see cref="char"/>.</summary>
    /// <returns>The created pattern.</returns>
    public abstract IArgumentPattern<char> Char();

    /// <summary>Creates a pattern which ensures that arguments are of type <see cref="short"/>.</summary>
    /// <returns>The created pattern.</returns>
    public abstract IArgumentPattern<short> Short();

    /// <summary>Creates a pattern which ensures that arguments are of type <see cref="ushort"/>.</summary>
    /// <returns>The created pattern.</returns>
    public abstract IArgumentPattern<ushort> UShort();

    /// <summary>Creates a pattern which ensures that arguments are of type <see cref="int"/>.</summary>
    /// <returns>The created pattern.</returns>
    public abstract IArgumentPattern<int> Int();

    /// <summary>Creates a pattern which ensures that arguments are of type <see cref="uint"/>.</summary>
    /// <returns>The created pattern.</returns>
    public abstract IArgumentPattern<uint> UInt();

    /// <summary>Creates a pattern which ensures that arguments are of type <see cref="long"/>.</summary>
    /// <returns>The created pattern.</returns>
    public abstract IArgumentPattern<long> Long();

    /// <summary>Creates a pattern which ensures that arguments are of type <see cref="ulong"/>.</summary>
    /// <returns>The created pattern.</returns>
    public abstract IArgumentPattern<ulong> ULong();

    /// <summary>Creates a pattern which ensures that arguments are of type <see cref="float"/>.</summary>
    /// <returns>The created pattern.</returns>
    public abstract IArgumentPattern<float> Float();

    /// <summary>Creates a pattern which ensures that arguments are of type <see cref="double"/>.</summary>
    /// <returns>The created pattern.</returns>
    public abstract IArgumentPattern<double> Double();

    /// <summary>Creates a pattern which ensures that arguments are of a type <typeparamref name="TEnum"/>.</summary>
    /// <typeparam name="TEnum">The type of the arguments matched by the created pattern, an enum type.</typeparam>
    /// <returns>The created pattern.</returns>
    public abstract IArgumentPattern<TEnum> Enum<TEnum>() where TEnum : Enum;

    /// <summary>Creates a pattern which ensures that arguments are of type <see cref="string"/>.</summary>
    /// <returns>The created pattern.</returns>
    public abstract IArgumentPattern<string> NonNullableString();

    /// <summary>Creates a pattern which ensures that arguments are either <see langword="null"/> or of type <see cref="string"/>.</summary>
    /// <returns>The created pattern.</returns>
    public abstract IArgumentPattern<string?> NullableString();

    /// <summary>Creates a pattern which ensures that arguments are of type <see cref="object"/>.</summary>
    /// <returns>The created pattern.</returns>
    public abstract IArgumentPattern<object> NonNullableObject();

    /// <summary>Creates a pattern which ensures that arguments are either <see langword="null"/> or of type <see cref="object"/>.</summary>
    /// <returns>The created pattern.</returns>
    public abstract IArgumentPattern<object?> NullableObject();

    /// <summary>Creates a pattern which ensures that arguments are of type <see cref="ITypeSymbol"/>.</summary>
    /// <remarks>Non-null arguments passed to parameters of type <see cref="Type"/> will fit the created pattern.</remarks>
    /// <returns>The created pattern.</returns>
    public abstract IArgumentPattern<ITypeSymbol> NonNullableType();

    /// <summary>Creates a pattern which ensures that arguments are either <see langword="null"/> or of type <see cref="ITypeSymbol"/>.</summary>
    /// <remarks>Arguments passed to parameters of type <see cref="Type"/> will fit the created pattern.</remarks>
    /// <returns>The created pattern.</returns>
    public abstract IArgumentPattern<ITypeSymbol?> NullableType();

    /// <summary>Creates a pattern which ensures that arguments are arrays with elements that all match the provided pattern.</summary>
    /// <typeparam name="TElement">The element-type of the arguments matched by the created pattern.</typeparam>
    /// <param name="elementPattern">The pattern of each element of the matched arguments.</param>
    /// <returns>The created pattern.</returns>
    public abstract IArgumentPattern<TElement[]> NonNullableArray<TElement>(IArgumentPattern<TElement> elementPattern);

    /// <summary>Creates a pattern which ensures that arguments are either <see langword="null"/> or arrays with elements that all match the provided pattern.</summary>
    /// <typeparam name="TElement">The element-type of the arguments matched by the created pattern.</typeparam>
    /// <param name="elementPattern">The pattern of each element of the matched arguments.</param>
    /// <returns>The created pattern.</returns>
    public abstract IArgumentPattern<TElement[]?> NullableArray<TElement>(IArgumentPattern<TElement> elementPattern);
}
