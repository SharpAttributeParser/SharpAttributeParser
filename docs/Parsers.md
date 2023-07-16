# Attribute Parsers

`SharpAttributeParser` provides three services through which attributes are parsed:
* `IAttributeParser`
* `ISemanticAttributeParser`
* `ISyntacticAttributeParser`

These can be accessed through `AttributeParser`, `SemanticAttributeParser`, and `SyntacticAttributeParser`, or registered with a `IServiceCollection`:

```csharp
using SharpAttributeParser.Extensions;

services.AddSharpAttributeParser();
```

#### Parser API

The API of all three `Parser`-services are shown below.

```csharp
bool IAttributeParser.TryParse(IArgumentRecorder, AttributeData, AttributeSyntax);

bool ISemanticAttributeParser.TryParse(ISemanticArgumentRecorder, AttributeData);

bool ISyntacticAttributeParser.TryParse(ISyntacticArgumentRecorder, AttributeData, AttributeSyntax);
```

#### Recorders

All `Parser`-services require a user-provided `Recorder`-object, which is responsible for recording the parsed data. Read about `Recorders` [here](Recorders.md).

#### Parsing Modes

`SharpAttirbuteParser` can be used to parse attributes in two different modes:
* **Semantic parsing** - Records the values of attribute arguments.
* **Syntactic parsing** - Records how each attribute argument was expressed.

The `Parser`-service `IAttributeParser` can be used to parse an attribute both semantically and syntactically. Read more about parsing modes [here](ParsingModes.md).