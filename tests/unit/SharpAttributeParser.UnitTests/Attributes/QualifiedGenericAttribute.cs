namespace SharpAttributeParser;

using System;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Major Code Smell", "S2326: Unused type parameters should be removed", Justification = "Used when parsing the attribute.")]
[AttributeUsage(AttributeTargets.Class)]
public sealed class QualifiedGenericAttribute<T1, T2> : Attribute { }
