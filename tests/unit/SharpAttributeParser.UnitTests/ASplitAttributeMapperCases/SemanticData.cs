namespace SharpAttributeParser.ASplitAttributeMapperCases;

using Microsoft.CodeAnalysis;

internal sealed class SemanticData
{
    public ITypeSymbol? T1 { get; set; }
    public bool T1Recorded { get; set; }

    public ITypeSymbol? T2 { get; set; }
    public bool T2Recorded { get; set; }

    public object? ValueA { get; set; }
    public bool ValueARecorded { get; set; }

    public object? ValueB { get; set; }
    public bool ValueBRecorded { get; set; }
}
