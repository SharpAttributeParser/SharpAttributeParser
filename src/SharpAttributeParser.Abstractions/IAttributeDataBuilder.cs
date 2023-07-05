namespace SharpAttributeParser;

using System;

/// <summary>Responsible for incrementally constructing instances of <typeparamref name="TData"/>.</summary>
/// <typeparam name="TData">The type constructed by the <see cref="IAttributeDataBuilder{TData}"/>.</typeparam>
public interface IAttributeDataBuilder<out TData>
{
    /// <summary>Builds the <typeparamref name="TData"/> under construction.</summary>
    /// <returns>The built <typeparamref name="TData"/>.</returns>
    /// <exception cref="InvalidOperationException"/>
    public abstract TData Build();
}
