# SharpAttributeParser [![NuGet version (SharpAttributeParser)](https://img.shields.io/nuget/v/SharpAttributeParser.svg?style=plastic)](https://www.nuget.org/packages/SharpAttributeParser/) ![GitHub](https://img.shields.io/github/license/ErikWe/sharp-attribute-parser?style=plastic)

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

The parsed arguments are passed to the provided `Recorder`, which is responsible for recording the arguments. The `Recorder` is implemented by the user, and typically a single implementation can only handle a single attribute-type. Recorders are described in more detail below.

##### Semantic vs. Syntactic parsing

Attributes can be parsed *semantically* or *syntactically*. Semantic parsing is the simplest form, and allows type-arguments, constructor arguments, and named arguments to be parsed. Syntactic parsing includes all features of semantic parsing, and additionally allows the `Location` of the parsed arguments to be recorded. Useful for Analyzers and Source Generators wishing to produce `Diagnostic` because of some invalid argument.

## Recorders

All parsers require a user-implemented `Recorder`, responsible for recording the parsed arguments. Typically, each distinct attribute-type will require a separate implementation. Recorders will receive the values of the parsed arguments, together with the symbol of the associated parameter.

In most cases, `Recorders` may extend one of the abstract classes `ASemanticArgumentRecorder` and `ASyntacticArgumentRecorder`.

#### Recorder Example

Below is an implementation of a `ISemanticArgumentRecorder`, recording the arguments of an `ExampleAttribute`, as shown at the top of this page.

```csharp
public class ExampleRecorder : ASemanticArgumentRecorder
{
    public ITypeSymbol T { get; set; }
    public IReadOnlyList<int> Sequence { get; set; }
    public string Name { get; set; }
    public int? Answer { get; set; }

    protected override IEnumerable<(string, DSemanticGenericRecorder)> AddGenericRecorders()
    {
        yield return ("T", Adapters.For(RecordT));
    }

    protected override IEnumerable<(string, DSemanticArrayRecorder)> AddArrayRecorders()
    {
        yield return ("Sequence", Adapters.For<int>(RecordSequence));
    }

    protected override IEnumerable<(string, DSemanticSingleRecorder)> AddSingleRecorders()
    {
        yield return ("Name", Adapters.For<string>(RecordName));
        yield return ("Answer", Adapters.For<int>(RecordAnswer));
    }

    private void RecordT(ITypeSymbol t) => T = t;
    private void RecordSequence(IReadOnlyList<int> sequence) => Sequence = sequence;
    private void RecordName(string name) => Name = name;
    private void RecordAnswer(int answer) => Answer = answer;
}
```