using System;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Design", "CA1050: Declare types in namespaces")]
[SuppressMessage("Major Code Smell", "S2326: Unused type parameters should be removed", Justification = "Used when parsing the attribute.")]
[SuppressMessage("Major Bug", "S3903: Types should be defined in named namespaces")]
[AttributeUsage(AttributeTargets.Class)]
public sealed class CombinedAttribute<T1, T2> : Attribute
{
    public object? SimpleValue { get; }
    public object?[]? ArrayValue { get; }
    public object?[] ParamsValue { get; }

    public object? SimpleNamedValue { get; set; }
    public object?[]? ArrayNamedValue { get; set; }

    public CombinedAttribute(object? simpleValue, object?[]? arrayValue, params object?[] paramsValue)
    {
        SimpleValue = simpleValue;
        ArrayValue = arrayValue;
        ParamsValue = paramsValue;
    }
}
