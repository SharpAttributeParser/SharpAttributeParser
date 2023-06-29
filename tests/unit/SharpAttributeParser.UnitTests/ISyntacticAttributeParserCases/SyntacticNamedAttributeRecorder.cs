namespace SharpAttributeParser.Tests.ISyntacticAttributeParserCases;

using Microsoft.CodeAnalysis;

using System.Collections.Generic;

public sealed class SyntacticNamedAttributeRecorder : ASyntacticArgumentRecorder
{
    public object? Value { get; private set; }
    public Location? ValueLocation { get; private set; }
    public bool ValueRecorded { get; private set; }

    public IReadOnlyList<object?>? Values { get; private set; }
    public Location? ValuesCollectionLocations { get; private set; }
    public IReadOnlyList<Location>? ValuesElementLocations { get; private set; }
    public bool ValuesRecorded { get; private set; }

    protected override IEnumerable<(string, DSyntacticSingleRecorder)> AddSingleRecorders()
    {
        yield return ("Value", RecordValue);
    }

    protected override IEnumerable<(string, DSyntacticArrayRecorder)> AddArrayRecorders()
    {
        yield return ("Values", RecordValues);
    }

    private bool RecordValue(object? value, Location location)
    {
        Value = value;
        ValueLocation = location;
        ValueRecorded = true;

        return true;
    }

    private bool RecordValues(IReadOnlyList<object?>? values, Location collectionLocation, IReadOnlyList<Location> elementLocations)
    {
        Values = values;
        ValuesCollectionLocations = collectionLocation;
        ValuesElementLocations = elementLocations;
        ValuesRecorded = true;

        return true;
    }
}
