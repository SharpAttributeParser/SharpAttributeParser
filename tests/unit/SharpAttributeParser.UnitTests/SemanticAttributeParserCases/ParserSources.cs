namespace SharpAttributeParser.SemanticAttributeParserCases;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used as test input.")]
internal sealed class ParserSources : ATestDataset<ISemanticAttributeParser>
{
    protected override IEnumerable<ISemanticAttributeParser> GetSamples() => new[]
    {
        DependencyInjection.GetRequiredService<ISemanticAttributeParser>()
    };
}
