namespace SharpAttributeParser.Tests.ISyntacticAttributeParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.Threading.Tasks;

using Xunit;

public class TryParse_Combined
{
    private static bool Target(ISyntacticAttributeParser parser, ISyntacticArgumentRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => parser.TryParse(recorder, attributeData, attributeSyntax);

    [Theory]
    [ClassData(typeof(Datasets.CombinedAttributeSources))]
    public async Task Combined_Params_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticCombinedAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueLocation);

        Assert.Null(recorder.ArrayValues);
        Assert.Null(recorder.ArrayValuesCollectionLocation);
        Assert.Null(recorder.ArrayValuesElementLocations);

        Assert.Null(recorder.ParamsValues);
        Assert.Null(recorder.ParamsValuesCollectionLocation);
        Assert.Null(recorder.ParamsValuesElementLocations);

        Assert.Null(recorder.NamedValue);
        Assert.Null(recorder.NamedValueLocation);

        Assert.Null(recorder.NamedValues);
        Assert.Null(recorder.NamedValuesCollectionLocation);
        Assert.Null(recorder.NamedValuesElementLocations);

        var source = """
            [Combined<string, int>("42", new object[0], "42", 42, NamedValue = typeof(int), NamedValues = new object[] { 42, "42" })]
            public class Foo { }
            """;

        var (compilation, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);

        var expectedT1Location = ExpectedLocation.TypeArgument(attributeSyntax, 0);
        var expectedT2Location = ExpectedLocation.TypeArgument(attributeSyntax, 1);
        var expectedValueLocation = ExpectedLocation.SingleArgument(attributeSyntax, 0);
        var (expectedArrayValuesCollectionLocation, expectedArrayValuesElementLocations) = ExpectedLocation.ArrayArgument(attributeSyntax, 1);
        var (expectedParamsValuesCollectionLocation, expectedParamsValuesElementLocations) = ExpectedLocation.ParamsArgument(attributeSyntax, 2, 2);
        var expectedNamedValueLocation = ExpectedLocation.SingleArgument(attributeSyntax, 4);
        var (expectedNamedValuesCollectionLocation, expectedNamedValuesElementLocations) = ExpectedLocation.ArrayArgument(attributeSyntax, 5);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Equal(stringType, recorder.T1);
        Assert.Equal(intType, recorder.T2);
        Assert.Equal("42", recorder.Value!);
        Assert.Empty(recorder.ArrayValues!);
        Assert.Equal(new object[] { "42", 42 }, recorder.ParamsValues);
        Assert.Equal(intType, recorder.NamedValue);
        Assert.Equal(new object[] { 42, "42" }, recorder.NamedValues);

        Assert.Equal(expectedT1Location, recorder.T1Location);
        Assert.Equal(expectedT2Location, recorder.T2Location);
        Assert.Equal(expectedValueLocation, recorder.ValueLocation);
        Assert.Equal(expectedArrayValuesCollectionLocation, recorder.ArrayValuesCollectionLocation);
        Assert.Equal(expectedArrayValuesElementLocations, recorder.ArrayValuesElementLocations);
        Assert.Equal(expectedParamsValuesCollectionLocation, recorder.ParamsValuesCollectionLocation);
        Assert.Equal(expectedParamsValuesElementLocations, recorder.ParamsValuesElementLocations);
        Assert.Equal(expectedNamedValueLocation, recorder.NamedValueLocation);
        Assert.Equal(expectedNamedValuesCollectionLocation, recorder.NamedValuesCollectionLocation);
        Assert.Equal(expectedNamedValuesElementLocations, recorder.NamedValuesElementLocations);
    }

    [Theory]
    [ClassData(typeof(Datasets.CombinedAttributeSources))]
    public async Task Combined_EmptyParams_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticCombinedAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueLocation);

        Assert.Null(recorder.ArrayValues);
        Assert.Null(recorder.ArrayValuesCollectionLocation);
        Assert.Null(recorder.ArrayValuesElementLocations);

        Assert.Null(recorder.ParamsValues);
        Assert.Null(recorder.ParamsValuesCollectionLocation);
        Assert.Null(recorder.ParamsValuesElementLocations);

        Assert.Null(recorder.NamedValue);
        Assert.Null(recorder.NamedValueLocation);

        Assert.Null(recorder.NamedValues);
        Assert.Null(recorder.NamedValuesCollectionLocation);
        Assert.Null(recorder.NamedValuesElementLocations);

        var source = """
            [Combined<string, int>("42", new object[0], NamedValue = typeof(int), NamedValues = new object[] { 42, "42" })]
            public class Foo { }
            """;

        var (compilation, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);

        var expectedT1Location = ExpectedLocation.TypeArgument(attributeSyntax, 0);
        var expectedT2Location = ExpectedLocation.TypeArgument(attributeSyntax, 1);
        var expectedValueLocation = ExpectedLocation.SingleArgument(attributeSyntax, 0);
        var (expectedArrayValuesCollectionLocation, expectedArrayValuesElementLocations) = ExpectedLocation.ArrayArgument(attributeSyntax, 1);
        var expectedNamedValueLocation = ExpectedLocation.SingleArgument(attributeSyntax, 2);
        var (expectedNamedValuesCollectionLocation, expectedNamedValuesElementLocations) = ExpectedLocation.ArrayArgument(attributeSyntax, 3);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Equal(stringType, recorder.T1);
        Assert.Equal(intType, recorder.T2);
        Assert.Equal("42", recorder.Value!);
        Assert.Empty(recorder.ArrayValues!);
        Assert.Empty(recorder.ParamsValues!);
        Assert.Equal(intType, recorder.NamedValue);
        Assert.Equal(new object[] { 42, "42" }, recorder.NamedValues);

        Assert.Equal(expectedT1Location, recorder.T1Location);
        Assert.Equal(expectedT2Location, recorder.T2Location);
        Assert.Equal(expectedValueLocation, recorder.ValueLocation);
        Assert.Equal(expectedArrayValuesCollectionLocation, recorder.ArrayValuesCollectionLocation);
        Assert.Equal(expectedArrayValuesElementLocations, recorder.ArrayValuesElementLocations);
        Assert.Equal(Location.None, recorder.ParamsValuesCollectionLocation);
        Assert.Empty(recorder.ParamsValuesElementLocations!);
        Assert.Equal(expectedNamedValueLocation, recorder.NamedValueLocation);
        Assert.Equal(expectedNamedValuesCollectionLocation, recorder.NamedValuesCollectionLocation);
        Assert.Equal(expectedNamedValuesElementLocations, recorder.NamedValuesElementLocations);
    }

    [Theory]
    [ClassData(typeof(Datasets.CombinedAttributeSources))]
    public async Task Combined_OneElementParams_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticCombinedAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueLocation);

        Assert.Null(recorder.ArrayValues);
        Assert.Null(recorder.ArrayValuesCollectionLocation);
        Assert.Null(recorder.ArrayValuesElementLocations);

        Assert.Null(recorder.ParamsValues);
        Assert.Null(recorder.ParamsValuesCollectionLocation);
        Assert.Null(recorder.ParamsValuesElementLocations);

        Assert.Null(recorder.NamedValue);
        Assert.Null(recorder.NamedValueLocation);

        Assert.Null(recorder.NamedValues);
        Assert.Null(recorder.NamedValuesCollectionLocation);
        Assert.Null(recorder.NamedValuesElementLocations);

        var source = """
            [Combined<string, int>("42", new object[0], 42, NamedValue = typeof(int), NamedValues = new object[] { 42, "42" })]
            public class Foo { }
            """;

        var (compilation, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);

        var expectedT1Location = ExpectedLocation.TypeArgument(attributeSyntax, 0);
        var expectedT2Location = ExpectedLocation.TypeArgument(attributeSyntax, 1);
        var expectedValueLocation = ExpectedLocation.SingleArgument(attributeSyntax, 0);
        var (expectedArrayValuesCollectionLocation, expectedArrayValuesElementLocations) = ExpectedLocation.ArrayArgument(attributeSyntax, 1);
        var (expectedParamsValuesCollectionLocation, expectedParamsValuesElementLocations) = ExpectedLocation.ParamsArgument(attributeSyntax, 2, 1);
        var expectedNamedValueLocation = ExpectedLocation.SingleArgument(attributeSyntax, 3);
        var (expectedNamedValuesCollectionLocation, expectedNamedValuesElementLocations) = ExpectedLocation.ArrayArgument(attributeSyntax, 4);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Equal(stringType, recorder.T1);
        Assert.Equal(intType, recorder.T2);
        Assert.Equal("42", recorder.Value!);
        Assert.Empty(recorder.ArrayValues!);
        Assert.Equal(new object[] { 42 }, recorder.ParamsValues);
        Assert.Equal(intType, recorder.NamedValue);
        Assert.Equal(new object[] { 42, "42" }, recorder.NamedValues);

        Assert.Equal(expectedT1Location, recorder.T1Location);
        Assert.Equal(expectedT2Location, recorder.T2Location);
        Assert.Equal(expectedValueLocation, recorder.ValueLocation);
        Assert.Equal(expectedArrayValuesCollectionLocation, recorder.ArrayValuesCollectionLocation);
        Assert.Equal(expectedArrayValuesElementLocations, recorder.ArrayValuesElementLocations);
        Assert.Equal(expectedParamsValuesCollectionLocation, recorder.ParamsValuesCollectionLocation);
        Assert.Equal(expectedParamsValuesElementLocations, recorder.ParamsValuesElementLocations!);
        Assert.Equal(expectedNamedValueLocation, recorder.NamedValueLocation);
        Assert.Equal(expectedNamedValuesCollectionLocation, recorder.NamedValuesCollectionLocation);
        Assert.Equal(expectedNamedValuesElementLocations, recorder.NamedValuesElementLocations);
    }

    [Theory]
    [ClassData(typeof(Datasets.CombinedAttributeSources))]
    public async Task Combined_Array_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticCombinedAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueLocation);

        Assert.Null(recorder.ArrayValues);
        Assert.Null(recorder.ArrayValuesCollectionLocation);
        Assert.Null(recorder.ArrayValuesElementLocations);

        Assert.Null(recorder.ParamsValues);
        Assert.Null(recorder.ParamsValuesCollectionLocation);
        Assert.Null(recorder.ParamsValuesElementLocations);

        Assert.Null(recorder.NamedValue);
        Assert.Null(recorder.NamedValueLocation);

        Assert.Null(recorder.NamedValues);
        Assert.Null(recorder.NamedValuesCollectionLocation);
        Assert.Null(recorder.NamedValuesElementLocations);

        var source = """
            [Combined<string, int>("42", new object[0], new object[] { "42", 42 }, NamedValue = typeof(int), NamedValues = new object[] { 42, "42" })]
            public class Foo { }
            """;

        var (compilation, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);

        var expectedT1Location = ExpectedLocation.TypeArgument(attributeSyntax, 0);
        var expectedT2Location = ExpectedLocation.TypeArgument(attributeSyntax, 1);
        var expectedValueLocation = ExpectedLocation.SingleArgument(attributeSyntax, 0);
        var (expectedArrayValuesCollectionLocation, expectedArrayValuesElementLocations) = ExpectedLocation.ArrayArgument(attributeSyntax, 1);
        var (expectedParamsValuesCollectionLocation, expectedParamsValuesElementLocations) = ExpectedLocation.ArrayArgument(attributeSyntax, 2);
        var expectedNamedValueLocation = ExpectedLocation.SingleArgument(attributeSyntax, 3);
        var (expectedNamedValuesCollectionLocation, expectedNamedValuesElementLocations) = ExpectedLocation.ArrayArgument(attributeSyntax, 4);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Equal(stringType, recorder.T1);
        Assert.Equal(intType, recorder.T2);
        Assert.Equal("42", recorder.Value!);
        Assert.Empty(recorder.ArrayValues!);
        Assert.Equal(new object[] { "42", 42 }, recorder.ParamsValues);
        Assert.Equal(intType, recorder.NamedValue);
        Assert.Equal(new object[] { 42, "42" }, recorder.NamedValues);

        Assert.Equal(expectedT1Location, recorder.T1Location);
        Assert.Equal(expectedT2Location, recorder.T2Location);
        Assert.Equal(expectedValueLocation, recorder.ValueLocation);
        Assert.Equal(expectedArrayValuesCollectionLocation, recorder.ArrayValuesCollectionLocation);
        Assert.Equal(expectedArrayValuesElementLocations, recorder.ArrayValuesElementLocations);
        Assert.Equal(expectedParamsValuesCollectionLocation, recorder.ParamsValuesCollectionLocation);
        Assert.Equal(expectedParamsValuesElementLocations, recorder.ParamsValuesElementLocations);
        Assert.Equal(expectedNamedValueLocation, recorder.NamedValueLocation);
        Assert.Equal(expectedNamedValuesCollectionLocation, recorder.NamedValuesCollectionLocation);
        Assert.Equal(expectedNamedValuesElementLocations, recorder.NamedValuesElementLocations);
    }
}
