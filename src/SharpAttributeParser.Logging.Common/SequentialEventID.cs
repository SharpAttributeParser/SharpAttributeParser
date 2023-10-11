namespace SharpAttributeParser.Logging;

using Microsoft.Extensions.Logging;

/// <summary>Used to retrieve sequential <see cref="int"/> for creating <see cref="EventId"/>.</summary>
public sealed class SequentialEventID
{
    private int NextField;

    /// <summary>Retrieves the <see cref="int"/> ID of the next <see cref="EventId"/>.</summary>
    public int Next
    {
        get
        {
            NextField += 1;

            return NextField;
        }
    }
}
