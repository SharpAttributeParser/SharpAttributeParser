namespace SharpAttributeParser.Tests.ISyntacticAttributeParserCases;

using Microsoft.CodeAnalysis;

using SharpAttributeParser;

using System.Collections.Generic;

public sealed class SyntacticParamsAttributeRecorder : ASyntacticArgumentRecorder
{
    public IReadOnlyList<object?>? Value { get; private set; }
    public Location? ValueCollectionLocation { get; private set; }
    public IReadOnlyList<Location>? ValueElementLocations { get; private set; }
    public bool ValueRecorded { get; private set; }

    protected override IEnumerable<(string, DSyntacticArrayRecorder)> AddArrayRecorders()
    {
        yield return ("Value", RecordValue);
    }

    private bool RecordValue(IReadOnlyList<object?>? value, Location collectionLocation, IReadOnlyList<Location> elementLocations)
    {
        Value = value;
        ValueCollectionLocation = collectionLocation;
        ValueElementLocations = elementLocations;
        ValueRecorded = true;

        return true;
    }
}
