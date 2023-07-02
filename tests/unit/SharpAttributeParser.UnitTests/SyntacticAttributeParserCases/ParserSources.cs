namespace SharpAttributeParser.SyntacticAttributeParserCases;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used as test input.")]
internal sealed class ParserSources : ATestDataset<ISyntacticAttributeParser>
{
    protected override IEnumerable<ISyntacticAttributeParser> GetSamples() => new[]
    {
        DependencyInjection.GetRequiredService<ISyntacticAttributeParser>()
    };
}
