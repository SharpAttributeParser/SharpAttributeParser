namespace SharpAttributeParser.Patterns.ArgumentPatternFactoryCases.EnumPatternCases;

using Moq;

using OneOf;
using OneOf.Types;

using System;
using System.Diagnostics.CodeAnalysis;

using Xunit;

public sealed class TryFit_Enum
{
    private static OneOf<Error, TEnum> Target<TEnum>(IArgumentPattern<TEnum> pattern, object? argument) => pattern.TryFit(argument);

    private FactoryContext Context { get; } = FactoryContext.Create();

    [Fact]
    public void StringSplitOptions_StringSplitOptions_ResultsInMatch() => ResultsInMatch(StringSplitOptions.TrimEntries, StringSplitOptions.TrimEntries);

    [Fact]
    public void StringComparison_StringComparison_ResultsInMatch() => ResultsInMatch(StringComparison.Ordinal, StringComparison.Ordinal);

    [Fact]
    public void StringComparison_Int_ResultsInMatch() => ResultsInMatch(StringComparison.Ordinal, 4);

    [Fact]
    public void IntBasedEnum_Int_ResultsInMatch() => ResultsInMatch(IntEnum.None, 0);

    [Fact]
    public void IntBasedEnum_Short_ResultsInError() => ResultsInError<IntEnum>((short)0);

    [Fact]
    public void UIntBasedEnum_UInt_ResultsInMatch() => ResultsInMatch(UIntEnum.None, (uint)0);

    [Fact]
    public void UIntBasedEnum_UShort_ResultsInError() => ResultsInError<UIntEnum>((ushort)0);

    [Fact]
    public void LongBasedEnum_Long_ResultsInMatch() => ResultsInMatch(LongEnum.None, (long)0);

    [Fact]
    public void LongBasedEnum_Int_ResultsInError() => ResultsInError<LongEnum>(0);

    [Fact]
    public void ULongBasedEnum_ULong_ResultsInMatch() => ResultsInMatch(ULongEnum.None, (ulong)0);

    [Fact]
    public void ULongBasedEnum_UInt_ResultsInError() => ResultsInError<ULongEnum>((uint)0);

    [Fact]
    public void ShortBasedEnum_Short_ResultsInMatch() => ResultsInMatch(ShortEnum.None, (short)0);

    [Fact]
    public void ShortBasedEnum_SByte_ResultsInError() => ResultsInError<ShortEnum>((sbyte)0);

    [Fact]
    public void UShortBasedEnum_UShort_ResultsInMatch() => ResultsInMatch(UShortEnum.None, (ushort)0);

    [Fact]
    public void UShortBasedEnum_Byte_ResultsInError() => ResultsInError<UShortEnum>((byte)0);

    [Fact]
    public void ByteBasedEnum_Byte_ResultsInMatch() => ResultsInMatch(ByteEnum.None, (byte)0);

    [Fact]
    public void ByteBasedEnum_UShort_ResultsInError() => ResultsInError<ByteEnum>((ushort)0);

    [Fact]
    public void SByteBasedEnum_SByte_ResultsInMatch() => ResultsInMatch(SByteEnum.None, (sbyte)0);

    [Fact]
    public void SByteBasedEnum_Short_ResultsInError() => ResultsInError<SByteEnum>((short)0);

    [Fact]
    public void StringComparison_StringSplitOptions_ResultsInError() => ResultsInError<StringComparison>(StringSplitOptions.TrimEntries);

    [Fact]
    public void StringComparison_Object_ResultsInError() => ResultsInError<StringComparison>(Mock.Of<object>());

    [Fact]
    public void StringComparison_Null_ResultsInError() => ResultsInError<StringComparison>(null);

    [AssertionMethod]
    private void ResultsInMatch<TEnum>(TEnum expected, object? argument) where TEnum : Enum
    {
        var pattern = ((IArgumentPatternFactory)Context.Factory).Enum<TEnum>();

        var result = Target(pattern, argument);

        OneOfAssertions.Equal(expected, result);
    }

    [AssertionMethod]
    private void ResultsInError<TEnum>(object? argument) where TEnum : Enum
    {
        var pattern = ((IArgumentPatternFactory)Context.Factory).Enum<TEnum>();

        var result = Target(pattern, argument);

        OneOfAssertions.Equal(new Error(), result);
    }

    [SuppressMessage("Minor Code Smell", "S2344: Enumeration type names should not have \"Flags\" or \"Enum\" suffixes")]
    private enum IntEnum
    {
        None = 0
    }

    [SuppressMessage("Minor Code Smell", "S2344: Enumeration type names should not have \"Flags\" or \"Enum\" suffixes")]
    private enum UIntEnum : uint
    {
        None = 0
    }

    [SuppressMessage("Minor Code Smell", "S2344: Enumeration type names should not have \"Flags\" or \"Enum\" suffixes")]
    private enum LongEnum : long
    {
        None = 0
    }

    [SuppressMessage("Minor Code Smell", "S2344: Enumeration type names should not have \"Flags\" or \"Enum\" suffixes")]
    private enum ULongEnum : ulong
    {
        None = 0
    }

    [SuppressMessage("Minor Code Smell", "S2344: Enumeration type names should not have \"Flags\" or \"Enum\" suffixes")]
    private enum ShortEnum : short
    {
        None = 0
    }

    [SuppressMessage("Minor Code Smell", "S2344: Enumeration type names should not have \"Flags\" or \"Enum\" suffixes")]
    private enum UShortEnum : ushort
    {
        None = 0
    }

    [SuppressMessage("Minor Code Smell", "S2344: Enumeration type names should not have \"Flags\" or \"Enum\" suffixes")]
    private enum ByteEnum : byte
    {
        None = 0
    }

    [SuppressMessage("Minor Code Smell", "S2344: Enumeration type names should not have \"Flags\" or \"Enum\" suffixes")]
    private enum SByteEnum : sbyte
    {
        None = 0
    }
}
