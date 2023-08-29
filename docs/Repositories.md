# Repositories

`Repositories` are used by the abstract `Mappers` to handle the mappings from attribute parameters to `MappedRecorders` - read more about `Mappers` [here](Mappers.md). `SharpAttributeParser` defines five `Repository` APIs, each used by the corresponding abstract `Mapper`:
* `ISemanticMappingRepository`
* `ISyntacticMappingRepository`
* `ICombinedMappingRepository`
* `ISplitMappingRepository`
* `IAdaptiveMappingRepository`

When extending an abstract `Mapper`, the `Repository` is modified by overriding the abstract method `AddMappings`. This method is invoked by the base-class when the `Mapper` is initialized - which occurs the first time the `Mapper` is used. After initialization, new mappings should not be added to the `Repository`.

### Appending Mappings

When appending mappings to a `Repository`, a key is used to identify the parameter being mapped. The following keys are supported:

* Type Parameters
  * `int` - Identifies a type parameter by their index.
  * `string` - Identifies a type parameter by their name.
* Constructor Parameters
  * `string` - Identifies a constructor parameter by their name.
* Named Parameters
  * `string` - Identifies a named parameter by their name.

Together with the key, a `MappedRecorder` should be provided - responsible for recording arguments of the parameters that match the provided key.

##### Creating MappedRecorders

The `Repositories` include support for creating `MappedRecorders` through a `MappedRecorderFactory` - a factory provided by the `Repository`. These factories allow `MappedRecorders` to be constructed from regular `delegates`.

### Example

Below is an example of how the `Repositories` provided by the `ASemanticMapper<TRecord>` can be used, to add mappings for different kinds of parameters.

```csharp
class ExampleMapper : ASemanticMapper<ExampleRecord>
{
    protected override void AddMappings(IAppendableSemanticMappingRepository<ExampleRecord> repository)
    {
        // Add mappings from parameters to recorders
        repository.TypeParameters.AddIndexedMapping(0, (factory) => factory.Create(RecordTypeArgument));
        repository.ConstructorParameters.AddNamedMapping("ConstructorArgument", (factory) => factory.Create(ConstructorArgumentPattern, RecordConstructorArgument));
        repository.ConstructorParameters.AddNamedMapping("OptionalArgument", (factory) => factory.Create(OptionalArgumentPattern, RecordOptionalArgument));
        repository.ConstructorParameters.AddNamedMapping("ParamsArgument", (factory) => factory.Create(ParamsArgumentPattern, RecordParamsArgument));
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