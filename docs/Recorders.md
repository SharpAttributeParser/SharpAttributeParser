# Recorders

When parsing attributes, a user-provided `Recorder`-object is required. The responsibility of the `Recorder` is to record the data parsed by the `Parser`.

> In the [recommended pattern](RecommendedPattern.md), the user is not expected to implement a `Recorder` - but is instead tasked with implementing a `Mapper`, which you can read more about [here](Mappers.md).

`SharpAttributeParser` defines three `Recorder` APIs, one for each [parsing mode](ParsingModes.md):
* `ISemanticRecorder`
* `ISyntacticRecorder`
* `ICombinedRecorder`

### Recorder APIs

Simplified, but conceptually accurate, forms of the `Recorder` APIs are shown below.

```csharp
bool ISemanticRecorder.TryRecordTypeArgument(ITypeParameterSymbol, ITypeSymbol);
bool ISemanticRecorder.TryRecordConstructorArgument(IParameterSymbol, object?);
bool ISemanticRecorder.TryRecordNamedArgument(string, object?);

bool ISyntacticRecorder.TryRecordTypeArgument(ITypeParameterSymbol, ExpressionSyntax);
bool ISyntacticRecorder.TryRecordNormalConstructorArgument(IParameterSymbol, ExpressionSyntax);
bool ISyntacticRecorder.TryRecordParamsConstructorArgument(IParameterSymbol, IReadOnlyList<ExpressionSyntax>);
bool ISyntacticRecorder.TryRecordDefaultConstructorArgument(IParameterSymbol);
bool ISyntacticRecorder.TryRecordNamedArgument(string, ExpressionSyntax);

bool ICombinedRecorder.TryRecordTypeArgument(ITypeParameterSymbol, ITypeSymbol, ExpressionSyntax);
bool ICombinedRecorder.TryRecordNormalConstructorArgument(IParameterSymbol, object?, ExpressionSyntax);
bool ICombinedRecorder.TryRecordParamsConstructorArgument(IParameterSymbol, object?, IReadOnlyList<ExpressionSyntax>);
bool ICombinedRecorder.TryRecordDefaultConstructorArgument(IParameterSymbol, object?);
bool ICombinedRecorder.TryRecordNamedArgument(string, object?, ExpressionSyntax);
```

The first argument of each method identifies a parameter, with the remaining arguments describing an argument of that parameter. Note that type-safety is lost when attributes are parsed semantically - the [recommended pattern](RecommendedPattern.md) resolves this issue.

### Implementation

Again, the [recommended pattern](RecommendedPattern.md) does not involve actually implementing a `Recorder`. If you have another pattern in mind, you will currently need to implement the relevant `Recorder`-API from scratch.

### Recorder Factories

If using [mappers](Mappers.md), `Recorders` are created through the following factory-services:
* `ISemanticRecorderFactory`
* `ISyntacticRecorderFactory`
* `ICombinedRecorderFactor`

These can be injected using the add-on package [SharpAttributeParser.Mappers.DependencyInjection](https://www.nuget.org/packages/SharpAttributeParser.Mappers.DependencyInjection/), or through the implementations `SemanticRecorderFactory`, `SyntacticRecorderFactory`, and `CombinedRecorderFactory`.

Simplified APIs of the factory-services are shown below:

```csharp
ISemanticRecorder ISemanticRecorderFactory.Create<TRecord>(ISemanticMapper<TRecord>, TRecord);
ISyntacticRecorder ISyntacticRecorderFactory.Create<TRecord>(ISyntacticMapper<TRecord>, TRecord);
ICombinedRecorder ICombinedRecorderFactory.Create<TRecord>(ICombinedMapper<TRecord>, TRecord);
```