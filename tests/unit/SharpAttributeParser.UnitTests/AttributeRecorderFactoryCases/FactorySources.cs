namespace SharpAttributeParser.AttributeRecorderFactoryCases;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used as test input.")]
internal sealed class FactorySources : ATestDataset<IAttributeRecorderFactory>
{
    protected override IEnumerable<IAttributeRecorderFactory> GetSamples() => new[]
    {
        DependencyInjection.GetRequiredService<IAttributeRecorderFactory>()
    };
}
