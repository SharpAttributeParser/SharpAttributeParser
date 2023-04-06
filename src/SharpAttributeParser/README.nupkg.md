# SharpAttributeParser

Parses C\# attributes using the Roslyn API, primarily intended for Analyzers and Source Generators. The parser supports type-arguments, constructor arguments, and named arguments:

```csharp
[Example<Type>(new[] { 0, 1, 1, 2 }, name: "Fib", Answer = 41 + 1)]
public class Foo { }
```

## Usage

Attributes are parsed using either of the following services:

* `bool ISemanticAttributeParser.TryParse(ISemanticArgumentRecorder, AttributeData)`
* `bool ISyntacticAttributeParser.TryParse(ISyntacticArgumentRecorder, AttributeData, AttributeSyntax)`

These services are available as `SemanticAttributeParser` and `SyntacticAttributeParser`, or they can be registered with a `IServiceCollection`:

```csharp
using SharpAttributeParser.Extensions;

services.AddSharpAttributeParser();
```

See [GitHub](https://github.com/ErikWe/sharp-attribute-parser) for more information.