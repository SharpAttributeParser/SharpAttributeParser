namespace SharpAttributeParser.Tests.ISyntacticAttributeParserCases;

using Microsoft.CodeAnalysis;

using System.Collections.Generic;

public sealed class SyntacticCombinedAttributeRecorder : ASyntacticArgumentRecorder
{
    public ITypeSymbol? T1 { get; private set; }
    public Location? T1Location { get; private set; }

    public ITypeSymbol? T2 { get; private set; }
    public Location? T2Location { get; private set; }

    public object? Value { get; private set; }
    public Location? ValueLocation { get; private set; }
    public bool ValueRecorded { get; private set; }

    public IReadOnlyList<object?>? ArrayValues { get; private set; }
    public Location? ArrayValuesCollectionLocation { get; private set; }
    public IReadOnlyList<Location>? ArrayValuesElementLocations { get; private set; }
    public bool ArrayValuesRecorded { get; private set; }

    public IReadOnlyList<object?>? ParamsValues { get; private set; }
    public Location? ParamsValuesCollectionLocation { get; private set; }
    public IReadOnlyList<Location>? ParamsValuesElementLocations { get; private set; }
    public bool ParamsValuesRecorded { get; private set; }

    public object? NamedValue { get; private set; }
    public Location? NamedValueLocation { get; private set; }
    public bool NamedValueRecorded { get; private set; }

    public IReadOnlyList<object?>? NamedValues { get; private set; }
    public Location? NamedValuesCollectionLocation { get; private set; }
    public IReadOnlyList<Location>? NamedValuesElementLocations { get; private set; }
    public bool NamedValuesRecorded { get; private set; }

    protected override IEnumerable<(string, DGenericRecorder)> AddGenericRecorders()
    {
        yield return ("T1", RecordT1);
        yield return ("T2", RecordT2);
    }

    protected override IEnumerable<(string, DSingleRecorder)> AddSingleRecorders()
    {
        yield return ("Value", RecordValue);
        yield return ("NamedValue", RecordNamedValue);
    }

    protected override IEnumerable<(string, DArrayRecorder)> AddArrayRecorders()
    {
        yield return ("ArrayValues", RecordArrayValues);
        yield return ("ParamsValues", RecordParamsValues);
        yield return ("NamedValues", RecordNamedValues);
    }

    private bool RecordT1(ITypeSymbol value, Location location)
    {
        T1 = value;
        T1Location = location;

        return true;
    }

    private bool RecordT2(ITypeSymbol value, Location location)
    {
        T2 = value;
        T2Location = location;

        return true;
    }

    private bool RecordValue(object? value, Location location)
    {
        Value = value;
        ValueLocation = location;
        ValueRecorded = true;

        return true;
    }

    private bool RecordArrayValues(IReadOnlyList<object?>? value, Location collectionLocation, IReadOnlyList<Location> elementLocations)
    {
        ArrayValues = value;
        ArrayValuesCollectionLocation = collectionLocation;
        ArrayValuesElementLocations = elementLocations;
        ArrayValuesRecorded = true;

        return true;
    }

    private bool RecordParamsValues(IReadOnlyList<object?>? value, Location collectionLocation, IReadOnlyList<Location> elementLocations)
    {
        ParamsValues = value;
        ParamsValuesCollectionLocation = collectionLocation;
        ParamsValuesElementLocations = elementLocations;
        ParamsValuesRecorded = true;

        return true;
    }

    private bool RecordNamedValue(object? value, Location location)
    {
        NamedValue = value;
        NamedValueLocation = location;
        NamedValueRecorded = true;

        return true;
    }

    private bool RecordNamedValues(IReadOnlyList<object?>? value, Location collectionLocation, IReadOnlyList<Location> elementLocations)
    {
        NamedValues = value;
        NamedValuesCollectionLocation = collectionLocation;
        NamedValuesElementLocations = elementLocations;
        NamedValuesRecorded = true;

        return true;
    }
}
