namespace SharpAttributeParser.ExampleCases.RecommendedPatternCases.DependencyInjectionCases;

using Microsoft.CodeAnalysis;

using System;
using System.Threading.Tasks;

using Xunit;

public sealed class Examples
{
    private ISemanticParser Parser { get; }
    private IExampleRecorderFactory RecorderFactory { get; }

    public Examples(ISemanticParser parser, IExampleRecorderFactory recorderFactory)
    {
        Parser = parser;
        RecorderFactory = recorderFactory;
    }

    [Fact]
    public async Task CreatedRecordActuallyContainsArguments()
    {
        var source = """
            using SharpAttributeParser.ExampleCases.MainReadmeCases;

            [Example<string>(System.StringComparison.Ordinal, "41", 42, 43, NamedArgument = typeof(double))]
            public class Foo { }
            """;

        var (compilation, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var recorderA = RecorderFactory.Create();
        var recorderB = RecorderFactory.Create();

        var outcome = Parser.TryParse(recorderA, attributeData);

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
