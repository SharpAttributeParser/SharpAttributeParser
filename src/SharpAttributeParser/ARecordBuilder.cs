namespace SharpAttributeParser;

using System;

/// <summary>An abstract <see cref="IRecordBuilder{TRecord}"/>.</summary>
/// <typeparam name="TRecord">The type constructed by the <see cref="ARecordBuilder{TRecord}"/>.</typeparam>
public abstract class ARecordBuilder<TRecord> : IRecordBuilder<TRecord>
{
    private bool HasBeenBuilt { get; set; }

    TRecord IRecordBuilder<TRecord>.Build()
    {
        if (CheckFullyConstructed() is false)
        {
            throw new InvalidOperationException($"Cannot build the {typeof(TRecord).Name}, as it has not yet been fully constructed.");
        }

        HasBeenBuilt = true;

        return GetTarget();
    }

    /// <summary>Retrieves the <typeparamref name="TRecord"/> under construction.</summary>
    /// <returns>The <typeparamref name="TRecord"/> under construction.</returns>
    protected abstract TRecord GetTarget();

    /// <summary>Checks whether the <typeparamref name="TRecord"/> has been fully constructed.</summary>
    /// <returns>A <see cref="bool"/> indicating whether the <typeparamref name="TRecord"/> has been fully constructed.</returns>
    protected virtual bool CheckFullyConstructed() => true;

    /// <summary>Verifies that the <typeparamref name="TRecord"/> can be further modified, and throws an <see cref="InvalidOperationException"/> otherwise.</summary>
    /// <exception cref="InvalidOperationException"/>
    protected void VerifyCanModify()
    {
        if (HasBeenBuilt)
        {
            throw new InvalidOperationException($"The {typeof(TRecord).Name} has been built, and may not be further modified.");
        }
    }
}
