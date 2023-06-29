namespace SharpAttributeParser.Tests;

using System;
using System.Diagnostics.CodeAnalysis;

[AttributeUsage(AttributeTargets.Class)]
[SuppressMessage("Major Code Smell", "S2326: Unused type parameters should be removed", Justification = "Used when parsing the attribute.")]
public sealed class QualifiedGenericAttribute<T> : Attribute { }
