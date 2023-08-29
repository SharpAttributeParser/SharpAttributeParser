using System;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Design", "CA1050: Declare types in namespaces")]
[SuppressMessage("Major Bug", "S3903: Types should be defined in named namespaces")]
[AttributeUsage(AttributeTargets.Class)]
public sealed class OptionalAttribute : Attribute
{
    public object? ValueA { get; }
    public object? ValueB { get; }

    public OptionalAttribute(object? valueA = null, object? valueB = null)
    {
        ValueA = valueA;
        ValueB = valueB;
    }
}
