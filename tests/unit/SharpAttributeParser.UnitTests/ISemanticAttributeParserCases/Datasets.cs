namespace SharpAttributeParser.Tests.ISemanticAttributeParserCases;

using Microsoft.CodeAnalysis;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

internal static class Datasets
{
    public static ISemanticArgumentRecorder GetNullRecorder() => null!;
    public static AttributeData GetNullAttributeData() => null!;

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used as test input.")]
    public sealed class ParserSources : ATestDataset<ISemanticAttributeParser>
    {
        protected override IEnumerable<ISemanticAttributeParser> GetSamples() => new[]
        {
            DependencyInjection.GetRequiredService<ISemanticAttributeParser>()
        };
    }

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used as test input.")]
    public sealed class GenericAttributeSources : ATestDataset<(ISemanticAttributeParser, SemanticGenericAttributeRecorder)>
    {
        protected override IEnumerable<(ISemanticAttributeParser, SemanticGenericAttributeRecorder)> GetSamples() => new (ISemanticAttributeParser, SemanticGenericAttributeRecorder)[]
        {
            (new SemanticAttributeParser(), new SemanticGenericAttributeRecorder())
        };
    }

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used as test input.")]
    public sealed class SingleConstructorAttributeSources : ATestDataset<(ISemanticAttributeParser, SemanticSingleConstructorAttributeRecorder)>
    {
        protected override IEnumerable<(ISemanticAttributeParser, SemanticSingleConstructorAttributeRecorder)> GetSamples() => new (ISemanticAttributeParser, SemanticSingleConstructorAttributeRecorder)[]
        {
            (new SemanticAttributeParser(), new SemanticSingleConstructorAttributeRecorder())
        };
    }

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used as test input.")]
    public sealed class ArrayConstructorAttributeSources : ATestDataset<(ISemanticAttributeParser, SemanticArrayConstructorAttributeRecorder)>
    {
        protected override IEnumerable<(ISemanticAttributeParser, SemanticArrayConstructorAttributeRecorder)> GetSamples() => new (ISemanticAttributeParser, SemanticArrayConstructorAttributeRecorder)[]
        {
            (new SemanticAttributeParser(), new SemanticArrayConstructorAttributeRecorder())
        };
    }

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used as test input.")]
    public sealed class ParamsAttributeSources : ATestDataset<(ISemanticAttributeParser, SemanticParamsAttributeRecorder)>
    {
        protected override IEnumerable<(ISemanticAttributeParser, SemanticParamsAttributeRecorder)> GetSamples() => new (ISemanticAttributeParser, SemanticParamsAttributeRecorder)[]
        {
            (new SemanticAttributeParser(), new SemanticParamsAttributeRecorder())
        };
    }

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used as test input.")]
    public sealed class NamedAttributeSources : ATestDataset<(ISemanticAttributeParser, SemanticNamedAttributeRecorder)>
    {
        protected override IEnumerable<(ISemanticAttributeParser, SemanticNamedAttributeRecorder)> GetSamples() => new (ISemanticAttributeParser, SemanticNamedAttributeRecorder)[]
        {
            (new SemanticAttributeParser(), new SemanticNamedAttributeRecorder())
        };
    }

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used as test input.")]
    public sealed class CombinedAttributeSources : ATestDataset<(ISemanticAttributeParser, SemanticCombinedAttributeRecorder)>
    {
        protected override IEnumerable<(ISemanticAttributeParser, SemanticCombinedAttributeRecorder)> GetSamples() => new (ISemanticAttributeParser, SemanticCombinedAttributeRecorder)[]
        {
            (new SemanticAttributeParser(), new SemanticCombinedAttributeRecorder())
        };
    }
}
