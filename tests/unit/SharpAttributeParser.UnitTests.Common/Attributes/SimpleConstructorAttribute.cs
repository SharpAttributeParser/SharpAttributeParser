using System;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Design", "CA1050: Declare types in namespaces")]
[SuppressMessage("Major Bug", "S3903: Types should be defined in named namespaces")]
[AttributeUsage(AttributeTargets.Class)]
public sealed class SimpleConstructorAttribute : Attribute
{
    public object? ValueA { get; }
    public object? ValueB { get; }

    public SimpleConstructorAttribute(object? valueA, object? valueB)
    {
        ValueA = valueA;
        ValueB = valueB;
    }
}
