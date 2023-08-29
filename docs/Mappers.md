# Mappers

> `Mappers` are provided by the add-on package [SharpAttributeParser.Mappers](https://www.nuget.org/packages/SharpAttributeParser.Mappers/).

Each `Parser`-service of `SharpAttributeParser` requires a user-provided `Recorder`-object. The role of `Mappers` is to act as blueprints for constructing `Recorders`. `SharpAttributeParser` defines three types of `Mappers`, one for each [parsing mode](ParsingModes.md).
* `ISemanticMapper<TRecord>`
* `ISyntacticMapper<TRecord>`
* `ICombinedMapper<TRecord>`

The type parameter of each `Mapper` describes the type to which the parsed data is recorded by the constructed `Recorders`.

### Usage

A `Mapper` can be used to construct a `Recorder` using the following factory-services:
* `ISemanticRecorderFactory`
* `ISyntacticRecorderFactory`
* `ICombinedRecorderFactor`

These can be injected using the add-on package [SharpAttributeParser.Mappers.DependencyInjection](https://www.nuget.org/packages/SharpAttributeParser.Mappers.DependencyInjection/), or through the implementations `SemanticRecorderFactory`, `SyntacticRecorderFactory`, and `CombinedRecorderFactory`.

### Implementation

Typically, each attribute-class will require a separate `Mapper`-implementation. These can be implemented by manually implementing the `Mapper`-API, or by extending one of five abstract base-classes:
* `ASemanticMapper<TRecord>`
* `ASyntacticMapper<TRecord>`
* `ACombinedMapper<TRecord>`
* `ASplitMapper<TSemanticRecord, TSyntacticRecord>`
* `AAdaptiveMapper<TCombinedRecord, TSemanticRecord>`

The first three base-classes each directly correspond to a `Recorder`-kind, while the last two base-classes each allow construction of two different kinds of `Recorders` (`ISemanticRecorder` and `ISyntacticRecorder` for `ASplitMapper`, `ICombinedRecorder` and `ISemanticRecorder` for `AAdaptiveMapper`).

##### Extending abstract Mappers

> The following demonstration will use `ASemanticMapper`, but the other four base-`Mappers` have very similar behaviour.

`ASemanticMapper<TRecord>` contains a `Repository` with mappings from parameters to `MappedRecorders`, reponsible for recording arguments of that specific parameter. The `Repository` is modified by overriding the abstract method `AddMappings`:

```csharp
void AddMappings(IAppendableSemanticMappingRepository<TRecord>);
```

This method is invoked by the base-class when the `Mapper` is initialized - which occurs the first time the `Mapper` is used. After initialization, new mappings should not be added to the `Repository`. Read more about `Repositories` [here](Repositories.md).

Below is an example of a simple `Mapper` implemented through `ASemanticMapper<TRecord>`. See the [recommended pattern](docs/RecommendedPattern/RecommendedPattern.md) for a slightly improved pattern. 

```csharp
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