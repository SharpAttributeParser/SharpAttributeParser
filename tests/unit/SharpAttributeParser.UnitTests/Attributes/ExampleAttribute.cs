using System;
using System.Diagnostics.CodeAnalysis;

[AttributeUsage(AttributeTargets.Class)]
[SuppressMessage("Design", "CA1050: Declare types in namespaces")]
[SuppressMessage("Major Code Smell", "S2326: Unused type parameters should be removed", Justification = "Used when parsing the attribute.")]
[SuppressMessage("Major Bug", "S3903: Types should be defined in named namespaces")]
public sealed class ExampleAttribute<T> : Attribute
{
    public int[] Sequence { get; }
    public string Name { get; }

    public int Answer { get; set; }

    public ExampleAttribute(int[] sequence, string name)
    {
        Sequence = sequence;
        Name = name;
    }
}
