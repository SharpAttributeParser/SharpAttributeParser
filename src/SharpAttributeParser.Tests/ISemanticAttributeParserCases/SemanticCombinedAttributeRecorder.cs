namespace SharpAttributeParser.Tests.ISemanticAttributeParserCases;

using Microsoft.CodeAnalysis;

using System.Collections.Generic;

public sealed class SemanticCombinedAttributeRecorder : ASemanticArgumentRecorder
{
    public ITypeSymbol? T1 { get; private set; }
    public ITypeSymbol? T2 { get; private set; }

    public object? Value { get; private set; }
    public bool ValueRecorded { get; private set; }

    public IReadOnlyList<object?>? ArrayValues { get; private set; }
    public bool ArrayValuesRecorded { get; private set; }

    public IReadOnlyList<object?>? ParamsValues { get; private set; }
    public bool ParamsValuesRecorded { get; private set; }

    public object? NamedValue { get; private set; }
    public bool NamedValueRecorded { get; private set; }

    public IReadOnlyList<object?>? NamedValues { get; private set; }
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

    private bool RecordT1(ITypeSymbol value)
    {
        T1 = value;

        return true;
    }

    private bool RecordT2(ITypeSymbol value)
    {
        T2 = value;

        return true;
    }

    private bool RecordValue(object? value)
    {
        Value = value;
        ValueRecorded = true;

        return true;
    }

    private bool RecordArrayValues(IReadOnlyList<object?>? value)
    {
        ArrayValues = value;
        ArrayValuesRecorded = true;

        return true;
    }

    private bool RecordParamsValues(IReadOnlyList<object?>? value)
    {
        ParamsValues = value;
        ParamsValuesRecorded = true;

        return true;
    }

    private bool RecordNamedValue(object? value)
    {
        NamedValue = value;
        NamedValueRecorded = true;

        return true;
    }

    private bool RecordNamedValues(IReadOnlyList<object?>? value)
    {
        NamedValues = value;
        NamedValuesRecorded = true;

        return true;
    }
}
