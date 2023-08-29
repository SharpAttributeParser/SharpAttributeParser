# SharpAttributeParser [![NuGet version](https://img.shields.io/nuget/v/SharpAttributeParser.svg?style=plastic)](https://www.nuget.org/packages/SharpAttributeParser/) ![License](https://img.shields.io/github/license/ErikWe/sharp-attribute-parser?style=plastic) ![.NET Target](https://img.shields.io/badge/.NET%20Standard-2.0-blue?style=plastic)

Parses C\# attributes using the Roslyn API, primarily intended for Analyzers and Source Generators - simplifying the process of interpreting `AttributeData` and extracting strongly-typed descriptions of attributes.

### Features

`SharpAttributeParser` allows parsing of the following attribute argument kinds:
* Type Arguments
* Constructor Arguments (normal, default, params)
* Named Arguments

Attribute arguments can be parsed in three modes:
* Semantic Mode - Records the actual attribute arguments.
* Syntactic Mode - Records syntactic information about the attribute arguments, using `ExpressionSyntax`.
* Combined Mode - Combines the Semantic and Syntactic modes.

### Usage

> The add-on package [SharpAttributeParser.Mappers](https://www.nuget.org/packages/SharpAttributeParser.Mappers/) is required to use the library as demonstrated here.

A `Mapper` is a user-implemented component used to map each parameter of an attribute-class to a `MappedRecorder`, which in turn is responsible for recording the arguments of that specific parameter. Typically, each attribute-class requires a separate `Mapper`-implementation. Below is an implementation of a `Mapper`, used to semantically parse the arguments of an attribute to `ExampleRecord`. See the [recommended pattern](docs/RecommendedPattern/RecommendedPattern.md) for a slightly improved pattern.

```csharp
// Extending ASemanticMapper provides some common functionality
class ExampleMapper : ASemanticMapper<ExampleRecord>
{
    protected override void AddMappings(IAppendableSemanticMappingRepository<ExampleRecord> repository)
    {
        // Add mappings from parameters to recorders
        repository.TypeParameters.AddIndexedMapping(0, (factory) => factory.Create(RecordTypeArgument));
        repository.ConstructorParameters.AddNamedMapping("constructorArgument", (factory) => factory.Create(ConstructorArgumentPattern, RecordConstructorArgument));
        repository.ConstructorParameters.AddNamedMapping("optionalArgument", (factory) => factory.Create(OptionalArgumentPattern, RecordOptionalArgument));
        repository.ConstructorParameters.AddNamedMapping("paramsArgument", (factory) => factory.Create(ParamsArgumentPattern, RecordParamsArgument));
        repository.NamedParameters.AddNamedMapping("NamedArgument", (factory) => factory.Create(NamedArgumentPattern, RecordNamedArgument));
    }

    // Create the patterns used to make arguments strongly typed
    IArgumentPattern<StringComparison> ConstructorArgumentPattern(IArgumentPatternFactory factory) => factory.Enum<StringComparison>();
    IArgumentPattern<string?> OptionalArgumentPattern(IArgumentPatternFactory factory) => factory.NullableString();
    IArgumentPattern<int[]> ParamsArgumentPattern(IArgumentPatternFactory factory) => factory.NonNullableArray(factory.Int());
    IArgumentPattern<ITypeSymbol?> NamedArgumentPattern(IArgumentPatternFactory factory) => factory.NullableType();


    // Record the arguments of parameters
    void RecordTypeArgument(ExampleRecord dataRecord, ITypeSymbol typeArgument) => dataRecord.TypeArgument = typeArgument;
    void RecordConstructorArgument(ExampleRecord dataRecord, StringComparison constructorArgument) => dataRecord.ConstructorArgument = constructorArgument;
    void RecordOptionalArgument(ExampleRecord dataRecord, string? optionalArgument) => dataRecord.OptionalArgument = optionalArgument;
    void RecordParamsArgument(ExampleRecord dataRecord, IReadOnlyList<int> paramsArgument) => dataRecord.ParamsArgument = paramsArgument;
    void RecordNamedArgument(ExampleRecord dataRecord, ITypeSymbol namedArgument) => dataRecord.NamedArgument = namedArgument;
}
```

This `Mapper` can then be used to parse attributes:

```csharp
// The AttributeData describing the attribute being parsed
AttributeData attributeData;

// The required services. Can be injected using the add-on package SharpAttributeParser.Mappers.DependencyInjection
ISemanticParser parser; // new SemanticParser();
ISemanticRecorderFactory recorderFactory; // new SemanticRecorderFactory();

// Instantiate the mapper implemented above
var mapper = new ExampleMapper();

// Use the mapper to create a recorder
ISemanticRecorder recorder = recorderFactory.Create(mapper, new ExampleRecord());

// Use the recorder to parse the attribute
bool success = parser.TryParse(recorder, attributeData);

if (success)
{
    // Retrieve the ExampleRecord containing the parsed arguments
    ExampleRecord dataRecord = recorder.GetRecord();

    ...
}
```

### Documentation

See the [docs](docs/README.md) for more information.