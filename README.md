# SharpAttributeParser [![NuGet version](https://img.shields.io/nuget/v/SharpAttributeParser.svg?style=plastic)](https://www.nuget.org/packages/SharpAttributeParser/) ![License](https://img.shields.io/github/license/ErikWe/sharp-attribute-parser?style=plastic) ![.NET Target](https://img.shields.io/badge/.NET%20Standard-2.0-blue?style=plastic)

Parses C\# attributes using the Roslyn API, primarily intended for Analyzers and Source Generators.

## Usage

> This example shows the simplest possible usage. See [recommended pattern](docs/RecommendedPattern/RecommendedPattern.md) for an alternative.

Given the following attribute:

```csharp
[Example<string>(new[] { 0, 1, 1, 2 }, name: "Fib", Answer = 41 + 1)]
public class Foo { }
```

And the following representation of the attribute:

```csharp
class ExampleRecord
{
    public ITypeSymbol T { get; set; }
    public IReadOnlyList<int> Sequence { get; set; }
    public string Name { get; set; }
    public int? Answer { get; set; }
}
```

A `Mapper` needs to be implemented, responsible for mapping parameters of the target attribute to `Recorders`:

```csharp
class ExampleMapper : ASemanticAttributeMapper<ExampleRecord>
{
    protected override IEnumerable<(OneOf<int, string>, DTypeArgumentRecorder)> AddTypeParameterMappings()
    {
        yield return (0, Adapters.TypeArgument.For(RecordT));
    }

    protected override IEnumerable<(string, DArgumentRecorder)> AddParameterMappings()
    {
        yield return (nameof(ExampleAttribute<object>.Sequence), Adapters.ArrayArgument.For<int>(RecordSequence));
        yield return (nameof(ExampleAttribute<object>.Name), Adapters.SimpleArgument.For<string>(RecordName));
        yield return (nameof(ExampleAttribute<object>.Answer), Adapters.SimpleArgument.For<int>(RecordAnswer));
    }

    void RecordT(ExampleRecord dataRecord, ITypeSymbol t) => dataRecord.T = t;
    void RecordSequence(ExampleRecord dataRecord, IReadOnlyList<int> sequence) => dataRecord.Sequence = sequence;
    void RecordName(ExampleRecord dataRecord, string name) => datarecord.Name = name;
    void RecordAnswer(ExampleRecord dataRecord, int answer) => dataRecord.Answer = answer;
}
```

Finally, we can use `SharpAttributeParser` to parse the attribute:

```csharp
// The AttributeData representing the attribute.
AttributeData attributeData;

// Services are injected through DI.
ISemanticAttributeParser parser;
ISemanticAttributeRecorderFactory factory;
ISemanticAttributeMapper<ExampleRecord> mapper;

ISemanticAttributeRecorder<ExampleRecord> recorder = factory.Create(mapper, new ExampleRecord());

bool outcome = parser.TryParse(recorder, attributeData);

if (outcome is true)
{
    ExampleRecord result = recorder.GetRecord();

    // 2, string, 42
    Console.WriteLine($"{result.Sequence[3]}, {result.T.Name}, {result.Answer}");
}
```

## Getting Started

See [Getting Started](docs/GettingStarted.md) and the [docs](docs/README.md) for more information.