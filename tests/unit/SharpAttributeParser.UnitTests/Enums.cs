﻿using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Design", "CA1050: Declare types in namespaces")]
[SuppressMessage("Minor Code Smell", "S2344: Enumeration type names should not have \"Flags\" or \"Enum\" suffixes")]
[SuppressMessage("Major Bug", "S3903: Types should be defined in named namespaces")]
public enum IntEnum
{
    None = 0,
    Some = 1
}

[SuppressMessage("Design", "CA1028: Enum Storage should be Int32")]
[SuppressMessage("Design", "CA1050: Declare types in namespaces")]
[SuppressMessage("Minor Code Smell", "S2344: Enumeration type names should not have \"Flags\" or \"Enum\" suffixes")]
[SuppressMessage("Major Bug", "S3903: Types should be defined in named namespaces")]
public enum UIntEnum : uint
{
    None = 0,
    Some = 1
}

[SuppressMessage("Design", "CA1028: Enum Storage should be Int32")]
[SuppressMessage("Design", "CA1050: Declare types in namespaces")]
[SuppressMessage("Minor Code Smell", "S2344: Enumeration type names should not have \"Flags\" or \"Enum\" suffixes")]
[SuppressMessage("Major Bug", "S3903: Types should be defined in named namespaces")]
public enum LongEnum : long
{
    None = 0,
    Some = 1
}

[SuppressMessage("Design", "CA1028: Enum Storage should be Int32")]
[SuppressMessage("Design", "CA1050: Declare types in namespaces")]
[SuppressMessage("Minor Code Smell", "S2344: Enumeration type names should not have \"Flags\" or \"Enum\" suffixes")]
[SuppressMessage("Major Bug", "S3903: Types should be defined in named namespaces")]
public enum ULongEnum : ulong
{
    None = 0,
    Some = 1
}

[SuppressMessage("Design", "CA1028: Enum Storage should be Int32")]
[SuppressMessage("Design", "CA1050: Declare types in namespaces")]
[SuppressMessage("Minor Code Smell", "S2344: Enumeration type names should not have \"Flags\" or \"Enum\" suffixes")]
[SuppressMessage("Major Bug", "S3903: Types should be defined in named namespaces")]
public enum ShortEnum : short
{
    None = 0,
    Some = 1
}

[SuppressMessage("Design", "CA1028: Enum Storage should be Int32")]
[SuppressMessage("Design", "CA1050: Declare types in namespaces")]
[SuppressMessage("Minor Code Smell", "S2344: Enumeration type names should not have \"Flags\" or \"Enum\" suffixes")]
[SuppressMessage("Major Bug", "S3903: Types should be defined in named namespaces")]
public enum UShortEnum : ushort
{
    None = 0,
    Some = 1
}

[SuppressMessage("Design", "CA1028: Enum Storage should be Int32")]
[SuppressMessage("Design", "CA1050: Declare types in namespaces")]
[SuppressMessage("Minor Code Smell", "S2344: Enumeration type names should not have \"Flags\" or \"Enum\" suffixes")]
[SuppressMessage("Major Bug", "S3903: Types should be defined in named namespaces")]
public enum ByteEnum : byte
{
    None = 0,
    Some = 1
}

[SuppressMessage("Design", "CA1028: Enum Storage should be Int32")]
[SuppressMessage("Design", "CA1050: Declare types in namespaces")]
[SuppressMessage("Minor Code Smell", "S2344: Enumeration type names should not have \"Flags\" or \"Enum\" suffixes")]
[SuppressMessage("Major Bug", "S3903: Types should be defined in named namespaces")]
public enum SByteEnum : sbyte
{
    None = 0,
    Some = 1
}
