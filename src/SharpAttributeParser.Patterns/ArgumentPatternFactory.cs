namespace SharpAttributeParser.Patterns;

using Microsoft.CodeAnalysis;

using OneOf;
using OneOf.Types;

using System;

/// <inheritdoc cref="IArgumentPatternFactory"/>
public sealed class ArgumentPatternFactory : IArgumentPatternFactory
{
    private static readonly Lazy<IArgumentPattern<bool>> Bool = new(() => NonNullablePattern<bool>.PatternSingleton);
    private static readonly Lazy<IArgumentPattern<byte>> Byte = new(() => NonNullablePattern<byte>.PatternSingleton);
    private static readonly Lazy<IArgumentPattern<sbyte>> SByte = new(() => NonNullablePattern<sbyte>.PatternSingleton);
    private static readonly Lazy<IArgumentPattern<char>> Char = new(() => NonNullablePattern<char>.PatternSingleton);
    private static readonly Lazy<IArgumentPattern<short>> Short = new(() => NonNullablePattern<short>.PatternSingleton);
    private static readonly Lazy<IArgumentPattern<ushort>> UShort = new(() => NonNullablePattern<ushort>.PatternSingleton);
    private static readonly Lazy<IArgumentPattern<int>> Int = new(() => NonNullablePattern<int>.PatternSingleton);
    private static readonly Lazy<IArgumentPattern<uint>> UInt = new(() => NonNullablePattern<uint>.PatternSingleton);
    private static readonly Lazy<IArgumentPattern<long>> Long = new(() => NonNullablePattern<long>.PatternSingleton);
    private static readonly Lazy<IArgumentPattern<ulong>> ULong = new(() => NonNullablePattern<ulong>.PatternSingleton);
    private static readonly Lazy<IArgumentPattern<float>> Float = new(() => NonNullablePattern<float>.PatternSingleton);
    private static readonly Lazy<IArgumentPattern<double>> Double = new(() => NonNullablePattern<double>.PatternSingleton);

    private static readonly Lazy<IArgumentPattern<string>> NonNullableString = new(() => NonNullablePattern<string>.PatternSingleton);
    private static readonly Lazy<IArgumentPattern<string?>> NullableString = new(() => NullablePattern<string>.PatternSingleton);
    private static readonly Lazy<IArgumentPattern<object>> NonNullableObject = new(() => NonNullablePattern<object>.PatternSingleton);
    private static readonly Lazy<IArgumentPattern<object?>> NullableObject = new(() => NullablePattern<object>.PatternSingleton);
    private static readonly Lazy<IArgumentPattern<ITypeSymbol>> NonNullableType = new(() => NonNullablePattern<ITypeSymbol>.PatternSingleton);
    private static readonly Lazy<IArgumentPattern<ITypeSymbol?>> NullableType = new(() => NullablePattern<ITypeSymbol>.PatternSingleton);

    /// <summary>The singleton <see cref="ArgumentPatternFactory"/>.</summary>
    public static ArgumentPatternFactory Singleton { get; } = new();

    IArgumentPattern<bool> IArgumentPatternFactory.Bool() => Bool.Value;
    IArgumentPattern<byte> IArgumentPatternFactory.Byte() => Byte.Value;
    IArgumentPattern<sbyte> IArgumentPatternFactory.SByte() => SByte.Value;
    IArgumentPattern<char> IArgumentPatternFactory.Char() => Char.Value;
    IArgumentPattern<short> IArgumentPatternFactory.Short() => Short.Value;
    IArgumentPattern<ushort> IArgumentPatternFactory.UShort() => UShort.Value;
    IArgumentPattern<int> IArgumentPatternFactory.Int() => Int.Value;
    IArgumentPattern<uint> IArgumentPatternFactory.UInt() => UInt.Value;
    IArgumentPattern<long> IArgumentPatternFactory.Long() => Long.Value;
    IArgumentPattern<ulong> IArgumentPatternFactory.ULong() => ULong.Value;
    IArgumentPattern<float> IArgumentPatternFactory.Float() => Float.Value;
    IArgumentPattern<double> IArgumentPatternFactory.Double() => Double.Value;

    IArgumentPattern<TEnum> IArgumentPatternFactory.Enum<TEnum>() => new EnumPattern<TEnum>();

    IArgumentPattern<string> IArgumentPatternFactory.NonNullableString() => NonNullableString.Value;
    IArgumentPattern<string?> IArgumentPatternFactory.NullableString() => NullableString.Value;
    IArgumentPattern<object> IArgumentPatternFactory.NonNullableObject() => NonNullableObject.Value;
    IArgumentPattern<object?> IArgumentPatternFactory.NullableObject() => NullableObject.Value;
    IArgumentPattern<ITypeSymbol> IArgumentPatternFactory.NonNullableType() => NonNullableType.Value;
    IArgumentPattern<ITypeSymbol?> IArgumentPatternFactory.NullableType() => NullableType.Value;

    IArgumentPattern<TElement[]> IArgumentPatternFactory.NonNullableArray<TElement>(IArgumentPattern<TElement> elementPattern)
    {
        if (elementPattern is null)
        {
            throw new ArgumentNullException(nameof(elementPattern));
        }

        return new NonNullableArrayPattern<TElement>(elementPattern);
    }

    IArgumentPattern<TElement[]?> IArgumentPatternFactory.NullableArray<TElement>(IArgumentPattern<TElement> elementPattern)
    {
        if (elementPattern is null)
        {
            throw new ArgumentNullException(nameof(elementPattern));
        }

        var nonNullablePattern = new NonNullableArrayPattern<TElement>(elementPattern);

        return new NullableArrayPattern<TElement>(nonNullablePattern);
    }

    private sealed class NonNullablePattern<T> : IArgumentPattern<T>
    {
        public static IArgumentPattern<T> PatternSingleton { get; } = new NonNullablePattern<T>();

        private NonNullablePattern() { }

        OneOf<Error, T> IArgumentPattern<T>.TryFit(object? argument)
        {
            if (argument is not T tArgument)
            {
                return new Error();
            }

            return tArgument;
        }
    }

    private sealed class NullablePattern<T> : IArgumentPattern<T?>
    {
        public static IArgumentPattern<T?> PatternSingleton { get; } = new NullablePattern<T>();

        private NullablePattern() { }

        OneOf<Error, T?> IArgumentPattern<T?>.TryFit(object? argument)
        {
            if (argument is null)
            {
                return OneOf<Error, T?>.FromT1(default);
            }

            if (argument is not T tArgument)
            {
                return new Error();
            }

            return tArgument;
        }
    }

    private sealed class EnumPattern<TEnum> : IArgumentPattern<TEnum>
    {
        private readonly Func<object?, OneOf<Error, TEnum>> PatternDelegate;

        public EnumPattern()
        {
            PatternDelegate = GetPatternDelegate();
        }

        OneOf<Error, TEnum> IArgumentPattern<TEnum>.TryFit(object? argument)
        {
            if (argument is TEnum enumArgument)
            {
                return enumArgument;
            }

            return PatternDelegate(argument);
        }

        private Func<object?, OneOf<Error, TEnum>> GetPatternDelegate()
        {
            var underlyingType = typeof(TEnum).GetEnumUnderlyingType();

            if (underlyingType == typeof(int))
            {
                return fitEnumDelegate<int>(static (enumType, value) => Enum.ToObject(enumType, value));
            }

            if (underlyingType == typeof(uint))
            {
                return fitEnumDelegate<uint>(static (enumType, value) => Enum.ToObject(enumType, value));
            }

            if (underlyingType == typeof(long))
            {
                return fitEnumDelegate<long>(static (enumType, value) => Enum.ToObject(enumType, value));
            }

            if (underlyingType == typeof(ulong))
            {
                return fitEnumDelegate<ulong>(static (enumType, value) => Enum.ToObject(enumType, value));
            }

            if (underlyingType == typeof(short))
            {
                return fitEnumDelegate<short>(static (enumType, value) => Enum.ToObject(enumType, value));
            }

            if (underlyingType == typeof(ushort))
            {
                return fitEnumDelegate<ushort>(static (enumType, value) => Enum.ToObject(enumType, value));
            }

            if (underlyingType == typeof(byte))
            {
                return fitEnumDelegate<byte>(static (enumType, value) => Enum.ToObject(enumType, value));
            }

            if (underlyingType == typeof(sbyte))
            {
                return fitEnumDelegate<sbyte>(static (enumType, value) => Enum.ToObject(enumType, value));
            }

            return errorDelegate;

            Func<object?, OneOf<Error, TEnum>> fitEnumDelegate<TUnderlying>(Func<Type, TUnderlying, object> factoryDelegate)
            {
                return fitEnum;

                OneOf<Error, TEnum> fitEnum(object? argument)
                {
                    if (argument is TUnderlying matchingArgument)
                    {
                        return (TEnum)factoryDelegate(typeof(TEnum), matchingArgument);
                    }

                    return new Error();
                }
            }

            OneOf<Error, TEnum> errorDelegate(object? argument) => new Error();
        }
    }

    private sealed class NonNullableArrayPattern<TElement> : IArgumentPattern<TElement[]>
    {
        private readonly IArgumentPattern<TElement> ElementPattern;

        public NonNullableArrayPattern(IArgumentPattern<TElement> elementPattern)
        {
            ElementPattern = elementPattern;
        }

        OneOf<Error, TElement[]> IArgumentPattern<TElement[]>.TryFit(object? argument)
        {
            if (argument is null)
            {
                return new Error();
            }

            if (argument is TElement[] tArrayArgument)
            {
                var convertedArguments = new TElement[tArrayArgument.Length];

                for (var i = 0; i < convertedArguments.Length; i++)
                {
                    var convertedArgument = ElementPattern.TryFit(tArrayArgument[i]);

                    if (convertedArgument.IsT0)
                    {
                        return new Error();
                    }

                    convertedArguments[i] = convertedArgument.AsT1;
                }

                return convertedArguments;
            }

            if (argument is object[] objectArrayArgument)
            {
                var convertedArguments = new TElement[objectArrayArgument.Length];

                for (var i = 0; i < convertedArguments.Length; i++)
                {
                    var convertedArgument = ElementPattern.TryFit(objectArrayArgument[i]);

                    if (convertedArgument.IsT0)
                    {
                        return new Error();
                    }

                    convertedArguments[i] = convertedArgument.AsT1;
                }

                return convertedArguments;
            }

            return new Error();
        }
    }

    private sealed class NullableArrayPattern<TElement> : IArgumentPattern<TElement[]?>
    {
        private readonly IArgumentPattern<TElement[]> NonNullableCollectionPattern;

        public NullableArrayPattern(IArgumentPattern<TElement[]> nonNullableCollectionPattern)
        {
            NonNullableCollectionPattern = nonNullableCollectionPattern;
        }

        OneOf<Error, TElement[]?> IArgumentPattern<TElement[]?>.TryFit(object? argument)
        {
            if (argument is null)
            {
                return OneOf<Error, TElement[]?>.FromT1(null);
            }

            return NonNullableCollectionPattern.TryFit(argument).Match
            (
                static (error) => error,
                OneOf<Error, TElement[]?>.FromT1
            );
        }
    }
}
