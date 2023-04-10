namespace SharpAttributeParser.Tests.ISyntacticAttributeParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Xunit;

public class TryParse
{
    private static bool Target(ISyntacticAttributeParser parser, ISyntacticArgumentRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => parser.TryParse(recorder, attributeData, attributeSyntax);

    [Theory]
    [ClassData(typeof(Datasets.ParserSources))]
    public async Task NullRecorder_ArgumentNullException(ISyntacticAttributeParser parser)
    {
        var source = """
            [Generic<string>]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var recorder = Datasets.GetNullRecorder();

        var exception = Record.Exception(() => Target(parser, recorder, attributeData, attributeSyntax));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(Datasets.GenericAttributeSources))]
    public async Task NullAttributeData_ArgumentNullException(ISyntacticAttributeParser parser, ISyntacticArgumentRecorder recorder)
    {
        var source = """
            [Generic<string>]
            public class Foo { }
            """;

        var (_, _, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");
        var attributeData = Datasets.GetNullAttributeData();

        var exception = Record.Exception(() => Target(parser, recorder, attributeData, attributeSyntax));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(Datasets.GenericAttributeSources))]
    public async Task NullAttributeSyntax_ArgumentNullException(ISyntacticAttributeParser parser, ISyntacticArgumentRecorder recorder)
    {
        var source = """
            [Generic<string>]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");
        var attributeSyntax = Datasets.GetNullAttributeSyntax();

        var exception = Record.Exception(() => Target(parser, recorder, attributeData, attributeSyntax));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(Datasets.GenericAttributeSources))]
    public async Task NonExistingAttribute_False_RecorderNotPopulated(ISyntacticAttributeParser parser, SyntacticGenericAttributeRecorder recorder)
    {
        var source = """
            [NonExisting]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.False(result);

        Assert.Null(recorder.T);
        Assert.Null(recorder.TLocation);
    }

    [Theory]
    [ClassData(typeof(Datasets.GenericAttributeSources))]
    public async Task NonExistingConstructor_False_RecorderNotPopulated(ISyntacticAttributeParser parser, SyntacticGenericAttributeRecorder recorder)
    {
        var source = """
            [Generic<string>(4)]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.False(result);

        Assert.Null(recorder.T);
        Assert.Null(recorder.TLocation);
    }

    [Theory]
    [ClassData(typeof(Datasets.SingleConstructorAttributeSources))]
    public async Task SyntacticalError_False_RecorderNotPopulated(ISyntacticAttributeParser parser, SyntacticSingleConstructorAttributeRecorder recorder)
    {
        var source = """
            [SingleConstructor(4?)]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.False(result);

        Assert.Null(recorder.Value);
    }

    [Theory]
    [ClassData(typeof(Datasets.ParserSources))]
    [SuppressMessage("Minor Code Smell", "S1125: Boolean literals should not be redundant", Justification = "Prefer equality check over negation.")]
    public async Task Generic_FalseReturningRecorder_False(ISyntacticAttributeParser parser)
    {
        var recorder = Mock.Of<ISyntacticArgumentRecorder>(static (recorder) => recorder.TryRecordGenericArgument(It.IsAny<ITypeParameterSymbol>(), It.IsAny<ITypeSymbol>(), It.IsAny<Location>()) == false);

        var source = """
            [Generic<string>]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.False(result);
    }

    [Theory]
    [ClassData(typeof(Datasets.GenericAttributeSources))]
    public async Task Generic_WithoutBody_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticGenericAttributeRecorder recorder)
    {
        Assert.Null(recorder.T);
        Assert.Null(recorder.TLocation);

        var source = """
            [Generic<string>]
            public class Foo { }
            """;

        var (compilation, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var expectedType = compilation.GetSpecialType(SpecialType.System_String);
        var expectedLocation = DependencyInjection.GetRequiredService<IArgumentLocator>().TypeArgument(((GenericNameSyntax)attributeSyntax.Name).TypeArgumentList.Arguments[0]);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Equal(expectedType, recorder.T);
        Assert.Equal(expectedLocation, recorder.TLocation);
    }

    [Theory]
    [ClassData(typeof(Datasets.GenericAttributeSources))]
    public async Task Generic_WithBody_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticGenericAttributeRecorder recorder)
    {
        Assert.Null(recorder.T);
        Assert.Null(recorder.TLocation);

        var source = """
            [Generic<string>()]
            public class Foo { }
            """;

        var (compilation, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var expectedType = compilation.GetSpecialType(SpecialType.System_String);
        var expectedLocation = DependencyInjection.GetRequiredService<IArgumentLocator>().TypeArgument(((GenericNameSyntax)attributeSyntax.Name).TypeArgumentList.Arguments[0]);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Equal(expectedType, recorder.T);
        Assert.Equal(expectedLocation, recorder.TLocation);
    }

    [Theory]
    [ClassData(typeof(Datasets.GenericAttributeSources))]
    public async Task Generic_Tuple_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticGenericAttributeRecorder recorder)
    {
        Assert.Null(recorder.T);
        Assert.Null(recorder.TLocation);

        var source = """
            [Generic<(string, int)>]
            public class Foo { }
            """;

        var (compilation, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);

        var expectedType = compilation.GetTypeByMetadataName("System.ValueTuple`2")!.Construct(stringType, intType);
        var expectedLocation = DependencyInjection.GetRequiredService<IArgumentLocator>().TypeArgument(((GenericNameSyntax)attributeSyntax.Name).TypeArgumentList.Arguments[0]);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Equal(expectedType, recorder.T);
        Assert.Equal(expectedLocation, recorder.TLocation);
    }

    [Theory]
    [ClassData(typeof(Datasets.ParserSources))]
    [SuppressMessage("Minor Code Smell", "S1125: Boolean literals should not be redundant", Justification = "Prefer equality check over negation.")]
    public async Task ConstructorSingle_FalseReturningRecorder_False(ISyntacticAttributeParser parser)
    {
        var recorder = Mock.Of<ISyntacticArgumentRecorder>(static (recorder) => recorder.TryRecordConstructorArgument(It.IsAny<IParameterSymbol>(), It.IsAny<object?>(), It.IsAny<Location>()) == false);

        var source = """
            [SingleConstructor(null)]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.False(result);
    }

    [Theory]
    [ClassData(typeof(Datasets.SingleConstructorAttributeSources))]
    public async Task ConstructorSingle_NullLiteral_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticSingleConstructorAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueLocation);

        var source = """
            [SingleConstructor(null)]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var expectedLocation = DependencyInjection.GetRequiredService<IArgumentLocator>().SingleArgument(attributeSyntax.ArgumentList!.Arguments[0].Expression);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Null(recorder.Value);
        Assert.Equal(expectedLocation, recorder.ValueLocation);
    }

    [Theory]
    [ClassData(typeof(Datasets.SingleConstructorAttributeSources))]
    public async Task ConstructorSingle_Value_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticSingleConstructorAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueLocation);

        var source = """
            [SingleConstructor("42")]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var expectedLocation = DependencyInjection.GetRequiredService<IArgumentLocator>().SingleArgument(attributeSyntax.ArgumentList!.Arguments[0].Expression);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Equal("42", recorder.Value);
        Assert.Equal(expectedLocation, recorder.ValueLocation);
    }

    [Theory]
    [ClassData(typeof(Datasets.SingleConstructorAttributeSources))]
    public async Task ConstructorSingle_Labelled_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticSingleConstructorAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueLocation);

        var source = """
            [SingleConstructor(value: "42")]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var expectedLocation = DependencyInjection.GetRequiredService<IArgumentLocator>().SingleArgument(attributeSyntax.ArgumentList!.Arguments[0].Expression);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Equal("42", recorder.Value);
        Assert.Equal(expectedLocation, recorder.ValueLocation);
    }

    [Theory]
    [ClassData(typeof(Datasets.ParserSources))]
    [SuppressMessage("Minor Code Smell", "S1125: Boolean literals should not be redundant", Justification = "Prefer equality check over negation.")]
    public async Task ConstructorArray_FalseReturningRecorder_False(ISyntacticAttributeParser parser)
    {
        var recorder = Mock.Of<ISyntacticArgumentRecorder>(static (recorder) => recorder.TryRecordConstructorArgument(It.IsAny<IParameterSymbol>(), It.IsAny<IReadOnlyList<object?>?>(), It.IsAny<Location>(), It.IsAny<IReadOnlyList<Location>>()) == false);

        var source = """
            [ArrayConstructor(null)]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.False(result);
    }

    [Theory]
    [ClassData(typeof(Datasets.ArrayConstructorAttributeSources))]
    public async Task ConstructorArray_NullLiteral_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticArrayConstructorAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueCollectionLocation);
        Assert.Null(recorder.ValueElementLocations);

        var source = """
            [ArrayConstructor(null)]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var (expectedCollectionLocation, expectedElementLocations) = DependencyInjection.GetRequiredService<IArgumentLocator>().ArrayArgument(attributeSyntax.ArgumentList!.Arguments[0].Expression);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Null(recorder.Value);
        Assert.Equal(expectedCollectionLocation, recorder.ValueCollectionLocation);
        Assert.Equal(expectedElementLocations, recorder.ValueElementLocations);
    }

    [Theory]
    [ClassData(typeof(Datasets.ArrayConstructorAttributeSources))]
    public async Task ConstructorArray_Values_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticArrayConstructorAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueCollectionLocation);
        Assert.Null(recorder.ValueElementLocations);

        var source = """
            [ArrayConstructor(new object[] { "42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 } })]
            public class Foo { }
            """;

        var (compilation, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var intType = compilation.GetSpecialType(SpecialType.System_Int32);

        var (expectedCollectionLocation, expectedElementLocations) = DependencyInjection.GetRequiredService<IArgumentLocator>().ArrayArgument(attributeSyntax.ArgumentList!.Arguments[0].Expression);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Equal(7, recorder.Value!.Count);
        Assert.Equal("42", recorder.Value[0]);
        Assert.Null(recorder.Value[1]);
        Assert.Equal(intType, recorder.Value[2]);
        Assert.Equal("Foo", recorder.Value[3]);
        Assert.Equal(42, recorder.Value[4]);
        Assert.Equal((double)42, recorder.Value[5]);
        Assert.Equal(new object[] { "42", 42 }, (object?[])recorder.Value[6]!);

        Assert.Equal(expectedCollectionLocation, recorder.ValueCollectionLocation);
        Assert.Equal(expectedElementLocations, recorder.ValueElementLocations);
    }

    [Theory]
    [ClassData(typeof(Datasets.ParserSources))]
    [SuppressMessage("Minor Code Smell", "S1125: Boolean literals should not be redundant", Justification = "Prefer equality check over negation.")]
    public async Task NamedSingle_FalseReturningRecorder_False(ISyntacticAttributeParser parser)
    {
        var recorder = Mock.Of<ISyntacticArgumentRecorder>(static (recorder) => recorder.TryRecordNamedArgument(It.IsAny<string>(), It.IsAny<object?>(), It.IsAny<Location>()) == false);

        var source = """
            [Named(Value = null)]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.False(result);
    }

    [Theory]
    [ClassData(typeof(Datasets.NamedAttributeSources))]
    public async Task NamedSingle_NonExistingNamedArugment_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticNamedAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueLocation);

        var source = """
            [Named(NonExisting = 42, Value = null)]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var expectedLocation = DependencyInjection.GetRequiredService<IArgumentLocator>().SingleArgument(attributeSyntax.ArgumentList!.Arguments[1].Expression);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Null(recorder.Value);
        Assert.Equal(expectedLocation, recorder.ValueLocation);
    }

    [Theory]
    [ClassData(typeof(Datasets.NamedAttributeSources))]
    public async Task NamedSingle_NullLiteral_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticNamedAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueLocation);

        var source = """
            [Named(Value = null)]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var expectedLocation = DependencyInjection.GetRequiredService<IArgumentLocator>().SingleArgument(attributeSyntax.ArgumentList!.Arguments[0].Expression);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Null(recorder.Value);
        Assert.Equal(expectedLocation, recorder.ValueLocation);
    }

    [Theory]
    [ClassData(typeof(Datasets.NamedAttributeSources))]
    public async Task NamedSingle_Value_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticNamedAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueLocation);

        var source = """
            [Named(Value = "42")]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var expectedLocation = DependencyInjection.GetRequiredService<IArgumentLocator>().SingleArgument(attributeSyntax.ArgumentList!.Arguments[0].Expression);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Equal("42", recorder.Value);
        Assert.Equal(expectedLocation, recorder.ValueLocation);
    }

    [Theory]
    [ClassData(typeof(Datasets.NamedAttributeSources))]
    public async Task NamedSingle_NonExisting_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticNamedAttributeRecorder recorder)
    {
        Assert.Null(recorder.Value);
        Assert.Null(recorder.ValueLocation);

        var source = """
            [Named(NonExisting = 42, Value = "42")]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var expectedLocation = DependencyInjection.GetRequiredService<IArgumentLocator>().SingleArgument(attributeSyntax.ArgumentList!.Arguments[1].Expression);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Equal("42", recorder.Value);
        Assert.Equal(expectedLocation, recorder.ValueLocation);
    }

    [Theory]
    [ClassData(typeof(Datasets.ParserSources))]
    [SuppressMessage("Minor Code Smell", "S1125: Boolean literals should not be redundant", Justification = "Prefer equality check over negation.")]
    public async Task NamedArray_FalseReturningRecorder_False(ISyntacticAttributeParser parser)
    {
        var recorder = Mock.Of<ISyntacticArgumentRecorder>(static (recorder) => recorder.TryRecordNamedArgument(It.IsAny<string>(), It.IsAny<IReadOnlyList<object?>?>(), It.IsAny<Location>(), It.IsAny<IReadOnlyList<Location>>()) == false);

        var source = """
            [Named(Values = null)]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.False(result);
    }

    [Theory]
    [ClassData(typeof(Datasets.NamedAttributeSources))]
    public async Task NamedArray_NullLiteral_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticNamedAttributeRecorder recorder)
    {
        Assert.Null(recorder.Values);
        Assert.Null(recorder.ValuesCollectionLocations);
        Assert.Null(recorder.ValuesElementLocations);

        var source = """
            [Named(Values = null)]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var (expectedCollectionLocation, expectedElementLocations) = DependencyInjection.GetRequiredService<IArgumentLocator>().ArrayArgument(attributeSyntax.ArgumentList!.Arguments[0].Expression);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Null(recorder.Values);
        Assert.Equal(expectedCollectionLocation, recorder.ValuesCollectionLocations);
        Assert.Equal(expectedElementLocations, recorder.ValuesElementLocations);
    }

    [Theory]
    [ClassData(typeof(Datasets.NamedAttributeSources))]
    public async Task NamedArray_Values_True_RecorderPopulated(ISyntacticAttributeParser parser, SyntacticNamedAttributeRecorder recorder)
    {
        Assert.Null(recorder.Values);
        Assert.Null(recorder.ValuesCollectionLocations);
        Assert.Null(recorder.ValuesElementLocations);

        var source = """
            [Named(Values = new object[] { "42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 } })]
            public class Foo { }
            """;

        var (compilation, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var intType = compilation.GetSpecialType(SpecialType.System_Int32);

        var (expectedCollectionLocation, expectedElementLocations) = DependencyInjection.GetRequiredService<IArgumentLocator>().ArrayArgument(attributeSyntax.ArgumentList!.Arguments[0].Expression);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Equal(7, recorder.Values!.Count);
        Assert.Equal("42", recorder.Values[0]);
        Assert.Null(recorder.Values[1]);
        Assert.Equal(intType, recorder.Values[2]);
        Assert.Equal("Foo", recorder.Values[3]);
        Assert.Equal(42, recorder.Values[4]);
        Assert.Equal((double)42, recorder.Values[5]);
        Assert.Equal(new object[] { "42", 42 }, (object?[])recorder.Values[6]!);

        Assert.Equal(expectedCollectionLocation, recorder.ValuesCollectionLocations);
        Assert.Equal(expectedElementLocations, recorder.ValuesElementLocations);
    }

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

        var intType = compilation.GetSpecialType(SpecialType.System_Int32);

        var expectedValueLocation = DependencyInjection.GetRequiredService<IArgumentLocator>().SingleArgument(attributeSyntax.ArgumentList!.Arguments[0].Expression);
        var (expectedArrayValuesCollectionLocation, expectedArrayValuesElementLocations) = DependencyInjection.GetRequiredService<IArgumentLocator>().ArrayArgument(attributeSyntax.ArgumentList!.Arguments[1].Expression);
        var (expectedParamsValuesCollectionLocation, expectedParamsValuesElementLocations) = DependencyInjection.GetRequiredService<IArgumentLocator>().ParamsArguments(attributeSyntax.ArgumentList!.Arguments[2].Expression, attributeSyntax.ArgumentList!.Arguments[3].Expression);
        var expectedNamedValueLocation = DependencyInjection.GetRequiredService<IArgumentLocator>().SingleArgument(attributeSyntax.ArgumentList!.Arguments[4].Expression);
        var (expectedNamedValuesCollectionLocation, expectedNamedValuesElementLocations) = DependencyInjection.GetRequiredService<IArgumentLocator>().ArrayArgument(attributeSyntax.ArgumentList!.Arguments[5].Expression);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Equal("42", recorder.Value!);
        Assert.Empty(recorder.ArrayValues!);
        Assert.Equal(new object[] { "42", 42 }, recorder.ParamsValues);
        Assert.Equal(intType, recorder.NamedValue);
        Assert.Equal(new object[] { 42, "42" }, recorder.NamedValues);

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

        var intType = compilation.GetSpecialType(SpecialType.System_Int32);

        var expectedValueLocation = DependencyInjection.GetRequiredService<IArgumentLocator>().SingleArgument(attributeSyntax.ArgumentList!.Arguments[0].Expression);
        var (expectedArrayValuesCollectionLocation, expectedArrayValuesElementLocations) = DependencyInjection.GetRequiredService<IArgumentLocator>().ArrayArgument(attributeSyntax.ArgumentList!.Arguments[1].Expression);
        var (expectedParamsValuesCollectionLocation, expectedParamsValuesElementLocations) = DependencyInjection.GetRequiredService<IArgumentLocator>().ArrayArgument(attributeSyntax.ArgumentList!.Arguments[2].Expression);
        var expectedNamedValueLocation = DependencyInjection.GetRequiredService<IArgumentLocator>().SingleArgument(attributeSyntax.ArgumentList!.Arguments[3].Expression);
        var (expectedNamedValuesCollectionLocation, expectedNamedValuesElementLocations) = DependencyInjection.GetRequiredService<IArgumentLocator>().ArrayArgument(attributeSyntax.ArgumentList!.Arguments[4].Expression);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Equal("42", recorder.Value!);
        Assert.Empty(recorder.ArrayValues!);
        Assert.Equal(new object[] { "42", 42 }, recorder.ParamsValues);
        Assert.Equal(intType, recorder.NamedValue);
        Assert.Equal(new object[] { 42, "42" }, recorder.NamedValues);

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
