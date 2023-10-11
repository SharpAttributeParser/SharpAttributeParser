namespace SharpAttributeParser;

using System;

/// <summary>An abstract <see cref="IRecordBuilder{TRecord}"/>, responsible for building data records.</summary>
/// <typeparam name="TRecord">The type of the built data record.</typeparam>
public abstract class ARecordBuilder<TRecord> : IRecordBuilder<TRecord>
{
    private bool HasBeenBuilt;

    private readonly bool ThrowOnMultipleBuilds;

    /// <summary>Instantiates a <see cref="ARecordBuilder{TRecord}"/>, reponsible for building data records.</summary>
    /// <param name="throwOnMultipleBuilds">Indicates whether an <see cref="InvalidOperationException"/> should be thrown if build is invoked more than once.</param>
    protected ARecordBuilder(bool throwOnMultipleBuilds = false)
    {
        ThrowOnMultipleBuilds = throwOnMultipleBuilds;
    }

    /// <inheritdoc/>
    public TRecord Build()
    {
        if (CanBuildRecord() is false)
        {
            throw new InvalidOperationException($"Cannot build the {typeof(TRecord).Name}, due to the current state of the record.");
        }

        if (HasBeenBuilt && ThrowOnMultipleBuilds)
        {
            throw new InvalidOperationException($"The {typeof(TRecord).Name} has already been built.");
        }

        HasBeenBuilt = true;

        return GetRecord() ?? throw new InvalidOperationException($"The {typeof(TRecord).Name} under construction was unexpectedly null.");
    }

    /// <summary>Retrieves the data record under construction.</summary>
    /// <returns>The data record under construction.</returns>
    protected abstract TRecord GetRecord();

    /// <summary>Checks whether the data record can be built, considering the current state of the builder.</summary>
    /// <returns>A <see cref="bool"/> indicating whether the data record can be built.</returns>
    protected virtual bool CanBuildRecord() => true;

    /// <summary>Determines whether the data record may be further modified.</summary>
    /// <returns>A <see cref="bool"/> indicating whether the data record may be further modified.</returns>
    protected bool CanModify() => HasBeenBuilt is false;

    /// <summary>Verifies that the data record may be further modified, and throws an <see cref="InvalidOperationException"/> otherwise.</summary>
    protected void VerifyCanModify()
    {
        if (CanModify() is false)
        {
            throw new InvalidOperationException($"The {typeof(TRecord).Name} has been built, and may not be modified.");
        }
    }
}
