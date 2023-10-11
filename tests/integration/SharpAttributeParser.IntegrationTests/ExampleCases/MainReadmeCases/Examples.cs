namespace SharpAttributeParser.ExampleCases.MainReadmeCases;

using Microsoft.CodeAnalysis;

using SharpAttributeParser.Mappers;

using System;
using System.Threading.Tasks;

using Xunit;

public sealed class Examples
{
    private readonly ISemanticParser Parser;
    private readonly ISemanticRecorderFactory RecorderFactory;

    public Examples(ISemanticParser parser, ISemanticRecorderFactory recorderFactory)
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

        var recorder = RecorderFactory.Create(new ExampleMapper(), new ExampleRecord());

        var success = Parser.TryParse(recorder, attributeData);

        var dataRecord = recorder.GetRecord();

        Assert.True(success);

        Assert.Equal(compilation.GetSpecialType(SpecialType.System_String), dataRecord.TypeArgument);
        Assert.Equal(StringComparison.Ordinal, dataRecord.ConstructorArgument);
        Assert.Equal("41", dataRecord.OptionalArgument);
        Assert.Equal(new[] { 42, 43 }, dataRecord.ParamsArgument);
        Assert.Equal(compilation.GetSpecialType(SpecialType.System_Double), dataRecord.NamedArgument);
    }
}
