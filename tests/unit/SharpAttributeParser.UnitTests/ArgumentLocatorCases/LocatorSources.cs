namespace SharpAttributeParser.ArgumentLocatorCases;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used as test input.")]
internal sealed class LocatorSources : ATestDataset<IArgumentLocator>
{
    protected override IEnumerable<IArgumentLocator> GetSamples() => new[]
    {
        DependencyInjection.GetRequiredService<IArgumentLocator>()
    };
}
