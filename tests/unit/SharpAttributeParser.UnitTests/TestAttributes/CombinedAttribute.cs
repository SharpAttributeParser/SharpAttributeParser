using System;
using System.Diagnostics.CodeAnalysis;

[AttributeUsage(AttributeTargets.Class)]
[SuppressMessage("Design", "CA1050: Declare types in namespaces")]
[SuppressMessage("Major Code Smell", "S2326: Unused type parameters should be removed", Justification = "Used when parsing the attribute.")]
[SuppressMessage("Major Bug", "S3903: Types should be defined in named namespaces")]
public sealed class CombinedAttribute<T1, T2> : Attribute
{
    public object? NamedValue { get; set; }
    public object?[]? NamedValues { get; set; }

    public object? Value { get; }
    public object?[]? ArrayValues { get; }
    public object?[] ParamsValues { get; }

    public CombinedAttribute(object? value, object?[]? arrayValues, params object?[] paramsValues)
    {
        Value = value;
        ArrayValues = arrayValues;
        ParamsValues = paramsValues;
    }
}
