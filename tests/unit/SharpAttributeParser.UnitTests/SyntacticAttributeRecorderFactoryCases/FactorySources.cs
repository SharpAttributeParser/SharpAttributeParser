namespace SharpAttributeParser.SyntacticAttributeRecorderFactoryCases;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used as test input.")]
internal sealed class FactorySources : ATestDataset<ISyntacticAttributeRecorderFactory>
{
    protected override IEnumerable<ISyntacticAttributeRecorderFactory> GetSamples() => new[]
    {
        DependencyInjection.GetRequiredService<ISyntacticAttributeRecorderFactory>()
    };
}
