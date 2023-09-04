namespace SharpAttributeParser.ExampleCases.RecommendedPatternCases.NonDependencyInjectionCases;

using Microsoft.CodeAnalysis;

using SharpAttributeParser.Mappers;

using System;
using System.Threading.Tasks;

using Xunit;

public sealed class Examples
{
    [Fact]
    public async Task CreatedRecordActuallyContainsArguments()
    {
        var sourceA = """
            using SharpAttributeParser.ExampleCases.RecommendedPatternCases;

            [Example<string>(System.StringComparison.Ordinal, "41", 42, 43, NamedArgument = typeof(double))]
            public class Foo { }
            """;

        var sourceB = """
            using SharpAttributeParser.ExampleCases.RecommendedPatternCases;

            [Example<int>(System.StringComparison.Ordinal, "41", 42, 43, NamedArgument = typeof(double))]
            public class Foo { }
            """;

        var (compilationA, attributeDataA, _) = await CompilationStore.GetComponents(sourceA, "Foo");
        var (compilationB, attributeDataB, _) = await CompilationStore.GetComponents(sourceB, "Foo");

        var parser = new SemanticParser();

        var mapper = new ExampleMapper();

        var recorderFactory = new SemanticRecorderFactory();
        var exampleRecorderFactory = new ExampleRecorderFactory(recorderFactory, mapper);
        var exampleParser = new ExampleParser(parser, exampleRecorderFactory);

        var dataRecordA = ((IExampleParser)exampleParser).TryParse(attributeDataA);
        var dataRecordB = ((IExampleParser)exampleParser).TryParse(attributeDataB);

        Assert.NotNull(dataRecordA);
        Assert.Equal(compilationA.GetSpecialType(SpecialType.System_String), dataRecordA.TypeArgument);
        Assert.Equal(StringComparison.Ordinal, dataRecordA.ConstructorArgument);
        Assert.Equal("41", dataRecordA.OptionalArgument);
        Assert.Equal(new[] { 42, 43 }, dataRecordA.ParamsArgument);
        Assert.Equal(compilationA.GetSpecialType(SpecialType.System_Double), dataRecordA.NamedArgument);

        Assert.NotNull(dataRecordB);
        Assert.Equal(compilationB.GetSpecialType(SpecialType.System_Int32), dataRecordB.TypeArgument);
    }
}
