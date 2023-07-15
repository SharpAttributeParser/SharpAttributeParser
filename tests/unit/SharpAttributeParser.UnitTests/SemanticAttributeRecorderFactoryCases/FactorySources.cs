namespace SharpAttributeParser.SemanticAttributeRecorderFactoryCases;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used as test input.")]
internal sealed class FactorySources : ATestDataset<ISemanticAttributeRecorderFactory>
{
    protected override IEnumerable<ISemanticAttributeRecorderFactory> GetSamples() => new[]
    {
        DependencyInjection.GetRequiredService<ISemanticAttributeRecorderFactory>()
    };
}
