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
        var source = """
            using SharpAttributeParser.ExampleCases.MainReadmeCases;

            [Example<string>(System.StringComparison.Ordinal, "41", 42, 43, NamedArgument = typeof(double))]
            public class Foo { }
            """;

        var (compilation, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var parser = new SemanticParser();

        var mapper = new ExampleMapper();

        var recorderFactory = new SemanticRecorderFactory();
        var exampleRecorderFactory = new ExampleRecorderFactory(recorderFactory, mapper);

        var recorderA = ((IExampleRecorderFactory)exampleRecorderFactory).Create();
        var recorderB = ((IExampleRecorderFactory)exampleRecorderFactory).Create();

        var outcome = parser.TryParse(recorderA, attributeData);

        var dataRecordA = recorderA.GetRecord();

        var exceptionBuildRecordB = Record.Exception(recorderB.GetRecord);

        Assert.True(outcome);
        Assert.Equal(compilation.GetSpecialType(SpecialType.System_String), dataRecordA.TypeArgument);
        Assert.Equal(StringComparison.Ordinal, dataRecordA.ConstructorArgument);
        Assert.Equal("41", dataRecordA.OptionalArgument);
        Assert.Equal(new[] { 42, 43 }, dataRecordA.ParamsArgument);
        Assert.Equal(compilation.GetSpecialType(SpecialType.System_Double), dataRecordA.NamedArgument);

        Assert.IsType<InvalidOperationException>(exceptionBuildRecordB);
    }
}
