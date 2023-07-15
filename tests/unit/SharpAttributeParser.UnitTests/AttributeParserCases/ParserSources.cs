namespace SharpAttributeParser.AttributeParserCases;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used as test input.")]
internal sealed class ParserSources : ATestDataset<IAttributeParser>
{
    protected override IEnumerable<IAttributeParser> GetSamples() => new[]
    {
        DependencyInjection.GetRequiredService<IAttributeParser>()
    };
}
