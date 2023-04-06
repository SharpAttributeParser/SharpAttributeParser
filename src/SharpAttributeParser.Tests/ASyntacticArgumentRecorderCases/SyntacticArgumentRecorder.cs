namespace SharpAttributeParser.Tests.ASyntacticArgumentRecorderCases;

using Microsoft.CodeAnalysis;

using System.Collections.Generic;

internal sealed class SyntacticArgumentRecorder : ASyntacticArgumentRecorder
{
    public ITypeSymbol? TGeneric { get; private set; }
    public object? Value { get; private set; }
    public IReadOnlyList<object?>? Values { get; private set; }

    public Location? TGenericLocation { get; private set; }
    public Location? ValueLocation { get; private set; }
    public Location? ValuesCollectionLocation { get; private set; }
    public IReadOnlyList<Location>? ValuesElementLocations { get; private set; }

    protected override IEqualityComparer<string> Comparer { get; }

    public SyntacticArgumentRecorder(IEqualityComparer<string> comparer)
    {
        Comparer = comparer;
    }

    protected override IEnumerable<(string, DSyntacticGenericRecorder)> AddGenericRecorders()
    {
        yield return ("TGeneric", RecordTGeneric);
    }

    protected override IEnumerable<(string, DSyntacticSingleRecorder)> AddSingleRecorders()
    {
        yield return ("Value", RecordValue);
    }

    protected override IEnumerable<(string, DSyntacticArrayRecorder)> AddArrayRecorders()
    {
        yield return ("Values", RecordValues);
    }

    private bool RecordTGeneric(ITypeSymbol value, Location location)
    {
        TGeneric = value;
        TGenericLocation = location;

        return true;
    }

    private bool RecordValue(object? value, Location location)
    {
        Value = value;
        ValueLocation = location;

        return true;
    }

    private bool RecordValues(IReadOnlyList<object?>? values, Location collectionLocation, IReadOnlyList<Location> elementLocations)
    {
        Values = values;
        ValuesCollectionLocation = collectionLocation;
        ValuesElementLocations = elementLocations;

        return true;
    }
}
