# Parsers

`SharpAttributeParser` provides three services through which attributes are parsed, one for each [parsing mode](ParsingModes.md):
* `ISemanticParser`
* `ISyntacticParser`
* `ICombinedParser`

These can be injected using the add-on package [SharpAttributeParser.DependencyInjection](https://www.nuget.org/packages/SharpAttributeParser.DependencyInjection/), or through the implementations `SemanticParser`, `SyntacticParser`, and `CombinedParser`.

### Parser APIs

The API of all three `Parser`-services are shown below.

```csharp
bool ISemanticParser.TryParse(ISemanticRecorder, AttributeData);
bool ISyntacticParser.TryParse(ISyntacticRecorder, AttributeData, AttributeSyntax);
bool ICombinedParser.TryParse(ICombinedRecorder, AttributeData, AttributeSyntax);
```

The provided `Recorder`-objects are responsible for recording the parsed data - read more about `Recorders` [here](Recorders.md). The `bool` returned by the `Parsers` indicate whether the attempt to parse the provided attribute was successful. If `true`, the `Recorder` is expected to be able to provide a record of the parsed data.

### Usage

Below is a demonstration of how a `ISemanticParser` can be used to semantically parse an attribute. The services `ISyntacticParser` and `ICombinedParser` are used in very similar fashion, but also requires an `AttributeSyntax` with the syntactic description of the attribute.

```csharp
// The parser. Can be injected using the add-on package 'SharpAttributeParser.DependencyInjection'
ISemanticParser parser; // new SemanticParser();

// The AttributeData describing the attribute being parsed
AttributeData attributeData;

// The ISemanticRecorder, reponsible for recording the parsed data
ISemanticRecorder recorder;

// Use the recorder to parse the attribute
bool success = parser.TryParse(recorder, attributeData);

if (success)
{
    // Retrieve the record of the parsed arguments
    var result = recorder.GetRecord();

    ...
}
```