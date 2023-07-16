# Getting Started

This article will explain in detail how to use `SharpAttributeParser` to parse an attribute. The pattern used will be as simple as possible, and therefore have many shortcomings. See [recommended pattern](RecommendedPattern/RecommendedPattern.md) for a more involved alternative.

#### 1. Attribute Definition

First, the attribute that will be used as an example throughout this article:

```csharp
[Example<Type>(new[] { 0, 1, 1, 2 }, name: "Fib", Answer = 41 + 1)]
class Foo { }
```

#### 2. Representation Structure

The desired result of parsing the attribute is the following structure:

```csharp
class ExampleRecord
{
    public ITypeSymbol T { get; set; }
    public IReadOnlyList<int> Sequence { get; set; }
    public string Name { get; set; }
    public int? Answer { get; set; }
}
```

#### 3. Mapper Implementation

We implement a `Mapper`, extending the abstract class `ASemanticAttributeMapper`.

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

#### 4. Usage

We are now able to parse the attribute:

```csharp
// The AttributeData representing the attribute.
AttributeData attributeData;

SemanticAttributeParser parser = new SemanticAttributeParser();

ExampleMapper mapper = new ExampleMapper();

SemanticAttributeRecorderFactory factory = new SemanticAttributeRecorderFactory();

ISemanticAttributeRecorder<IExampleRecord> recorder = factory.Create(mapper, new ExampleRecord());

bool outcome = parser.TryParse(recorder, attributeData);

if (outcome is true)
{
    ExampleRecord result = recorder.GetRecord();
}
```