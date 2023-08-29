namespace SharpAttributeParser.Patterns;

using Microsoft.CodeAnalysis;

using OneOf;
using OneOf.Types;

using System;

/// <inheritdoc cref="IArgumentPatternFactory"/>
public sealed class ArgumentPatternFactory : IArgumentPatternFactory
{
    private static Lazy<IArgumentPattern<bool>> Bool { get; } = new(() => NonNullablePattern<bool>.PatternSingleton);
    private static Lazy<IArgumentPattern<byte>> Byte { get; } = new(() => NonNullablePattern<byte>.PatternSingleton);
    private static Lazy<IArgumentPattern<sbyte>> SByte { get; } = new(() => NonNullablePattern<sbyte>.PatternSingleton);
    private static Lazy<IArgumentPattern<char>> Char { get; } = new(() => NonNullablePattern<char>.PatternSingleton);
    private static Lazy<IArgumentPattern<short>> Short { get; } = new(() => NonNullablePattern<short>.PatternSingleton);
    private static Lazy<IArgumentPattern<ushort>> UShort { get; } = new(() => NonNullablePattern<ushort>.PatternSingleton);
    private static Lazy<IArgumentPattern<int>> Int { get; } = new(() => NonNullablePattern<int>.PatternSingleton);
    private static Lazy<IArgumentPattern<uint>> UInt { get; } = new(() => NonNullablePattern<uint>.PatternSingleton);
    private static Lazy<IArgumentPattern<long>> Long { get; } = new(() => NonNullablePattern<long>.PatternSingleton);
    private static Lazy<IArgumentPattern<ulong>> ULong { get; } = new(() => NonNullablePattern<ulong>.PatternSingleton);
    private static Lazy<IArgumentPattern<float>> Float { get; } = new(() => NonNullablePattern<float>.PatternSingleton);
    private static Lazy<IArgumentPattern<double>> Double { get; } = new(() => NonNullablePattern<double>.PatternSingleton);

    private static Lazy<IArgumentPattern<string>> NonNullableString { get; } = new(() => NonNullablePattern<string>.PatternSingleton);
    private static Lazy<IArgumentPattern<string?>> NullableString { get; } = new(() => NullablePattern<string>.PatternSingleton);
    private static Lazy<IArgumentPattern<object>> NonNullableObject { get; } = new(() => NonNullablePattern<object>.PatternSingleton);
    private static Lazy<IArgumentPattern<object?>> NullableObject { get; } = new(() => NullablePattern<object>.PatternSingleton);
    private static Lazy<IArgumentPattern<ITypeSymbol>> NonNullableType { get; } = new(() => NonNullablePattern<ITypeSymbol>.PatternSingleton);
    private static Lazy<IArgumentPattern<ITypeSymbol?>> NullableType { get; } = new(() => NullablePattern<ITypeSymbol>.PatternSingleton);

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
        private Func<object?, OneOf<Error, TEnum>> PatternDelegate { get; }

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
        private IArgumentPattern<TElement> ElementPattern { get; }

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
        private IArgumentPattern<TElement[]> NonNullableCollectionPattern { get; }

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
