namespace SharpAttributeParser.Tests.ISyntacticAttributeParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

internal static class Datasets
{
    public static ISyntacticArgumentRecorder GetNullRecorder() => null!;
    public static AttributeData GetNullAttributeData() => null!;
    public static AttributeSyntax GetNullAttributeSyntax() => null!;

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used as test input.")]
    public sealed class ParserSources : ATestDataset<ISyntacticAttributeParser>
    {
        protected override IEnumerable<ISyntacticAttributeParser> GetSamples() => new[]
        {
            DependencyInjection.GetRequiredService<ISyntacticAttributeParser>()
        };
    }

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used as test input.")]
    public sealed class GenericAttributeSources : ATestDataset<(ISyntacticAttributeParser, SyntacticGenericAttributeRecorder)>
    {
        protected override IEnumerable<(ISyntacticAttributeParser, SyntacticGenericAttributeRecorder)> GetSamples() => new (ISyntacticAttributeParser, SyntacticGenericAttributeRecorder)[]
        {
            (new SyntacticAttributeParser(new ArgumentLocator()), new SyntacticGenericAttributeRecorder())
        };
    }

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used as test input.")]
    public sealed class SingleConstructorAttributeSources : ATestDataset<(ISyntacticAttributeParser, SyntacticSingleConstructorAttributeRecorder)>
    {
        protected override IEnumerable<(ISyntacticAttributeParser, SyntacticSingleConstructorAttributeRecorder)> GetSamples() => new (ISyntacticAttributeParser, SyntacticSingleConstructorAttributeRecorder)[]
        {
            (new SyntacticAttributeParser(new ArgumentLocator()), new SyntacticSingleConstructorAttributeRecorder())
        };
    }

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used as test input.")]
    public sealed class ArrayConstructorAttributeSources : ATestDataset<(ISyntacticAttributeParser, SyntacticArrayConstructorAttributeRecorder)>
    {
        protected override IEnumerable<(ISyntacticAttributeParser, SyntacticArrayConstructorAttributeRecorder)> GetSamples() => new (ISyntacticAttributeParser, SyntacticArrayConstructorAttributeRecorder)[]
        {
            (new SyntacticAttributeParser(new ArgumentLocator()), new SyntacticArrayConstructorAttributeRecorder())
        };
    }

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used as test input.")]
    public sealed class NamedAttributeSources : ATestDataset<(ISyntacticAttributeParser, SyntacticNamedAttributeRecorder)>
    {
        protected override IEnumerable<(ISyntacticAttributeParser, SyntacticNamedAttributeRecorder)> GetSamples() => new (ISyntacticAttributeParser, SyntacticNamedAttributeRecorder)[]
        {
            (new SyntacticAttributeParser(new ArgumentLocator()), new SyntacticNamedAttributeRecorder())
        };
    }

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used as test input.")]
    public sealed class CombinedAttributeSources : ATestDataset<(ISyntacticAttributeParser, SyntacticCombinedAttributeRecorder)>
    {
        protected override IEnumerable<(ISyntacticAttributeParser, SyntacticCombinedAttributeRecorder)> GetSamples() => new (ISyntacticAttributeParser, SyntacticCombinedAttributeRecorder)[]
        {
            (new SyntacticAttributeParser(new ArgumentLocator()), new SyntacticCombinedAttributeRecorder())
        };
    }
}
