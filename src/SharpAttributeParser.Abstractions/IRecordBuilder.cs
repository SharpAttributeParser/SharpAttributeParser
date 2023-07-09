namespace SharpAttributeParser;

using System;

/// <summary>Responsible for incrementally constructing instances of <typeparamref name="TRecord"/>.</summary>
/// <typeparam name="TRecord">The type constructed by the <see cref="IRecordBuilder{TRecord}"/>.</typeparam>
public interface IRecordBuilder<out TRecord>
{
    /// <summary>Builds the <typeparamref name="TRecord"/> under construction.</summary>
    /// <returns>The built <typeparamref name="TRecord"/>.</returns>
    /// <exception cref="InvalidOperationException"/>
    public abstract TRecord Build();
}
