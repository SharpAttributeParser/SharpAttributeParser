using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Design", "CA1050: Declare types in namespaces")]
[SuppressMessage("Minor Code Smell", "S2344: Enumeration type names should not have \"Flags\" or \"Enum\" suffixes")]
[SuppressMessage("Major Bug", "S3903: Types should be defined in named namespaces")]
public enum IntEnum
{
    None = 0,
    Some = 1
}
