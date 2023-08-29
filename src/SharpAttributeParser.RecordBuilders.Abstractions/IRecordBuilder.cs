namespace SharpAttributeParser;

using System;

/// <summary>Responsible for building data records.</summary>
/// <typeparam name="TRecord">The type of the built data record.</typeparam>
public interface IRecordBuilder<out TRecord>
{
    /// <summary>Builds a data record.</summary>
    /// <returns>The built data record.</returns>
    /// <exception cref="InvalidOperationException"/>
    public abstract TRecord Build();
}
