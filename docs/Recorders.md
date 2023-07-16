# Attribute Recorders

Each `Parser` of `SharpAttributeParser` requires a user-provided `Recorder`-object. The responsibility of the `Recorder` is to record the data parsed by the `Parser`.

> In the [recommended pattern](RecommendedPattern/RecommendedPattern.md), the user is not expected to implement a `Recorder`. The user is instead tasked with implementing a `Mapper`, which you can read more about [here](Mappers.md).

Each [parsing mode](ParsingModes.md) has a corresponding `Recorder`, but this article will only consider semantic parsing - as all modes are very similar.

#### Usage

The API of `ISemanticAttributeRecorder` is shown below. An implemented `Recorder` is used by passing it to a `Parser`. When this is done, the `Parser` will invoke exactly one of the methods in the API for every attribute argument that is parsed.

```csharp
bool TryRecordTypeArgument(ITypeParameterSymbol, ITypeSymbol);
bool TryRecordConstructorArgument(IParameterSymbol, object?);
bool TryRecordNamedArgument(string, object?);
```

The first argument passed to the `Recorder` identifies an attribute parameter, while the second argument represents the parsed argument of that parameter.

#### Implementation

The [recommended pattern](RecommendedPattern/RecommendedPattern.md) does not involve actually implementing a `Recorder`. If you have another pattern in mind, you will currently need to implement the `Recorder` from scratch.