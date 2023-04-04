﻿using System;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Design", "CA1050: Declare types in namespaces")]
[SuppressMessage("Major Bug", "S3903: Types should be defined in named namespaces")]
[AttributeUsage(AttributeTargets.Class)]
public sealed class NamedAttribute : Attribute
{
    public object? Value { get; set; }
    public object?[]? Values { get; set; }
}
