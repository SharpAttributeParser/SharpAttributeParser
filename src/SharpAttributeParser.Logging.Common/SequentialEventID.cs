namespace SharpAttributeParser.Logging;

using Microsoft.Extensions.Logging;

/// <summary>Used to retrieve sequential <see cref="int"/> for creating <see cref="EventId"/>.</summary>
public sealed class SequentialEventID
{
    private int NextSetter { get; set; }

    /// <summary>Retrieves the <see cref="int"/> ID of the next <see cref="EventId"/>.</summary>
    public int Next
    {
        get
        {
            NextSetter += 1;

            return NextSetter;
        }
    }
}
